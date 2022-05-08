using Unity.Entities;

public partial class LifeTimeSystem : SystemBase
{
	private BeginSimulationEntityCommandBufferSystem ecbSystem;

	protected override void OnCreate()
	{
		ecbSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
	}

	protected override void OnUpdate()
	{
		var ecb = ecbSystem.CreateCommandBuffer().AsParallelWriter();

		var deltaTime = Time.DeltaTime;

		Entities.ForEach((Entity entity, int nativeThreadIndex, ref LifeTime ttl) =>
		{
			ttl.time += deltaTime;
			if (ttl.time > ttl.maxTime)
				ecb.DestroyEntity(nativeThreadIndex, entity);

		}).ScheduleParallel();
		ecbSystem.AddJobHandleForProducer(Dependency);
	}
}
