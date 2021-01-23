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

	float disposeStartTime = 0f;
	public float DelayTime = 0f;

	public enum PoolDisposeType
	{ 
		Default,

		Delayed
	}

	public PoolDisposeType poolDisposeType;

	//bool IsDispoing;

	public void OnEnable()
	{
		disposeStartTime = 0f;
	}


	private void Update()
	{
		if (poolDisposeType == PoolDisposeType.Default)
		{

		}
		else if (poolDisposeType == PoolDisposeType.Delayed)
		{
			disposeStartTime += Time.deltaTime;
			if (disposeStartTime >= DelayTime)
			{
				var pool = PoolManager.GetOrCreate(OriginalObjectName);
				pool.Dispose(this);
			}
		}
	}

}
