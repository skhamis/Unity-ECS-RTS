//using Unity.Collections;
//using Unity.Entities;
//using Unity.Transforms;
//using Unity.Mathematics;
//using UnityEngine;

//// ComponentSystems run on the main thread. Use these when you have to do work that cannot be called from a job.
//public class UnitSpawnerSystem : ComponentSystem
//{
//    EntityQuery m_Spawners;

//    protected override void OnCreate()
//    {
//        m_Spawners = GetEntityQuery(typeof(UnitSpawner), typeof(Translation));
//    }

//    protected override void OnUpdate()
//    {
//        // Get all the spawners in the scene.
//        using (var spawners = m_Spawners.ToEntityArray(Allocator.TempJob))
//        {
//            foreach (var spawner in spawners)
//            {
//                for (int i = 0; i < 15; i++)
//                {
//                    for (int j = 0; j < 15; j++)
//                    {
//                        // Create an entity from the prefab set on the spawner component.
//                        var prefab = EntityManager.GetSharedComponentData<UnitSpawner>(spawner).prefab;
//                        var entity = EntityManager.Instantiate(prefab);

//                        // Copy the position of the spawner to the new entity.
//                        Translation position = EntityManager.GetComponentData<Translation>(spawner);
//                        position.Value.x = position.Value.x + 2 * i;
//                        position.Value.z = position.Value.z + 2 * j;

//                        EntityManager.SetComponentData(entity, position);

//                        var aabb = new AABB
//                        {
//                            //0.5f will represent halfwidth for now
//                            max = position.Value + 0.5f,
//                            min = position.Value - 0.5f,

//                        };
//                        EntityManager.SetComponentData(entity, aabb);
//                    }
//                }

//                // Destroy the spawner so this system only runs once.
//                EntityManager.DestroyEntity(spawner);
//            }
//        }
//    }
//}
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

// JobComponentSystems can run on worker threads.
// However, creating and removing Entities can only be done on the main thread to prevent race conditions.
// The system uses an EntityCommandBuffer to defer tasks that can't be done inside the Job.
[UpdateInGroup(typeof(SimulationSystemGroup))]
public class UnitSpawnerSystem : JobComponentSystem
{
    // BeginInitializationEntityCommandBufferSystem is used to create a command buffer which will then be played back
    // when that barrier system executes.
    // Though the instantiation command is recorded in the SpawnJob, it's not actually processed (or "played back")
    // until the corresponding EntityCommandBufferSystem is updated. To ensure that the transform system has a chance
    // to run on the newly-spawned entities before they're rendered for the first time, the HelloSpawnerSystem
    // will use the BeginSimulationEntityCommandBufferSystem to play back its commands. This introduces a one-frame lag
    // between recording the commands and instantiating the entities, but in practice this is usually not noticeable.
    BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;

    protected override void OnCreate()
    {
        // Cache the BeginInitializationEntityCommandBufferSystem in a field, so we don't have to create it every frame
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    struct SpawnJob : IJobForEachWithEntity<UnitSpawner, LocalToWorld>
    {
        public EntityCommandBuffer CommandBuffer;

        public void Execute(Entity entity, int index, [ReadOnly] ref UnitSpawner spawner,
            [ReadOnly] ref LocalToWorld location)
        {
            for (int x = 0; x < spawner.CountX; x++)
            {
                for (int y = 0; y < spawner.CountY; y++)
                {
                    var instance = CommandBuffer.Instantiate(spawner.Prefab);

                     var position = math.transform(location.Value,
                         new float3(x * 2, 0, y * 2));

                    //TODO: Eventually switch to the new Unity.Physics AABB 
                    var aabb = new AABB
                    {
                        //0.5f will represent halfwidth for now
                        max = position + 0.5f,
                        min = position - 0.5f,

                    };

                    


                    CommandBuffer.SetComponent(instance, new Translation { Value = position });

                    //Weirdly have to do it in code now
                    CommandBuffer.AddComponent(instance, aabb);
                    CommandBuffer.AddComponent(instance, new PlayerInput());
                    CommandBuffer.AddComponent(instance, new UnitNavAgent());


                }
            }

            CommandBuffer.DestroyEntity(entity);
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        //Instead of performing structural changes directly, a Job can add a command to an EntityCommandBuffer to perform such changes on the main thread after the Job has finished.
        //Command buffers allow you to perform any, potentially costly, calculations on a worker thread, while queuing up the actual insertions and deletions for later.

        // Schedule the job that will add Instantiate commands to the EntityCommandBuffer.
        var job = new SpawnJob
        {
            CommandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer()
        }.ScheduleSingle(this, inputDeps);


        // SpawnJob runs in parallel with no sync point until the barrier system executes.
        // When the barrier system executes we want to complete the SpawnJob and then play back the commands (Creating the entities and placing them).
        // We need to tell the barrier system which job it needs to complete before it can play back the commands.
        m_EntityCommandBufferSystem.AddJobHandleForProducer(job);

        return job;
    }
}

