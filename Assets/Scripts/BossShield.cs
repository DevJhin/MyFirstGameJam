using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 보스 실드 기믹. <see cref = "BossShieldSwitch"/>와 이벤트를 주고받으며 동작.
/// </summary>
public class BossShield : FieldObject
{
    /// <summary>
    /// 이 실드의 보유자.
    /// </summary>
    public Enemy OwnerEnemy;

    /// <summary>
    /// 보호막이 활성화 되어있는 상태인가?
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// 실드 회복에 소모되는 시간.
    /// </summary>
    public float ShieldRecoveryTime;


    private SpriteRenderer bodyRendrerer;
    private Collider2D bodyCollider;

    

    public void Start()
    {
        bodyRendrerer = GetComponentInChildren<SpriteRenderer>();
        bodyCollider = GetComponentInChildren<Collider2D>();

        RecoverShield();
       
        MessageSystem.Instance.Subscribe<ShieldSwitchOffEvent>(OnShieldSwitchOffEvent);
        MessageSystem.Instance.Subscribe<StageClearEvent>(OnStageClearEvent);
    }


    public void OnDisable()
    {
        MessageSystem.Instance.Unsubscribe<ShieldSwitchOffEvent>(OnShieldSwitchOffEvent);
        MessageSystem.Instance.Unsubscribe<StageClearEvent>(OnStageClearEvent);
    }


    /// <summary>
    /// Shield의 스위치 이벤틀
    /// </summary>
    /// <param name="e"></param>
    public void OnShieldSwitchOffEvent(IEvent e)
    {
        if (!IsActive) return;

        var dte = e as ShieldSwitchOffEvent;


        CurrentHP -= 1;

        //모든 스위치가 모두 비활성화 되었을 경우, 실드를 비활성화시킨다.
        if (CurrentHP == 0)
        {
            DisableShield();

            // 일정시간 후에 실드를 복수한다.
            // FIXME: Invoke는 편의상 쓰고 나중에 수정할 것.
            Invoke("RecoverShield", ShieldRecoveryTime);
        }

    }


    /// <summary>
    /// StageClearEvent의 Callback. 실드 작동을 정지해야 함.
    /// </summary>
    public void OnStageClearEvent(IEvent e)
    {
        //실드 재생을 즉시 중단하고 비활성화시켜야 한다.
        DisableShield();

    }


    /// <summary>
    /// 보스의 실드를 복구하는 동작을 수행.
    /// </summary>
    public void RecoverShield()
    {
        if (OwnerEnemy.IsDead) return;

        IsActive = true;
        CurrentHP = MaxHP;

        //적 개체를 다시 무적으로 설정.
        OwnerEnemy.IsInvinsible = true;
        
        //실드가 회복되었다는 이벤트를 뿌린다.
        MessageSystem.Instance.Publish(new ShieldRecoverEvent());

        bodyRendrerer.enabled = true;
        bodyCollider.enabled = true;
    }


    /// <summary>
    /// 보스의 실드를 비활성화한다.
    /// </summary>
    public void DisableShield()
    {
        IsActive = false;

        //적 개체의 무적상태를 비활성화.
        OwnerEnemy.IsInvinsible = false;

        bodyRendrerer.enabled = false;
        bodyCollider.enabled = false;
    }


    public override void Dispose()
    {
        base.Dispose();

        OwnerEnemy = null;

        bodyRendrerer = null;
        bodyCollider = null;

    }


}
