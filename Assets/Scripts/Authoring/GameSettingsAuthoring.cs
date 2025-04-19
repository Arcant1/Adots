using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class GameSettingsAuthoring : MonoBehaviour
{
	public float asteroidVelocity = 10f;
	public float playerForce = 50f;
	public float bulletVelocity = 500f;
	public int levelWidth = 2048;
	public int levelHeight = 2048;
	public float lookSpeedHorizontal = 2f;
	public float lookSpeedVertical = 2f;
	public float asteroidSpawnProb = 0.48f;
	public float ufoSpawnProb = 0.825f;

	public class Baker : Baker<GameSettingsAuthoring>
	{
		public override void Bake(GameSettingsAuthoring authoring)
		{
			Entity entity = GetEntity(TransformUsageFlags.None);
			
			// Add the GameSettings component to the entity
			AddComponent(entity, new GameSettings
			{
				asteroidVelocity = authoring.asteroidVelocity,
				asteroidSpawnProb = authoring.asteroidSpawnProb,
				levelWidth = authoring.levelWidth,
				levelHeight = authoring.levelHeight,
				lookSpeedHorizontal = authoring.lookSpeedHorizontal,
				lookSpeedVertical = authoring.lookSpeedVertical,
				ufoSpawnProb = authoring.ufoSpawnProb
			});
			
			// Make this entity a singleton by adding a special tag component
			AddComponent<GameSettingsTag>(entity);
		}
	}
}

// Tag component to mark the GameSettings entity as a singleton
public struct GameSettingsTag : IComponentData {}
