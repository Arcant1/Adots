using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

using UnityEngine;

[DisallowMultipleComponent]
public class WeaponAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
	public float damageValue, fireRate, bulletVelocity, nextTime, range;
	public int bulletQuantity;
	public bool canShoot;
	public string fireSfx;
	public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
	{
		dstManager.AddComponentData(entity, new Weapon
		{
			damageValue = damageValue,
			fireRate = fireRate,
			bulletVelocity = bulletVelocity,
			canShoot = canShoot,
			range = range,
			lastTime = nextTime,
			fireSfx = new FixedString64Bytes(fireSfx),
			bulletQuantity = bulletQuantity
		});
	}
}
