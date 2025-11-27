using Unity.Entities;
using UnityEngine;

namespace Streaming.SceneManagement.Common
{
    public class RelevantAuthoring : MonoBehaviour
    {
        class Baker : Baker<RelevantAuthoring>
        {
            public override void Bake(RelevantAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<Relevant>(entity);
            }
        }
    }

    public struct Relevant : IComponentData
    {
    }
}
