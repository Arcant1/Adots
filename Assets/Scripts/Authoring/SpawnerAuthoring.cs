using System.Collections.Generic;

using Unity.Entities;

using UnityEngine;

[DisallowMultipleComponent]
public class SpawnerAuthoring : MonoBehaviour
{
	public GameObject spawnObject;

	public void Convert(Entity entity, EntityManager dstManager)
	{
		if (spawnObject == null) return;
		//dstManager.AddComponentData(entity, new Spawner() { spawnPrefab = conversionSystem.GetPrimaryEntity(spawnObject) });
	}

	public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
	{
		if (spawnObject != null && !referencedPrefabs.Contains(spawnObject))
			referencedPrefabs.Add(spawnObject);
	}
}
