using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class SetBulletSpawnOffsetAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public GameObject bulletSpawn;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var bulletOffset = default(BulletSpawnOffset);
        var offset = bulletSpawn.transform.position;
        bulletOffset.Value = offset;
        dstManager.AddComponentData(entity,bulletOffset);
    }
}
