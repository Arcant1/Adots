using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

using UnityEngine;

public partial class UFOAttackSystem : SystemBase
{
	private EntityQuery playerQuery;
	private BeginSimulationEntityCommandBufferSystem ecbSystem;
	private Entity _playerPrefab;
	private Entity _bulletPrefab;
	private EntityQuery gameSettingsQuery;
	protected override void OnCreate()
	{
		RequireSingletonForUpdate<GameSettings>();
		gameSettingsQuery = GetEntityQuery(ComponentType.ReadWrite<GameSettings>());
		RequireForUpdate(gameSettingsQuery);
		playerQuery = GetEntityQuery(ComponentType.ReadWrite<PlayerTag>());
		ecbSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
		RequireSingletonForUpdate<PlayerTag>();
	}
	protected override void OnUpdate()
	{
		var dt = Time.DeltaTime;
		var settings = GetSingleton<GameSettings>();
		var ecb = ecbSystem.CreateCommandBuffer();
		var ecbp = ecb.AsParallelWriter();
		var playerEntity = GetSingletonEntity<PlayerTag>();
		var currentTime = UnityEngine.Time.time;
		if (_bulletPrefab == Entity.Null)
		{
			_bulletPrefab = GetSingleton<UFOBulletAuthoring>().prefab;
			return;
		}
		Entity bulletPrefab = _bulletPrefab;

		var playerPosition = GetComponent<Translation>(playerEntity).Value;
		Entities
			.WithName("CheckIfUFOCanShoot")
			.WithAll<UFOtag, Weapon>()
			.ForEach((ref Weapon weaponConfig, in Translation position) =>
			{
				weaponConfig.canShoot = false;
				if (currentTime >= weaponConfig.lastTime && (math.distance(playerPosition, position.Value) < weaponConfig.range))
				{
					weaponConfig.canShoot = true;
					weaponConfig.lastTime = currentTime + 1 / weaponConfig.fireRate;
				}
			})
			.ScheduleParallel();

		Entities
			.WithAll<UFOtag>()
			.WithNone<DeathTag>()
			.ForEach((Entity ufoEntity, int nativeThreadIndex, in Translation position, in Weapon weaponConfig) =>
			{
				if (weaponConfig.canShoot)
				{
					var directionOfLaser = playerPosition - position.Value;
					var newRot = quaternion.LookRotation(directionOfLaser, math.forward());
					var laser = ecbp.Instantiate(nativeThreadIndex, bulletPrefab);
					ecbp.SetComponent(nativeThreadIndex, laser, position);
					ecbp.SetComponent(nativeThreadIndex, laser, new Rotation { Value = newRot });
					ecbp.SetComponent(nativeThreadIndex, laser, new PhysicsVelocity { Linear = weaponConfig.bulletVelocity * math.normalize(directionOfLaser) });
					ecbp.SetComponent(nativeThreadIndex, laser, new Damage { damageValue = weaponConfig.damageValue });
				}
			})
			.ScheduleParallel();


		ecbSystem.AddJobHandleForProducer(Dependency);

	}
}
