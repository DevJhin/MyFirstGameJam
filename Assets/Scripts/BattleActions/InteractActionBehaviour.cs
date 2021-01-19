using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 일정내 범위에 있는 <see cref = "IInteractable"/>한 FieldObject와 상호작용할 수 있는 BattleAction 구현체.
/// </summary>
public class InteractActionBehaviour : BattleActionBehaviour
{
    public float Radius;

    public InteractActionBehaviour(BattleAction ba, FieldObject owner) : base(ba, owner)
    {
        var interactAction = ba as InteractAction;
        Radius = interactAction.InteractRange;
    }

    public override void Finish()
    {
        
    }

    /// <summary>
    /// 주변 반경의 모든 Interactable 한 FieldObject에게 이벤트를 전달합니다.
    /// </summary>
    public override void Start()
    {
        Vector3 center = owner.transform.position;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(center, Radius);

        foreach (var collider in colliders)
        {
            FieldObject target = collider.GetComponent<FieldObject>();

            if (target is IInteractable && target is IEventListener)
            {
                MessageSystem.Instance.Send(new InteractEvent(), target as IEventListener);
            }

        }

    }

    public override void Update()
    {
        
    }

}
