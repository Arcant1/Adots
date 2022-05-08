using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

using UnityEngine;

public partial class CollectionSystem : SystemBase
{
	protected override void OnUpdate()
	{
		float dt = Time.DeltaTime;
		var ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
		var ecb = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>().CreateCommandBuffer().AsParallelWriter();



	}
}
