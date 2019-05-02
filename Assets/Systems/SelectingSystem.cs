using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

// ComponentSystems run on the main thread. Use these when you have to do work that cannot be called from a job.
public class SelectingSystem : ComponentSystem
{
    EntityQuery m_highlights;
    EntityQuery m_selectedUnits;

    protected override void OnCreate()
    {
        m_highlights = GetEntityQuery(typeof(HighlightSpawner));
        m_selectedUnits = GetEntityQuery(typeof(Selecting));
    }

    protected override void OnUpdate()
    {
        //Get all selected units
        using (var selectedUnits = m_selectedUnits.ToEntityArray(Allocator.TempJob))
        using (var highlights = m_highlights.ToEntityArray(Allocator.TempJob))
        {
            //TODO Find a better way to spawn highlight prefabs 
            // Works right now since we know there will be at least one HighlightSpawner
            var highlight = highlights[0];
            var prefab = EntityManager.GetComponentData<HighlightSpawner>(highlight).Prefab;

            foreach (var selectedUnit in selectedUnits)
            {
                //Remove the component from the unit so this system doesn't continually run
                EntityManager.RemoveComponent<Selecting>(selectedUnit);

                //Get our prefab from our spawner and set Translation (to produce a LocalToWorld)
                var entity = EntityManager.Instantiate(prefab);
                EntityManager.AddComponent(entity, typeof(Highlight));

                //For a child to sucessfully have a parent it needs:
                // 1. LocalToWorld (either a translation or rotation)
                // 2. LocalToParent
                // 3. Parent
                EntityManager.SetComponentData(entity, new Translation { Value = new float3(0, -0.5f, 0) });
                var localParent = EntityManager.GetComponentData<LocalToWorld>(selectedUnit).Value;
                EntityManager.AddComponentData(entity, new LocalToParent { Value = localParent });
                EntityManager.AddComponentData(entity, new Parent { Value = selectedUnit });
            }
        }
    }
}