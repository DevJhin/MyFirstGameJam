using System;
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

    /// <summary>
    /// 애니메이션 조절
    /// </summary>
    protected Animator AnimController;

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

    // 리퀘스트 구조체 만들면 이런 느낌 되지 않을까.
    // 로직은 대충 req.Priority 크기 비교해서 기준에 따라 실행 시킬 듯?
    // 이게 싫으면 유니티 Animator에서 State 잘 만들어야 함.
    // public struct AnimationRequest
    // {
    //     public string StateName;
    //
    //     // 낮든 높든 여튼 기준에 따라서 실행할지 여부를 이걸로 결정할거
    //     public int Priority;
    //
    //     // 이 아래는 필요한 기능들 잡다하게 추가하면 될 듯.
    //     public float AnimationScale;
    // }
}
