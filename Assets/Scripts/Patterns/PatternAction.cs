using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 실행할 Action들을 관리하고, 이들의 실행 방식을 정의합니다.
/// </summary>
public class PatternAction : ScriptableObject
{
    /// <summary>
    /// 패턴 동작을 실행합니다.
    /// </summary>
    /// <param name="actor">행동의 주체가 되는 FieldObject 객체.</param>
    public virtual void Execute(FieldObject actor)
    {

        
    }


}
