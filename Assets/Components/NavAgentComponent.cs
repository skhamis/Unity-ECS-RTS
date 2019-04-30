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

public class NavAgentComponent : ComponentDataProxy<NavAgent> { }
