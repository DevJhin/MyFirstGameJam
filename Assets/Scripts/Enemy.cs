﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 적 개체 FieldObject.
/// </summary>
public class Enemy : FieldObject, IEventListener
{
    public EnemyPatternAI Controller { get; private set; }

    public EnemyBehaviour Behaviour { get; private set; }

    /// <summary>
    /// 실행중인 ActionSchedule.
    /// </summary>
    public ActionSchedule MainActionSchedule;

    void Awake()
    {
        Controller = new EnemyPatternAI(this);
        Behaviour = new EnemyBehaviour(this);
    }


    void Update()
    {
        Controller.OnUpdate();
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
