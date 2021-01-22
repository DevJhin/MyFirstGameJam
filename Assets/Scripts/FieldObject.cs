using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 필드에 있을 수 있는 캐릭터, 기믹의 최상위 오브젝트
/// </summary>
public class FieldObject : MonoBehaviour, IDisposable
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

    /// <summary>
    /// 애니메이션 조절
    /// </summary>
    public Animator AnimController;

    private void Start()
    {
        AnimController = GetComponent<Animator>();
    }

    /// <summary>
    /// 애니메이션 실행 (임시)
    /// 원래 모든 애니메이션마다 바로 실행시키는게 아니라 Request를 날리고 그걸 검증하는 단계가 필요하지만, 거기까지 구현해야 할까?
    /// </summary>
    public void SetAnimation(string stateName)
    {
        AnimController.Play(stateName);
    }


    public virtual void Dispose()
    {

    }


}
