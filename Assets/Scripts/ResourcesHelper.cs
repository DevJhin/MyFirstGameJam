using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 리소스의 생성 및 파괴 로직이 매핑된 클래스입니다.
/// 이 클래스를 통해 게임오브젝트를 로딩, 생성, 파괴할 경우 오브젝트 풀링 대상을 구분하여 처리합니다.
/// </summary>
public static class ResourcesHelper
{
	/// <summary>
	/// Resources에서 이 폴더들 안에 있으면, assetName으로 찾을 수 있게 한다.
	/// </summary>
	private const string poolableFolderPath = "Poolables/";

	/// <summary>
	/// 풀링 가능한 오브젝트 원본 로드
	/// </summary>
	/// <param name="name">오브젝트 이름</param>
	/// <returns>원본 오브젝트</returns>
	public static GameObject LoadPoolableOrigin(string name)
	{
		return Resources.Load<GameObject>($"{poolableFolderPath}{name}");
	}
}
