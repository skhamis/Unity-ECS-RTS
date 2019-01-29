using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;


public enum NavAgentStatus
{
    Idle = 0,
    Moving = 1,
}



[System.Serializable]
public struct NavAgent : IComponentData
{
    public float3 finalDestination;
    public NavAgentStatus agentStatus;
}

public class NavAgentComponent : ComponentDataWrapper<NavAgent> { }
