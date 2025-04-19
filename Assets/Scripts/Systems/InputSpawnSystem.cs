using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial class InputSpawnSystem : SystemBase
{
    #region Protected Methods

    protected override void OnCreate()
    {
        RequireForUpdate<GameSettingsTag>();
        RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();

        // Get a reference to player and bullet entities
        playerQuery = GetEntityQuery(ComponentType.ReadOnly<PlayerTag>());
        bulletQuery = GetEntityQuery(ComponentType.ReadOnly<BulletTag>());
    }

    protected override void OnUpdate()
    {
        if (!SystemAPI.HasSingleton<EndSimulationEntityCommandBufferSystem.Singleton>())
        {
            return;
        }

        var endSimECB = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        EntityCommandBuffer ecb = endSimECB.CreateCommandBuffer(World.Unmanaged);

        // Make sure GameSettings is available
        if (!SystemAPI.HasSingleton<GameSettings>())
        {
            return;
        }

        GameSettings settings = SystemAPI.GetSingleton<GameSettings>();
        bool shootPressed = Input.GetKey(KeyCode.Space);

        // Just check if we have any player entities
        bool playerExists = !playerQuery.IsEmpty;

        // Make sure EntitiesReferences singleton is available
        if (!SystemAPI.HasSingleton<EntitiesReferences>())
        {
            return;
        }

        _playerPrefab = SystemAPI.GetSingleton<EntitiesReferences>().scoutPrefabEntity;

        // Spawn player if needed
        if (shootPressed && !playerExists && _playerPrefab != Entity.Null)
        {
            Entity playerEntity = EntityManager.Instantiate(_playerPrefab);

            // Set position at origin
            EntityManager.SetComponentData(playerEntity, new LocalTransform
            {
                Position = new float3(0, 0, 0),
                Rotation = quaternion.identity,
                Scale = 1f
            });

            // Make sure it has physics components
            if (!EntityManager.HasComponent<PhysicsVelocity>(playerEntity))
            {
                EntityManager.AddComponent<PhysicsVelocity>(playerEntity);
            }

            if (!EntityManager.HasComponent<PhysicsMass>(playerEntity))
            {
                EntityManager.AddComponent<PhysicsMass>(playerEntity);

                // Set a reasonable mass
                var mass = new PhysicsMass
                {
                    InverseInertia = new float3(1f),
                    InverseMass = 0.1f, // Higher inverse mass (lower actual mass)
                    Transform = new RigidTransform(quaternion.identity, float3.zero),
                    CenterOfMass = float3.zero
                };
                EntityManager.SetComponentData(playerEntity, mass);
            }

            // Ensure it has a PhysicsCollider component
            if (!EntityManager.HasComponent<PhysicsCollider>(playerEntity))
            {
                Debug.LogWarning("Player missing PhysicsCollider - add a PhysicsColliderAuthoring component to your player prefab");
            }

            Debug.Log("Player spawned at origin");
        }

        // Update game manager with bullet count (if needed)
        if (GameManager.instance != null)
        {
            int bulletCount = bulletQuery.IsEmpty ? 0 : bulletQuery.CalculateEntityCount();
            GameManager.instance.SetBulletsPerSecond(bulletCount);
        }
    }

    #endregion Protected Methods

    #region Private Fields

    private Entity _playerPrefab;
    private EntityQuery bulletQuery;
    private EntityQuery playerQuery;

    #endregion Private Fields
}