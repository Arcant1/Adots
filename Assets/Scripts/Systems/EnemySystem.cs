using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

public class EnemySystem
{
	private float3 UP = new float3(0, 0, 1);
	private Unity.Mathematics.Random rng = new Unity.Mathematics.Random(1234);
	/*protected override void OnUpdate()
	{
		MovementRaycast raycaster = new MovementRaycast() { pw = World.GetOrCreateSystem<BuildPhysicsWorld>().PhysicsWorld };
		rng.NextInt(); // Need to roll the random to not get the same number as before
		Random rngTemp = rng;
		Entities
			.WithAll<Enemy>()
			.ForEach((ref Translation mov, ref Enemy enemy, in Translation translation) =>
			{
				if (math.distance(translation.Value, enemy.previousCell) > 0.9f) // TODO change this
				{
					enemy.previousCell = math.round(translation.Value);
					NativeList<float3> validDirections = new NativeList<float3>(Allocator.Temp); // Must use NativeList to be able to use burst compiler

					// Make 4 ray casts
					if (!raycaster.CheckRay(translation.Value, new float3(0, 0, -1), mov.Value)) // DOWN
					{
						validDirections.Add(new float3(0, 0, -1));
					}
					if (!raycaster.CheckRay(translation.Value, new float3(0, 0, 1), mov.Value))    // UP
					{
						validDirections.Add(new float3(0, 0, 1));
					}
					if (!raycaster.CheckRay(translation.Value, new float3(-1, 0, 0), mov.Value))    // LEFT
					{
						validDirections.Add(new float3(-1, 0, 0));
					}
					if (!raycaster.CheckRay(translation.Value, new float3(1, 0, 0), mov.Value)) // RIGHT
					{
						validDirections.Add(new float3(1, 0, 0));
					}
					// Select one direction
					if (validDirections.Length > 0)
						mov.Value = validDirections[rngTemp.NextInt(validDirections.Length)];
					validDirections.Dispose(); // Always dispose the list to prevent memory leaks
				}
			})
			.Schedule();
		this.CompleteDependency();
	}
	*/
	private struct MovementRaycast
	{
		[ReadOnly] public PhysicsWorld pw; // Evil hack, it needs to be readonly in order to use Burst compiler without complaining
		public bool CheckRay(float3 pos, float3 direction, float3 currentDirection)
		{
			if (direction.Equals(-currentDirection)) return true;

			RaycastInput ray = new RaycastInput
			{
				Start = pos,
				End = pos + (direction * 0.9f),
				Filter = new CollisionFilter()
				{
					GroupIndex = 0,
					BelongsTo = 1u << 1,
					CollidesWith = (1u << 2)
				}
			};
			return pw.CastRay(ray);
		}
	}
}
