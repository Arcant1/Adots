using System.Diagnostics;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
[BurstCompile]
public partial struct  HyperSpaceSystem : ISystem
{
	public void OnCreate(ref SystemState state)
	{
		state.RequireForUpdate<PlayerTag>();
	}
	public void OnUpdate(ref SystemState state)
	{
		var rand = new Unity.Mathematics.Random((uint)Stopwatch.GetTimestamp());

		//Entity player = GetSingletonEntity<PlayerTag>();
		//if (Input.GetKeyDown(KeyCode.H))
		//{
		//	float3 randomPos = rand.NextFloat3(5, 10);
		//	randomPos.z = 0;
		//	SetComponent(player, new Translation { Value = GetComponent<Translation>(player).Value + randomPos.xyz });
		//}
	}
}
