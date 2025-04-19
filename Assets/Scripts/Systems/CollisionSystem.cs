using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
[BurstCompile]
public partial struct CollisionSystem : ISystem {
	//private struct CollisionSystemJob : ICollisionEventsJob {
	//	public BufferLookup<CollisionBuffer> collisions;
	//	public void Execute(CollisionEvent collisionEvent) {
	//		if (collisions.HasComponent(collisionEvent.EntityA)) {
	//			collisions[collisionEvent.EntityA].Add(new CollisionBuffer() { entity = collisionEvent.EntityB });
	//		}
	//		if (collisions.HasComponent(collisionEvent.EntityB)) {
	//			collisions[collisionEvent.EntityB].Add(new CollisionBuffer() { entity = collisionEvent.EntityA });
	//		}
	//	}
	//}

	//private struct TriggerSystemJob : ITriggerEventsJob {

	//	public BufferLookup<TriggerBuffer> triggers;

	//	public void Execute(TriggerEvent triggerEvent) {
	//		if (triggers.HasComponent(triggerEvent.EntityA)) {
	//			triggers[triggerEvent.EntityA].Add(new TriggerBuffer() { entity = triggerEvent.EntityB });
	//		}
	//		if (triggers.HasComponent(triggerEvent.EntityB)) {
	//			triggers[triggerEvent.EntityB].Add(new TriggerBuffer() { entity = triggerEvent.EntityA });
	//		}
	//	}
	//}
	//protected override void OnUpdate() {
	//	PhysicsWorld pw = World.GetOrCreateSystem<BuildPhysicsWorld>().PhysicsWorld;
	//	ISimulation sim = World.GetOrCreateSystem<StepPhysicsWorld>().Simulation;

	//	Entities.ForEach((DynamicBuffer<CollisionBuffer> collisions) => {
	//		collisions.Clear();
	//	}).Schedule();

	//	JobHandle collisionJobHandle = new CollisionSystemJob() { collisions = GetBufferFromEntity<CollisionBuffer>() }
	//		.Schedule(sim, Dependency);
	//	collisionJobHandle.Complete();

	//	// Do the same thing for triggers
	//	Entities.ForEach((DynamicBuffer<TriggerBuffer> triggers) => {
	//		triggers.Clear();
	//	}).Schedule();

	//	JobHandle triggersJobHandle = new TriggerSystemJob() { triggers = GetBufferFromEntity<TriggerBuffer>() }
	//		.Schedule(sim, Dependency);
	//	triggersJobHandle.Complete();
	//}
}

