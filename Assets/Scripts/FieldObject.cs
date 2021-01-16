using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 필드에 있을 수 있는 캐릭터, 기믹의 최상위 오브젝트
/// </summary>
public class FieldObject : MonoBehaviour
{
    /// <summary>
    /// 현재 체력
    /// </summary>
    public int CurrentHP;

    /// <summary>
    /// 최대 체력
    /// </summary>
    public int MaxHP;

    /// <summary>
    /// 아군, 적군 식별 용 그룹
    /// </summary>
    public EntityGroup EntityGroup;
}
