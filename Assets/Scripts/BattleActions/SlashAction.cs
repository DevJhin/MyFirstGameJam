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
    /// 공격 반경.
    /// </summary>
    public float Radius;

    /// <summary>
    /// 공격 각도 범위.
    /// </summary>
    public float Angle;

    /// <summary>
    /// CircleCast 거리.
    /// </summary>
    public float CircleCastDistance;

    /// <summary>
    /// 공격 가능 대상 Collider LayerMask.
    /// </summary>
    public LayerMask CollideLayerMask;


    private readonly static Color DebugColor = new Color(0, 1, 0, 0.3f);

    public override void Execute(FieldObject actor)
    {
        Vector2 point = actor.transform.position;
        Vector2 forward = actor.transform.right;

        var hitInfos = Physics2D.CircleCastAll(point, Radius, forward, CircleCastDistance, CollideLayerMask.value);

        float halfAngle = Angle * 0.5f;

        // 이번 공격으로 피격된 대상은 이곳에 저장됩니다. Execute 1회 실행에 1회 이상 피격당하지 않습니다.
        List<IEventListener> attackedEntity = new List<IEventListener>();

        foreach (var hitInfo in hitInfos)
        {
            //범위 내에서 충돌이 발생한 Collider만 충돌로 판정해야 한다.
            Vector2 dir = (hitInfo.point - point).normalized;
            float angle = Vector2.Angle(forward, dir);
            bool withinRange = angle < halfAngle;

            if (withinRange)
            {
                //범위 내 Collider인 경우, 충돌 이벤트 처리.
                // 콜라이더 자신 또는 부모 오브젝트에 IEventListener가 존재할 경우 이벤트를 발생시킵니다.
                FieldObject target = hitInfo.collider.GetComponent<FieldObject>();
                if(target == null)
                    target = hitInfo.collider.GetComponentInParent<FieldObject>();

                // 적이면 데미지 가함
                if (actor.EntityGroup.IsHostileTo(target.EntityGroup))
                {
                    var info = new DamageInfo
                    {
                        Sender = actor,
                        Target = target,
                        amount = 10
                    };
                    var cmd = DamageCommand.Create(info);
                    cmd.Execute();
                }

                GLDebug.DrawLine(point, hitInfo.point, Color.red, time: 1f);
            }
            else
            {
                //범위 밖 Collider인 경우, 충돌 이벤트 처리 수행하지 않음.
                GLDebug.DrawLine(point, hitInfo.point, Color.white, time: 1f);
            }
        }

        GLDebug.DrawSector(new Circle(point, Radius, forward), Angle, DebugColor, 1f);

    }

}
