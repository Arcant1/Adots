using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public partial class SeekAndDestroySystem : SystemBase {
	private EndSimulationEntityCommandBufferSystem ecbSystem;
	protected override void OnCreate() {
		ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
	}
	protected override void OnUpdate()
    {
        Entities
			.WithName("SeekAndDestroySystem")
			.ForEach((ref Translation translation, in Rotation rotation, in Seeker seeker) => {
				
			

        }).Schedule();
    }
}
