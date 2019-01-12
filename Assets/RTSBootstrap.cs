using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class RTSBootstrap {

    public static EntityArchetype PlayerUnitArchetype;
    private static MeshInstanceRenderer cubeRenderer;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize()
    {
        // This method creates archetypes for entities we will spawn frequently in this game.
        // Archetypes are optional but can speed up entity spawning substantially.
        var entityManager = World.Active.GetOrCreateManager<EntityManager>();


        PlayerUnitArchetype = entityManager.CreateArchetype(
            typeof(Position),
            typeof(Rotation),
            typeof(MoveSpeed),
            typeof(PlayerInput)
        );
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void InitializeWithScene()
    {
        cubeRenderer = GetLookFromPrototype("CubePrototype");

        NewGame();
    }

    public static void NewGame()
    {
        // Access the ECS entity manager
        var entityManager = World.Active.GetOrCreateManager<EntityManager>();

        for (int i = 0; i < 3; i++)
        {
            Entity playerUnit = entityManager.CreateEntity(PlayerUnitArchetype);
            entityManager.SetComponentData(playerUnit, new Position { Value = new float3(i * 5, 0.5f, 0) });
            entityManager.AddSharedComponentData(playerUnit, cubeRenderer);
        }
        
    }

    public static MeshInstanceRenderer GetLookFromPrototype(string protoName)
    {
        var proto = GameObject.Find(protoName);
        var result = proto.GetComponent<MeshInstanceRendererComponent>().Value;
        //Object.Destroy(proto);
        return result;
    }
}