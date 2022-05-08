using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
public class HealthAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
	public float killTimer, hpValue;
	public string dmgSfx, deathSfx;
	public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
	{
		dstManager.AddComponentData(entity, new Health()
		{
			invincibleTimer = 0,
			killTimer = killTimer,
			value = hpValue,
			damageSfx = new Unity.Collections.FixedString64Bytes(dmgSfx),
			deathSfx = new Unity.Collections.FixedString64Bytes(deathSfx)
		});
	}
}
