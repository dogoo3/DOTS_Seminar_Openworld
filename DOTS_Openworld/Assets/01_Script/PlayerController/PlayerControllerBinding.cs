using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

/*
 move.x = a, d
 move.y = w, s
 
 */
public struct InputComponent : IComponentData
{
    public float2 Move;
}


/*
   입력읽기는 프레임 로직 돌기전 한번만

 
    프레임 입력값 in
 */
[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial class PlayerControllerBinding : SystemBase
{
    private PlayerController inputActions;

    /// <summary>
    ///  InputActions 인스턴스 생성 & Enable
    /// </summary>
    protected override void OnCreate()
    {
        base.OnCreate();

        // 새 인풋 시스템 클래스 생성 + 활성화
        inputActions = new PlayerController();
        inputActions.Enable();

        // 인풋 값을 담아둘 엔티티 생성
        var e = EntityManager.CreateEntity();
        EntityManager.AddComponentData(e, new InputComponent());

    }

    /// <summary>
    /// 매 프레임 호출
    /// - InputActions에서 Movement 값을 읽어서
    /// - ECS 월드의 InputComponent 싱글톤에 덮어쓴다
    /// </summary>
    protected override void OnUpdate()
    {
        // WASD 등으로 세팅한 2D Vector 입력 읽기
        var v = inputActions.ActionMap.Movement.ReadValue<Vector2>();

        SystemAPI.SetSingleton(new InputComponent
        {
            Move = new float2(v.x, v.y)
        });
    }
}

/// <summary>
/// 실제 플레이어 엔티티를 이동시키는 ISystem
/// 위에서 세팅한 InputComponent 읽음 -> PlayerTag엔티티 이동
/// </summary>
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct PlayerMoveSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        // 프레임별 델타 타임
        float dt = SystemAPI.Time.DeltaTime;

        var input = SystemAPI.GetSingleton<InputComponent>();
        float2 move2D = input.Move;

        // 이동 속도
        float moveSpeed = 15f;

        foreach (var transform in
                 SystemAPI.Query<RefRW<LocalTransform>>()
                          .WithAll<PlayerTag>()) // -> 플레이어 엔티티 조건
        {
            // XZ 평면 기준 이동 방향
            float3 dir = new float3(move2D.x, 0, move2D.y);

            // 입력이 0,0 이 아닐 때만 이동
            if (math.lengthsq(dir) > 0f)
            {
                dir = math.normalize(dir);
                transform.ValueRW.Position += dir * moveSpeed * dt;
            }
        }
    }
}
