using Unity.Entities;
using Unity.Mathematics;

[System.Serializable]
public struct AABB : IComponentData
{
    public float3 min;
    public float3 max;
}

public class AABBComponent : ComponentDataProxy<AABB> { }
