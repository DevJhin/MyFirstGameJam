using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// BulletEmiiter에서 Spawn되는 투사체 FieldObject.
/// </summary>
public class Bullet : FieldObject, IEventListener
{
    private SpriteRenderer spriteRenderer;

    /// <summary>
    /// Bullet의 SpriteRendrer 레퍼런스.
    /// </summary>
    public SpriteRenderer SpriteRenderer
    {
        get
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();

            return spriteRenderer;
        }
    }


    /// <summary>
    /// 현재 사용중인 Pool.
    /// </summary>
    public Pool Pool;

    /// <summary>
    /// Bullet의 PooledObject 레퍼런스.
    /// </summary>
    public PooledObject PooledObject;

    /// <summary>
    /// Bullet을 발사한 주체.
    /// </summary>
    public FieldObject Attacker;

    /// <summary>
    /// 충돌 시 데미지.
    /// </summary>
    public float Damage = 1f;

    /// <summary>
    /// 이 Bullet의 이동을 활성화하는가?
    /// </summary>
    public bool EnableMove = true;

    /// <summary>
    /// 시작시 설정된 속도.
    /// </summary>
    public float InitSpeed;

    /// <summary>
    /// 직선 이동 속도.
    /// </summary>
    public float Speed
    {
        get; set;
    }


    /// <summary>
    /// 이동 방향 각도(Degree).
    /// </summary>
    [ShowInInspector, ReadOnly]
    public float Angle
    {
        get => radAngle * Mathf.Rad2Deg;
        set => radAngle = value * Mathf.Deg2Rad;
    }

    /// <summary>
    /// 자전을 활성화하는가?(이동 방향 회전은 <see mem="Angle"/> 참고)
    /// </summary>
    public bool EnableRotation;

    /// <summary>
    /// 자전 각속도.
    /// </summary>
    public float RotationAngularVelocity;

    /// <summary>
    /// Bullet의 사용이 시작된 이후, Destroy 될 때까지의 시간.
    /// </summary>
    public float Lifetime = 1f;

    /// <summary>
    /// 매 Update마다 충돌 검사를 할 것인가?
    /// </summary>
    public bool EnableCollision = true;

    /// <summary>
    /// 충돌 판정 형태.
    /// </summary>
    public CollisionShape CollisionShape;

    /// <summary>
    /// Box 충돌 판정 사이즈.
    /// </summary>
    public Vector2 CollisionBoxScale;

    /// <summary>
    /// Circle 충돌 판정 반경.
    /// </summary>
    public float CollisionCircleRadius;

    /// <summary>
    /// 충돌 후에 이 객체를 Destroy할 것인가?
    /// </summary>
    public bool DisposeAfterCollision;

    /// <summary>
    /// 충돌 가능 LayerMask.
    /// </summary>
    public LayerMask CollisionLayerMask;

    /// <summary>
    /// 시뮬레이션 속도(Timescale).
    /// </summary>
    public float SimulationSpeed;


    /*Private Fields*/

    /// <summary>
    /// 내부적으로 사용되는 이동 방향 각(Radian).
    /// </summary>
    private float radAngle;

    /// <summary>
    /// Bullet의 사용이 시작된 시간.
    /// </summary>
    private float birthTime;

    /// <summary>
    /// 남은 Lifetime. 0이 되면 Emitter에 의해 사용이 종료됨.
    /// </summary>
    [SerializeField]
    private float remainingLifetime;

    /// <summary>
    /// Physics.NonAlloc 계열 함수에서 최대로 받아올 수 있는 Collider2D 갯수.
    /// </summary>
    private readonly static int ColliderBufferSize = 3;

    /// <summary>
    /// Physics.NonAlloc 계열 함수에 사용하기 위한 Collider Buffer.
    /// </summary>
    private Collider2D[] colliderBuffer = new Collider2D[ColliderBufferSize];


    public List<BulletEffector> Effectors = new List<BulletEffector>();

    private bool IsActive = false;

    /// <summary>
    /// 없어질 때 생기는 이펙트 이름
    /// </summary>
    public string ExplosionFXName;

    private void Awake()
    {
        PooledObject = GetComponent<PooledObject>();
    }

    private void Start()
    {
        Pool = PoolManager.GetOrCreate(PooledObject.OriginalObjectName);
    }

    private void OnEnable()
    {
        MessageSystem.Instance.Subscribe<StageClearEvent>(OnStageClearEvent);
    }

    private void OnDisable()
    {
        MessageSystem.Instance.Unsubscribe<StageClearEvent>(OnStageClearEvent);
    }


    /// <summary>
    /// BulletEmitter가 Bullet을 Emit하면서 사용을 시작했을 때 실행됩니다.
    /// </summary>
    public void OnStart()
    {
        birthTime = Time.time;
        remainingLifetime = Lifetime;
        IsActive = true;

    }


    /// <summary>
    /// Bullet의 상태를 갱신합니다. 매 프레임마다 Emitter에 의해 Update됩니다.
    /// </summary>
    /// <param name="dt">Scaled된 DeltaTime.</param>
    public void Update()
    {
        if (!IsActive) return;

        float dt = Time.deltaTime * SimulationSpeed;

        Move(dt);

        ApplyEffectors();

        if (EnableCollision)
        {
            CheckCollision();
        }

        remainingLifetime -= dt;
        if (remainingLifetime <= 0)
        {
            ReturnPool();
        }
    }


    /// <summary>
    /// Emitter가 이 Bullet 객체의 사용을 종료하고, Pool로 반환하기 전에 호출됩니다.
    /// </summary>
    public void ReturnPool()
    {
        if (!IsActive) return;

        Effectors = null;
        Pool.Dispose(PooledObject);

        if (ExplosionFXName != null)
        {
            PoolManager.GetOrCreate(ExplosionFXName).Instantiate(transform.position, Quaternion.identity);
        }


        IsActive = false;
    }


    /// <summary>
    /// Bullet의 남은 Lifetime을 반환합니다.
    /// </summary>
    /// <returns></returns>
    public float RemainingLifetime()
    {
        return remainingLifetime;
    }


    /// <summary>
    /// Bullet의 이동 작업을 수행합니다.
    /// </summary>
    private void Move(float dt)
    {
        float dx = Mathf.Cos(radAngle) * dt * Speed;
        float dy = Mathf.Sin(radAngle) * dt * Speed;

        Vector3 deltaPos = new Vector3(dx, dy, 0);

        Vector3 nextPos = transform.position + deltaPos;


        if (EnableRotation)
        {
            transform.eulerAngles += new Vector3(0f, 0f, RotationAngularVelocity * dt);
        }

        transform.position = nextPos;
    }


    /// <summary>
    /// 충돌 검사를 수행합니다.
    /// </summary>
    private void CheckCollision()
    {
        int numColliders = 0;

        if (CollisionShape == CollisionShape.Circle)
        {
            numColliders = Physics2D.OverlapCircleNonAlloc(transform.position, CollisionCircleRadius, colliderBuffer, CollisionLayerMask);
        }
        else if (CollisionShape == CollisionShape.Box)
        {
            numColliders = Physics2D.OverlapBoxNonAlloc(transform.position, CollisionBoxScale, transform.eulerAngles.z, colliderBuffer, CollisionLayerMask);
        }

        for (int i = 0; i < numColliders; i++)
        {
            ProcessCollision(colliderBuffer[i]);
        }
    }


    /// <summary>
    /// Collision 검사 결과로 얻은 Collider2D에 대한 작업(DamgeEvent 등)을 처리합니다.
    /// </summary>
    /// <param name="collider">대상 콜라이더.</param>
    private void ProcessCollision(Collider2D collider)
    {
        FieldObject target = collider.GetComponent<FieldObject>();

        if (target == null)
            target = collider.GetComponentInParent<FieldObject>();


        bool canSendDamageEvent = Attacker != null && target != null && Attacker.EntityGroup.IsHostileTo(target.EntityGroup);

        // 적이면 데미지 가함
        if (canSendDamageEvent)
        {
            var info = new DamageInfo
            {
                Sender = Attacker,
                Target = target,
                amount = (int)Damage
            };
            var cmd = DamageCommand.Create(info);
            cmd.Execute();
        }

        if (DisposeAfterCollision)
        {
            ReturnPool();
            return;
        }
    }


    /// <summary>
    ///  Visible 상태를 설정합니다.
    /// </summary>
    /// <param name="value"> 설정값.</param>
    /// <param name="withCollision">Collision 판정 실행 여부를 Visibility와 함께 변경할것인가?</param>
    public void SetVisibility(bool value, bool withCollision = false)
    {
        SpriteRenderer.enabled = value;

        if (withCollision)
        {
            EnableCollision = withCollision;
        }
    }


    /// <summary>
    /// 주어진 위치와 방향에 따라 Bullet을 배치합니다.
    /// </summary>
    /// <param name="position">배치할 위치.</param>
    /// <param name="direction">배치 방향.</param>
    public void Locate(Vector3 position, Vector3 direction)
    {
        transform.position = position;
        Angle = Vector2.SignedAngle(Vector2.right, direction);
    }


    /// <summary>
    /// 가속도를 적용합니다.
    /// </summary>
    /// <param name="value">가속도 값.</param>
    public void ApplyAcceleration(Vector2 value)
    {
        float dx = Mathf.Cos(radAngle) * Speed;
        float dy = Mathf.Sin(radAngle) * Speed;

        Vector2 velocity = new Vector2(dx, dy);

        velocity += value;

        Angle = Vector2.SignedAngle(Vector2.right, velocity.normalized);
        Speed = velocity.magnitude;
    }


    /// <summary>
    /// 각 Bullet에 Effector 효과를 적용합니다.
    /// </summary>
    /// <param name="bullet"></param>
    private void ApplyEffectors()
    {
        foreach (var effector in Effectors)
        {
            effector.Apply(this);
        }
    }

    public bool OnEvent(IEvent e)
    {
        if (e is DamageEvent)
        {
            SoundManager.Instance.PlayClipAtPoint("PlayerDeflect", transform.position);

            ReturnPool();
            return true;
        }
        return false;
    }

    public void OnStageClearEvent(IEvent e)
    {
        ReturnPool();
    }

}
