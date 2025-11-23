using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    private Entity playerEntity;
    private EntityManager entityManager;

    void Start()
    {
        // EntityManager 인스턴스를 가져옵니다.
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        // Relevant 컴포넌트를 가진 플레이어 엔티티를 찾기
        playerEntity = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntityQuery(typeof(Streaming.SceneManagement.Common.Relevant)).GetSingletonEntity();
    }

    void Update()
    {
        if (playerEntity != Entity.Null)
        {
            // 플레이어 엔티티의 LocalTransform 컴포넌트를 가져오기
            var playerTransform = entityManager.GetComponentData<LocalTransform>(playerEntity);

            // 빈 게임 오브젝트의 위치와 회전값을 플레이어 엔티티와 일치시킴
            transform.position = playerTransform.Position;
            transform.rotation = playerTransform.Rotation;
        }
    }
}
