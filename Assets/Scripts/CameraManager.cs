using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 카메라 로직들은 전부 여기서 조작
/// </summary>
public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    /// <summary>
    /// 따라갈 타겟
    /// </summary>
    public Transform FollowTarget;

    [Header("Camera Follow Variables")]

    /// <summary>
    /// 카메라가 타겟을 쫓아가는데 걸리는 시간
    /// </summary>
    public float FollowDuration = 0.05f;

    /// <summary>
    /// 카메라가 타겟을 따라가기 시작하는 카메라와 타겟의 거리 (X)
    /// </summary>
    public float StartFollowX = 1;

    /// <summary>
    /// 카메라가 타겟을 따라가기 시작하는 카메라와 타겟의 거리 (Y)
    /// </summary>
    public float StartFollowY = 1;

    /// <summary>
    /// 카메라가 움직일 방향으로 타겟보다 얼마나 떨어져있을 것인지?
    /// </summary>
    public float FocusDistance = 1;

    /// <summary>
    /// 디버그 모드
    /// </summary>
    public bool DebugMode = false;

    /// <summary>
    /// 전체적인 렌더링 담당할 메인 카메라
    /// </summary>
    private Camera mainCam;

    /// <summary>
    /// SmoothDamp velocity
    /// </summary>
    Vector3 refVel = Vector3.zero;

    void Awake()
    {
        mainCam = Camera.main;
    }

    void LateUpdate()
    {
        if (DebugMode)
        {
            
        }

        Vector3 finalPos = transform.position;

        if (FollowTarget)
        {
            Vector3 dest = FollowTarget.position;
            dest.z = transform.position.z;

            Vector3 dampPos = Vector3.SmoothDamp(transform.position, dest, ref refVel, FollowDuration);

            finalPos = dampPos;
        }

        // 위치 변경되면 그 때 업데이트
        if (finalPos != transform.position)
        {
            transform.position = finalPos;
        }
    }
}
