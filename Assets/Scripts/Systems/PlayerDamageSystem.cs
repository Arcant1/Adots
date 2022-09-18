using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[BurstCompile]
public partial class PlayerDamageSystem : SystemBase {
	private EndFixedStepSimulationEntityCommandBufferSystem ecbSystem;
	protected override void OnCreate() {
		ecbSystem = World.GetOrCreateSystem<EndFixedStepSimulationEntityCommandBufferSystem>();
	}
	protected override void OnUpdate() {
		float dt = Time.DeltaTime;
		EntityCommandBuffer ecb = ecbSystem.CreateCommandBuffer();
		EntityCommandBuffer.ParallelWriter ecbp = ecb.AsParallelWriter();

		Entities
			.WithAll<PlayerTag>()
			.WithNone<DeathTag>()
			.WithName("ApplyDamageToHealthOnTrigger")
			.ForEach((Entity entity, int nativeThreadIndex, int entityInQueryIndex, DynamicBuffer<TriggerBuffer> triggerBuffer, ref Health health) =>           // The player has a trigger buffer
			{
				for (int i = 0; i < triggerBuffer.Length; i++) {
					Entity otherEntity = triggerBuffer[i].entity;
					if (HasComponent<DeathTag>(otherEntity)) {
						return;
					}
					if (HasComponent<PowerPill>(entity)) return;
					if (HasComponent<Damage>(triggerBuffer[i].entity)) {
						health.value -= GetComponent<Damage>(triggerBuffer[i].entity).damageValue;
						if (health.value <= 0) {
							//AudioManager.instance.PlaySfxRequest(health.deathSfx.ToString());
							ecbp.AddComponent(nativeThreadIndex, entity, new DeathTag { timer = health.killTimer });
						}
						ecbp.AddComponent(nativeThreadIndex, otherEntity, new DeathTag { timer = 0 });
					}
				}
			})
			.ScheduleParallel();
		CompleteDependency();

		Entities
			.WithAll<PlayerTag>()
			.WithNone<DeathTag>()
			.WithName("ApplyDamageToHealthOnCollision")
			.ForEach((Entity entity, int entityInQueryIndex, DynamicBuffer<CollisionBuffer> collisionBuffer, ref Health health) =>           // The player has a trigger buffer
			{
				for (int i = 0; i < collisionBuffer.Length; i++) {
					Entity otherEntity = collisionBuffer[i].entity;
					if (!HasComponent<UFObulletTag>(otherEntity)) {
						return;
					}
					if (HasComponent<PowerPill>(entity)) return;

					if (HasComponent<Damage>(collisionBuffer[i].entity)) {
						health.value -= GetComponent<Damage>(collisionBuffer[i].entity).damageValue;
						//AudioManager.instance.PlaySfxRequest(health.damageSfx.ToString());
					}
				}
			})
			.ScheduleParallel();
		CompleteDependency();

		ecbSystem.AddJobHandleForProducer(Dependency);

	}
}
