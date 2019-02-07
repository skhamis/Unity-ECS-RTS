using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;

public class AABBCollisionSystem : JobComponentSystem
{

    protected override void OnCreateManager()
    {
        base.OnCreateManager();

    }

    public struct AABBCollisionJob : IJob
    {

        public void Execute()
        {
            //TODO: Implement some rudimentary collisions
        }
    }


    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new AABBCollisionJob();
        return job.Schedule(inputDeps);
    }
}
