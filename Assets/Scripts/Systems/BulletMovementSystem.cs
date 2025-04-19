using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct BulletMovementSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.TryGetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>(out var endSimECB))
        {
            return;
        }

        EntityCommandBuffer ecb = endSimECB.CreateCommandBuffer(state.WorldUnmanaged);
        
        float deltaTime = SystemAPI.Time.DeltaTime;
        
        // Update bullets lifetime
        foreach (var (bulletProperties, entity) in 
            SystemAPI.Query<RefRW<BulletProperties>>()
                .WithAll<BulletTag>()
                .WithEntityAccess())
        {
            // Update lifetime
            bulletProperties.ValueRW.CurrentTime += deltaTime;
            
            // If bullet has exceeded its lifetime, mark it for destruction
            if (bulletProperties.ValueRO.CurrentTime >= bulletProperties.ValueRO.Lifetime)
            {
                ecb.DestroyEntity(entity);
                continue;
            }
        }
        
        // Update bullet positions based on physics velocity
        foreach (var (transform, velocity, entity) in 
            SystemAPI.Query<RefRW<LocalTransform>, RefRO<PhysicsVelocity>>()
                .WithAll<BulletTag>()
                .WithEntityAccess())
        {
            // Update position based on velocity
            transform.ValueRW.Position += velocity.ValueRO.Linear * deltaTime;
        }
    }
} 