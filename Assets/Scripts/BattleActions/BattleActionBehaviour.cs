using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

/// <summary>
/// 배틀액션 행동 구현 부분
/// </summary>
public abstract class BattleActionBehaviour : IDisposable
{
    /// <summary>
    /// 현재 스킬 실행 중인가?
    /// </summary>
    public bool IsActive = false;


    protected FieldObject owner;

    public BattleActionBehaviour(BattleAction action, FieldObject owner)
    {
        this.owner = owner;
    }

    /// <summary>
    /// 처음 시작할 때
    /// </summary>
    public abstract void Start();

    /// <summary>
    /// 매 프레임
    /// </summary>
    public abstract void Update();

    /// <summary>
    /// 끝났을 때
    /// </summary>
    public abstract void Finish();


    public virtual void Dispose()
    {
        owner = null;
    }
}

public static class BattleActionBehaviourFactory
{
    /// <summary>
    /// 배틀 액션 행동 생성하는 부분
    /// </summary>
    /// <param name="ba">생성 데이터</param>
    /// <returns>생성된 액션</returns>
    public static BattleActionBehaviour Create(BattleAction ba, FieldObject owner)
    {
        switch (ba.BattleActionBehaviourName)
        {
            case "SlashActionBehaviour":
                return new SlashActionBehaviour(ba, owner);
            case "InteractActionBehaviour":
                return new InteractActionBehaviour(ba, owner);
            case "SpawnActionBehaviour":
                return new SpawnActionBehaviour(ba, owner);
            default:
                Debug.LogError("Wrong BattleAction Name");
                return null;
        }
    }
}
