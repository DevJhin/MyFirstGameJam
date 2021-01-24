using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 최종 스테이지 클리어 시 출력되는 UI
/// </summary>
public class UIOnVictory : UIBase
{
    enum Buttons
    {
        RestartBtn
    }
    protected override void Start()
    {
        base.Start();

        Bind<Button>(typeof(Buttons));
        GetButton((int)Buttons.RestartBtn).onClick.AddListener(OnRestart);

        MessageSystem.Instance.Subscribe<StageClearEvent>(OnStageClearEvent);
        gameObject.SetActive(false);
    }

    private void OnStageClearEvent(IEvent e)
    {
        Debug.Log("클리어!");
        if(Game.Instance.IsLastStage)
        {
            gameObject.SetActive(true);
        }
    }

    private void OnRestart()
    {
        Game.Instance.UnloadCurrentStage();
        UIManager.Instance.Clear();
        UIManager.Instance.LoadUI<UIStartMenu>("UIStartMenu");
    }

    private void OnDisable()
    {
        MessageSystem.Instance.Unsubscribe<StageClearEvent>(OnStageClearEvent);
    }
}
