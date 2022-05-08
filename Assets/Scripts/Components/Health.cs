using Unity.Entities;
using Unity.Collections;

public struct Health : IComponentData
{
	public float value, invincibleTimer, killTimer;
	public FixedString64Bytes damageSfx, deathSfx;
}
