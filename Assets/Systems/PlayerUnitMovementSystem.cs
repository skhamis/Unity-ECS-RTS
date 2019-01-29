using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Jobs;
using System.ComponentModel;

public class PlayerUnitMovementSystem : JobComponentSystem
{

    public struct PlayerUnitMovementJob : IJobProcessComponentData<PlayerInput, NavAgent, PlayerUnitSelect>
    {
        public float dT;

        public void Execute
            (ref PlayerInput pInput, ref NavAgent navAgent, ref PlayerUnitSelect selected)
        {
           if (pInput.RightClick)
            {
                navAgent.finalDestination = pInput.MousePosition;
                navAgent.agentStatus = NavAgentStatus.Moving;

            }
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new PlayerUnitMovementJob();
        return job.Schedule(this, inputDeps);
    }
}
