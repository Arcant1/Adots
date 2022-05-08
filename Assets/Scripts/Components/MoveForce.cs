using Unity.Entities;

[GenerateAuthoringComponent]
public struct MoveForce : IComponentData
{
	public float ThrustForce;
	public float RotateForce;
	public float MouseRotateForce;
}
