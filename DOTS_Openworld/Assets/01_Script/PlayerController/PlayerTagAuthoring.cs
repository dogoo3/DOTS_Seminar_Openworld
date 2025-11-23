using UnityEngine;
using Unity.Entities;

public struct PlayerTag : IComponentData { }

public class PlayerTagAuthoring : MonoBehaviour
{
    class Baker : Baker<PlayerTagAuthoring>
    {
        public override void Bake(PlayerTagAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent<PlayerTag>(entity);
        }
    }
}
