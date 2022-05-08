using System.Diagnostics;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial class AsteroidSpawnSystem : SystemBase
{
	private BeginSimulationEntityCommandBufferSystem ecbSystem;
	private EntityQuery gameSettingsQuery;
	private Entity _asteroidPrefab;

	protected override void OnCreate()
	{
		gameSettingsQuery = GetEntityQuery(ComponentType.ReadWrite<GameSettings>());
		ecbSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
		RequireForUpdate(gameSettingsQuery);
	}


	[BurstCompile]
	protected override void OnUpdate()
	{
		if (_asteroidPrefab == Entity.Null)
		{
			_asteroidPrefab = GetSingleton<AsteroidAuthoring>().prefab;
			return;
		}
		var asteroidPrefab1 = _asteroidPrefab;		// Workaround
		var settings = GetSingleton<GameSettings>();
		var rand = new Unity.Mathematics.Random((uint)Stopwatch.GetTimestamp());
		var ecb = ecbSystem.CreateCommandBuffer();
		var ecbp = ecb.AsParallelWriter();
		if (rand.NextFloat(0, 100) < settings.asteroidSpawnProb)
		{
			Job.WithCode(() =>
			{
				var padding = 0f;
				var xPos = rand.NextFloat(-1f * ((settings.levelWidth) / 2 - padding), (settings.levelWidth) / 2 - padding);
				var yPos = rand.NextFloat(-1f * ((settings.levelHeight) / 2 - padding), (settings.levelHeight) / 2 - padding);
				var chooseSide = rand.NextUInt(0, 4);
				if (chooseSide == 0) { xPos = -1f * ((settings.levelWidth) / 2 - padding); }
				else if (chooseSide == 1) { xPos = settings.levelWidth / 2 - padding; }
				else if (chooseSide == 2) { yPos = -1f * ((settings.levelHeight) / 2 - padding); }
				else if (chooseSide == 3) { yPos = settings.levelHeight / 2 - padding; }
				var pos = new Translation { Value = new float3(xPos, yPos, 0f) };
				var e = ecb.Instantiate(asteroidPrefab1);
				ecb.SetComponent(e, pos);
				var randomVelocity = new Vector3(rand.NextFloat(-1f, 1f), rand.NextFloat(-1f, 1f), 0f).normalized * settings.asteroidVelocity;
				ecb.SetComponent(e, new PhysicsVelocity() { Linear = randomVelocity });
			}).Schedule();
			ecbSystem.AddJobHandleForProducer(Dependency);
		}

		Entities
			.WithName("SpawnAsteroidsSons")
			.WithAll<AsteroidTag, DeathTag>()
			.ForEach((Entity e, int nativeThreadIndex, DynamicBuffer<OnKillPrefabBuffer> onKillPrefabBuffers, in Translation position, in Rotation rot, in OnKill onKill, in AsteroidTag asteroid, in AsteroidSpawnOffset offset) =>
			{
				var newAsteroid1 = ecbp.Instantiate(nativeThreadIndex, onKillPrefabBuffers[0].entity);
				ecbp.SetComponent(nativeThreadIndex, newAsteroid1, new Translation { Value = position.Value + math.mul(rot.Value.value.xyz, offset.Value1) });
				ecbp.SetComponent(nativeThreadIndex, newAsteroid1, rot);
				var newAsteroid2 = ecbp.Instantiate(nativeThreadIndex, onKillPrefabBuffers[1].entity);
				ecbp.SetComponent(nativeThreadIndex, newAsteroid2, new Translation { Value = position.Value + math.mul(rot.Value.value.xyz, offset.Value2) });
				ecbp.SetComponent(nativeThreadIndex, newAsteroid2, rot);
				ecbp.DestroyEntity(nativeThreadIndex, e);
			})
			.ScheduleParallel();

		ecbSystem.AddJobHandleForProducer(Dependency);

	}
}
