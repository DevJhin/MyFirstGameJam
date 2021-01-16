using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 투사체 생성에 필요한 데이터
/// </summary>
[CreateAssetMenu(menuName = "MyFirstGameJam/Data")]
public class BulletData : ScriptableObject
{
	public IEventListener attacker = null;
	public string prefabName;
	public Transform shootPoint;

	public float damage = 1f;
	public float speed = 10f;
	public Color color = Color.white;

	public int spawnCount = 1;

	public float spreadAngle = 0f;
	public float spacing = 0f;
	public float direction = 0f;
}

/// <summary>
/// 투사체 생성 및 발사 로직을 관리하는 클래스
/// </summary>
public static class BulletHelper
{
	/// <summary>
	/// 투사체 하나를 생성하고 Bullet 컴포넌트를 반환합니다.
	/// </summary>
	/// <param name="prefabName">Resources/Bullets 폴더 내의 오브젝트명</param>
	static Bullet CreateBullet(string prefabName)
	{
		var pool = PoolManager.GetOrCreate(prefabName, 30);
		var bullet = pool.Instantiate(Vector3.zero, Quaternion.identity);

		// FIXME 총알 이런 식으로 만들어내면 안 됨.
		return bullet.gameObject.GetOrAddComponent<Bullet>();
	}
	/// <summary>
	/// 투사체 하나를 생성하고 Bullet 컴포넌트의 위치, 회전, 속도, 색상값을 입력된 데이터를 통해 초기화합니다.
	/// </summary>
	static Bullet GetBullet(ref BulletData bulletData)
	{
		Bullet bullet = CreateBullet(bulletData.prefabName);

		bullet.attacker = bulletData.attacker;
		bullet.posX = bulletData.shootPoint.position.x;
		bullet.posY = bulletData.shootPoint.position.y;
		bullet.DirectionDegree = bulletData.direction;
		bullet.speed = bulletData.speed;
		bullet.damage = bulletData.damage;
		bullet.SpriteRenderer.color = bulletData.color;

		bullet.transform.position = bulletData.shootPoint.position;

		return bullet;
	}

	/// <summary>
	/// BulletData를 통해 투사체를 생성합니다.
	/// </summary>
	/// <param name="bulletData"></param>
	public static void SpawnBullet(BulletData bulletData)
	{
		int spawnCount = bulletData.spawnCount;

		if (spawnCount == 1)
		{
			Bullet bullet = GetBullet(ref bulletData);
			bullet.transform.eulerAngles = new Vector3(0f, 0f, bullet.DirectionDegree);
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
	/// <param name="prefabName">프리팹의 이름을 입력합니다. Resources/Bullets 내에 있어야 합니다. (예: Resources/Bullets/Carrot 프리팹 호출 시, Carrot 입력)</param>
	/// <param name="shootPoint">발사 지점이 되는 Transform을 입력합니다. 따로 방향을 지정하지 않을 경우, 회전값도 이 shootPoint를 따르게 됩니다.</param>
	/// <param name="shooter">투사체를 발사하는 주체를 입력합니다. 해당 대상은 자신이 쏜 총알에 피격당하지 않습니다. 총알의 피격대상이 아니라면 null로 두어도 됩니다.</param>
	/// <param name="direction">별도로 회전값을 제어하고 싶으면 입력합니다. null일 경우 기본적으로 shootPoint의 방향을 따르게 됩니다.</param>
	/// <param name="damage">투사체가 적대 개체에 적중 시 적용시킬 데미지</param>
	/// <param name="speed">투사체가 진행하는 속도를 입력합니다.</param>
	/// <param name="color">투사체의 색상을 지정합니다. 기본적으로 White을 사용합니다.</param>
	/// <param name="spawnCount">한 번에 발사할 투사체의 개수를 입력합니다.</param>
	/// <param name="spreadAngle">투사체들이 몇 도(degree)에 걸쳐 발사될 지 결정합니다. spawnCount가 1일 경우 아무 동작도 하지 않습니다.</param>
	/// <param name="spacing">발사 시점에서 투사체들이 얼마나 떨어져 있는지 결정합니다. spawnCount가 1일 경우 아무 동작도 하지 않습니다.</param>
	public static void SpawnBullet(string prefabName, Transform shootPoint, FieldObject shooter = null,
		float? direction = null, float damage = 1f, float speed = 10f, Color? color = null, int spawnCount = 1, float spreadAngle = 0f, float spacing = 0f)
	{
		BulletData data = ScriptableObject.CreateInstance<BulletData>();
		data.prefabName = prefabName;
		data.shootPoint = shootPoint;
		data.attacker = (shooter is IEventListener) ? (IEventListener) shooter : null;
		data.direction = (direction != null) ? (float) direction : shootPoint.eulerAngles.z;
		data.damage = damage;
		data.speed = speed;
		data.color = (color != null) ? (Color) color : Color.white;
		data.spawnCount = spawnCount;
		data.spreadAngle = spreadAngle;
		data.spacing = spacing;

		SpawnBullet(data);
	}

}
