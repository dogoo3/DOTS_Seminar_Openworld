using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class CameraStateAuthoring : MonoBehaviour
{
    [Header("TPS 카메라 설정")]
    [Range(1f, 20f)]
    public float Distance = 5f;
    public Vector3 Offset = new Vector3();

    [Header("카메라 따라가기 속도")]
    [Range(1f, 30f)]
    public float FollowSpeed = 10f;

    class Baker : Baker<CameraStateAuthoring>
    {
        public override void Bake(CameraStateAuthoring authoring)
        {
            // 엔티티 생성 (Transform 불필요 -> TransformUsageFlags.None)
            var entity = GetEntity(TransformUsageFlags.None);

            // CameraState 컴포넌트 추가
            AddComponent(entity, new CameraState
            {
                Distance = authoring.Distance,
                Offset = new float3(
                    authoring.Offset.x,
                    authoring.Offset.y,
                    authoring.Offset.z
                ),
                FollowSpeed = authoring.FollowSpeed
            });
        }
    }
}
