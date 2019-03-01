using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Unity.Collections;

public class PlayerUnitSelectSystem : JobComponentSystem
{
    [UpdateAfter(typeof(PlayerUnitSelectSystem))]
    public class SelectBarrier : BarrierSystem { }

    [Inject] private SelectBarrier barrier;

    struct PlayerUnitSelectJob : IJobProcessComponentDataWithEntity<PlayerInput, AABB>
    {

        public EntityCommandBuffer.Concurrent commandBuffer;
        [ReadOnly] public ComponentDataFromEntity<PlayerUnitSelect> Selected;
        public Ray ray;

        public void Execute
            (Entity entity, int i, [ReadOnly] ref PlayerInput input, [ReadOnly] ref AABB aabb)
        {
            if (input.LeftClick)
            {
                //If selected component exists on our unit, unselect before we recalc selected
                if(Selected.Exists(entity))
                {
                    commandBuffer.RemoveComponent<PlayerUnitSelect>(i, entity);
                    commandBuffer.AddComponent(i, entity, new Deselecting());
                }

                //Add select component to unit
                if(RTSPhysics.Intersect(aabb, ray))
                {
                    commandBuffer.AddComponent(i, entity, new PlayerUnitSelect());
                    commandBuffer.AddComponent(i, entity, new Selecting());
                }
            }

        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new PlayerUnitSelectJob {
             commandBuffer = barrier.CreateCommandBuffer().ToConcurrent(),
             Selected = GetComponentDataFromEntity<PlayerUnitSelect>(),
             ray = Camera.main.ScreenPointToRay(Input.mousePosition),
        };
        return job.Schedule(this, inputDeps);
    }
}
