using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 플레이어의 HP 잔량을 표시하는 UI 캔버스에 부착되는 컴포넌트
/// * 이 클래스명을 수정할 경우, Resources/UI 내의 프리팹 명도 동일하게 수정하거나(권장), 또는 호출 시 프리팹명을 명시적으로 입력해야 합니다.
/// </summary>
public class UIPlayerHpBar : UIBase
{
	enum Sliders
	{
		HPBar
	}

	private Player player = null;
	private float sliderValue = float.NaN;

	protected override void Init()
	{
		Bind<Slider>(typeof(Sliders));
	}

	/// <summary>
	/// HP를 확인할 플레이어 컴포넌트를 세팅합니다.
	/// </summary>
	public void SetPlayer(Player player)
	{
		this.player = player;
	}

	private void Update()
	{
		UpdateHpBar();
	}


	/// <summary>
	/// player의 체력 값으로 HP바 슬라이더를 업데이트합니다
	/// </summary>
	private void UpdateHpBar()
	{
		if (player == null)
			return;

		float currentRatio = player.CurrentHP / (float)player.MaxHP;
		if(!Mathf.Approximately(sliderValue, currentRatio))
		{
			Get<Slider>((int)Sliders.HPBar).value = currentRatio;
			sliderValue = currentRatio;
		}
	}
}
