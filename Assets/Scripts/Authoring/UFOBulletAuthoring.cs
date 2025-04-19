using Unity.Entities;
using UnityEngine;

public struct UFOBulletData : IComponentData
{
	public Entity prefab;
}

public class UFOBulletAuthoring : MonoBehaviour
{
	public GameObject prefabGameObject;

	public class Baker : Baker<UFOBulletAuthoring>
	{
		public override void Bake(UFOBulletAuthoring authoring)
		{
			Entity entity = GetEntity(TransformUsageFlags.None);
			AddComponent(entity, new UFOBulletData
			{
				prefab = GetEntity(authoring.prefabGameObject, TransformUsageFlags.Dynamic)
			});
		}
	}
}
