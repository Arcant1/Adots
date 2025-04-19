using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
public partial struct PowerPillSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        // Se requiere el singleton del EndSimulationEntityCommandBufferSystem.
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
    }

    public void OnUpdate(ref SystemState state)
    {
        // Intenta obtener el singleton del EndSimulationEntityCommandBufferSystem.
        if (!SystemAPI.TryGetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>(out var endSimECB))
        {
            return;
        }

        // Crea el EntityCommandBuffer a partir del WorldUnmanaged.
        EntityCommandBuffer ecb = endSimECB.CreateCommandBuffer(state.WorldUnmanaged);
        float dt = SystemAPI.Time.DeltaTime;

        // --------------------------------------------------------------------
        // ForEach: ApplyPowerPillEffectsAndDestroyPowerPill
        // Recorre cada jugador (con PlayerTag y sin PowerPill) que tenga un DynamicBuffer<TriggerBuffer>,
        // junto con componentes modificables (Health y Weapon) y la entidad.
        foreach ((
            DynamicBuffer<TriggerBuffer> triggerBuffer,
            RefRW<Health> health,
            RefRW<Weapon> weapon,
            Entity playerEntity) in SystemAPI.Query<DynamicBuffer<TriggerBuffer>, RefRW<Health>, RefRW<Weapon>>()
                                                     .WithAll<PlayerTag>()
                                                     .WithNone<PowerPill>()
                                                     .WithEntityAccess())
        {
            for (int i = 0; i < triggerBuffer.Length; i++)
            {
                Entity triggerEntity = triggerBuffer[i].entity;
                if (SystemAPI.HasComponent<PowerPill>(triggerEntity))
                {
                    // Accede al PowerPill del trigger de solo lectura.
                    RefRO<PowerPill> pill = SystemAPI.GetComponentRO<PowerPill>(triggerEntity);
                    // Agrega el componente PowerPill al jugador, copiando los datos.
                    ecb.AddComponent(playerEntity, pill.ValueRO);
                    // Destruye la entidad del trigger.
                    ecb.DestroyEntity(triggerEntity);
                }
            }
        }

        foreach ((
            RefRW<PowerPill> pill,
            Entity entity) in SystemAPI.Query<RefRW<PowerPill>>()
                                         .WithAll<PlayerTag>()
                                         .WithEntityAccess())
        {
            pill.ValueRW.pillTimer -= dt;
            if (pill.ValueRW.pillTimer <= 0)
            {
                ecb.RemoveComponent<PowerPill>(entity);
            }
        }

        foreach ((
            RefRO<ShieldAnimationData> shieldAnimationData,
            Entity entity) in SystemAPI.Query<RefRO<ShieldAnimationData>>()
                                         .WithAll<PlayerTag>()
                                         .WithEntityAccess())
        {
            bool hasPowerPill = SystemAPI.HasComponent<PowerPill>(entity);
            state.EntityManager.SetEnabled(shieldAnimationData.ValueRO.shield, hasPowerPill);
        }

    }

    public void OnDestroy(ref SystemState state)
    {
    }
}
