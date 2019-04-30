using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class PlayerInputSystem : JobComponentSystem
{
    [BurstCompile]
    struct PlayerInputJob : IJobForEach<PlayerInput>
    {
        public bool leftClick;
        public bool rightClick;
        
        public void Execute(ref PlayerInput input)
        {
            input.LeftClick = leftClick;
            input.RightClick = rightClick;
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {

        var job = new PlayerInputJob
        {
            leftClick = Input.GetMouseButtonDown(0),
            rightClick = Input.GetMouseButtonDown(1),
        };
        return job.Schedule(this, inputDeps);
    }
}
