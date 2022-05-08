using Unity.Entities;
public struct Spawner : IComponentData
{
	public Entity spawnPrefab;		// Entity to spawn
	public Entity spawnObject;		// Spawner Entity
}
