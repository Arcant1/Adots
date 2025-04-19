//using System.Diagnostics;
//using Unity.Burst;
//using Unity.Entities;
//using Unity.Mathematics;
//using Unity.Physics;
//using Unity.Transforms;
//using UnityEngine;

//[BurstCompile]
//public partial class AsteroidSpawnSystem : SystemBase
//{
//    #region Public Methods

//    [BurstCompile]
//    public void OnCreate(ref SystemState state)
//    {
//        gameSettingsQuery = GetEntityQuery(ComponentType.ReadWrite<GameSettings>());
//        state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
//        RequireForUpdate(gameSettingsQuery);
//    }

//    #endregion Public Methods

//    #region Protected Methods

//    [BurstCompile]
//    protected override void OnUpdate()
//    {
//        if (_asteroidPrefab == Entity.Null)
//        {
//            //_asteroidPrefab = SystemAPI.GetSingleton<AsteroidAuthoring>().prefab;
//        }
//        var asteroidPrefab1 = _asteroidPrefab;      // Workaround
//        var settings = SystemAPI.GetSingleton<GameSettings>();
//        var rand = new Unity.Mathematics.Random((uint)Stopwatch.GetTimestamp());
//        var ecb = ecbSystem.CreateCommandBuffer();
//        var ecbp = ecb;
//        if (rand.NextFloat(0, 100) < settings.asteroidSpawnProb)
//        {
//            var padding = 0f;
//            var xPos = rand.NextFloat(-1f * ((settings.levelWidth / 2) - padding), (settings.levelWidth / 2) - padding);
//            var yPos = rand.NextFloat(-1f * ((settings.levelHeight / 2) - padding), (settings.levelHeight / 2) - padding);
//            var chooseSide = rand.NextUInt(0, 4);
//            if (chooseSide == 0) { xPos = -1f * ((settings.levelWidth / 2) - padding); }
//            else if (chooseSide == 1) { xPos = (settings.levelWidth / 2) - padding; }
//            else if (chooseSide == 2) { yPos = -1f * ((settings.levelHeight / 2) - padding); }
//            else if (chooseSide == 3) { yPos = (settings.levelHeight / 2) - padding; }
//            var pos = new LocalTransform { Position = new float3(xPos, yPos, 0f) };
//            var e = ecb.Instantiate(asteroidPrefab1);
//            ecb.SetComponent(e, pos);
//            var randomVelocity = new Vector3(rand.NextFloat(-1f, 1f), rand.NextFloat(-1f, 1f), 0f).normalized * settings.asteroidVelocity;
//            ecb.SetComponent(e, new PhysicsVelocity() { Linear = randomVelocity });
//            ecbSystem.AddJobHandleForProducer(Dependency);
//        }

//        foreach ((
//            DynamicBuffer<OnKillPrefabBuffer> onKillPrefabBuffers,
//            RefRO<LocalTransform> localTransform,
//            RefRW<OnKill> onKill,
//            RefRW<AsteroidTag> asteroid,
//            RefRO<AsteroidSpawnOffset> offset,
//            Entity entity)
//            in SystemAPI.Query<
//                DynamicBuffer<OnKillPrefabBuffer>,
//                RefRO<LocalTransform>,
//                RefRW<OnKill>,
//                RefRW<AsteroidTag>,
//                RefRO<AsteroidSpawnOffset>
//                >().WithEntityAccess())
//        {
//            var newAsteroid1 = ecbp.Instantiate(onKillPrefabBuffers[0].entity);
//            ecbp.SetComponent(newAsteroid1, new LocalTransform { Position = localTransform.ValueRO.Position + math.mul(localTransform.ValueRO.Rotation.value.xyz, offset.ValueRO.Value1) });
//            var newAsteroid2 = ecbp.Instantiate(onKillPrefabBuffers[1].entity);
//            ecbp.SetComponent(newAsteroid2, new LocalTransform { Position = localTransform.ValueRO.Position + math.mul(localTransform.ValueRO.Rotation.value.xyz, offset.ValueRO.Value2) });
//            ecbp.DestroyEntity(entity);
//        }

//        ecbSystem.AddJobHandleForProducer(Dependency);
//    }

//    #endregion Protected Methods

//    #region Private Fields

//    private Entity _asteroidPrefab;
//    private BeginSimulationEntityCommandBufferSystem ecbSystem;
//    private EntityQuery gameSettingsQuery;

//    #endregion Private Fields
//}