using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
[BurstCompile]
public partial class SpreadShotSystem : SystemBase
{
	private EndSimulationEntityCommandBufferSystem ecbSystem;
	protected override void OnCreate()
	{
		ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
	}
	protected override void OnUpdate()
	{
		var ecbp = ecbSystem.CreateCommandBuffer().AsParallelWriter();
		float dt = Time.DeltaTime;

		Entities
			.WithName("ApplySpreadShotEffect")
			.WithAll<PlayerTag>()
			.ForEach((Entity playerEntity, int entityInQueryIndex, DynamicBuffer<TriggerBuffer> triggerBuffer, ref Health health, ref Weapon weaponConfig) =>           // The player has a trigger buffer
			{
				for (int i = 0; i < triggerBuffer.Length; i++)
				{
					Entity otherEntity = triggerBuffer[i].entity;
					if (HasComponent<SpreadShot>(otherEntity))             // If it is a PowerPill
					{
						if (HasComponent<SpreadShot>(playerEntity))
						{
							weaponConfig.bulletQuantity += 1000;
						}
						else
						{
							ecbp.AddComponent(entityInQueryIndex, playerEntity, GetComponent<SpreadShot>(otherEntity));            // Add the component to the player entity
						}
						ecbp.DestroyEntity(entityInQueryIndex, otherEntity);                         // Mark for kill now
					}
				}
			}).ScheduleParallel();
		ecbSystem.AddJobHandleForProducer(Dependency);
	}

}
