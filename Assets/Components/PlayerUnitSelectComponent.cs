using Unity.Entities;
using System;


[Serializable]
public struct PlayerUnitSelect : IComponentData
{
    //Not actually using this, currently in the tutorial we are just
    //adding and removing the component
    public BlittableBool isSelected;
}

public class PlayerUnitSelectComponent : ComponentDataWrapper<PlayerUnitSelect> { }