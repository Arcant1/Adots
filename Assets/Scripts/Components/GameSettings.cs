using Unity.Entities;

public struct GameSettings : IComponentData
{
	public float asteroidSpawnProb;
	public float ufoSpawnProb;
	public float asteroidVelocity;
	public int levelWidth;
	public int levelHeight;
	public int levelDepth;
	public float lookSpeedHorizontal;
	public float lookSpeedVertical;
	
	// Bullet settings
	public float bulletVelocity;
	public float bulletLifetime;
}
