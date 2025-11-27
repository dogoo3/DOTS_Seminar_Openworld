/*using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

*//*
 * InputComponent
 *  - 이번 프레임의 이동 입력값
 *    move.x = A / D
 *    move.y = W / S
 */
/*public struct InputComponent : IComponentData
{
    public float2 Move;
}
*/
/*
 * Player 입력 읽기 시스템
 * - Initialization 그룹에서 돌면서
 * - 새 Input System(PlayerController)로부터 이동값을 읽고
 * - InputComponent 싱글톤에 저장
 *//*
[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct PlayerInputSystem : ISystem
{
    // ISystem은 필드에 managed 타입 못 들고 있어서
    // static 으로 빼서 브리지로 사용
    private  PlayerController s_InputActions;

    public void OnCreate(ref SystemState state)
    {
        if (s_InputActions == null)
        {
            s_InputActions = new PlayerController();
            s_InputActions.Enable();
        }

        // 입력값을 담아둘 엔티티 1개 생성
        var e = state.EntityManager.CreateEntity();
        state.EntityManager.AddComponentData(e, new InputComponent());
    }

    public void OnDestroy(ref SystemState state)
    {
        // 월드가 파괴될 때 InputActions 정리
        if (s_InputActions != null)
        {
            s_InputActions.Disable();
            s_InputActions.Dispose();
            s_InputActions = null;
        }
    }

    public void OnUpdate(ref SystemState state)
    {
        // WASD Vector2 읽기
        var v = s_InputActions.ActionMap.Movement.ReadValue<Vector2>();

        SystemAPI.SetSingleton(new InputComponent
        {
            Move = new float2(v.x, v.y)
        });
    }
}

*//*
 * 실제 플레이어 이동 시스템
 * - Simulation 그룹에서 InputComponent 싱글톤을 읽고
 * - PlayerTag 가진 엔티티의 LocalTransform.Position 변경
 *//*
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct PlayerMoveSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;

        var input = SystemAPI.GetSingleton<InputComponent>();
        float2 mv = input.Move;
        float moveSpeed = 15f;

        foreach (var transform in
                 SystemAPI.Query<RefRW<LocalTransform>>()
                          .WithAll<PlayerTag>())
        {
            float3 dir = new float3(mv.x, 0, mv.y);

            if (math.lengthsq(dir) > 0f)
            {
                dir = math.normalize(dir);
                transform.ValueRW.Position += dir * moveSpeed * dt;
            }
        }
    }
}
*/