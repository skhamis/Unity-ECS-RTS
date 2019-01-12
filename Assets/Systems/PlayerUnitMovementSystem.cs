
using System.Drawing;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class PlayerUnitMovementSystem : JobComponentSystem
{
    [BurstCompile]
    struct PlayerUnitMovementJob : IJobProcessComponentData<PlayerInput, Position, Selected>
    {


        public void Execute(ref PlayerInput input, ref Position pos, ref Selected selected)
        {
            if(input.RightClick)
            {
                pos.Value = input.MousePosition;
            }
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new PlayerUnitMovementJob();
        return job.Schedule(this, inputDeps);
    }
}
