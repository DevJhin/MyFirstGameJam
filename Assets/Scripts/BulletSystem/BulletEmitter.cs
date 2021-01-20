using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Bullet 발사를 수행하는 FieldObject.
/// </summary>
public class BulletEmitter : PatternObject
{
	/// <summary>
	/// Unity OnEnable 실행과 함께 Emitter의 Play를 실행하는가?
	/// </summary>
	public bool PlayOnPatternStart = true;

	/// <summary>
	/// 이 Emitter가 Stop 되었을 때 모든 Bullet을 Dispose 하는가?
	/// </summary>
	public bool DisposeAllOnStop = true;
	
	/// <summary>
	/// BulletEmitterSpec의 레퍼런스.
	/// </summary>
	[HideLabel, Title("Specifications")]
	public BulletEmitterSpec_Reference spec;


	/// <summary>
	/// 현재 사용중인 Bullet Spec.
	/// </summary>
	private BulletSpec usingBulletSpec;

	/// <summary>
	/// BulletEmitter의 재생시간.
	/// </summary>
	private float playbackTime;

	/// <summary>
	/// 현재 이 Emitter가 실행 중인가?
	/// </summary>
	private bool isPlaying;


	/// <summary>
	/// 현재 이 Emitter가 재생되고 있는가?
	/// </summary>
	public bool IsPlaying
	{
		get=>IsPlaying;
	}


	public override void OnPatternStart(BulletPattern pattern)
	{
		if (PlayOnPatternStart)
		{
			Play();
		}

	}


	public override void OnPatternFinish(BulletPattern pattern)
	{
		if (isPlaying)
		{
			Stop();
		}
	}



	/// <summary>
	/// BulletEmitter를 재생합니다.
	/// </summary>
	[Button(ButtonSizes.Medium)]
	public void Play()
	{
		isPlaying = true;

		playbackTime = 0;
		OnCycleResetFlag = true;

	}

	[Button(ButtonSizes.Medium)]
	/// <summary>
	/// BulletEmitter 실행을 중단합니다. 설정에 따라서는 DisposeAll이 호출될 수 있습니다.
	/// </summary>
	public void Stop()
	{
		isPlaying = false;
	}


	/// <summary>
	/// 새로운 Cycle이 시작되어 Reset할 필요가 있을 때 Set 됩니다.
	/// </summary>
	private bool OnCycleResetFlag;

	/// <summary>
	/// 이번 Cycle에서 Emit을 실행할 PlaybackTime 시점 에대한 Queue. 
	/// </summary>
	private Queue<float> emitTimeQueue = new Queue<float>();


	/// <summary>
	/// 새로운 Cycle 실행에 필요한 작업을 수행합니다.
	/// </summary>
	private void OnCycleReset()
	{
		emitTimeQueue.Clear();

		float emitTime = 0;
		var burstData = spec.value.Burst;

		if (burstData.Enabled)
		{
			for (int i = 0; i < burstData.Count; i++)
			{
				emitTimeQueue.Enqueue(emitTime);
				emitTime += burstData.RepeatInterval;
			}
		}
		else
		{
			emitTimeQueue.Enqueue(emitTime);
		}

	}


	private void Update()
	{
		float timeScale = spec.value.GetSimulationTime();
		float scaledDeltaTime = Time.deltaTime * timeScale;

		//PlaybackTime이 갱신되어 새로운 Cycle을 실행.
		if (OnCycleResetFlag)
		{
			OnCycleReset();
			OnCycleResetFlag = false;
		}

		while (emitTimeQueue.Count > 0 && emitTimeQueue.Peek() <= playbackTime)
		{
			Emit();
			emitTimeQueue.Dequeue();
		}

		//PlaybackTime을 갱신합니다.
		//TimeScale이 음수면 문제가 될 수 있음.
		playbackTime += scaledDeltaTime;

		if (playbackTime > spec.value.Duration)
		{
			playbackTime %= spec.value.Duration;
			OnCycleResetFlag = true;
		}
		
	}


	/// <summary>
	/// Emitter가 Particle을 즉시 Emit합니다.
	/// </summary>
	public void Emit()
	{
		Bullet bullet = GetBulletFromSpec();

		bullet.SimulationSpeed = spec.value.GetSimulationTime();
		bullet.Lifetime = spec.value.LifeTime;
		bullet.Speed = spec.value.StartSpeed;
		bullet.Effectors = spec.value.Effectors;
		bullet.Attacker = OwnerPattern.SpawnOwner;

		if (spec.value.OverrideCollisionLayerMask)
		{
			bullet.CollisionLayerMask = spec.value.CollisionLayerMask;
		}

		LocateBullet(bullet);

		bullet.OnStart();
	}

	/// <summary>
	/// 하나의 Bullet을 배치합니다.
	/// </summary>
	/// <param name="bullet"></param>
	private void LocateBullet(Bullet bullet)
	{
		Vector3 fireCenterPos = transform.position;
		Vector2 fireDirection = transform.right;

		bullet.Locate(fireCenterPos, fireDirection);
	}


	/// <summary>
	/// 투사체 하나를 생성하고 Bullet 컴포넌트를 반환합니다.
	/// </summary>
	private Bullet GetBulletFromSpec()
	{
		var bulletSpec = spec.value.BulletSpec;
		var pooledObject = PoolManager.GetOrCreate(bulletSpec.PrefabName, bulletSpec.InitialPoolSize).Instantiate(Vector3.zero, Quaternion.identity);

		Bullet bullet = pooledObject.gameObject.GetComponent<Bullet>();
		if (bullet == null)
		{
			Debug.LogError("생성된 poolObject가 Bullet Component를 가지지 않습니다.");
			return null;
		}

		//BulletSpec 값으로 초기화. 
		bullet.Damage = bulletSpec.Damage;

		bullet.CollisionShape = bulletSpec.CollisionShape;
		bullet.CollisionBoxScale = bulletSpec.CollisionBoxScale;
		bullet.CollisionCircleRadius = bulletSpec.CollisionCircleRadius;

		bullet.DisposeAfterCollision = bulletSpec.DisposeAfterCollision;
		bullet.CollisionLayerMask = bulletSpec.CollisionLayerMask;
		
		return bullet;
	}


}
