using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

// ComponentSystems run on the main thread. Use these when you have to do work that cannot be called from a job.
public class HighlightSystem : ComponentSystem
{
    ComponentGroup m_highlights;
    ComponentGroup m_selectedUnits;

    protected override void OnCreateManager()
    {
        m_highlights = GetComponentGroup(typeof(Highlight));
        m_selectedUnits = GetComponentGroup(
            typeof(PlayerUnitSelect),
            typeof(SelectingComponent));
    }

    protected override void OnUpdate()
    {
        //Get all selected units
        using (var selectedUnits = m_selectedUnits.ToEntityArray(Allocator.TempJob))
        using (var highlights = m_highlights.ToEntityArray(Allocator.TempJob))
        {
            foreach (var selectedUnit in selectedUnits)
            {
                var highlight = highlights[0];
                var prefab = EntityManager.GetSharedComponentData<Highlight>(highlight).prefab;
                var entity = EntityManager.Instantiate(prefab);

                var attach = EntityManager.CreateEntity(typeof(Attach));
                
                EntityManager.SetComponentData(entity, new Position { Value = new float3(0,-0.5f,0) });
                EntityManager.SetComponentData(entity, new Scale { Value = new float3(2, 0.1f, 2) });
                EntityManager.SetComponentData(attach, new Attach { Parent = selectedUnit, Child = entity });

                //Remove the "tagging" component so we only attach it once
                EntityManager.RemoveComponent(selectedUnit, typeof(SelectingComponent));
            }
        }
    }
}