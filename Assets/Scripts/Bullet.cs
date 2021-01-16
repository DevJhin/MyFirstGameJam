using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// 피해를 입히는 모든 투사체에 적용되는 스크립트. 지정된 속도와 방향각에 따라 이동합니다.
/// Collider2D가 없을 경우 자동으로 부착되나 충돌 범위 지정을 위해 프리팹에 직접 콜라이더 컴포넌트를 세팅할 것을 권장합니다.
/// </summary>
public class Bullet : FieldObject
{
    /// <summary>
    /// 총을 쏜 주체를 입력합니다. 이 대상은 자신이 쏜 총알에 맞지 않습니다.
    /// </summary>
    public FieldObject attacker;

    public float speed;
    public float damage = 1f;

    /// <summary>
    /// 투사체의 진행 방향을 라디안 값으로 나타냅니다. (360도 기준으로 제어하려면 DirectionDegree를 사용하세요)
    /// </summary>
    private float directionAngle;

    /// <summary>
    /// 투사체의 진행 방향을 0~360도로 나타냅니다.
    /// </summary>
    [ShowInInspector, ReadOnly]
    public float DirectionDegree
    {
        get => directionAngle * Mathf.Rad2Deg;
        set => directionAngle = value * Mathf.Deg2Rad;
    }

    [HideInInspector] public float posX;
    [HideInInspector] public float posY;

    [InfoBox("true일 경우 투사체가 진행 방향을 바라봅니다. 완전한 원형 투사체 등 회전시키지 않아도 된다면 false입니다."),
     SerializeField]
    private bool hasDirection = true;

    [ShowIf(nameof(hasDirection))] public bool isAutoRotate;
    [ShowIf(nameof(isAutoRotate))] public float autoRotateSpeed;

    [SerializeField] private SpriteRenderer _spriteRenderer;

    /// <summary>
    /// 이 풀링 오브젝트의 풀
    /// </summary>
    private Pool pool;

    /// <summary>
    /// 이 프로젝타일이 사용하고 있는 풀링된 오브젝트
    /// </summary>
    private PooledObject pooledObject;

    public SpriteRenderer SpriteRenderer
    {
        get
        {
            if (_spriteRenderer == null)
                _spriteRenderer = GetComponent<SpriteRenderer>();

            return _spriteRenderer;
        }
    }

    // 화면 밖으로 나가고 해당 시간이 경과하면 파괴됩니다. 생성 시 또는 카메라 범위 내에 있을 경우 초기화됩니다.
    float timeToDisable = 0.5f;
    float invisibleTime = 0f;

    private void Start()
    {
        // FIXME: bullet 안에 pooledObject랑 pool을 넣는 것 자체가 좋은 코드 아님. 독립시켜야 함.
        pooledObject = GetComponent<PooledObject>();
        pool = PoolManager.GetOrCreate(pooledObject.OriginalObjectName);
    }

    private void OnEnable()
    {
        invisibleTime = 0f;

        if (!hasDirection)
            isAutoRotate = false;

        var col = gameObject.GetComponent<Collider2D>();
        if (col == null)
        {
            col = gameObject.AddComponent<CircleCollider2D>();
        }

        col.isTrigger = true;
    }

    void Update()
    {
        Move();

        // 오브젝트가 카메라 렌더링 대상에서 벗어난 지 일정 시간이 경과하면 비활성화됩니다.
        if (!SpriteRenderer.isVisible)
        {
            invisibleTime += Time.deltaTime;
            if (invisibleTime > timeToDisable)
            {
                // FIXME 이런 식으로 해제하는건 옳지 않음.
                pool.Dispose(pooledObject);
            }
        }
        else if (invisibleTime > 0f)
        {
            invisibleTime = 0f;
        }
    }

    /// <summary>
    /// 투사체 이동 로직으로 매 프레임 호출됩니다.
    /// hasDirection == true일 경우 투사체의 X축이 이동 방향을 바라보게 됩니다.
    /// </summary>
    private void Move()
    {
        if (speed == 0f || !gameObject.activeSelf)
            return;

        posX += Mathf.Cos(directionAngle) * speed * Time.deltaTime;
        posY += Mathf.Sin(directionAngle) * speed * Time.deltaTime;

        Vector3 nextPos = new Vector3(posX, posY, transform.position.z);

        if (isAutoRotate)
        {
            transform.eulerAngles += new Vector3(0f, 0f, autoRotateSpeed * Time.deltaTime);
        }
        else if (hasDirection && transform.position != nextPos)
        {
            transform.eulerAngles = new Vector3(0f, 0f, DirectionDegree);
        }

        transform.position = nextPos;
    }

    /// <summary>
    /// 적 콜라이더에 닿은 투사체는 피해를 입히고 비활성화됩니다.
    /// </summary>
    // TODO: 적인지 감지하는 로직은 추후 EntityGroupHelper.IsHostileTo(entity.EntityGroup, attacker.EntityGroup) 와 같이 수정할 것
    private void OnTriggerEnter2D(Collider2D other)
    {
        FieldObject target = other.GetComponent<FieldObject>();
        if (target == null)
            target = other.GetComponentInParent<FieldObject>();

        // 적이면 데미지 가함
        if (attacker.EntityGroup.IsHostileTo(target.EntityGroup))
        {
            var info = new DamageInfo
            {
                Sender = attacker,
                Target = target,
                amount = 10
            };
            var cmd = DamageCommand.Create(info);
            cmd.Execute();

            // FIXME 여기에 넣는게 맞을까?
            pool.Dispose(pooledObject);
        }
    }
}
