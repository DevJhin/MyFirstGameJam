using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 데미지 주는 것은 모두 이 커맨드를 통해서 실행됨.
/// </summary>
public class DamageCommand
{
    private DamageInfo damageInfo;

    public void Execute()
    {
        if (damageInfo.Target != null)
        {
            DamageEvent de = new DamageEvent(damageInfo);
            if (damageInfo.Target is IEventListener)
            {
                MessageSystem.Instance.Send(de, damageInfo.Target as IEventListener);
            }
            else
            {
                Debug.LogError("데미지 이벤트를 받을 대상이 아님. 체크할 것");
            }
        }
    }

    /// <summary>
    /// 데미지 커맨드 생성
    /// </summary>
    public static DamageCommand Create(DamageInfo info)
    {
        var cmd = new DamageCommand();
        cmd.damageInfo = info;

        return cmd;
    }
}

/// <summary>
/// 데미지 관련 정보들은 모두 여기에 넣을 것
/// </summary>
public struct DamageInfo
{
    /// <summary>
    /// 데미지를 보내는 오브젝트
    /// </summary>
    public FieldObject Sender;

    /// <summary>
    /// 데미지를 받을 타겟
    /// </summary>
    public FieldObject Target;

    /// <summary>
    /// 데미지 받을 양
    /// </summary>
    public int amount;
}
