using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class ShieldAnimationAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
	public GameObject shield;
	public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
		dstManager.AddComponentData(entity, new ShieldAnimationData {
			shield = conversionSystem.GetPrimaryEntity(shield),
		});
	}
}

public struct ShieldAnimationData : IComponentData {
	public Entity shield;
}
