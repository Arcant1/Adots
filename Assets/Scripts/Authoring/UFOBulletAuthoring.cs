using Unity.Entities;
using Unity.Mathematics;

using UnityEngine;

[GenerateAuthoringComponent]
public struct UFOBulletAuthoring : IComponentData
{
	public Entity prefab;
}
