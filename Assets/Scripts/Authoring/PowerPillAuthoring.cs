using Unity.Entities;
using UnityEngine;

class PowerPillAuthoring : MonoBehaviour
{
    [SerializeField] public GameObject prefab;
}

class PowerPillAuthoringBaker : Baker<PowerPillAuthoring>
{
    public override void Bake(PowerPillAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new _PowerPillAuth { prefab = GetEntity(authoring.prefab, TransformUsageFlags.Dynamic) });
    }
}
