using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 리소스의 생성 및 파괴 로직이 매핑된 클래스입니다.
/// 이 클래스를 통해 게임오브젝트를 로딩, 생성, 파괴할 경우 오브젝트 풀링 대상을 구분하여 처리합니다.
/// </summary>
public class ResourcesHelper
{
	/// <summary>
	/// 오브젝트 풀링 처리할 프리팹이 들어 있는 폴더명을 입력하면, 입력된 경로에 해당 폴더가 포함되어 있을 때 PoolManager를 통해 처리합니다.
	/// </summary>
	static List<string> _poolableFolderPath = new List<string>()
	{
		"Bullets/"
	};
	
	public static T Load<T>(string path) where T : Object
	{
		if (typeof(T) == typeof(GameObject))
		{
			string name = ToGameObjectName(path);

			GameObject go = PoolManager.Instance.GetOrigin(name);
			if (go != null)
				return go as T;
		}

		return Resources.Load<T>(path);
	}

	/// <summary>
	/// 게임오브젝트의 경로에서 상위 폴더를 잘라내고 이름만 반환합니다.
	/// </summary>
	static string ToGameObjectName(string path)
	{
		int index = path.LastIndexOf('/');
		if (index > 0)
			path = path.Substring(index + 1);

		return path;
	}

	/// <summary>
	/// 해당 오브젝트가 오브젝트 풀링 대상인지 확인합니다. Poolable 컴포넌트가 있거나 지정한 폴더 내에 있다면 true가 반환됩니다.
	/// </summary>
	static bool IsPoolable(GameObject obj, string path)
	{
		if(obj.GetComponent<Poolable>() != null)
			return true;

		foreach (string poolablePath in _poolableFolderPath)
		{
			if (path.Contains(poolablePath))
			{
				obj.AddComponent<Poolable>();
				return true;
			}
		}

		return false;
	}

	/// <summary>
	/// 오브젝트의 생성을 대신합니다. 오브젝트 풀링 대상을 구분하여 처리합니다.
	/// </summary>
	public static GameObject Instantiate(string path, Transform parent = null, bool useDefaultName = false)
	{
		GameObject origin = Load<GameObject>(path);
		if(origin == null)
		{
			Debug.LogError($"[ResourcesEX::Instantiate] Failed to Load GameObject Prefab : {path}");
			return null;
		}

		if(IsPoolable(origin, path))
		{
			return PoolManager.Instance.ActivePool(origin, parent).gameObject;
		}

		GameObject obj = GameObject.Instantiate(origin, parent);
		if (!useDefaultName)
		{
			obj.name = origin.name;
		}

		return obj;
	}

	/// <summary>
	/// 오브젝트의 파괴를 대신합니다. 오브젝트 풀링 대상을 구분하여 처리합니다.
	/// </summary>
	public static void Destroy(GameObject target, float delay = 0f)
	{
		if (target == null)
			return;

		Poolable poolable = target.GetComponent<Poolable>();
		if (poolable != null)
		{
			PoolManager.Instance.DeactivePool(poolable);
			return;
		}

		GameObject.Destroy(target, delay);
	}
}
