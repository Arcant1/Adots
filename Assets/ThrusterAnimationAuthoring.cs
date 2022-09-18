
using Unity.Entities;

using UnityEngine;

public class ThrusterAnimationAuthoring : MonoBehaviour, IConvertGameObjectToEntity {
	public GameObject leftThrust, rightThrust;

	public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
		dstManager.AddComponentData(entity, new ThrustAnimationData {
			left = conversionSystem.GetPrimaryEntity(leftThrust),
			right = conversionSystem.GetPrimaryEntity(rightThrust)
		});

	}
}
