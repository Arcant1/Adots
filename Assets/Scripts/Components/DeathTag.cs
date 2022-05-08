using Unity.Entities;

[GenerateAuthoringComponent]
public struct DeathTag : IComponentData
{
	public float timer;
}
