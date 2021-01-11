using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 특정 스크립트에 종속되지 않는 유용한 기능들을 전역 함수가 정리된 클래스
/// </summary>
public class Utility
{
    /// <summary>
    /// 방향 벡터를 입력하면 해당 방향을 X축을 통해 바라보는 각도를 반환합니다. transform.eulerAngles의 Z값에 사용됩니다.
    /// </summary>
    /// <param name="isForwardY">해당 방향을 X축 대신 Y축으로 바라보는 각도를 반환합니다.</param>
    /// <returns></returns>
    public static float LookDirToAngle(Vector2 dir, bool isForwardY = false)
	{
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        return isForwardY ? (angle - 90f) : angle;
    }
}
