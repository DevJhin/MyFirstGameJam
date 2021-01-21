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


    [SerializeField] private SpriteRenderer renderer;


    

    public void Start()
    {
        IsActive = true;
        CurrentHP = MaxHP;

        MessageSystem.Instance.Subscribe<ShieldSwitchOffEvent>(OnShieldSwitchOffEvent);
    }


    public void OnDisable()
    {
        MessageSystem.Instance.Unsubscribe<ShieldSwitchOffEvent>(OnShieldSwitchOffEvent);
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


    public void RecoverShield()
    {
        IsActive = true;
        CurrentHP = MaxHP;

        //실드가 회복되었다는 이벤트를 뿌린다.
        MessageSystem.Instance.Publish(new ShieldRecoverEvent());

        renderer.enabled = true;
    }


    public void DisableShield()
    {
        IsActive = false;

        renderer.enabled = false;
    }


}
