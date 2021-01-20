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
	/// 한 주기마다 순간적으로 빠르게 여러 번 Emit할 수 있는 Burst 기능에 대한 Module. 
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


}


/// <summary>
/// BulletEmitterSpec의 ValueReference 패턴을 위한 클래스.
/// </summary>
[System.Serializable]
public class BulletEmitterSpec_Reference : ValueReference<BulletEmitterSpec, BulletEmitterSpecAsset>
{
	

}

