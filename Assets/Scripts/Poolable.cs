using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// 이 클래스를 컴포넌트로 가진 모든 오브젝트는 오브젝트 풀링의 대상이 됩니다.
/// ResourcesEx에 명시된 폴더 내의 프리팹은 생성 시 이 오브젝트가 자동으로 부착됩니다.
/// </summary>
public class Poolable : MonoBehaviour
{
	
	[InfoBox("오브젝트 풀 최초 생성 시 만들 오브젝트 수\n(0 이하일 경우 기본값만큼 생성됩니다)")]
	public int PoolCount = -1;

	/// <summary>
	/// 오브젝트의 활성화 상태를 반환합니다.
	/// </summary>
	[ShowInInspector, ReadOnly] public bool IsOn { get; private set; }

	private void OnEnable()
	{
		IsOn = true;
	}
	private void OnDisable()
	{
		IsOn = false;
	}
}
