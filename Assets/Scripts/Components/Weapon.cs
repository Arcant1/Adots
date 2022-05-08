using System;
using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public struct Weapon : IComponentData
{
	public float damageValue;
	public float fireRate;
	public float bulletVelocity;
	public bool canShoot;
	public float lastTime;
	public FixedString64Bytes fireSfx;
	public float range;
	public int bulletQuantity;
}


