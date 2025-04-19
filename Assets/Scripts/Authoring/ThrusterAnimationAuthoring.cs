using Unity.Entities;
using UnityEngine;

// Authoring component for thruster animation
public class ThrusterAnimationAuthoring : MonoBehaviour 
{
	public GameObject leftThrust;
	public GameObject rightThrust;

	public class ThrusterAnimationBaker : Baker<ThrusterAnimationAuthoring>
	{
		public override void Bake(ThrusterAnimationAuthoring authoring)
		{
			Entity entity = GetEntity(TransformUsageFlags.Dynamic);
			
			// Add thruster animation component data
			AddComponent(entity, new ThrustAnimationData
			{
				left = GetEntity(authoring.leftThrust, TransformUsageFlags.Dynamic),
				right = GetEntity(authoring.rightThrust, TransformUsageFlags.Dynamic),
				isThrusting = false,
				isRotatingLeft = false,
				isRotatingRight = false,
				isBraking = false
			});
		}
	}
} 