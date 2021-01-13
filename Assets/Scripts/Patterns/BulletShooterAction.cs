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
	[Range(-180f, 180f)] public float bulletRotation = 0f;

	[DetailedInfoBox("참고: Spawn Count가 1이면 추가 세팅은 불필요합니다.", 
		"Spread Angle은 여러 개의 투사체 발사 시 각도를, Spacing은 발사 시작 지점에서 투사체 간의 거리를 나타냅니다. 투사체가 1개일 경우 두 변수 모두 사용되지 않습니다.")]
	[BoxGroup("Multi Bullet Setting"), Range(1, 10)] public int spawnCount = 1;
	[BoxGroup("Multi Bullet Setting"), Range(0f, 360f)] public float spreadAngle = 0f;
	[BoxGroup("Multi Bullet Setting"), Range(0f, 3f)] public float spacing = 0f;


	public override void Execute(FieldObject actor)
	{
		BulletHelper.SpawnBullet(BulletPrefabName, actor.transform, actor.transform.eulerAngles.z + bulletRotation, bulletSpeed, color, spawnCount, spreadAngle, spacing);
	}

}
