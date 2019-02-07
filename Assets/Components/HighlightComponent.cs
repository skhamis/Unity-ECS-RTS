using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

[System.Serializable]
public struct Highlight : ISharedComponentData
{
    public GameObject prefab;
}

public class HighlightComponent : SharedComponentDataWrapper<Highlight> { }
