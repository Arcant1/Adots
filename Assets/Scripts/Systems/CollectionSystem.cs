using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

using UnityEngine;

public partial struct CollectionSystem : ISystem
{
	public void OnUpdate(ref SystemState state)
	{
		float dt = SystemAPI.Time.DeltaTime;
		//var ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
		//var ecb = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>().CreateCommandBuffer().AsParallelWriter();



	}
}
