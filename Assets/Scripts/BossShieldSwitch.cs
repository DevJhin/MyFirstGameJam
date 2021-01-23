using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// BossShield를 비활성화하기 위해 사용되는 Switch 기믹.
/// 플레이어가 공격하면 비활성화 되어 Shield의 체력을 깎는 이벤트를 Publish 합니다.
/// 실드가 재생되면 다시 활성화됩니다.
/// </summary>

public class BossShieldSwitch : FieldObject
{


    /// <summary>
    /// 이 Switch 기믹에 사용되는 EventTrigger.
    /// </summary>
    public DamageEventTrigger owningEventTrigger;

    /// <summary>
    /// 보호막이 활성화 되어있는 상태인가?
    /// </summary>
    public bool IsActive { get; private set; }

    private SpriteRenderer shieldRenderer;

    /// <summary>
    /// 불 이펙트
    /// </summary>
    public GameObject fireParticle;

    public void Start()
    {
        shieldRenderer = GetComponentInChildren<SpriteRenderer>();

        EnableSwitch();

        MessageSystem.Instance.Subscribe<TriggerEvent>(OnTriggerEvent);
        MessageSystem.Instance.Subscribe<ShieldRecoverEvent>(OnShieldRecoverEvent);
        MessageSystem.Instance.Subscribe<StageClearEvent>(OnStageClearEvent);
    }


    public void OnDisable()
    {
        MessageSystem.Instance.Unsubscribe<TriggerEvent>(OnTriggerEvent);
        MessageSystem.Instance.Unsubscribe<ShieldRecoverEvent>(OnShieldRecoverEvent);
        MessageSystem.Instance.Unsubscribe<StageClearEvent>(OnStageClearEvent);
    }


    /// <summary>
    /// ShieldRecoverEvent의 Callback. 실드가 재생되면 활성화상태로 변경.
    /// </summary>
    public void OnShieldRecoverEvent(IEvent e)
    {
        if (IsActive) return;

        EnableSwitch();

    }


    /// <summary>
    /// TriggerEvent의 Callback. 피격당하면 비활성화상태로 변경.
    /// </summary>
    /// <param name="e"></param>
    public void OnTriggerEvent(IEvent e)
    {
        if (!IsActive) return;

        var dte = e as TriggerEvent;

        // 이 이벤트는 다른 Switch의 EventTrigger가 보낸 것일 수도 있다.
        // 따라서, 받은 TriggerEvent가 내가 가지고 있는 EventTrigger가 보낸 것인지 확인.
        if (dte.Sender == owningEventTrigger)
        {
            MessageSystem.Instance.Publish(new ShieldSwitchOffEvent());

            SoundManager.Instance.PlayClipAtPoint("EnemyHurt", transform.position);

            DisableSwitch();


        }
    }


    /// <summary>
    /// StageClearEvent의 Callback. 완전 비활성화 되었음을 알리기 위해 임시로 검정으로 변경.
    /// </summary>
    public void OnStageClearEvent(IEvent e)
    {
        DisableSwitch();

    }



    /// <summary>
    /// 스위치를 활성화(때려야 되는) 상태로 만듭니다.
    /// </summary>
    public void EnableSwitch()
    {
        IsActive = true;
        owningEventTrigger.IsActive = true;

        fireParticle.SetActive(true);
    }


    /// <summary>
    /// 스위치를 비활성화(이미 때린) 상태로 만듭니다.
    /// </summary>
    public void DisableSwitch()
    {
        IsActive = false;
        owningEventTrigger.IsActive = false;

        fireParticle.SetActive(false);

    }


}
