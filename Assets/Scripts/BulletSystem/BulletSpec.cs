﻿using Sirenix.OdinInspector;
using UnityEngine;


/// <summary>
/// Bullet의 기본 동작을 정의한 클래스.
/// </summary>
[CreateAssetMenu(menuName = "MyFirstGameJam/BulletSpecs")]
public class BulletSpec : ScriptableObject
{
	/// <summary>
	/// Bullet의 Prefab이름.(경로: "Resources/Poolable")
	/// </summary>
	public string PrefabName;

	/// <summary>
	/// 초기 PoolSize.
	/// </summary>
	public int InitialPoolSize = 30;

	/// <summary>
	/// 피격 데미지.
	/// </summary>
	public float Damage = 1f;

	/// <summary>
	/// 충돌판정 형태.
	/// </summary>
	public CollisionShape CollisionShape;
}


/// <summary>
/// 형태에 따른 충돌판정 방식을 결정하는 enum.
/// </summary>
public enum CollisionShape
{
	Circle,
	Box
}