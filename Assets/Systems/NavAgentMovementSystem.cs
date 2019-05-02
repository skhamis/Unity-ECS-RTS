using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;

public class NavAgentMovementSystem : JobComponentSystem
{
    public struct NavAgentMovementJob : IJobForEach<Translation, UnitNavAgent>
    {
        public float dT;

        public void Execute(ref Translation position, [ReadOnly] ref UnitNavAgent agent)
        {
            float distance = math.distance(agent.finalDestination, position.Value);
            float3 direction = math.normalize(agent.finalDestination - position.Value);
            float speed = 5;
            if(!(distance < 0.5) && agent.agentStatus == NavAgentStatus.Moving)
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
