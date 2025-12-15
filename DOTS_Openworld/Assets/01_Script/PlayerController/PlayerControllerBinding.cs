using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using Unity.Burst;

/*
 - Move.x : A(-1), D(+1) 좌우 이동
 - Move.y : S(-1), W(+1) 전후 이동
 - Look.x : 마우스 X축 이동량 (좌우 회전에 사용)
 - Jump   : 스페이스바가 이번 프레임에 눌렸는지 (true/false)
 */
public struct InputComponent : IComponentData
{
    public float2 Move;   // WASD 이동 입력 (-1 ~ 1)
    public float2 Look;   // 마우스 델타 값 (이번 프레임 이동량)
    public bool Jump;     // 이번 프레임에 점프 키가 눌렸는지
}

/*
 - Distance : 플레이어로부터 카메라까지의 거리 (뒤로 얼마나 떨어질지)
 - Offset   : 플레이어 위치에서 카메라가 바라볼 타겟 위치의 오프셋
 - FollowSpeed : 카메라가 플레이어를 따라가는 속도 
 */
public struct CameraState : IComponentData
{
    public float Distance;
    public float3 Offset;
    public float FollowSpeed;
}


[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial class PlayerControllerBinding : SystemBase
{
    private PlayerController inputActions;
    protected override void OnCreate()
    {
        base.OnCreate();
        inputActions = new PlayerController();
        inputActions.Enable();  // 입력 수신 시작

        // 마우스 커서를 화면 중앙에 고정
        Cursor.lockState = CursorLockMode.Locked;  
        Cursor.visible = false;                     

        var inputEntity = EntityManager.CreateEntity();
        EntityManager.AddComponentData(inputEntity, new InputComponent());

    }

 
    protected override void OnDestroy()
    {
        base.OnDestroy();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        inputActions?.Disable();
    }


    protected override void OnUpdate()
    {
        // W=+Y, S=-Y, A=-X, D=+X
        var move = inputActions.ActionMap.Movement.ReadValue<Vector2>();

        // Mouse Delta - 이번 프레임에 마우스가 얼마나 움직였는지
        var look = inputActions.ActionMap.Look.ReadValue<Vector2>();

        // WasPressedThisFrame() - 이번 프레임에 눌렸는지 (한 번만 true)
        bool jump = inputActions.ActionMap.Jump.WasPressedThisFrame();

        // 다른 시스템들이 SystemAPI.GetSingleton<InputComponent>()로 읽기위해 SetSignlton
        SystemAPI.SetSingleton(new InputComponent
        {
            Move = new float2(move.x, move.y),
            Look = new float2(look.x, look.y),
            Jump = jump
        });
    }
}


 // PlayerMoveSystem - 플레이어 이동/회전 시스템
[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct PlayerMoveSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        // DeltaTime 
        float dt = SystemAPI.Time.DeltaTime;

        // 입력값 읽기
        var input = SystemAPI.GetSingleton<InputComponent>();

        float2 move2D = input.Move; // WASD 입력
        float2 look2D = input.Look; // 마우스 델타

        bool jump = input.Jump; // 점프

        float moveSpeed = 8f; // 이동속도
        float mouseSensitivity = 0.15f; // 마우스 민감도
        float jumpForce = 10f; // 점프력

        // 플레이어 찾기
        foreach (var (transform, velocity) in
                 SystemAPI.Query<RefRW<LocalTransform>, RefRW<PhysicsVelocity>>()
                          .WithAll<PlayerTag>())
        {
            // 1. 마우스 회전 -> 플레이어 Yaw

            // 마우스 X 이동량을 각도로 변환
            float yawDelta = look2D.x * mouseSensitivity;

            // 현재 회전값에 Y축 회전 추가
            quaternion currentRot = transform.ValueRO.Rotation;
            quaternion yawRotation = quaternion.RotateY(math.radians(yawDelta));
            quaternion newRotation = math.mul(yawRotation, currentRot);

            // Y축 회전만 유지 (X, Z축 기울어짐 방지)
            float3 forward = math.mul(newRotation, new float3(0, 0, 1));
            forward.y = 0;  // 수평 방향만 유지
            if (math.lengthsq(forward) > 0.001f)
            {
                forward = math.normalize(forward);
                newRotation = quaternion.LookRotationSafe(forward, math.up());
            }

            // 회전값 적용
            var t = transform.ValueRO;
            t.Rotation = newRotation;
            transform.ValueRW = t;

            float3 currentVel = velocity.ValueRO.Linear;
            float3 newVel = currentVel;


            // 이동 입력이 있을 때
            if (math.lengthsq(move2D) > 0f)
            {
                // 플레이어의 현재 방향 벡터 계산
                float3 playerForward = math.mul(newRotation, new float3(0, 0, 1));  // 앞
                float3 playerRight = math.mul(newRotation, new float3(1, 0, 0));    // 오른쪽

                // XZ 평면에서만 이동 (Y축 무시)
                playerForward.y = 0;
                playerRight.y = 0;
                playerForward = math.normalize(playerForward);
                playerRight = math.normalize(playerRight);

                float3 moveDir = playerForward * move2D.y + playerRight * move2D.x;

                if (math.lengthsq(moveDir) > 0f)
                {
                    moveDir = math.normalize(moveDir);
                    // 이동갱신
                    t.Position += moveDir * moveSpeed * dt;
                    transform.ValueRW = t;
                }
            }

            // 바닥 판정: Y축 속도가 거의 0이면 바닥에 있다고 가정
            bool isGrounded = math.abs(currentVel.y) < 0.1f;

            if (jump && isGrounded)
            {
                // 위쪽으로 순간 속도 부여
                newVel.y = jumpForce;
            }

            velocity.ValueRW = new PhysicsVelocity
            {
                Linear = new float3(0, newVel.y, 0),  
                Angular = float3.zero  
            };
        }
    }
}


// TPSCameraSystem - 3인칭 카메라 시스템
[UpdateInGroup(typeof(SimulationSystemGroup))]
// 플레이어 이동 후 카메라 업데이트
[UpdateAfter(typeof(PlayerMoveSystem))]  
public partial struct TPSCameraSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<CameraState>();
    }

    public void OnUpdate(ref SystemState state)
    {
        // delta time 가져오기
        float dt = SystemAPI.Time.DeltaTime;

        // Unity의 MainCamera 태그가 붙은 카메라를 사용
        var mainCamera = Camera.main;
        if (mainCamera == null) return;  // 카메라 없으면 종료

        // CameraStateAuthoring에서 설정한 값
        var cameraState = SystemAPI.GetSingleton<CameraState>();

        // 플레이어 찾기
        foreach (var playerTransform in
                 SystemAPI.Query<RefRO<LocalTransform>>()
                          .WithAll<PlayerTag>())
        {
            float3 playerPos = playerTransform.ValueRO.Position;      // 플레이어 월드 위치
            quaternion playerRot = playerTransform.ValueRO.Rotation;  // 플레이어 회전

            // 플레이어 뒤쪽 방향 (플레이어가 바라보는 반대 방향)
            float3 cameraBack = math.mul(playerRot, new float3(0, 0, -1));

            // 카메라가 바라볼 타겟 위치 (플레이어 위치 + 오프셋)
            float3 targetPos = playerPos + cameraState.Offset;

            // 카메라 실제 위치 (타겟에서 뒤로 Distance만큼)
            float3 desiredCameraPos = targetPos + cameraBack * cameraState.Distance;

            // dt를 사용 -> 카메라 이동 (Lerp)
            float3 currentCameraPos = mainCamera.transform.position;
            float smoothFactor = 1f - math.exp(-cameraState.FollowSpeed * dt);
            float3 newCameraPos = math.lerp(currentCameraPos, desiredCameraPos, smoothFactor);

            // 카메라 transform 적용
            mainCamera.transform.position = newCameraPos;
            mainCamera.transform.LookAt(targetPos);  // 타겟을 바라봄
        }
    }
}
