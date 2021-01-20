using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// 타원 궤도를 움직일 수 있는 BulletPattern.
/// </summary>
public class EllipseMovePatternObject : PatternObject
{
    /// <summary>
    /// 특정 위치를 항상 향하게 만들 BulletEmitter.
    /// </summary>
    public BulletEmitter LookAtEmitter;

    public bool EmitterLookAt = true;

    /// <summary>
    /// 궤도 중심이 되는 Transform.
    /// </summary>
    [Title("Revolve Settings")]
    public Transform Target;

    /// <summary>
    /// 타원 궤도의 X축 반경.
    /// </summary>
    public float XRadius;

    /// <summary>
    /// 타원 궤도의 Y축 반경.
    /// </summary>
    public float YRadius;

    /// <summary>
    /// 타원 궤도에서의 초기 각도(Degree).
    /// </summary>
    [Range(0, 360)] 
    public float InitialAngle;

    /// <summary>
    /// 타원 궤도 이동 주기.
    /// </summary>
    public float Frequency;



    /// <summary>
    /// 자전 속도.
    /// </summary>
    [Title("Rotation Settings")]
    public float RotateAngularSpeed;

    /// <summary>
    /// 타원 궤도에서의 위치를 결정하는 변수.
    /// </summary>
    private float t;


    /// <summary>
    /// PI * 2.
    /// </summary>
    private readonly static float DoublePI = Mathf.PI * 2;



    public override void OnPatternStart(BulletPattern pattern)
    {
        //초기 각도에 따라 t 값을 설정한다.
        t = InitialAngle * Mathf.Deg2Rad;

    }

    public override void OnPatternFinish(BulletPattern pattern)
    {
        
        
    }




    void Update()
    { 
        t += Time.deltaTime * Frequency * DoublePI * LookAtEmitter.spec.value.GetSimulationTime();

        if (Target != null)
        { 
            EllipseRotate();
        }

        if (Target!=null && LookAtEmitter != null)
        {
            if (EmitterLookAt)
            { 
                LookAt2D(LookAtEmitter.transform, Target.position);
            }
        }
    
        transform.Rotate(0, 0, RotateAngularSpeed*Time.deltaTime);   
    }

    /// <summary>
    /// 타원 공전을 수행합니다.
    /// </summary>
    void EllipseRotate()
    {
        float x = Mathf.Cos(t) * XRadius;
        float y = Mathf.Sin(t) * YRadius;

        Vector3 pos = new Vector3(x, y, 0);
        pos += Target.position;
        
        transform.position = pos;
        
    }


    /// <summary>
    /// 2D 좌표계에서 transform.right가 특정한 위치를 바라보도록 설정.
    /// </summary>
    /// <param name="actor">방향을 변경할 대상 Transform.</param>
    /// <param name="target">대상 위치.</param>
    public static void LookAt2D(Transform actor, Vector2 target)
    {
        actor.right = target - (Vector2)actor.position;


    }


}
