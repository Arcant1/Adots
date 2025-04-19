using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;


public struct Seeker : IComponentData {
	public Entity currentTarget;
	public float maxRange;
}
