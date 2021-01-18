using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Emitter에 의해 사용 중인 Bullet의 상태를 변경할 수 있는 클래스.
/// </summary>
public abstract class BulletEffector : ScriptableObject
{
    /// <summary>
    /// 매 프레임마다 Bullet에게 적용되는 동작.
    /// </summary>
    /// <param name="bullet">효과를 적용할 Bullet.</param>
    public abstract void Apply(Bullet bullet);
}
