using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// 타원 궤도를 움직일 수 있는 BulletPattern.
/// </summary>
public class RevolvePatternObject : PatternObject
{

    /// <summary>
    /// 자전 속도.
    /// </summary>
    [Title("Rotation Settings")]
    public float RotateAngularSpeed;


    public override void OnPatternStart(BulletPattern pattern)
    {

    }

    public override void OnPatternFinish(BulletPattern pattern)
    {
        
        
    }


    void Update()
    { 
    
        transform.Rotate(0, 0, RotateAngularSpeed*Time.deltaTime);   
    }


}
