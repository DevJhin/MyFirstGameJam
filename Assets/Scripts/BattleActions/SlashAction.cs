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
        foreach (var hitInfo in hitInfos)
        {
            //범위 내에서 충돌이 발생한 Collider만 충돌로 판정해야 한다.
            Vector2 dir = (hitInfo.point - point).normalized;
            float angle = Vector2.Angle(forward, dir);
            bool withinRange = angle < halfAngle;
            
            if (withinRange)
            {
                //범위 내 Collider인 경우, 충돌 이벤트 처리.
                //TODO: 충돌 이벤트 처리 작업 추가할 것.
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
