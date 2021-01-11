using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 피해를 입히는 모든 투사체에 적용되는 스크립트. 지정된 속도와 방향각에 따라 이동합니다.
/// </summary>
public class Bullet : FieldObject
{
    public float speed;

    /// <summary>
    /// 투사체의 진행 방향을 라디안 값으로 나타냅니다. (360도 기준으로 제어하려면 DirectionDegree를 사용하세요)
    /// </summary>
    private float directionAngle;
    /// <summary>
    /// 투사체의 진행 방향을 0~360도로 나타냅니다. 
    /// </summary>
    public float DirectionDegree
	{
        get => directionAngle * Mathf.Rad2Deg;
        set => directionAngle = value * Mathf.Deg2Rad;
	}

    // TODO : 타겟 레이어 설정 및 유효한 타겟에 충돌 시 처리
    LayerMask target;

    [HideInInspector] public float posX;
    [HideInInspector] public float posY;

    // 회전시키지 않아도 된다면 true
    [SerializeField] private bool notRotate = true;
    public bool isAutoRotate;
    public float autoRotateSpeed;
    
    [SerializeField] private SpriteRenderer _spriteRenderer;
    public SpriteRenderer SpriteRenderer
	{
		get
		{
            if(_spriteRenderer == null)
                _spriteRenderer = GetComponent<SpriteRenderer>();

            return _spriteRenderer;
        }
	}
    
    // 화면 밖으로 나가고 일정 시간이 지나면 파괴됩니다.
    float timeToDisable = 0.5f;
    float invisibleTime;

	private void OnEnable()
	{
        invisibleTime = 0f;
    }
	void Update()
    {
        posX += Mathf.Cos(directionAngle) * speed * Time.deltaTime;
        posY += Mathf.Sin(directionAngle) * speed * Time.deltaTime;

        Vector3 nextPos = new Vector3(posX, posY, transform.position.z);

        if (!notRotate && transform.position != nextPos)
		{
            float angle = Utility.LookDirToAngle(nextPos - transform.position);
            transform.eulerAngles = new Vector3(0f, 0f, DirectionDegree);
        }
        else if (isAutoRotate)
        {
            transform.eulerAngles += new Vector3(0f, 0f, autoRotateSpeed * Time.deltaTime);
        }

        transform.position = nextPos;
		
        if (!SpriteRenderer.isVisible)
        {
            invisibleTime += Time.deltaTime;
            if (invisibleTime > timeToDisable)
            {
                // TODO: 오브젝트 풀링 도입 시 비활성화 처리
                Destroy(gameObject);
            }
        }
    }

}
