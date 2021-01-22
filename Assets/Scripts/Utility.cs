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

    /// <summary>
    /// 오브젝트에서 지정한 컴포넌트를 찾아서 반환합니다. 없으면 컴포넌트를 추가한 후 반환합니다.
    /// </summary>
    public static T GetOrAddComponent<T>(GameObject obj) where T : Component
    {
        T component = obj.GetComponent<T>();

        return (component != null) ? component : obj.AddComponent<T>();
    }

    /// <summary>
    /// 입력한 이름의 자식 오브젝트를 찾아서 T 타입의 컴포넌트를 반환합니다.  
    /// UI Canvas 생성 시 산하 요소를 탐색하여 바인딩할 때 사용됩니다.
    /// </summary>
    /// <returns>타입이 GameObject일 경우 오브젝트를 반환하며, 그 외에는 오브젝트에 붙어 있는 T 타입의 컴포넌트를 반환합니다. 없을 경우 null을 반환합니다. 탐색에 실패한 경우 null을 반환합니다.</returns>
	public static T FindChild<T>(GameObject parent, string name) where T : Object
	{
		if ((parent == null) || (string.IsNullOrEmpty(name)))
			return null;

        if(typeof(T) ==typeof(GameObject))
		{
            Transform tr = FindChild<Transform>(parent, name);
            if (tr == null)
                return null;

            return tr.gameObject as T;
        }

        foreach (T component in parent.GetComponentsInChildren<T>())
        {
            if (component.name == name)
                return component;
        }

        return null;
	}
}

public static class CustomUtilityExtension
{
    public static T GetOrAddComponent<T>(this GameObject obj) where T : Component
    {
        return Utility.GetOrAddComponent<T>(obj);
    }
}