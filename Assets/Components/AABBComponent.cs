using Unity.Entities;
using Unity.Mathematics;

public struct AABB : IComponentData
{
    public float3 min;
    public float3 max;
}
