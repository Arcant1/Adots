using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
[BurstCompile]
public partial class PowerPillSystem : SystemBase
{

	protected override void OnUpdate()
	{
		var ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
		var ecb = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>().CreateCommandBuffer().AsParallelWriter();
		float dt = Time.DeltaTime;

		Entities
			.WithName("ApplyPowerPillEffectsAndDestroyPowerPill")
			.WithAll<PlayerTag>()
			.ForEach((Entity playerEntity, int entityInQueryIndex, DynamicBuffer<TriggerBuffer> triggerBuffer, ref Health health, ref Weapon damage) =>           // The player has a trigger buffer
			{
				for (int i = 0; i < triggerBuffer.Length; i++)
				{
					Entity entity = triggerBuffer[i].entity;
					if (HasComponent<PowerPill>(entity))             // If it is a PowerPill
					{
						ecb.AddComponent(entityInQueryIndex, playerEntity, GetComponent<PowerPill>(entity));            // Add the component to the player entity
						health.invincibleTimer = GetComponent<PowerPill>(entity).pillTimer;
						ecb.DestroyEntity(entityInQueryIndex, entity);                         // Mark for kill now
					}
				}
			}).ScheduleParallel();

		Entities
			.WithName("ApplyPowerPillEffectInHealthAndRemovesOnTimeout")
			.WithAll<PlayerTag>()
			.ForEach((Entity e, int entityInQueryIndex, ref PowerPill pill) =>
			{
				pill.pillTimer -= dt;
				if (pill.pillTimer <= 0)
				{
					ecb.RemoveComponent<PowerPill>(entityInQueryIndex, e);
				}
			}).ScheduleParallel();

		ecbSystem.AddJobHandleForProducer(Dependency);
	}
}
