using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics.Stateful;
using Unity.Rendering;

using UnityEngine;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[BurstCompile]
public partial class AsteroidDamageSystem : SystemBase
{
	private EndFixedStepSimulationEntityCommandBufferSystem ecbSystem;
	protected override void OnCreate()
	{
		ecbSystem = World.GetOrCreateSystem<EndFixedStepSimulationEntityCommandBufferSystem>();
	}

	protected override void OnUpdate()
	{
		var points = new NativeArray<uint>(1, Allocator.TempJob);
		points[0] = 0;
		var ecb = ecbSystem.CreateCommandBuffer().AsParallelWriter();
		Entities
			.WithName("DestroyAsteroidsWithBulletOnTrigger")
			.WithAll<AsteroidTag>()
			.WithNone<DeathTag>()
			.ForEach((Entity entity, int nativeThreadIndex, ref DynamicBuffer<TriggerBuffer> triggerBuffer, ref Health health) =>
			{
				for (int i = 0; i < triggerBuffer.Length; i++)
				{
					var otherEntity = triggerBuffer[i].entity;
					if (!HasComponent<BulletTag>(otherEntity)) return;
					var damage = GetComponent<Damage>(otherEntity);
					health.value -= damage.damageValue;
					if (health.value <= 0)
					{
						ecb.AddComponent(nativeThreadIndex, entity, new DeathTag { timer = 0 });
						points[0] += GetComponent<OnKill>(entity).points;
					}
					ecb.AddComponent(nativeThreadIndex, otherEntity, new DeathTag { timer = 0 });
				}
			}).Schedule();
		CompleteDependency();

		Entities
			.WithName("DestroyAsteroidsWithInvincibleOnTrigger")
			.WithAll<AsteroidTag>()
			.ForEach((Entity entity, int nativeThreadIndex, ref DynamicBuffer<CollisionBuffer> collisionBuffer) =>
			{
				for (int i = 0; i < collisionBuffer.Length; i++)
				{
					var otherEntity = collisionBuffer[i].entity;
					if (!HasComponent<PlayerTag>(otherEntity) || HasComponent<AsteroidTag>(otherEntity)) return;
					if (GetComponent<Health>(otherEntity).invincibleTimer <= 0) return;
					ecb.AddComponent(nativeThreadIndex, entity, new DeathTag { timer = 0 });
					points[0] += GetComponent<OnKill>(entity).points;
				}
			})
			.Schedule();
		CompleteDependency();

		ecbSystem.AddJobHandleForProducer(Dependency);
		GameManager.instance.AddPoints(points[0]);
		points.Dispose();
	}
}
