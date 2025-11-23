using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
partial struct PrefabSystem : ISystem
{
    private bool _wasKeyPressed; // 상태를 추적하여 키 입력 확인

    public void OnCreate(ref SystemState state)
    {
        _wasKeyPressed = false; // 초기화
    }


    public void OnUpdate(ref SystemState state)
    {
        var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);

        var uninitializedQuery = SystemAPI.QueryBuilder().WithAll<GOPrefabData>()
           .WithNone<HybridCleanupData>().Build();

        var cleanupQuery = SystemAPI.QueryBuilder()
            .WithAll<HybridCleanupData>().Build();

        foreach (var entity in uninitializedQuery.ToEntityArray(Allocator.Temp))
        {
            Debug.Log("first");
            // HybridCleanupData 추가  
            ecb.AddComponent(entity, new HybridCleanupData { gameObjectReference = null });
        }


        foreach (var (prefabData, localTransform, cleanupData, entity)
               in SystemAPI.Query<GOPrefabData, RefRO<LocalTransform>, HybridCleanupData>().WithEntityAccess())
        {
            // 이 때는 한 Entity에 HybridPrefabData와 HybridCleanupData가 동시에 존재합니다.
            // 여기서 GameObject를 생성한 후 HybridPrefabData를 지우면, 딱 한번만 GameObject가 생성됩니다.  
            Debug.Log("second");
            cleanupData.gameObjectReference = GameObject.Instantiate(prefabData.prefab,
                localTransform.ValueRO.Position, localTransform.ValueRO.Rotation);

            ecb.RemoveComponent<HybridCleanupData>(entity);
        }


        /* 
         기존 Prefab 만 
         foreach (var (prefabData, localTransform) in SystemAPI.Query<GOPrefabData, RefRO<LocalTransform>>())
          {
              var myPrefab = prefabData.prefab; // Prefab 참조
              var myPosition = localTransform.ValueRO.Position; // 위치 정보 가져오기

              // GameObject.Instantiate를 사용하여 프리팹 생성
              GameObject instantiatedObject = GameObject.Instantiate(myPrefab, myPosition, quaternion.identity);

              // 생성된 게임 오브젝트에 원하는 추가 작업 수행
              instantiatedObject.name = "InstantiatedPrefab";
              instantiatedObject.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f); // 크기 변경
              instantiatedObject.GetComponent<Renderer>().material.color = Color.red; // 색상 변경 예제
          }*/
        //   }



        // 키 입력이 해제되었을 때 상태 초기화

        // ---------------- 클린업 단계
        foreach (var hybridCleanupData in cleanupQuery.ToComponentDataArray<HybridCleanupData>())
        {
            Debug.Log("third");
            // .ToComponentDataArray 는 NativeArray<IComponentData>를 반환합니다.
            // 여기서 혼자 남은 HybridCleanupData의 gameObjectReference를 액세스, GameObject를 삭제해줄 수 있습니다.
            GameObject.Destroy(hybridCleanupData.gameObjectReference);
        }

        foreach (var entity in cleanupQuery.ToEntityArray(Allocator.Temp))
        {
            // HybridCleanupData 제거 및 엔티티 삭제  
            Debug.Log("fourth");
            ecb.RemoveComponent<HybridCleanupData>(entity);
        }

    }
}