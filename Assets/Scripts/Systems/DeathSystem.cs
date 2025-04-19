using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

public partial struct DeathSystem : ISystem 
{

	//public  void OnCreate(ref SystemState state) {
	//	//ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
	//}
 //   public
	//	void OnUpdate(ref SystemState state) {
	//	NativeArray<uint> points = new NativeArray<uint>(1, Allocator.TempJob);
	//	points[0] = 0;
	//	EntityCommandBuffer ecb = ecbSystem.CreateCommandBuffer();
	//	float dt = SystemAPI.Time.DeltaTime;
	//	//Entities
	//	//	.WithAll<DeathTag>()
	//	//	.ForEach((Entity entity, int nativeThreadIndex, ref DeathTag killInfo) => {
	//	//		killInfo.timer -= dt;
	//	//		if (killInfo.timer < 0f) {
	//	//			ecb.DestroyEntity( entity);
	//	//		}
	//	//		if (HasComponent<OnKill>(entity)) {
	//	//			points[0] += GetComponent<OnKill>(entity).points;
	//	//		}
	//	//	})
	//	//	.WithBurst()
	//	//	.Schedule();
	//	//CompleteDependency();
		
	//	GameManager.instance.AddPoints(points[0]);
	//	points.Dispose();
	//}
}
