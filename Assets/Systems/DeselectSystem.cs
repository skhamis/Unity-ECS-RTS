using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

// ComponentSystems run on the main thread. Use these when you have to do work that cannot be called from a job.
public class DeselectSystem : ComponentSystem
{
    EntityQuery m_highlights;

    protected override void OnCreate()
    {
        m_highlights = GetEntityQuery(typeof(Highlight));
    }

    protected override void OnUpdate()
    {
        using (var highlights = m_highlights.ToEntityArray(Allocator.TempJob))
        {
            foreach (var highlight in highlights)
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