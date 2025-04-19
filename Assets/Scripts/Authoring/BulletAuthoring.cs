using Unity.Entities;
using UnityEngine;

// Authoring component for bullets
public class BulletAuthoring : MonoBehaviour
{
	public float speed = 15f;      // Default bullet speed
	public float lifetime = 3f;    // Default bullet lifetime in seconds
	
	public class BulletBaker : Baker<BulletAuthoring>
	{
		public override void Bake(BulletAuthoring authoring)
		{
			Entity entity = GetEntity(TransformUsageFlags.Dynamic);
			
			// No longer adding BulletTag here since BulletTagAuthoring already adds it
			// Just add bullet properties
			AddComponent(entity, new BulletProperties
			{
				Speed = authoring.speed,
				Lifetime = authoring.lifetime,
				CurrentTime = 0f
			});
		}
	}
}
