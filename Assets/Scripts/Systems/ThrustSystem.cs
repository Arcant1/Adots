using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(InputSpawnSystem))]
public partial class ThrustSystem : SystemBase
{
    #region Protected Methods

    protected override void OnCreate()
    {
        playerQuery = GetEntityQuery(
            ComponentType.ReadOnly<PlayerTag>(),
            ComponentType.ReadWrite<LocalTransform>(),
            ComponentType.ReadWrite<PhysicsVelocity>(),
            ComponentType.ReadOnly<PhysicsMass>()
        );
    }

    protected override void OnUpdate()
    {
        float dt = SystemAPI.Time.DeltaTime;

        // Handle player movement and rotation
        var playerEntities = playerQuery.ToEntityArray(Allocator.Temp);
        
        if (playerEntities.Length > 0)
        {
            Entity playerEntity = playerEntities[0];
            
            // Make sure player has required components
            if (EntityManager.HasComponent<LocalTransform>(playerEntity) &&
                EntityManager.HasComponent<PhysicsVelocity>(playerEntity) &&
                EntityManager.HasComponent<PhysicsMass>(playerEntity))
            {
                var transform = EntityManager.GetComponentData<LocalTransform>(playerEntity);
                var velocity = EntityManager.GetComponentData<PhysicsVelocity>(playerEntity);
                var mass = EntityManager.GetComponentData<PhysicsMass>(playerEntity);
                
                // Get input for rotation (left/right arrows)
                float rotateInput = 0f;
                if (Input.GetKey(KeyCode.LeftArrow)) rotateInput -= 1f;
                if (Input.GetKey(KeyCode.RightArrow)) rotateInput += 1f;
                
                // Get input for acceleration (A to accelerate, Z to brake)
                float accelerateInput = 0f;
                if (Input.GetKey(KeyCode.A)) accelerateInput += 1f;
                if (Input.GetKey(KeyCode.Z)) accelerateInput -= 1f;
                
                // Handle thruster animations if the entity has ThrustAnimationData
                if (EntityManager.HasComponent<ThrustAnimationData>(playerEntity))
                {
                    var thrustData = EntityManager.GetComponentData<ThrustAnimationData>(playerEntity);
                    
                    // Update animation state based on input
                    thrustData.isThrusting = accelerateInput > 0;
                    thrustData.isBraking = accelerateInput < 0;
                    thrustData.isRotatingLeft = rotateInput < 0;
                    thrustData.isRotatingRight = rotateInput > 0;
                    
                    // Apply animation - enable/disable thrusters based on state
                    if (thrustData.left != Entity.Null)
                    {
                        bool leftThrusterActive = thrustData.isThrusting || thrustData.isRotatingRight;
                        EntityManager.SetEnabled(thrustData.left, leftThrusterActive);
                    }
                    
                    if (thrustData.right != Entity.Null)
                    {
                        bool rightThrusterActive = thrustData.isThrusting || thrustData.isRotatingLeft;
                        EntityManager.SetEnabled(thrustData.right, rightThrusterActive);
                    }
                    
                    // Save updated thruster animation data
                    EntityManager.SetComponentData(playerEntity, thrustData);
                }
                
                // Handle self-destruct
                bool selfDestruct = Input.GetKeyDown(KeyCode.P);
                if (selfDestruct)
                {
                    EntityManager.AddComponent<DeathTag>(playerEntity);
                    EntityManager.SetComponentData(playerEntity, new DeathTag { timer = 0 });
                }
                
                // Apply rotation
                if (rotateInput != 0f)
                {
                    float rotationSpeed = 3f; // Radians per second
                    quaternion rotationDelta = quaternion.RotateZ(-rotateInput * rotationSpeed * dt);
                    quaternion newRotation = math.mul(transform.Rotation, rotationDelta);
                    
                    transform.Rotation = newRotation;
                    EntityManager.SetComponentData(playerEntity, transform);
                }
                
                // Apply acceleration in the direction the ship is facing
                if (accelerateInput != 0f)
                {
                    // Calculate ship's forward direction based on rotation
                    float3 forwardDirection = math.mul(transform.Rotation, new float3(0, 1, 0));

                    // Apply continuous force instead of impulse for gradual acceleration
                    float thrustForce = 50f; // Force applied per second
                    float3 force = forwardDirection * thrustForce * accelerateInput;

                    // Get current velocity
                    PhysicsVelocity newVelocity = velocity;

                    // Apply force (F = ma, so a = F/m)
                    float actualMass = 1.0f / mass.InverseMass; // Convert inverse mass back to mass
                    float3 acceleration = force / actualMass;

                    // v = v0 + a*t (apply acceleration over time)
                    newVelocity.Linear += acceleration * dt;

                    // Apply some drag to eventually limit speed naturally
                    float dragCoefficient = 0.1f;
                    newVelocity.Linear *= (1.0f - dragCoefficient * dt);

                    // Update the velocity component
                    EntityManager.SetComponentData(playerEntity, newVelocity);

                    // Update position based on physics (v*t)
                    transform.Position += newVelocity.Linear * dt;
                    EntityManager.SetComponentData(playerEntity, transform);
                }
                else
                {
                    // When not accelerating, maintain momentum but apply some drag
                    PhysicsVelocity newVelocity = velocity;

                    // Apply drag - gradual slowdown when not accelerating
                    float dragCoefficient = 0.05f; // Lower drag when coasting
                    newVelocity.Linear *= (1.0f - dragCoefficient * dt);

                    // Update velocity and position if we're still moving
                    if (math.lengthsq(newVelocity.Linear) > 0.001f)
                    {
                        EntityManager.SetComponentData(playerEntity, newVelocity);

                        // Update position based on current velocity
                        transform.Position += newVelocity.Linear * dt;
                        EntityManager.SetComponentData(playerEntity, transform);
                    }
                }
            }
            else
            {
                Debug.LogWarning("Player missing required components for movement");
            }
        }
        
        playerEntities.Dispose();
    }

    #endregion Protected Methods

    #region Private Fields

    private EntityQuery playerQuery;

    #endregion Private Fields
}
