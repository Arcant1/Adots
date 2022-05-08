using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class OnKillAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
	public string sfxName;
	public GameObject[] onDeathSpawnPrefabs;
	public uint pointValue;
	public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
	{
		var buffer = dstManager.AddBuffer<OnKillPrefabBuffer>(entity);
		foreach (var item in onDeathSpawnPrefabs)
		{
			buffer.Add(new OnKillPrefabBuffer { entity = conversionSystem.GetPrimaryEntity(item) });
		}

		dstManager.AddComponentData(
			entity,
			new OnKill
			{
				sfxName = new Unity.Collections.FixedString64Bytes(sfxName),
				points = pointValue,
			});
	}

	public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
	{
		foreach (var prefab in onDeathSpawnPrefabs)
		{
			if (prefab != null && !referencedPrefabs.Contains(prefab))
				referencedPrefabs.Add(prefab);
		}
	}
}
