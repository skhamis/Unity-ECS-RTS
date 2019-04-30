using System;
using Unity.Entities;
using UnityEngine;

public struct UnitSpawner : IComponentData
{
    public int CountX;
    public int CountY;
    public Entity Prefab;
}