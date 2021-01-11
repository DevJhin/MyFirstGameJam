using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 투사체 생성에 필요한 데이터
//	TODO: ScriptableObject 형태의 클래스로 변경 예정
/// </summary>
public struct BulletData
{
	public string prefabName;
	public Transform shootPoint;
	
	public float speed;
	public Color color;

	public int spawnCount;
	
	public float spreadAngle;
	public float spacing;
	public float direction;
}

/// <summary>
/// 투사체 생성 및 발사 로직을 관리하는 클래스
/// </summary>
public class BulletSystem
{
	/// <summary>
	/// 투사체 하나를 생성하고 Bullet 컴포넌트를 반환합니다.
	/// </summary>
	/// <param name="prefabName">Resources/Bullets 폴더 내의 오브젝트명</param>
	Bullet CreateBullet(string prefabName)
	{
		GameObject bullet = Object.Instantiate(Resources.Load($"Bullets/{prefabName}")) as GameObject;

		return bullet.GetComponent<Bullet>();
	}
	/// <summary>
	/// 투사체 하나를 생성하고 Bullet 컴포넌트의 위치, 회전, 속도, 색상값을 입력된 데이터를 통해 초기화합니다.
	/// </summary>
	Bullet GetBullet(ref BulletData bulletData)
	{
		Bullet bullet = CreateBullet(bulletData.prefabName);

		bullet.posX = bulletData.shootPoint.position.x;
		bullet.posY = bulletData.shootPoint.position.y;
		bullet.DirectionDegree = bulletData.direction;
		bullet.speed = bulletData.speed;
		bullet.SpriteRenderer.color = bulletData.color;

		bullet.transform.position = bulletData.shootPoint.position;
		bullet.transform.rotation = bulletData.shootPoint.rotation;

		return bullet;
	}

	/// <summary>
	/// BulletData를 통해 투사체를 생성합니다.
	/// </summary>
	/// <param name="bulletData"></param>
	public void SpawnBullet(BulletData bulletData)
	{
		int spawnCount = bulletData.spawnCount;

		if (spawnCount == 1)
		{
			Bullet bullet = GetBullet(ref bulletData);
		}
		else if (spawnCount > 1)
		{
			float initAngle = - (bulletData.spreadAngle / 2);
			float angleIncrease = bulletData.spreadAngle / (spawnCount - 1);
			float half = (spawnCount - 1) / 2f;

			for (int i = 0; i < spawnCount; i++)
			{
				Bullet bullet = GetBullet(ref bulletData);

				bullet.transform.Translate(new Vector2(0f, bulletData.spacing * (i - half)));
				
				bullet.posX = bullet.transform.position.x;
				bullet.posY = bullet.transform.position.y;

				bullet.DirectionDegree += initAngle + (i * angleIncrease);
				bullet.transform.eulerAngles = new Vector3(0.0f, 0.0f, bullet.DirectionDegree);
			}
		}
		else
		{
			Debug.LogError("[BulletSystem.SpawnBullet] Spawn Count is zero!");
		}
	}

	/// <summary>
	/// 초기화할 변수를 직접 입력하여 투사체를 생성합니다.
	/// </summary>
	public void SpawnBullet(string prefabName, Transform shootPoint, float direction, float speed = 10f, Color color = default(Color), int spawnCount = 1, float spreadAngle = 0f, float spacing = 0f)
	{
		if (color == default)
		{
			color = Color.white;
		}

		BulletData data = new BulletData()
		{
			prefabName = prefabName,
			shootPoint = shootPoint,
			direction = direction,
			speed = speed,
			color = color,
			spawnCount = spawnCount,
			spreadAngle = spreadAngle,
			spacing = spacing
		};

		SpawnBullet(data);
	}

}
