using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class PlayerInputSystem : JobComponentSystem
{
    [BurstCompile]
    struct PlayerInputJob : IJobProcessComponentData<PlayerInput>
    {
        public BlittableBool leftClick;
        public BlittableBool rightClick;
        public float3 mousePos;
        
        public void Execute(ref PlayerInput input)
        {
            input.LeftClick = leftClick;
            input.RightClick = rightClick;
            input.MousePosition = mousePos;
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float3 mousePos = Input.mousePosition;
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            
            mousePos = new float3(hit.point.x, 0, hit.point.z);
        }

        var job = new PlayerInputJob
        {
            leftClick = Input.GetMouseButtonDown(0),
            rightClick = Input.GetMouseButtonDown(1),
            mousePos = mousePos,

        };
        return job.Schedule(this, inputDeps);
    }
}
