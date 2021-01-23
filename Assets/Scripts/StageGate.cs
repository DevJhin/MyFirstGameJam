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
    /// 이 문을 사용할 수 있는가?(버튼 연타로 인한 중복 Stage 전환 방지)
    /// </summary>
    private bool IsActive = false;

    public void Start()
    {
        MessageSystem.Instance.Subscribe<StageClearEvent>(OnStageClearEvent);
        gameObject.SetActive(false);
    }

    public void OnDisable()
    {
        MessageSystem.Instance.Unsubscribe<ShieldRecoverEvent>(OnStageClearEvent);
    }


    /// <summary>
    /// StageClearEvent의 Callback.
    /// </summary>
    public void OnStageClearEvent(IEvent e)
    {
        IsActive = true;
        gameObject.SetActive(true);
    }



    public bool OnEvent(IEvent e)
    {
        if (e is InteractEvent)
        {
            if (IsActive)
            {
                Game.Instance.UnloadCurrentStage();
                Game.Instance.LoadStage(NextMapName);
                IsActive = false;

                return true;
            }
        }

        return false;
    }

    public override void Dispose()
    {
        base.Dispose();
        MessageSystem.Instance.Unsubscribe<ShieldRecoverEvent>(OnStageClearEvent);
    }
}
