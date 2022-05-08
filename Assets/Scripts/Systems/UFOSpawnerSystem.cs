using System.Diagnostics;
using UnityEngine;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
[BurstCompile]
public partial class UFOSpawnerSystem : SystemBase
{
	private BeginSimulationEntityCommandBufferSystem ecbSystem;
	private EntityQuery gameSettingsQuery;
	private Entity _UfoPrefab;

	protected override void OnCreate()
	{
		gameSettingsQuery = GetEntityQuery(ComponentType.ReadWrite<GameSettings>());
		ecbSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
		RequireForUpdate(gameSettingsQuery);
	}

	[BurstCompile]
	protected override void OnUpdate()
	{
		if (_UfoPrefab == Entity.Null)
		{
			_UfoPrefab = GetSingleton<UFOAuthoring>().prefab;
			return;
		}
		var UfoPrefab = _UfoPrefab;
		var settings = GetSingleton<GameSettings>();
		var rand = new Unity.Mathematics.Random((uint)Stopwatch.GetTimestamp());
		var ecb = ecbSystem.CreateCommandBuffer();
		var ecbp = ecb.AsParallelWriter();
		if (rand.NextFloat(0, 100) < settings.ufoSpawnProb)
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
				var e = ecb.Instantiate(UfoPrefab);
				ecb.SetComponent(e, pos);
				var randomVelocity = new Vector3(rand.NextFloat(-1f, 1f), rand.NextFloat(-1f, 1f), 0f).normalized * settings.asteroidVelocity;
				ecb.SetComponent(e, new PhysicsVelocity() { Linear = randomVelocity });
			}).Schedule();
			ecbSystem.AddJobHandleForProducer(Dependency);
		}

		Entities
			.WithName("SpawnUFOItems")
			.WithAll<UFOtag, DeathTag>()
			.ForEach((Entity entity, int nativeThreadIndex, DynamicBuffer<OnKillPrefabBuffer> onKillPrefabBuffers, in Translation position, in Rotation rot, in OnKill onKill, in UFOtag asteroid, in PhysicsVelocity vel) =>
			{
				var randomItemIndex = rand.NextInt(0, 2);
				var item = ecbp.Instantiate(nativeThreadIndex, onKillPrefabBuffers[randomItemIndex].entity);
				ecbp.SetComponent(nativeThreadIndex, item, new Translation { Value = position.Value });
				ecbp.SetComponent(nativeThreadIndex, item, rot);
				ecbp.SetComponent(nativeThreadIndex, item, new PhysicsVelocity { Linear = vel.Linear, Angular = rand.NextFloat3(20) });
			})
			.ScheduleParallel();

		ecbSystem.AddJobHandleForProducer(Dependency);
	}
}
