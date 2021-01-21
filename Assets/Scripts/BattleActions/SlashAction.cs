using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 부채꼴 영역 내의 적을 공격하는 Action.
/// </summary>
[CreateAssetMenu(menuName = "MyFirstGameJam/BattleActions/Slash", fileName = "New SlashBattleAction")]
public class SlashAction : BattleAction
{
    /// <summary>
    /// 1회 공격 피해량.
    /// </summary>
    public int Damage;

    /// <summary>
    /// 공격 반경.
    /// </summary>
    public float Radius;

    /// <summary>
    /// 공격 각도 범위.
    /// </summary>
    public float Angle;

    /// <summary>
    /// BattleAction 실행 시간.
    /// </summary>
    public float duration = 0.5f;

    /// <summary>
    /// CircleCast 거리.
    /// </summary>
    public float CircleCastDistance;

    /// <summary>
    /// 공격 가능 대상 Collider LayerMask.
    /// </summary>
    public LayerMask CollideLayerMask;



}
