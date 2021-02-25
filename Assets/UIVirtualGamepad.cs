using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

using TouchPhase = UnityEngine.InputSystem.TouchPhase;


public class UIVirtualGamepad : UIBase
{
    /// <summary>
    /// FIXME: 
    /// Gamepad UI는 LetterBox 위에 렌더링을 할 수 있어야 좋을 것 같은데 
    /// 현재 UIManager 구조상 UIBase를 GameCanvas에만 추가할 수 있어, 
    /// LetterBoxCanvas에 UI요소를 추가하는 것이 불가능함.
    /// 따라서, 임시로 Scene의 LetterBoxCanvas에 직접 추가하였음.
    /// </summary>
    protected override void Start()
    {
        base.Start();
        MessageSystem.Instance.Subscribe<StageLoadEvent>(OnStageLoadEvent);
    }

    private void OnDestroy()
    {
        MessageSystem.Instance.Unsubscribe<StageLoadEvent>(OnStageLoadEvent);
    }

    public void OnStageLoadEvent(IEvent e)
    {
        gameObject.SetActive(true);
    }


}