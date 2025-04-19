using Unity.Entities;

public struct MoveForce : IComponentData
{
	public float ThrustForce;
	public float RotateForce;
	public float MouseRotateForce;
}
