using UnityEngine;
using Unity.Entities;
using UnityEditor.Build;

public class PrefabAuthoring : MonoBehaviour
{
    public GameObject _prefab;

    class Baker : Baker<PrefabAuthoring>
    {
        public override void Bake(PrefabAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponentObject(entity, new GOPrefabData()
            {
                prefab = authoring._prefab
            });
        }
    }
}

public class GOPrefabData : IComponentData
{
    public GameObject prefab;
}

public class HybridCleanupData : ICleanupComponentData
{
    public GameObject gameObjectReference;
}