/*using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Streaming.SceneManagement.SectionMetadata
{
    public class CircleAuthoring : MonoBehaviour
    {
        public float radius;

        class Baker : Baker<CircleAuthoring>
        {
            public override void Bake(CircleAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new Circle
                {
                    Radius = authoring.radius,
                    Center = authoring.transform.position
                });
            }
        }

    }

    public struct Circle : IComponentData
    {
        public float Radius; // Proximity radius within which to consider loading a section
        public float3 Center;
    }
}
*/


using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
public struct Circle : IComponentData
{
    public float Radius; // Proximity radius within which to consider loading a section
    public float3 Center;
}
public class CircleAuthoring : MonoBehaviour
{
    public float radius = 10f;

    class Baker : Baker<CircleAuthoring>
    {
        public override void Bake(CircleAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);

            // 1. 이 오브젝트(또는 자식들)의 Renderer 찾기
            var renderer = authoring.GetComponentInChildren<Renderer>();

            // 2. 메쉬 Bounds 중심을 월드 좌표로 사용
            Vector3 centerWorld = authoring.transform.position;
            if (renderer != null)
                centerWorld = renderer.bounds.center;
            
            AddComponent(entity, new Circle
            {
                Radius = authoring.radius,
                Center = (float3)centerWorld
            });
        }
    }


}
