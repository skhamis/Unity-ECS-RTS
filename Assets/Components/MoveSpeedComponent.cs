using Unity.Entities;
using System;

[Serializable]
public struct MoveSpeed : IComponentData
{
    public float speed;
}

public class MoveSpeedComponent : ComponentDataProxy<MoveSpeed> { }