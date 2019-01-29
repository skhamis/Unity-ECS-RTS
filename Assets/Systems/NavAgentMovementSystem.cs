using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;

public class NavAgentMovementSystem : JobComponentSystem
{
    public struct NavAgentMovementJob : IJobProcessComponentData<Position, NavAgent>
    {
        public float dT;

        public void Execute(ref Position position, ref NavAgent agent)
        {
            float distance = math.distance(agent.finalDestination, position.Value);
            float3 direction = math.normalize(agent.finalDestination - position.Value);
            float speed = 5;
            if(!(distance < 1) && agent.agentStatus == NavAgentStatus.Moving)
            {
                position.Value += direction * speed * dT;
            }
        }
    }


    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new NavAgentMovementJob
        {
            dT = Time.deltaTime,
        };
        return job.Schedule(this, inputDeps);
    }
}
