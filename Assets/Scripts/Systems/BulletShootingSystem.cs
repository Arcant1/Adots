using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(ThrustSystem))]
public partial struct BulletShootingSystem : ISystem
{
    private EntityQuery playerQuery;
    private float lastShotTime;
    private float shotCooldown;

    public void OnCreate(ref SystemState state)
    {
        // Query to find the player entity
        playerQuery = state.GetEntityQuery(
            ComponentType.ReadOnly<PlayerTag>(),
            ComponentType.ReadOnly<LocalTransform>()
        );

        // Set initial shot cooldown time (0.25 seconds between shots)
        shotCooldown = 0.25f;
        lastShotTime = 0f;
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        // Check if there's a bullet prefab available
        if (!SystemAPI.HasSingleton<EntitiesReferences>())
        {
            Debug.LogWarning("BulletShootingSystem: No EntitiesReferences singleton found");
            return;
        }
        
        // Check if game settings are available
        if (!SystemAPI.HasSingleton<GameSettings>())
        {
            Debug.LogWarning("BulletShootingSystem: No GameSettings singleton found");
            return;
        }
        
        // Get game settings
        var gameSettings = SystemAPI.GetSingleton<GameSettings>();
        float defaultBulletSpeed = gameSettings.bulletVelocity;
        float defaultBulletLifetime = gameSettings.bulletLifetime;
        
        var references = SystemAPI.GetSingleton<EntitiesReferences>();
        Entity bulletPrefab = references.bulletPrefabEntity;
        
        if (bulletPrefab == Entity.Null)
        {
            Debug.LogWarning("BulletShootingSystem: No bullet prefab entity found");
            return;
        }
        
        // Check for shooting input with cooldown
        float currentTime = (float)SystemAPI.Time.ElapsedTime;
        bool canShoot = currentTime - lastShotTime >= shotCooldown;
        bool shootPressed = Input.GetKey(KeyCode.Space);

        if (!shootPressed || !canShoot)
        {
            return;
        }
        
        // Get the player entity
        if (playerQuery.IsEmpty)
        {
            Debug.LogWarning("BulletShootingSystem: No player entity found");
            return;
        }
        
        var players = playerQuery.ToEntityArray(Allocator.Temp);
        
        if (players.Length > 0)
        {
            Entity playerEntity = players[0];
            
            // Get player position and rotation
            if (state.EntityManager.HasComponent<LocalTransform>(playerEntity))
            {
                var playerTransform = state.EntityManager.GetComponentData<LocalTransform>(playerEntity);
                
                // Determine spawn position (in front of the player)
                float spawnDistance = 0.5f;  // Distance in front of player
                
                // Calculate forward direction based on player rotation
                float3 forwardDirection = math.mul(playerTransform.Rotation, new float3(0, 1, 0));
                float3 spawnPosition = playerTransform.Position + forwardDirection * spawnDistance;
                
                // Apply any additional offset if available
                if (state.EntityManager.HasComponent<BulletSpawnOffset>(playerEntity))
                {
                    var offset = state.EntityManager.GetComponentData<BulletSpawnOffset>(playerEntity).Value;
                    spawnPosition += offset;
                }
                
                // Spawn the bullet
                Entity bulletEntity = state.EntityManager.Instantiate(bulletPrefab);
                
                // Get prefab transform to preserve original scale
                float prefabScale = 1.0f;
                if (state.EntityManager.HasComponent<LocalTransform>(bulletPrefab))
                {
                    prefabScale = state.EntityManager.GetComponentData<LocalTransform>(bulletPrefab).Scale;
                }
                
                // Set bullet position and rotation, preserving prefab scale
                state.EntityManager.SetComponentData(bulletEntity, new LocalTransform
                {
                    Position = spawnPosition,
                    Rotation = playerTransform.Rotation,
                    Scale = prefabScale
                });
                
                // Ensure bullet has required components
                if (!state.EntityManager.HasComponent<PhysicsVelocity>(bulletEntity))
                {
                    state.EntityManager.AddComponent<PhysicsVelocity>(bulletEntity);
                }
                
                if (!state.EntityManager.HasComponent<BulletTag>(bulletEntity))
                {
                    state.EntityManager.AddComponent<BulletTag>(bulletEntity);
                }
                
                // Add bullet properties if missing
                if (!state.EntityManager.HasComponent<BulletProperties>(bulletEntity))
                {
                    state.EntityManager.AddComponent<BulletProperties>(bulletEntity);
                    state.EntityManager.SetComponentData(bulletEntity, new BulletProperties
                    {
                        Speed = defaultBulletSpeed,
                        Lifetime = defaultBulletLifetime,
                        CurrentTime = 0f
                    });
                }
                
                // Get bullet properties
                float bulletSpeed = defaultBulletSpeed; // Use game settings default
                if (state.EntityManager.HasComponent<BulletProperties>(bulletEntity))
                {
                    bulletSpeed = state.EntityManager.GetComponentData<BulletProperties>(bulletEntity).Speed;
                }
                
                // Add physics velocity for movement
                if (state.EntityManager.HasComponent<PhysicsVelocity>(bulletEntity))
                {
                    // Calculate bullet velocity based on player direction
                    float3 bulletVelocity = forwardDirection * bulletSpeed;
                    
                    // Add player's velocity to bullet velocity for more realistic physics
                    if (state.EntityManager.HasComponent<PhysicsVelocity>(playerEntity))
                    {
                        float3 playerVelocity = state.EntityManager.GetComponentData<PhysicsVelocity>(playerEntity).Linear;
                        bulletVelocity += playerVelocity;
                        
                        Debug.Log($"Adding player velocity: {playerVelocity} to bullet");
                    }
                    
                    // Set initial velocity in the direction the player is facing plus player's velocity
                    state.EntityManager.SetComponentData(bulletEntity, new PhysicsVelocity 
                    { 
                        Linear = bulletVelocity,
                        Angular = float3.zero
                    });
                }
                
                // Update last shot time
                lastShotTime = currentTime;
                
                Debug.Log($"Bullet fired at position {spawnPosition} with speed {bulletSpeed}");
            }
        }
        
        // Always dispose of NativeArrays
        players.Dispose();
    }
} 