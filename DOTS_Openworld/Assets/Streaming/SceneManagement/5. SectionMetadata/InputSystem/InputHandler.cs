using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct InputComponent : IComponentData
{
    public float2 mousePos;
    public float2 moveMents;
    public bool isMouseClicked;
    public bool isSpaceClicked;
}
public partial class InputHandler : SystemBase
{
    private PlayerController controls;

    protected override void OnCreate()
    {
        if (!SystemAPI.TryGetSingleton<InputComponent>(out InputComponent input))
        {
            Entity inputEntity  = EntityManager.CreateEntity(typeof(InputComponent));
            EntityManager.SetComponentData(inputEntity, new InputComponent());
        }

        controls = new PlayerController();
        controls.Enable();
    }

    protected override void OnUpdate()
    {
        Vector2 _moveMents = controls.ActionMap.Movement.ReadValue<Vector2>();
        Vector2 _mousePos = controls.ActionMap.MousePosition.ReadValue<Vector2>();
        bool _isSpaceClicked = controls.ActionMap.zoomInzoomOut.ReadValue<float>() > 0.5f;
        bool _isMouseClicked = controls.ActionMap.MouseInput.ReadValue<float>() > 0.5f;



        SystemAPI.SetSingleton(new InputComponent { 
            mousePos = _mousePos, 
            moveMents = _moveMents,
            isSpaceClicked = _isSpaceClicked,
            isMouseClicked = _isMouseClicked
        });
    }
}
