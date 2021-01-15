﻿using System.Collections;
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

    // TODO: 피격 시 이벤트 처리 플레이어에도 추가할 것.
    //       지금은 데미지 처리 테스트 중으로, 이 이벤트는 Enemy에만 적용되어 있습니다.
    
	public bool OnEvent(IEvent e)
	{
        if (e is DamageMessage)
        {
            DamageMessage damageMessage = e as DamageMessage;
            //OnDamaged(damageMessage);
            return true;
        }

        return false;
    }
    /*
    void OnDamaged(DamageMessage damageMessage)
    {
        // TODO: 데미지 받았을 때 처리 추가
        Debug.Log($"[{gameObject.name}] {damageMessage.attacker.name}의 공격으로 {damageMessage.damage}의 피해를 입었습니다. (피격 지점: {damageMessage.hitPoint})");
    }
    */
}
