using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

//[BurstCompile]
//public partial struct UFOAttackSystem : ISystem
//{
//    private EntityQuery playerQuery;
//    private BeginSimulationEntityCommandBufferSystem ecbSystem;
//    private Entity _playerPrefab;
//    private Entity _bulletPrefab;
//    private EntityQuery gameSettingsQuery;

//    [BurstCompile]
//    public void OnCreate(ref SystemState state)
//    {
//        state.RequireForUpdate<GameSettings>();
//        gameSettingsQuery = state.GetEntityQuery(ComponentType.ReadWrite<GameSettings>());
//        state.RequireForUpdate(gameSettingsQuery);
//        playerQuery = state.GetEntityQuery(ComponentType.ReadWrite<PlayerTag>());
//        ecbSystem = state.World.GetOrCreateSystemManaged<BeginSimulationEntityCommandBufferSystem>();
//        state.RequireForUpdate<PlayerTag>();
//    }

//    [BurstCompile]
//    public void OnUpdate(ref SystemState state)
//    {
//        float dt = SystemAPI.Time.DeltaTime;
//        GameSettings settings = SystemAPI.GetSingleton<GameSettings>();
//        var ecb = ecbSystem.CreateCommandBuffer();
//        Entity playerEntity = SystemAPI.GetSingletonEntity<PlayerTag>();
//        float currentTime = UnityEngine.Time.time;

//        if (_bulletPrefab == Entity.Null)
//        {
//            _bulletPrefab = SystemAPI.GetSingleton<UFOBulletData>().prefab;
//            return;
//        }
//        Entity bulletPrefab = _bulletPrefab;

//        // Get player position from LocalTransform
//        float3 playerPosition = state.EntityManager.GetComponentData<LocalTransform>(playerEntity).Position;

//        // Check if UFO can shoot job
//        var checkUFOCanShootJob = new CheckUFOCanShootJob
//        {
//            currentTime = currentTime,
//            playerPosition = playerPosition
//        };
//        checkUFOCanShootJob.ScheduleParallel();
//        state.Dependency.Complete();

//        // UFO shoot job
//        var ufoShootJob = new UFOShootJob
//        {
//            ecb = ecb.AsParallelWriter(),
//            bulletPrefab = bulletPrefab,
//            playerPosition = playerPosition
//        };
//        ufoShootJob.ScheduleParallel();
//        state.Dependency.Complete();

//        ecbSystem.AddJobHandleForProducer(state.Dependency);
//    }

//    [WithAll(typeof(UFOtag))]
//    private partial struct CheckUFOCanShootJob : IJobEntity
//    {
//        public float currentTime;
//        public float3 playerPosition;
        
//        public void Execute(ref Weapon weaponConfig, in LocalTransform transform)
//        {
//            weaponConfig.canShoot = false;
//            if (currentTime >= weaponConfig.lastTime && (math.distance(playerPosition, transform.Position) < weaponConfig.range))
//            {
//                weaponConfig.canShoot = true;
//                weaponConfig.lastTime = currentTime + 1 / weaponConfig.fireRate;
//            }
//        }
//    }

//    [WithAll(typeof(UFOtag))]
//    [WithNone(typeof(DeathTag))]
//    private partial struct UFOShootJob : IJobEntity
//    {
//        public EntityCommandBuffer.ParallelWriter ecb;
//        public Entity bulletPrefab;
//        public float3 playerPosition;
        
//        public void Execute([EntityIndexInQuery] int sortKey, in LocalTransform transform, in Weapon weaponConfig)
//        {
//            if (weaponConfig.canShoot)
//            {
//                float3 directionOfLaser = playerPosition - transform.Position;
//                var newRot = quaternion.LookRotation(math.normalize(directionOfLaser), math.up());
//                var laser = ecb.Instantiate(sortKey, bulletPrefab);
//                ecb.SetComponent(sortKey, laser, new LocalTransform
//                {
//                    Position = transform.Position,
//                    Rotation = newRot,
//                    Scale = 1f
//                });
//                ecb.SetComponent(sortKey, laser, new PhysicsVelocity { Linear = weaponConfig.bulletVelocity * math.normalize(directionOfLaser) });
//                ecb.SetComponent(sortKey, laser, new Damage { damageValue = weaponConfig.damageValue });
//            }
//        }
//    }
//}
