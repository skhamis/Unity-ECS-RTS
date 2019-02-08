using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

// ComponentSystems run on the main thread. Use these when you have to do work that cannot be called from a job.
public class DeselectSystem : ComponentSystem
{
    ComponentGroup m_highlights;

    protected override void OnCreateManager()
    {
        m_highlights = GetComponentGroup(typeof(Attached), typeof(Highlight));
    }

    protected override void OnUpdate()
    {
        using (var attachedHighlights = m_highlights.ToEntityArray(Allocator.TempJob))
        {
            foreach (var highlight in attachedHighlights)
            {
                var parent = EntityManager.GetComponentData<Parent>(highlight).Value;
                if (EntityManager.HasComponent<Deselecting>(parent))
                {
                    EntityManager.RemoveComponent<Deselecting>(parent);
                    EntityManager.DestroyEntity(highlight);
                }
            }
        }
    }
}