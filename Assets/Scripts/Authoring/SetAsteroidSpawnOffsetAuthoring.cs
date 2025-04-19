using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

//[DisallowMultipleComponent]
//public class SetAsteroidSpawnOffsetAuthoring : MonoBehaviour, IConvertGameObjectToEntity
//{
//	public GameObject bulletSpawn1, bulletSpawn2;

//	public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
//	{
//		var asteroidOffset1 = default(AsteroidSpawnOffset);
//		var offset1 = bulletSpawn1.transform.position;
//		asteroidOffset1.Value1 = offset1;
//		dstManager.AddComponentData(entity, asteroidOffset1);

//		var asteroidOffset2 = default(AsteroidSpawnOffset);
//		var offset2 = bulletSpawn1.transform.position;
//		asteroidOffset2.Value2 = offset2;
//		dstManager.AddComponentData(entity, asteroidOffset2);

//	}
//}
