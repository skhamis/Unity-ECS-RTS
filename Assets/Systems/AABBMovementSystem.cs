using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;

public class AABBMovementSystem : JobComponentSystem
{
    public struct AABBMovmentJob : IJobProcessComponentData<AABB, Position>
    {
        //Keep our box collider in sync with the position of the player
        public void Execute(ref AABB aabb, ref Position pos)
        {
            aabb.max = pos.Value + 0.5f;
            aabb.min = pos.Value - 0.5f;
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new AABBMovmentJob();
        return job.Schedule(this, inputDeps);
    }
}
