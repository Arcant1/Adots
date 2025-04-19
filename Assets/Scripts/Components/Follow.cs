using Unity.Entities;
using Unity.Mathematics;


public struct Follow : IComponentData
{
	public Entity target;
	public float distance, speedMovement, speedRotation;
	public float3 offset;
	public bool freezeXPos, freezeYPos, freezeZPos;
	public bool freezeRot;
}
