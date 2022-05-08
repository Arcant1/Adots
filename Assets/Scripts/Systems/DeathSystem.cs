using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public partial class DeathSystem : SystemBase
{
	private EndSimulationEntityCommandBufferSystem ecbSystem;

	protected override void OnCreate()
	{
		ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
	}
	protected override void OnUpdate()
    {
		var ecb = ecbSystem.CreateCommandBuffer().AsParallelWriter();
		var dt = Time.DeltaTime;
		Entities
			.WithAll<DeathTag>()
			.ForEach((Entity entity, int nativeThreadIndex, ref DeathTag killInfo) =>
			{
				killInfo.timer -= dt;
				if(killInfo.timer <0f)
				{
					ecb.DestroyEntity(nativeThreadIndex, entity);
				}
			})
			.WithBurst()
			.ScheduleParallel();
		ecbSystem.AddJobHandleForProducer(Dependency);
	}
}
