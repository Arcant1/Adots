using Unity.Entities;
using UnityEngine;

// Component to store thruster entities for animation
public struct ThrustAnimationData : IComponentData 
{
	public Entity left;
	public Entity right;
	public bool isThrusting;       // Forward thrust
	public bool isRotatingLeft;    // Left rotation
	public bool isRotatingRight;   // Right rotation
	public bool isBraking;         // Reverse thrust
} 