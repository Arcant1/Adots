using Unity.Entities;
public struct LifeTime : IComponentData
{
	/// <summary>
	/// Create a LifeTime component with a time to live
	/// </summary>
	/// <param name="maxTime">Max time to live</param>
	public LifeTime(float maxTime)
	{
		this.maxTime = maxTime;
		time = 0;
	}
	public float time;
	public float maxTime;
}
