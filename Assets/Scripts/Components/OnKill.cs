using Unity.Entities;
using Unity.Collections;
public struct OnKill : IComponentData
{
	public FixedString64Bytes sfxName;
	public Entity spawnPrefab;
	public uint points;
}
