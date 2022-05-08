using Unity.Entities;
using Unity.Mathematics;

using UnityEngine;

public class GameSettingsAuthoring : MonoBehaviour, IConvertGameObjectToEntity
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
	public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
	{
		var settings = default(GameSettings);

		settings.asteroidVelocity = asteroidVelocity;
		settings.asteroidSpawnProb = asteroidSpawnProb;
		settings.levelWidth = levelWidth;
		settings.levelHeight = levelHeight;
		settings.lookSpeedHorizontal = lookSpeedHorizontal;
		settings.ufoSpawnProb = ufoSpawnProb;
		settings.lookSpeedVertical = lookSpeedVertical;
		dstManager.AddComponentData(entity, settings);
	}
}
