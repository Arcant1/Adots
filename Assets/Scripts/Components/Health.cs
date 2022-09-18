using Unity.Entities;
using Unity.Collections;

public struct Health : IComponentData
{
	public float value, killTimer;
	public FixedString64Bytes damageSfx, deathSfx;
}
