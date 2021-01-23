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
	/// 한 Emit당 생성가능한 최대 Bullet 개수.
	/// </summary>
	[OnValueChanged("ResetBufferSize")]
	[MaxValue(1000)]
	public uint MaxBulletSpawnCount = 100;

	
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


	private void Awake()
	{
		ResetBufferSize();
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


	Bullet[] internalBulletBuffer;

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
		if (!isPlaying) return;

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
		//이번 Emit에서 Spawn할 갯수.
		int spawnCount = spec.value.GetSpawnCount();

		//버퍼 사이즈를 초과해서 Spawn을 수행하지 않는다.
		if (spawnCount > internalBulletBuffer.Length)
		{
			spawnCount = internalBulletBuffer.Length;
			Debug.LogWarning(
				$"Emitter의 BufferSize를 초과하여 Bullet의 생성이 제한되고 있습니다. MaxBufferSize를 재조정하거나 Emit 수를 줄이는 것을 고려하십시오."
				, this);
		}

		//BulletSpec에 따라 Bullet을 스폰한다.
		for (int i = 0; i < spawnCount; i++)
		{
			internalBulletBuffer[i] = GetBulletFromSpec();
		}
		
		//Bullet 들을 Spec에 따라 배치한다.
		LocateBullets(spawnCount);


		//Bullet을 활성화하여 사용을 시작한다.
		for (int i = 0; i < spawnCount; i++)
		{
			internalBulletBuffer[i].OnStart();
		}


		//Reference를 초기화한다.
		for (int i = 0; i < spawnCount; i++)
		{
			internalBulletBuffer[i]= null;
		}


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

		//BulletSpec의 값으로 초기화. 
		bullet.Damage = bulletSpec.Damage;

		bullet.CollisionShape = bulletSpec.CollisionShape;
		bullet.CollisionBoxScale = bulletSpec.CollisionBoxScale;
		bullet.CollisionCircleRadius = bulletSpec.CollisionCircleRadius;

		bullet.DisposeAfterCollision = bulletSpec.DisposeAfterCollision;
		bullet.CollisionLayerMask = bulletSpec.CollisionLayerMask;

		//Emit Spec의 값에 따라 초기화.
		bullet.SimulationSpeed = spec.value.GetSimulationTime();
		bullet.Lifetime = spec.value.LifeTime;
		bullet.Speed = spec.value.StartSpeed;
		bullet.Effectors = spec.value.Effectors;

		bullet.Attacker = OwnerPattern.SpawnOwner;
		bullet.EntityGroup = EntityGroup.Enemy;


		if (spec.value.OverrideStartColor)
		{
			bullet.SpriteRenderer.color = spec.value.StartColor;
		}

		if (spec.value.OverrideCollisionLayerMask)
		{
			bullet.CollisionLayerMask = spec.value.CollisionLayerMask;
		}



		return bullet;
	}

	private void LocateBullets(int spawnCount)
	{
		if (spawnCount <= 1)
		{
			Bullet bullet = internalBulletBuffer[0];

			Vector3 finalPos = transform.position;
			Vector3 finalDir = transform.right;

			bullet.Locate(finalPos, finalDir);
			return;
		}
		else if(spawnCount > 1)
		{
			var multiModule = spec.value.MultiShot;


			//Circle의 형태로 배치하는 방식일 경우...
			if (multiModule.LayoutMode == BulletEmitterSpec.MultiShotModule.LayoutShapeMode.Circle)
			{
				float initAngle = -(multiModule.SpreadAngle / 2);
				float angleIncrease = multiModule.SpreadAngle / (spawnCount - 1);
				float half = (spawnCount - 1) / 2f;

				for (int i = 0; i < spawnCount; i++)
				{
					Bullet bullet = internalBulletBuffer[i];

					float spreadOffset = multiModule.Spacing * (i - half);
					float angle = initAngle + (i * angleIncrease);

					Vector3 finalPos = transform.position + transform.up * spreadOffset;
					Vector3 finalDir = Quaternion.Euler(0, 0, angle) * transform.right;

					bullet.Locate(finalPos, finalDir);
				}
			}
			// Grid 형태로 배치하는 방식일 경우...
			else if(multiModule.LayoutMode == BulletEmitterSpec.MultiShotModule.LayoutShapeMode.Grid)
			{
				int xCount = multiModule.XCount;
				int yCount = multiModule.YCount;

				float xSpacing = multiModule.GridSpacing.x;
				float ySpacing = multiModule.GridSpacing.y;

				float xOffset = -xSpacing * ((xCount - 1) * 0.5f);
				float yOffset = -ySpacing * ((yCount - 1) * 0.5f);

				for (int i = 0; i < spawnCount; i++)
				{
					int xIndex = i % xCount;
					int yIndex = i / xCount;

					Bullet bullet = internalBulletBuffer[i];

					Vector2 localPos = new Vector2(xOffset + xIndex * xSpacing, yOffset + yIndex * ySpacing);
					Vector3 finalPos = transform.position + transform.rotation * localPos;

					//BoxGrid에서는 Emitter와 같은 방향을 바라보도록 하는 것이 일단 적절해 보인다.
					Vector3 finalDir = transform.right;

					bullet.Locate(finalPos, finalDir);
				}
				
			}
		}
		else
		{
			Debug.LogError("[BulletSystem.SpawnBullet] Spawn Count is zero!");
		}

	}

	/// <summary>
	/// BufferSize를 재조정합니다.
	/// </summary>
	private void ResetBufferSize()
	{
		internalBulletBuffer = new Bullet[MaxBulletSpawnCount];
	}

}
