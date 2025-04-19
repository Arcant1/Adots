using Unity.Entities;

public partial struct LifeTimeSystem : ISystem
{

	public  void OnCreate(ref SystemState state)
	{
		//ecbSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
	}

	public void OnUpdate(ref SystemState state)
	{
		//Entities.ForEach((Entity entity, int nativeThreadIndex, ref LifeTime ttl) =>
		//{
		//	ttl.time += deltaTime;
		//	if (ttl.time > ttl.maxTime)
		//		ecb.DestroyEntity(nativeThreadIndex, entity);

		//}).ScheduleParallel();
		//ecbSystem.AddJobHandleForProducer(Dependency);
	}
}
