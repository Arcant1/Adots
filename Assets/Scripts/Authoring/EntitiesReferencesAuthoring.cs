using Unity.Entities;

using UnityEngine;

public struct EntitiesReferences : IComponentData
{
    #region Public Fields

    public Entity bulletPrefabEntity;
    public Entity scoutPrefabEntity;
    public Entity shootLightPrefabEntity;

    #endregion Public Fields
}

public class EntitiesReferencesAuthoring : MonoBehaviour
{
    #region Public Fields

    public GameObject bulletPrefabGameObject;
    public GameObject scoutPrefabGameObject;
    public GameObject shootLightPrefabGameObject;

    #endregion Public Fields

    #region Public Classes

    public class Baker : Baker<EntitiesReferencesAuthoring>
    {
        #region Public Methods

        public override void Bake(EntitiesReferencesAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new EntitiesReferences
            {
                bulletPrefabEntity = GetEntity(authoring.bulletPrefabGameObject, TransformUsageFlags.Dynamic),
                shootLightPrefabEntity = GetEntity(authoring.shootLightPrefabGameObject, TransformUsageFlags.Dynamic),
                scoutPrefabEntity = GetEntity(authoring.scoutPrefabGameObject, TransformUsageFlags.Dynamic),
            });
        }

        #endregion Public Methods
    }

    #endregion Public Classes
}