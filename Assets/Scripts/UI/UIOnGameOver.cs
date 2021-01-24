using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 플레이어 사망 시 출력되는 UI
/// </summary>
public class UIOnGameOver : UIBase
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

        gameObject.SetActive(false);
        Invoke(nameof(ShowUI), 1.5f);
    }

    private void ShowUI()
    {
        gameObject.SetActive(true);
    }

    private void OnRestart()
    {
        Game.Instance.UnloadCurrentStage();
        UIManager.Instance.Clear();
        UIManager.Instance.LoadUI<UIStartMenu>("UIStartMenu");
    }
}
