using UnityEngine;
using Unity.Entities;

public struct SpeedRotateValueComponent : IComponentData
{
    public float speedValue;
    public float rotateValue;
}
public class CustomSpeedAuthoring : MonoBehaviour
{
    public float speedValue;
    public float rotateValue;
    private class Baker : Baker<CustomSpeedAuthoring>
    {
        public override void Bake(CustomSpeedAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new SpeedRotateValueComponent
            { 
                speedValue = authoring.speedValue,
                rotateValue = authoring.rotateValue,
            });
        }
    }
}
