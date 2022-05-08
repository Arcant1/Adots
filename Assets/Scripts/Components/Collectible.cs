using Unity.Entities;

[GenerateAuthoringComponent]
public struct Collectible : IComponentData
{
	public int points;
}
