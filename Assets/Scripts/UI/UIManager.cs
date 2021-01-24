using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI의 생성/관리/해제를 담당하는 클래스
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    /// <summary>
    /// 생성된 UI의 Sorting Order를 지정하기 위해 사용됩니다. UI가 생성될 때마다 1씩 증가합니다.
    /// </summary>
    private int currentOrder = 10;

    /// <summary>
    /// 생성된 UI를 보관하거나 탐색할 때 사용할 리스트
    /// </summary>
    private LinkedList<UIBase> uIBases = new LinkedList<UIBase>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        Clear();
    }

    /// <summary>
    /// Resources/UI 폴더에서 UI Canvas 프리팹을 로딩해서 생성합니다. 생성된 요소는 UI Manager에 보관됩니다.
    /// </summary>
    /// <typeparam name="T">캔버스에 부착될 컴포넌트</typeparam>
    /// <param name="name">프리팹 이름이 컴포넌트와 다를 경우 직접 입력합니다</param>
    public T LoadUI<T>(string name = null) where T : UIBase
    {
        if (string.IsNullOrEmpty(name))
            return null;

        var origin = Resources.Load($"UI/{name}");
        var obj = GameObject.Instantiate(origin, transform) as GameObject;
        T uiBase = obj.GetOrAddComponent<T>();

        uiBase.SetCanvasOrder(currentOrder++);
        uIBases.AddLast(uiBase);

        return uiBase;
    }

    /// <summary>
    /// 입력한 타입의 UI를 삭제합니다.
    /// </summary>
    /// <typeparam name="T">삭제할 UI Canvas에 부착된 컴포넌트명</typeparam>
    /// <returns>삭제에 성공하면 true, 현재 활성화된 UI 중에 해당 타입의 컴포넌트를 찾지 못했다면 false를 반환합니다.</returns>
    public bool CloseUI<T>() where T : UIBase
    {
        foreach (UIBase ui in uIBases)
        {
            if (ui is T)
            {
                uIBases.Remove(ui);
                GameObject.Destroy(ui.gameObject);
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 생성된 모든 UI 오브젝트를 파괴하고, Sorting Layer를 기본값으로 되돌립니다.
    /// </summary>
    public void Clear()
    {
        while (uIBases.Count > 0)
        {
            var ui = uIBases.Last.Value;
            uIBases.RemoveLast();
            if (ui.gameObject != null)
                GameObject.Destroy(ui.gameObject);
        }

        currentOrder = 10;
    }
}
