using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : FieldObject, IEventListener
{
    public PlayerBehavior Behavior
    {
        get; private set;
    }

    public PlayerController Controller
    {
        get; private set;
    }

    [Header("REFERENCES")]
    public LayerMask groundLayerMask;
    //public Rigidbody2D rb;
    public BoxCollider2D col;

    [Header("MOVEMENT")]
    public float moveSpeed;
    public float airDrag;
    public float jumpPower;
    /// <summary>
    /// 천장에 부딪히면 부딪힌 속도의 일정 비율로 튕겨나오는 비율을 결정합니다. 
    /// 0일 경우 얼마나 빠르게 부딪히든 동일하게 낙하하며, 1일 경우 부딪힐 당시의 속도와 동일합니다.
    /// </summary>
    public float ceilingReactionForceRatio;
    public float fallMultiplier;
    public float lowJumpFallMultiplier;
    public float jumpCheckTimer;
    public float raycastLength;
    public float xDirection;

    /// <summary>
    /// Attack 커맨드 입력시, 실행할 BattleAction.
    /// </summary>
    public BattleAction AttackBattleAction;

    void Awake()
    {
        Behavior = new PlayerBehavior(this);
        Controller = new PlayerController(this);
    }

    private void Update()
    {
        Behavior.OnPlayerUpdate();
    }

    public bool OnEvent(IEvent e)
    {
        if (e is DamageEvent)
        {
            DamageEvent damageEvent = e as DamageEvent;
            OnDamaged(damageEvent);
            return true;
        }

        return false;
    }

    void OnDamaged(DamageEvent damageEvent)
    {
        // TODO: 데미지 받았을 때 처리 추가
        CurrentHP -= damageEvent.damageInfo.amount;
    }
}
