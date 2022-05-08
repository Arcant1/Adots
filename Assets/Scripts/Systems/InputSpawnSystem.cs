using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

using UnityEngine;
[BurstCompile]
public partial class InputSpawnSystem : SystemBase {
	private EntityQuery playerQuery;
	private EndSimulationEntityCommandBufferSystem ecbSystem;
	private Entity _playerPrefab;
	private Entity _bulletPrefab;
	private EntityQuery gameSettingsQuery;
	protected override void OnCreate() {
		RequireSingletonForUpdate<GameSettings>();
		gameSettingsQuery = GetEntityQuery(ComponentType.ReadWrite<GameSettings>());
		RequireForUpdate(gameSettingsQuery);
		playerQuery = GetEntityQuery(ComponentType.ReadWrite<PlayerTag>());
		ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
	}
	protected override void OnUpdate() {
		float dt = Time.DeltaTime;
		GameSettings settings = GetSingleton<GameSettings>();
		EntityCommandBuffer ecb = ecbSystem.CreateCommandBuffer();
		EntityCommandBuffer.ParallelWriter ecbp = ecb.AsParallelWriter();


		bool selfDestruct = false, shootPressed = false;
		int playerCount = playerQuery.CalculateEntityCountWithoutFiltering();
		float currentTime = UnityEngine.Time.time;
		if (_playerPrefab == Entity.Null || _bulletPrefab == Entity.Null) {
			_playerPrefab = GetSingleton<PlayerAuthoring>().Prefab;
			_bulletPrefab = GetSingleton<BulletAuthoring>().prefab;
			return;
		}
		Entity bulletPrefab = _bulletPrefab;
		selfDestruct = Input.GetKeyDown(KeyCode.P);
		shootPressed = Input.GetKey(KeyCode.Space);

		if (shootPressed && playerCount < 1) {
			EntityManager.Instantiate(_playerPrefab);
			return;
		}

		Entities
			.WithName("CheckIfPlayerCanShoot")
			.WithAll<PlayerTag, Weapon>()
			.ForEach((ref Weapon weaponConfig) => {
				weaponConfig.canShoot = false;
				if (currentTime >= weaponConfig.lastTime + 1 / weaponConfig.fireRate && shootPressed) {
					weaponConfig.canShoot = true;
					weaponConfig.lastTime = currentTime;
					AudioManager.instance.PlaySfxRequest(weaponConfig.fireSfx.ToString());

				}
			})
			.WithoutBurst()
			.Run();

		ecbSystem.AddJobHandleForProducer(Dependency);

		Entities
			.WithName("SpawnsAndConfigSingleBullet")
			.WithAll<PlayerTag>()
			.WithNone<SpreadShot>()
			.ForEach((Entity entity, int nativeThreadIndex, in Translation position, in Rotation rot, in PhysicsVelocity vel, in BulletSpawnOffset offset, in Weapon weaponConfig) => {
				if (!weaponConfig.canShoot) { return; }
				Entity bullet = ecbp.Instantiate(nativeThreadIndex, bulletPrefab);
				Translation newPos = new Translation { Value = position.Value + math.mul(rot.Value.value.xyz, offset.Value) };
				Rotation newRot = new Rotation { Value = rot.Value };
				ecbp.SetComponent(nativeThreadIndex, bullet, newPos);
				ecbp.SetComponent(nativeThreadIndex, bullet, newRot);
				PhysicsVelocity newVel = new PhysicsVelocity { Linear = (weaponConfig.bulletVelocity * math.mul(rot.Value, new float3(0, 1, 0)).xyz) + vel.Linear };
				ecbp.SetComponent(nativeThreadIndex, bullet, newVel);
				ecbp.SetComponent(nativeThreadIndex, bullet, new Damage { damageValue = weaponConfig.damageValue });
			})
			.ScheduleParallel();

		ecbSystem.AddJobHandleForProducer(Dependency);

		Entities
			.WithName("SpawnsAndConfigBurstShot")
			.WithAll<PlayerTag, SpreadShot>()
			.ForEach((Entity entity, int nativeThreadIndex, in Translation position, in Rotation rot, in PhysicsVelocity vel, in BulletSpawnOffset offset, in SpreadShot spreadShot, in Weapon weaponConfig) => {
				if (!weaponConfig.canShoot) { return; }
				float spreadOffsetAngle = spreadShot.angleOfSpread / weaponConfig.bulletQuantity;
				for (int i = 0; i <= weaponConfig.bulletQuantity; i++) {
					Rotation newRot = new Rotation { Value = math.mul(rot.Value, quaternion.RotateZ(math.radians(-spreadShot.angleOfSpread / 2 + spreadOffsetAngle * i))) };
					Entity bullet = ecbp.Instantiate(nativeThreadIndex, bulletPrefab);
					Translation newPos = new Translation { Value = position.Value + math.mul(rot.Value.value.xyz, offset.Value) };
					ecbp.SetComponent(nativeThreadIndex, bullet, newPos);
					ecbp.SetComponent(nativeThreadIndex, bullet, newRot);
					PhysicsVelocity newVel = new PhysicsVelocity { Linear = (weaponConfig.bulletVelocity * math.mul(newRot.Value, new float3(0, 1, 0)).xyz) + vel.Linear };
					ecbp.SetComponent(nativeThreadIndex, bullet, newVel);
					ecbp.SetComponent(nativeThreadIndex, bullet, new Damage { damageValue = weaponConfig.damageValue });
				}
			})
			.ScheduleParallel();

		ecbSystem.AddJobHandleForProducer(Dependency);


		Entities
			.WithName("CheckForSuicideButton")
			.ForEach((Entity entity, int nativeThreadIndex) => {
				if (selfDestruct) {
					ecbp.AddComponent(nativeThreadIndex, entity, new DeathTag { timer = 0 });
				}
			})
			.ScheduleParallel();

		ecbSystem.AddJobHandleForProducer(Dependency);

		EntityQuery bulletQuery = GetEntityQuery(ComponentType.ReadOnly<BulletTag>());
		int bulletCount = bulletQuery.CalculateEntityCount();
		GameManager.instance.SetBulletsPerSecond(bulletCount);
	}
}
