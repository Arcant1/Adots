using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial class WrapperSystem : SystemBase
{
	private EndSimulationEntityCommandBufferSystem ecbSystem;

	Camera cam;
	float xMin = 0, xMax = 0, yMin = 0, yMax = 0;
	protected override void OnStartRunning()
	{
		cam = Camera.main;
		xMin = 0;
		xMax = 0;
		yMin = 0;
		yMax = 0;
		CalculateBounds(cam, ref xMin, ref xMax, ref yMin, ref yMax);
	}
	protected override void OnCreate()
	{
		ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
	}
	protected override void OnUpdate()
	{
		float xMin = this.xMin;
		float xMax = this.xMax;
		float yMin = this.yMin;
		float yMax = this.yMax;

		var ecb = ecbSystem.CreateCommandBuffer().AsParallelWriter();
		Entities
			.WithNone<BulletTag, UFObulletTag>()
			.ForEach((Entity entity, ref Translation translation) =>
		{
			if (translation.Value.x < xMin)
				translation.Value.x = xMax;
			if (translation.Value.x > xMax)
				translation.Value.x = xMin;
			if (translation.Value.y < yMin)
				translation.Value.y = yMax;
			if (translation.Value.y > yMax)
				translation.Value.y = yMin;
		}).ScheduleParallel();
		CompleteDependency();

		Entities
			.WithAny<UFObulletTag, BulletTag>()
			.ForEach((Entity e, int nativeThreadIndex, in Translation translation) =>
			{
				if ((translation.Value.x < xMin) ||
					(translation.Value.x > xMax) ||
					(translation.Value.y < yMin) ||
					(translation.Value.y > yMax))
					ecb.DestroyEntity(nativeThreadIndex, e); // Destroy immediately since I don't want to spawn nothing onKill
			})
			.ScheduleParallel();
		CompleteDependency();


	}
	private void CalculateBounds(Camera cam, ref float xMin, ref float xMax, ref float yMin, ref float yMax)
	{
		xMin = cam.ViewportToWorldPoint(Vector3.zero).x;    // Left side
		xMax = cam.ViewportToWorldPoint(Vector3.right).x;   // Right side
		yMin = cam.ViewportToWorldPoint(Vector3.zero).y;    // Bottom
		yMax = cam.ViewportToWorldPoint(Vector3.up).y;      // Top
	}
}
