using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Transforms;

using UnityEngine;
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public partial class ThrustSystem : SystemBase {
	private EntityQuery gameSettingsQuery;

	protected override void OnCreate() {
		RequireSingletonForUpdate<GameSettings>();
		gameSettingsQuery = GetEntityQuery(ComponentType.ReadWrite<GameSettings>());
		RequireForUpdate(gameSettingsQuery);
	}
	protected override void OnUpdate() {
		float dt = Time.DeltaTime;
		GameSettings settings = GetSingleton<GameSettings>();
		bool right = false, left = false, thrust = false, reverse = false;
		float mouseX = 0, mouseY = 0;
		thrust = Input.GetKey(KeyCode.UpArrow);
		reverse = Input.GetKey(KeyCode.DownArrow);
		left = Input.GetKey(KeyCode.LeftArrow);
		right = Input.GetKey(KeyCode.RightArrow);
		if (Input.GetMouseButton(1)) {
			mouseX = Input.GetAxis("Mouse X");
			mouseY = Input.GetAxis("Mouse Y");
		}
		Entities
			.WithAll<PlayerTag>()
			.ForEach((in ThrustAnimationData thrustAnim) => {
				if (left) {
					EntityManager.SetEnabled(thrustAnim.right, right);
					EntityManager.SetEnabled(thrustAnim.left, left);
				}
				else if (right) {
					EntityManager.SetEnabled(thrustAnim.right, right);
					EntityManager.SetEnabled(thrustAnim.left, left);
				}
				else if (thrust) {
					EntityManager.SetEnabled(thrustAnim.right, thrust);
					EntityManager.SetEnabled(thrustAnim.left, thrust);
				}
				else {
					EntityManager.SetEnabled(thrustAnim.right, false);
					EntityManager.SetEnabled(thrustAnim.left, false);
				}
			})
			.WithStructuralChanges()
			.Run();
		Entities
			.WithAll<PlayerTag>()
			.ForEach((Entity entity, int nativeThreadIndex, ref PhysicsVelocity physicsVelocity, ref PhysicsMass physicsMass, ref Rotation rotation, in MoveForce moveForce) => {
				float3 direction = math.mul(rotation.Value, new float3(0f, 1f, 0f));
				if (left) {
					rotation.Value = math.mul(rotation.Value, quaternion.RotateZ(math.radians(moveForce.RotateForce * dt)));
				}
				if (right) {
					rotation.Value = math.mul(rotation.Value, quaternion.RotateZ(math.radians(-moveForce.RotateForce * dt)));
				}
				if (thrust) {
					physicsVelocity.ApplyLinearImpulse(physicsMass, direction * moveForce.ThrustForce * dt);
				}
				if (reverse) {
					physicsVelocity.ApplyLinearImpulse(physicsMass, -direction * moveForce.ThrustForce * dt);
				}
				if (mouseX != 0 || mouseY != 0) {
					rotation.Value = math.mul(rotation.Value, quaternion.RotateZ(math.radians(-mouseX * moveForce.MouseRotateForce * dt)));
				}
			})
			.ScheduleParallel();
	}
}
