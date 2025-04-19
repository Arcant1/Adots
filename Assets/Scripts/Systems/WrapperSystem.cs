using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(InputSpawnSystem))]
public partial class WrapperSystem : SystemBase
{
    #region Protected Methods

    protected override void OnCreate()
    {
        playerQuery = GetEntityQuery(ComponentType.ReadOnly<PlayerTag>());
    }

    protected override void OnStartRunning()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogWarning("WrapperSystem: No main camera found");
            return;
        }
        CalculateBounds();
    }

    protected override void OnUpdate()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogWarning("WrapperSystem: No main camera found");
                return;
            }
            CalculateBounds();
        }

        // Handle player wrapping around screen bounds
        var playerEntities = playerQuery.ToEntityArray(Allocator.Temp);

        if (playerEntities.Length > 0)
        {
            Entity playerEntity = playerEntities[0];

            if (EntityManager.HasComponent<LocalTransform>(playerEntity))
            {
                var transform = EntityManager.GetComponentData<LocalTransform>(playerEntity);
                bool positionChanged = false;

                // Wrap position around screen boundaries
                if (transform.Position.x < screenMin.x)
                {
                    transform.Position.x = screenMax.x;
                    positionChanged = true;
                }
                else if (transform.Position.x > screenMax.x)
                {
                    transform.Position.x = screenMin.x;
                    positionChanged = true;
                }

                if (transform.Position.y < screenMin.y)
                {
                    transform.Position.y = screenMax.y;
                    positionChanged = true;
                }
                else if (transform.Position.y > screenMax.y)
                {
                    transform.Position.y = screenMin.y;
                    positionChanged = true;
                }

                // Only update if position was modified
                if (positionChanged)
                {
                    EntityManager.SetComponentData(playerEntity, transform);
                    Debug.Log($"Wrapped player to: {transform.Position}");
                }
            }
        }

        playerEntities.Dispose();
    }

    #endregion Protected Methods

    #region Private Fields

    private Camera mainCamera;
    private EntityQuery playerQuery;
    private float2 screenMax;
    private float2 screenMin;

    #endregion Private Fields

    #region Private Methods

    private void CalculateBounds()
    {
        if (mainCamera == null) return;

        // Calculate screen bounds in world space
        Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 topRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, 0));

        // Add a small buffer to avoid edge cases
        float buffer = 0.5f;

        screenMin = new float2(bottomLeft.x - buffer, bottomLeft.y - buffer);
        screenMax = new float2(topRight.x + buffer, topRight.y + buffer);

        Debug.Log($"Screen bounds: Min({screenMin.x}, {screenMin.y}) Max({screenMax.x}, {screenMax.y})");
    }

    #endregion Private Methods
}