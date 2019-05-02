using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Unity.Collections;

public class PlayerUnitSelectSystem : JobComponentSystem
{
    // EndSimulationBarrier is used to create a command buffer 
    // which will then be played back when that barrier system executes.
    EntityCommandBufferSystem m_EntityCommandBufferSystem;

    protected override void OnCreate()
    {
        // Cache the EndSimulationBarrier in a field, so we don't have to create it every frame
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<EntityCommandBufferSystem>();
    }

    struct PlayerUnitSelectJob : IJobForEachWithEntity<PlayerInput, AABB>
    {

        [ReadOnly] public EntityCommandBuffer CommandBuffer;

        [ReadOnly] public ComponentDataFromEntity<PlayerUnitSelect> Selected;
        public Ray ray;

        public void Execute
            (Entity entity, int index, [ReadOnly] ref PlayerInput input, [ReadOnly] ref AABB aabb)
        {
            if (input.LeftClick)
            {
                //If selected component exists on our unit, unselect before we recalc selected
                if(Selected.Exists(entity))
                {
                    CommandBuffer.RemoveComponent<PlayerUnitSelect>(entity);
                    CommandBuffer.AddComponent(entity, new Deselecting());
                }

                //Add select component to unit
                if(RTSPhysics.Intersect(aabb, ray))
                {
                    CommandBuffer.AddComponent(entity, new PlayerUnitSelect());
                    CommandBuffer.AddComponent(entity, new Selecting());
                } else
                {

                }
            }

        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new PlayerUnitSelectJob {
             CommandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer(),
             Selected = GetComponentDataFromEntity<PlayerUnitSelect>(),
             ray = Camera.main.ScreenPointToRay(Input.mousePosition),
        }.Schedule(this, inputDeps);

        // SpawnJob runs in parallel with no sync point until the barrier system executes.
        // When the barrier system executes we want to complete the SpawnJob and then play back the commands (Creating the entities and placing them).
        // We need to tell the barrier system which job it needs to complete before it can play back the commands.
        m_EntityCommandBufferSystem.AddJobHandleForProducer(job);

        return job;
    }
}
