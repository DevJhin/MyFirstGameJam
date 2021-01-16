using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// 이 클래스를 컴포넌트로 가진 모든 오브젝트는 오브젝트 풀링의 대상이 됩니다.
/// </summary>
public class PooledObject : MonoBehaviour
{
	/// <summary>
	/// 어느 풀의 오브젝트인지 이름으로 알기 위해서 추가
	/// 필요없어지면 없앨 것
	/// </summary>
	public string OriginalObjectName;
}
