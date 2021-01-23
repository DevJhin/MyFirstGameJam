using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 가속도를 Bullet에 반영하여 속도를 변경할 수 있는 Effector.
/// </summary>
[CreateAssetMenu(menuName = "MyFirstGameJam/Bullet Effectors/Speed Effector", fileName = "New SpeedEffector")]
public class SpeedEffector : BulletEffector
{
    public float StartSpeed;
    public float EndSpeed;


    /// <summary>
    /// 가속도를 Bullet에 적용합니다.
    /// </summary>
    public override void Apply(Bullet bullet)
    {
        float t = 1 - bullet.RemainingLifetime()/bullet.Lifetime;

        bullet.Speed = Mathf.Lerp(StartSpeed, EndSpeed, t);
    }

}
