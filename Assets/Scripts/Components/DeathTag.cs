using Unity.Entities;

public struct DeathTag : IComponentData
{
	public bool dead;
	public float timer;
}
