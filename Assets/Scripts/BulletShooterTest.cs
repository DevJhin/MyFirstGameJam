using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 투사체 발사를 테스트하기 위한 임시 클래스
/// </summary>
public class BulletShooterTest : FieldObject
{
	public float intervalTime = 0.3f;
	
	enum BulletType
	{
		Circle,
		Ammo
	}
	[SerializeField] BulletType prefabName = BulletType.Circle;

	public Transform shootPoint;
	public Color color = Color.white;

	[Range(0f, 50f)] public float speed = 10f;
	[Range(1, 10)] public int spawnCount = 1;

	[Range(0f, 180f)] public float spreadAngle = 0f;
	[Range(0f, 3f)] public float spacing = 0f;
	

	private void Start()
	{
		if (shootPoint == null)
			shootPoint = transform;

		StartCoroutine(nameof(BulletAutoFire));
	}

	IEnumerator BulletAutoFire()
	{
		while(true)
		{
			yield return new WaitForSeconds(intervalTime);

			Game.BulletSystem.SpawnBullet(System.Enum.GetName(typeof(BulletType), prefabName), shootPoint, shootPoint.eulerAngles.z, speed, color, spawnCount, spreadAngle, spacing);
		}
	}
}
