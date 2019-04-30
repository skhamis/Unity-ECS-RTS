using System;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct PlayerInput : IComponentData {

    public bool LeftClick;
    public bool RightClick; 
}

public class PlayerInputComponent : ComponentDataProxy<PlayerInput> { }
