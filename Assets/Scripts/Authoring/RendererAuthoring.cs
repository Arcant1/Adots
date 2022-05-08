using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class RendererAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public SpriteRenderer _renderer;


    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        //conversionSystem.AddHybridComponent(_renderer);
    }
}
