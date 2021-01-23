using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


/// <summary>
/// Emitter의 다양한 설정을 관리하는 구조체.
/// </summary>
[System.Serializable] [BoxGroup("Emitter Settings"), LabelWidth(125)]
public struct BulletEmitterSpec
{
	/// <summary>
	/// Emit할 Bullet에 대한 Spec.
	/// </summary>
	public BulletSpec BulletSpec;

	/// <summary>
	/// Emitter의 실행 주기. 기본적으로 이 주기마다 Bullet의 Emit을 수행합니다.
	/// </summary>
	[MinValue(0.05f)]
	public float Duration;

	/// <summary>
	/// Bullet의 초기 속도.
	/// </summary>
	public float StartSpeed;

	/// <summary>
	/// Bullet의 사용 시간.
	/// </summary>
	public float LifeTime;


	/// <summary>
	/// Emitter의 진행 속도를 조절할 수 있는 SimulationSpeed 기능이 활성화되어 있는가?
	/// </summary>
	public bool UseSimulationSpeed;

	/// <summary>
	/// UnityEngine의 Time.timeScale과는 별개로 Local한 TimeScale값.
	/// </summary>
	[ShowIf("UseSimulationSpeed"), MinValue(0.1)]
	public float SimulationSpeed;

	/// <summary>
	/// 기존에 설정된 SpriteRenderer 색을 무시하고, Emitter가 임의로 변경할 수 있습니까?
	/// </summary>
	public bool OverrideStartColor;

	/// <summary>
	/// Bullet이 Emit되었을 때 설정할 Color.(Override 활성화된 경우)
	/// </summary>
	[ColorPalette]
	public Color StartColor;

	/// <summary>
	/// Burst 기능 Module.
	/// </summary>
	[Toggle("Enabled")]
	public BurstModule Burst;

	/// <summary>
	/// MultiShot 기능 Module.
	/// </summary>
	[Toggle("Enabled")]
	public MultiShotModule MultiShot;


	[Title("Physics")]
	/// <summary>
	/// 기존에 설정된 Collision LayerMask를 무시하고, Emitter가 임의로 변경할 수 있습니까?
	/// </summary>
	public bool OverrideCollisionLayerMask;

	/// <summary>
	/// 충돌 LayerMask.
	/// </summary>
	[EnableIf("OverrideCollisionLayerMask")]
	public LayerMask CollisionLayerMask;


	/// <summary>
	/// 매 동작마다 실행하는 Effector.
	/// </summary>
	[Title("Effectors")]
	public List<BulletEffector> Effectors;


	/// <summary>
	/// SimulationTime을 반환합니다. UseSimulationSpeed가 false면, 항상 1.0f값을 반환합니다.
	/// </summary>
	public float GetSimulationTime()
	{
		if (!UseSimulationSpeed) return 1.0f;

		return SimulationSpeed; 
	}

	/// <summary>
	/// 1회의 Emit 시 Spawn할 Bullet의 개수를 반환합니다.
	/// </summary>
	/// <returns>
	/// MultiShot 설정이 활성화된 경우, MultiShot의 Count를 반환합니다.
	/// 그외의 경우에는 1을 반환합니다.
	/// </returns>
	public int GetSpawnCount()
	{
		return MultiShot.Enabled ? MultiShot.GetSpawnCount() : 1;
	}


	/// <summary>
	/// 짧은 간격으로 빠르게 여러 번 Emit할 수 있는 Burst 기능에 대한 Module. 
	/// </summary>
	[System.Serializable]
	public struct BurstModule
	{
		/// <summary>
		/// 이 Emitter의 Burst 기능이 활성화되어있는가?
		/// </summary>
		public bool Enabled;

		/// <summary>
		/// 한 Burst 당 Emit을 실행하는 횟수. RepeatInterval 간격으로 실행.
		/// </summary>
		[MinValue(1)]
		public int Count;

		/// <summary>
		/// 한 Burst에서 Emit을 실행하는 시간 간격.
		/// </summary>
		[MinValue(0.01f)]
		public float RepeatInterval;
	}


	/// <summary>
	/// 한 번에 
	/// </summary>
	[System.Serializable]
	public struct MultiShotModule
	{
		/// <summary>
		/// 이 Emitter의 Multishot 기능이 활성화되어있는가?
		/// </summary>
		public bool Enabled;


		public enum LayoutShapeMode
		{ 
			Circle,
			Grid
		}

		/// <summary>
		/// 
		/// </summary>
		public LayoutShapeMode LayoutMode;

		/// <summary>
		/// 한 Emit 당 발사할 탄환의 개수.
		/// </summary>
		[MinValue(1)]
		[ShowIf("LayoutMode", LayoutShapeMode.Circle)]
		public int Count;

		/// <summary>
		/// 
		/// </summary>
		[PropertyRange(-360f, 360f)]
		[ShowIf("LayoutMode", LayoutShapeMode.Circle)]
		public float SpreadAngle;

		[ShowIf("LayoutMode", LayoutShapeMode.Circle)]
		public float Spacing;

		/*Grid Settings*/

		[PropertyRange(1,30)]
		[ShowIf("LayoutMode", LayoutShapeMode.Grid)]
		public int XCount;

		[PropertyRange(1, 30)]
		[ShowIf("LayoutMode", LayoutShapeMode.Grid)]
		public int YCount;

		[ShowIf("LayoutMode", LayoutShapeMode.Grid)]
		public Vector2 GridSpacing;

		/// <summary>
		/// MultiShot 활성화 시, SpawnCount를 반환합니다.
		/// </summary>
		/// <returns></returns>
		public int GetSpawnCount()
		{

			if (LayoutMode == LayoutShapeMode.Circle)
			{
				return Count;
			}
			else if (LayoutMode == LayoutShapeMode.Grid)
			{
				return XCount * YCount;
			}

			return 0;
		}
	}


}


/// <summary>
/// BulletEmitterSpec의 ValueReference 패턴을 위한 클래스.
/// </summary>
[System.Serializable]
public class BulletEmitterSpec_Reference : ValueReference<BulletEmitterSpec, BulletEmitterSpecAsset>
{
	

}

