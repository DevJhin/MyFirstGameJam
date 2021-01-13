using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 데미지 판정 등을 줄 때 판단하는 팀 구분 그룹
/// </summary>
[Flags]
public enum EntityGroup
{
    None = 0,
    Friend = 1,
    Enemy = 2,
    Neutral = 4,
}

/// <summary>
/// EntityGroup에서 자주 사용할 법한
/// </summary>
public static class EntityGroupHelper
{
    /// <summary>
    /// 서로 아군인가?
    /// </summary>
    /// <param name="owner">기준</param>
    /// <param name="target">비교 대상</param>
    /// <returns></returns>
    public static bool IsFriendly(this EntityGroup owner, EntityGroup target)
    {
        return (owner & target) != 0;
    }

    /// <summary>
    /// 서로 적군인가?
    /// </summary>
    /// <param name="owner">기준</param>
    /// <param name="target">비교 대상</param>
    /// <returns></returns>
    public static bool IsHostileTo(this EntityGroup owner, EntityGroup target)
    {
        return (owner & target) == 0;
    }
}
