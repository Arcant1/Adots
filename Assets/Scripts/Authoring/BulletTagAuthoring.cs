using Unity.Entities;
using UnityEngine;

/// <summary>
/// Authoring component for adding a BulletTag to an entity
/// </summary>
public class BulletTagAuthoring : MonoBehaviour
{
    public class BulletTagBaker : Baker<BulletTagAuthoring>
    {
        public override void Bake(BulletTagAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<BulletTag>(entity);
        }
    }
} 