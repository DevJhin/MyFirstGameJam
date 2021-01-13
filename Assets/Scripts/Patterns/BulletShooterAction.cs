using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// 간단한 Bullet 발사 동작을 수행하는 PatternAction.
/// </summary>
[CreateAssetMenu(menuName = "MyFirstGameJam/Actions/BulletShooterAction", fileName = "New BulletShooterAction")]
public class BulletShooterAction : PatternAction
{
	public string BulletPrefabName;

	[ColorPalette] public Color color = Color.white;

	[Range(0f, 50f)] public float bulletSpeed = 10f;

	[BoxGroup("Multi Bullet Test"), Range(1, 10)] public int spawnCount = 1;
	[BoxGroup("Multi Bullet Test"), Range(0f, 180f)] public float spreadAngle = 0f;
	[BoxGroup("Multi Bullet Test"), Range(0f, 3f)] public float spacing = 0f;


	public override void Execute(FieldObject actor)
	{
		BulletHelper.SpawnBullet(BulletPrefabName, actor.transform, actor.transform.eulerAngles.z, bulletSpeed, color, spawnCount, spreadAngle, spacing);
	}

}
