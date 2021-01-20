using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// BulletPattern이 관리하는 객체. PatternObject는 반드시 BulletPattern과 함계 사용하는 것을 권장합니다.
/// </summary>
public class PatternObject : FieldObject
{
    /// <summary>
    /// 이 PatternObject를 보유한 BulletPattern.
    /// </summary>
    public BulletPattern OwnerPattern;


    /// <summary>
    /// 패턴이 시작될 때 호출되는 메서드.
    /// </summary>
    /// <param name="pattern"></param>
    public virtual void OnPatternStart(BulletPattern pattern)
    { 
        
    }


    /// <summary>
    /// 패턴이 종료되었을 때 호출되는 메서드.
    /// </summary>
    /// <param name="pattern">종료된 패턴</param>
    public virtual void OnPatternFinish(BulletPattern pattern)
    { 
    
    }
}
