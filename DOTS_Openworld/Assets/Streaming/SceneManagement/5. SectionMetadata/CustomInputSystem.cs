using Streaming.SceneManagement.Common;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct CustomInputSystem : ISystem
{
    private float2 prevMousePos;
    private bool isFirstFrame;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        prevMousePos = float2.zero;
        isFirstFrame = true;
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.TryGetSingleton<InputComponent>(out var inputComponent))
            return;

        foreach (var (playerLocalTransform, relevant, value) in
                 SystemAPI.Query<RefRW<LocalTransform>, RefRO<Relevant>, RefRO<SpeedRotateValueComponent>>())
        {
            float2 moveVector = inputComponent.moveMents;
            float2 currentMousePos = inputComponent.mousePos;

            // 첫 프레임만 초기화
            if (isFirstFrame)
            {
                prevMousePos = currentMousePos;
                isFirstFrame = false;
            }

            // 마우스 이동량
            float2 mouseDelta = currentMousePos - prevMousePos;
            prevMousePos = currentMousePos;

            // 마우스 감도 설정
          /*  float sensitivity = 0.5f;
            float yaw = math.radians(mouseDelta.x * sensitivity);
            quaternion yawRotation = quaternion.Euler(0, yaw, 0);
            playerLocalTransform.ValueRW.Rotation = math.mul(yawRotation, playerLocalTransform.ValueRO.Rotation);*/

            // 캐릭터 이동 처리
            float3 forward = math.mul(playerLocalTransform.ValueRO.Rotation, new float3(0, 0, 1));
            float3 right = math.mul(playerLocalTransform.ValueRO.Rotation, new float3(1, 0, 0));
            float3 moveDir = (forward * moveVector.y) + (right * moveVector.x);

            playerLocalTransform.ValueRW.Position += moveDir * value.ValueRO.speedValue * SystemAPI.Time.DeltaTime;

        }
    }
}


