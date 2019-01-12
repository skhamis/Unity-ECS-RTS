using System;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct PlayerInput : IComponentData {

    public BlittableBool LeftClick;
    public BlittableBool RightClick;
    public float3 MousePosition; 
}

public class PlayerInputComponent : ComponentDataWrapper<PlayerInput> { }
