using System.Collections.Generic;

using Unity.Entities;

using UnityEngine;

[DisallowMultipleComponent]
public class SpawnerAuthoring : MonoBehaviour,
	IConvertGameObjectToEntity,
	IDeclareReferencedPrefabs
{
	public GameObject spawnObject;

	public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
	{
		if (spawnObject == null) return;
		dstManager.AddComponentData(entity, new Spawner() { spawnPrefab = conversionSystem.GetPrimaryEntity(spawnObject) });
	}

	public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
	{
		if (spawnObject != null && !referencedPrefabs.Contains(spawnObject))
			referencedPrefabs.Add(spawnObject);
	}
}
