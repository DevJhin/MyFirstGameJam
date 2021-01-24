using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 최종 스테이지 클리어 시 출력되는 UI
/// </summary>
// TODO: 승리 시 이 UI를 생성해야 함
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
    }

    private void OnRestart()
    {
        Game.Instance.UnloadCurrentStage();
        UIManager.Instance.Clear();
        UIManager.Instance.LoadUI<UIStartMenu>("UIStartMenu");
    }
}
