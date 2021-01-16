using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

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
    public Rigidbody2D rb;
    public BoxCollider2D col;

    [Header("MOVEMENT")]
    public float moveSpeed;
    public float airDrag;
    public float jumpPower;
    public float gravityScale;
    public float fallMultiplier;
    public float lowJumpFallMultiplier;
    public float jumpCheckTimer;
    public float raycastLength;

    // Temp: 현재 땅에 붙어있는지 디버깅용
    [Button("Debug: Is Grounded?")]
    private bool IsGrounded() => Behavior.IsGroundedDebug;

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
        Debug.Log($"[{gameObject.name}] {damageEvent.attacker.name}의 공격으로 {damageEvent.damage}의 피해를 입었습니다. (피격 지점: {damageEvent.hitPoint})");
    }

}
