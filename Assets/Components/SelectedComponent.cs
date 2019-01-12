using Unity.Entities;
using System;


[Serializable]
public struct Selected : IComponentData
{
}

public class SelectedComponent : ComponentDataWrapper<Selected> { }