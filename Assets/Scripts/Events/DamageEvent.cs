using UnityEngine;

/// <summary>
/// 데미지 발생 메시지를 전달하는 클래스.
/// 공격자와 피해량, 피격 지점이 입력되어 피격 대상에게 전달됩니다.
/// </summary>
public class DamageEvent : IEvent
{
	/// <summary>
	/// 데미지 관련 정보들
	/// </summary>
	public DamageInfo damageInfo;

    public DamageEvent(DamageInfo info)
    {
	    damageInfo = info;
    }
}
