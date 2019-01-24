
using System;
using Unity.Entities;
using UnityEngine;

[Serializable]
// ISharedComponentData can have struct members with managed types.
public struct UnitSpawner : ISharedComponentData
{
    public GameObject prefab;
}

public class UnitSpawnerComponent : SharedComponentDataWrapper<UnitSpawner> { }