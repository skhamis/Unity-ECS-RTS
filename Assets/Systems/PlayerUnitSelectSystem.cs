using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Unity.Collections;

public class PlayerUnitSelectSystem : JobComponentSystem
{
    public class SelectBarrier : BarrierSystem { }
    [Inject] SelectBarrier barrier;

    struct PlayerUnitSelectJob : IJobProcessComponentDataWithEntity<PlayerInput, Position>
    {

        public EntityCommandBuffer.Concurrent commandBuffer;
        [ReadOnly] public ComponentDataFromEntity<PlayerUnitSelect> Selected;

        public void Execute
            (Entity entity, int i, [ReadOnly] ref PlayerInput input, [ReadOnly] ref Position pos)
        {
            if (input.LeftClick)
            {
                //If selected component exists on our unit, unselect before we recalc selected
                if(Selected.Exists(entity))
                {
                    commandBuffer.RemoveComponent<PlayerUnitSelect>(i, entity);
                }

                //Add select component to unit
                if (math.distance(input.MousePosition, pos.Value) <= 5)
                {
                    commandBuffer.AddComponent(i, entity, new PlayerUnitSelect());
                }
            }

        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new PlayerUnitSelectJob {
             commandBuffer = barrier.CreateCommandBuffer().ToConcurrent(),
             Selected = GetComponentDataFromEntity<PlayerUnitSelect>(),
        };
        return job.Schedule(this, inputDeps);
    }
}
