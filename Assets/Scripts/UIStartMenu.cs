using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStartMenu : UIBase
{
    enum Buttons
    {
        StartBtn,
        OptionBtn,
        QuitBtn
    }

    protected override void Start()
    {
        base.Start();

        Bind<Button>(typeof(Buttons));

        GetButton((int)Buttons.StartBtn).onClick.AddListener(OnClickStartBtn);
        //GetButton((int)Buttons.OptionBtn).onClick.AddListener(OnClickOptionBtn);
        GetButton((int)Buttons.QuitBtn).onClick.AddListener(OnClickQuitBtn);
    }

    /// <summary>
    /// 시작 버튼을 눌렀을 때 동작할 내용
    /// </summary>
    private void OnClickStartBtn()
    {
        UIManager.Instance.Clear();
        Game.Instance.LoadStage("Test");
    }
    /// <summary>
    /// 옵션 버튼을 눌렀을 때 동작할 내용
    /// </summary>
    private void OnClickOptionBtn()
    {

    }
    /// <summary>
    /// 종료 버튼을 눌렀을 때 동작할 내용
    /// </summary>
    private void OnClickQuitBtn()
    {
        Debug.Log("게임이 종료되었습니다.");
        UIManager.Instance.Clear();
        Application.Quit();
    }
}
