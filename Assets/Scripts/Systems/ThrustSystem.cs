using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using Unity.Physics.Extensions;
using UnityEngine;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public partial class ThrustSystem : SystemBase
{
	private EntityQuery gameSettingsQuery;

	protected override void OnCreate()
	{
		RequireSingletonForUpdate<GameSettings>();
		gameSettingsQuery = GetEntityQuery(ComponentType.ReadWrite<GameSettings>());
		RequireForUpdate(gameSettingsQuery);
	}
	protected override void OnUpdate()
	{
		var dt = Time.DeltaTime;
		var settings = GetSingleton<GameSettings>();
		byte right = 0, left = 0, thrust = 0, reverse = 0;
		float mouseX = 0, mouseY = 0;
		if (Input.GetKey(KeyCode.UpArrow))
		{
			thrust = 1;
		}
		if (Input.GetKey(KeyCode.DownArrow))
		{
			reverse = 1;
		}
		if (Input.GetKey(KeyCode.LeftArrow))
		{
			left = 1;
		}
		if (Input.GetKey(KeyCode.RightArrow))
		{
			right = 1;
		}
		if (Input.GetMouseButton(1))
		{
			mouseX = Input.GetAxis("Mouse X");
			mouseY = Input.GetAxis("Mouse Y");
		}

		Entities
			.WithAll<PlayerTag>()
			.ForEach((Entity entity, int nativeThreadIndex, ref PhysicsVelocity physicsVelocity, ref PhysicsMass physicsMass, ref Rotation rotation, in MoveForce moveForce) =>
			{
				var direction = math.mul(rotation.Value, new float3(0f, 1f, 0f));
				if (left == 1)
				{

					rotation.Value = math.mul(rotation.Value, quaternion.RotateZ(math.radians(moveForce.RotateForce * dt)));
				}
				if (right == 1)
				{
					rotation.Value = math.mul(rotation.Value, quaternion.RotateZ(math.radians(-moveForce.RotateForce * dt)));

				}
				if (thrust == 1)
				{
					physicsVelocity.ApplyLinearImpulse(physicsMass, direction * moveForce.ThrustForce * dt);

				}
				if (reverse == 1)
				{
					physicsVelocity.ApplyLinearImpulse(physicsMass, -direction * moveForce.ThrustForce * dt);
				}

				if (mouseX != 0 || mouseY != 0)
				{
					rotation.Value = math.mul(rotation.Value, quaternion.RotateZ(math.radians(-mouseX * moveForce.MouseRotateForce * dt)));
				}

			})
			.ScheduleParallel();
	}
}
