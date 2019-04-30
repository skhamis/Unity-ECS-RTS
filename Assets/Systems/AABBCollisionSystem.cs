using UnityEngine;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;

public class AABBCollisionSystem : JobComponentSystem
{
    [BurstCompile]
    public struct AABBCollisionJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<AABB> Colliders;

        public void Execute(int i)
        {
            for (int j = i + 1; j < Colliders.Length; j++)
            {
                if (RTSPhysics.Intersect(Colliders[i], Colliders[j]))
                {
                    //Debug.Log("Collision Detected");
                }
            }
        }
    }

    EntityQuery m_AABBGroup;

    protected override void OnCreateManager()
    {
        var query = new EntityQueryDesc
        {
            All = new ComponentType[] { typeof(AABB) }
        };
        m_AABBGroup = GetEntityQuery(query);
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var colliders = m_AABBGroup.ToComponentDataArray<AABB>(Allocator.TempJob);
        var aabbCollisionJob = new AABBCollisionJob
        {
            Colliders = colliders,
        };
        var collisionJobHandle = aabbCollisionJob.Schedule(colliders.Length, 32);
        collisionJobHandle.Complete();

        colliders.Dispose();
        return collisionJobHandle;
    }
}