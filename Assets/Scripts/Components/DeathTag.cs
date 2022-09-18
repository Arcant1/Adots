using Unity.Entities;

[GenerateAuthoringComponent]
public struct DeathTag : IComponentData
{
	public bool dead;
	public float timer;
}
