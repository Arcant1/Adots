using Unity.Entities;
using UnityEngine;

public class PlayerAuthoring : MonoBehaviour
{
    #region Public Classes

    public class Baker : Baker<PlayerAuthoring>
    {
        #region Public Methods

        public override void Bake(PlayerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            // Add PlayerTag for identification
            AddComponent<PlayerTag>(entity);
        }

        #endregion Public Methods
    }

    #endregion Public Classes
}