using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 가속도를 Bullet에 반영하여 속도를 변경할 수 있는 Effector.
/// </summary>
[CreateAssetMenu(menuName = "MyFirstGameJam/Bullet Effectors/Velocity Effector")]
public class VelocityEffector : BulletEffector
{
    /// <summary>
    /// 가속도 값.
    /// </summary>
    public Vector2 Acceleration;


    /// <summary>
    /// 가속도를 Bullet에 적용합니다.
    /// </summary>
    public override void Apply(Bullet bullet)
    {
        bullet.ApplyAcceleration(Acceleration);
    }

}
