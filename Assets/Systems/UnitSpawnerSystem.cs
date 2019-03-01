using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

// ComponentSystems run on the main thread. Use these when you have to do work that cannot be called from a job.
public class UnitSpawnerSystem : ComponentSystem
{
    ComponentGroup m_Spawners;

    protected override void OnCreateManager()
    {
        m_Spawners = GetComponentGroup(typeof(UnitSpawner), typeof(Position));
    }

    protected override void OnUpdate()
    {
        // Get all the spawners in the scene.
        using (var spawners = m_Spawners.ToEntityArray(Allocator.TempJob))
        {
            foreach (var spawner in spawners)
            {
                for (int i = 0; i < 15; i++)
                {
                    for (int j = 0; j < 15; j++)
                    {
                        // Create an entity from the prefab set on the spawner component.
                        var prefab = EntityManager.GetSharedComponentData<UnitSpawner>(spawner).prefab;
                        var entity = EntityManager.Instantiate(prefab);

                        // Copy the position of the spawner to the new entity.
                        var position = EntityManager.GetComponentData<Position>(spawner);
                        position.Value.x = position.Value.x + 2 * i;
                        position.Value.z = position.Value.z + 2 * j;

                        EntityManager.SetComponentData(entity, position);

                        var aabb = new AABB
                        {
                            //0.5f will represent halfwidth for now
                            max = position.Value + 0.5f,
                            min = position.Value - 0.5f,

                        };
                        EntityManager.SetComponentData(entity, aabb);
                    }
                }

                // Destroy the spawner so this system only runs once.
                EntityManager.DestroyEntity(spawner);
            }
        }
    }
}