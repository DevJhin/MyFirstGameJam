using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <see cref = "InteractEvent"/> 이벤트 수신시, Stage 전환을 실행하는 FieldObject.
/// </summary>
public class StageGate : FieldObject, IEventListener, IInteractable
{
    /// <summary>
    /// 전환할 맵 이름.
    /// </summary>
    public string NextMapName;

    /// <summary>
    /// 이 문이 사용되었는가?(버튼 연타로 인한 중복 Stage 전환 방지)
    /// </summary>
    private bool IsUsed = false;


    public bool OnEvent(IEvent e)
    {
        if (e is InteractEvent)
        {
            if (!IsUsed)
            {
                Game.Instance.UnloadCurrentStage();
                Game.Instance.LoadStage(NextMapName);
                IsUsed = true;
                return true;
            }
        }

        return false;
    }

}
