using Unity.Entities;

[GenerateAuthoringComponent]
public struct AsteroidAuthoring : IComponentData {
	public Entity prefab;
}
