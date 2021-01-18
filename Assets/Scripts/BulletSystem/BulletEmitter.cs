using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Bullet 발사를 수행하는 FieldObject.
/// </summary>
public class BulletEmitter : FieldObject
{
	/// <summary>
	/// Unity OnEnable 실행과 함께 Emitter의 Play를 실행하는가?
	/// </summary>
	public bool PlayOnEnable = true;

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
	/// 현재 사용중인 Pool.
	/// </summary>
	private Pool usingPool;

	/// <summary>
	/// 현재 사용중인 Bullet.
	/// </summary>
	private List<Bullet> activeBullets = new List<Bullet>();

	/// <summary>
	/// BulletEmitter의 재생시간.
	/// </summary>
	private float playbackTime;




	private void OnEnable()
	{
		if (PlayOnEnable)
		{
			Play();
		}
	}

	private void OnDisable()
	{
		if (IsPlaying)
		{ 
			Stop();
		}
	}


	/// <summary>
	/// 현재 이 Emitter가 재생되고 있는가?
	/// </summary>
	public bool IsPlaying
	{
		get; private set;
	}


	/// <summary>
	/// BulletEmitter를 재생합니다.
	/// </summary>
	[Button(ButtonSizes.Medium)]
	public void Play()
	{
		if (spec.value.BulletSpec == null)
		{
			Debug.LogError("Failed to play BulletEmitter: BulletSpec is null", this);
		}

		TryLoadPool(spec.value.BulletSpec);

		if (!IsPlaying)
		{ 
			IsPlaying = true;
		}

		playbackTime = 0;
		OnCycleResetFlag = true;

	}

	[Button(ButtonSizes.Medium)]
	/// <summary>
	/// BulletEmitter 실행을 중단합니다. 설정에 따라서는 DisposeAll이 호출될 수 있습니다.
	/// </summary>
	public void Stop()
	{
		IsPlaying = false;

		if (DisposeAllOnStop)
		{
			DisposeAll();
		}
		else
		{ 
		
		}
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

		//Stop된 이후에도 Particle이 남아있을 수 있도록 한다.
		if (!IsPlaying && activeBullets.Count>0)
		{
			UpdateBullets(scaledDeltaTime);
			return;
		}


		//PlaybackTime이 갱신되어 새로운 Cycle을 실행.
		if (OnCycleResetFlag)
		{
			OnCycleReset();
			OnCycleResetFlag = false;
		}

		while (emitTimeQueue.Count > 0)
		{
			float emitTime = emitTimeQueue.Peek();
			if (emitTime <= playbackTime)
			{
				Emit();
				emitTimeQueue.Dequeue();
			}
			else
			{
				break;
			}
		}

		UpdateBullets(scaledDeltaTime);

		//PlaybackTime을 갱신합니다.
		//TimeScale이 음수면 문제가 될 수 있음.
		playbackTime += scaledDeltaTime;

		if (playbackTime > spec.value.Duration)
		{
			playbackTime %= spec.value.Duration;
			OnCycleResetFlag = true;
		}
		
	}


	void UpdateBullets(float scaledDeltaTime)
	{
		// Active Bullet의 Update Loop.
		foreach (var bullet in activeBullets)
		{
			ApplyEffectors(bullet);

			bullet.OnUpdate(scaledDeltaTime);
		}


		activeBullets.ForEach(x => {
			if (x.RemainingLifetime() <= 0f)
			{
				x.OnDispose();
				usingPool.Dispose(x.PooledObject);
			}
		});

		//Timeout된 Bullet들을 ActiveList에서 제거합니다.
		//TODO:: Bullet의 OnDispose를 호출하면서 삭제하는 작업.
		activeBullets.RemoveAll(x => x.RemainingLifetime() <= 0f);
	}


	/// <summary>
	/// 각 Bullet에 Effector 효과를 적용합니다.
	/// </summary>
	/// <param name="bullet"></param>
	private void ApplyEffectors(Bullet bullet)
	{
		foreach (var effector in spec.value.Effectors)
		{
			effector.Apply(bullet);
		}
	}


	/// <summary>
	/// Emitter가 Particle을 즉시 Emit합니다.
	/// </summary>
	public void Emit()
	{
		if (usingBulletSpec != spec.value.BulletSpec)
		{
			OnBulletSpecChange(spec.value.BulletSpec);
		}

		if (usingPool == null) return;

		Bullet bullet = GetBulletFromPool();

		bullet.Lifetime = spec.value.LifeTime;
		bullet.Speed = spec.value.StartSpeed;

		LocateBullet(bullet);

		bullet.OnStart();
		activeBullets.Add(bullet);
	}

	/// <summary>
	/// BulletSpec값이 달라졌을 경우 실행됩니다.
	/// </summary>
	/// <param name="newBulletSpec"></param>
	private void OnBulletSpecChange(BulletSpec newBulletSpec)
	{
		DisposeAll();

		if (!TryLoadPool(newBulletSpec))
		{
			usingPool = null;
		}
	}


	/// <summary>
	/// BulletSpec을 바탕으로 Pool을 로드합니다.
	/// </summary>
	/// <param name="bulletSpec"></param>
	private bool TryLoadPool(BulletSpec bulletSpec)
	{
		if (bulletSpec == null) return false;

		usingPool = PoolManager.GetOrCreate(bulletSpec.PrefabName, bulletSpec.InitialPoolSize);
		usingBulletSpec = bulletSpec;
		return true;
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
	/// 조건과 관계없이 모든 Bullet을 Pool로 반환합니다.
	/// </summary>
	public void DisposeAll()
	{
		if (usingPool == null) return;

		foreach (var bullet in activeBullets)
		{
			bullet.OnDispose();
			usingPool.Dispose(bullet.PooledObject);
		}

		activeBullets.Clear();
	
	}


	/// <summary>
	/// 투사체 하나를 생성하고 Bullet 컴포넌트를 반환합니다.
	/// </summary>
	private Bullet GetBulletFromPool()
	{
		var poolObject = usingPool.Instantiate(Vector3.zero, Quaternion.identity);

		Bullet bullet = poolObject.gameObject.GetComponent<Bullet>();
		if (bullet == null)
		{
			Debug.LogError("생성된 poolObject가 Bullet Component를 가지지 않습니다.");
			return null;
		}

		return bullet;
	}

}
