using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 오브젝트 풀링을 담당합니다. ResourcesEx에서 풀링 대상 오브젝트에 대해 Instantiate/Destroy를 할 경우 이곳에서 활성화/비활성화 처리합니다.
/// </summary>
public class PoolManager
{
	public static PoolManager Instance { get; } = new PoolManager();

	// 여기서 생성된 모든 오브젝트의 상위 개체. DontDestroyOnLoad 씬에 저장되며 비활성화된 오브젝트들은 이곳에서 대기한다.
	Transform Root
	{
		get
		{
			if(_root == null)
			{
				_root = new GameObject(name: "PoolRoot").transform;
				GameObject.DontDestroyOnLoad(_root);
			}
			return _root;
		}
	}
	Transform _root;

	/// <summary>
	/// 활성화된 오브젝트를 DontDestroyOnLoad 씬에서 현재 씬으로 보내기 위해 사용되는 부모 Transform.
	/// 모든 씬에 공통적으로 생성되는 객체가 있다면 이 값을 대체할 수 있습니다.
	/// </summary>
	public Transform DefaultSceneParent
	{
		get
		{
			if(_sceneParent == null)
				_sceneParent = Camera.main.transform;

			return _sceneParent;
		}
	}
	Transform _sceneParent = Camera.main.transform;

	/// <summary>
	/// 풀링된 오브젝트를 관리하는 클래스. 오브젝트 프리팹 하나당 Pool 하나가 생성됩니다.
	/// </summary>
	class Pool
	{
		public GameObject Origin { get; private set; }
		public Transform Parent { get; set; }
		Stack<Poolable> _poolStack = new Stack<Poolable>();

		public Pool(GameObject originObject, int poolCount)
		{
			Origin = originObject;
			Parent = new GameObject(name: $"Pool: {originObject.name}").transform;

			if (poolCount <= 0)
				poolCount = PoolManager.DefaultPoolCount;

			for (int i = 0; i < poolCount; i++)
			{
				SetDeactive(Create());
			}
		}

		Poolable Create()
		{
			GameObject obj = GameObject.Instantiate(Origin);
			obj.name = Origin.name;
			Poolable poolable = obj.GetOrAddComponent<Poolable>();

			return poolable;
		}

		public Poolable SetActive(Transform parent)
		{
			Poolable poolable = (_poolStack.Count > 0) ?
				_poolStack.Pop() : Create();

			poolable.gameObject.SetActive(true);

			if (parent == null)
			{
				poolable.transform.SetParent(PoolManager.Instance.DefaultSceneParent);
			}
			poolable.transform.parent = parent;

			return poolable;
		}

		public void SetDeactive(Poolable poolable)
		{
			if (poolable == null)
				return;

			poolable.transform.parent = Parent;
			poolable.gameObject.SetActive(false);

			_poolStack.Push(poolable);
		}
	}

	
	Dictionary<string, Pool> _poolDic = new Dictionary<string, Pool>();
	/// <summary>
	/// 별도의 개수 입력이 없을 경우, 최초 생성 시 이 숫자만큼 오브젝트가 생성됩니다.
	/// </summary>
	public const int DefaultPoolCount = 30;

	/// <summary>
	/// 입력받은 오브젝트 프리팹을 원본으로 새로운 Pool을 생성하고, 딕셔너리에 등록합니다.
	/// 생성자에 의해 Poolable에서 지정한 개수만큼 오브젝트가 생성되며 비활성화 상태로 대기합니다.
	/// </summary>
	Pool CreatePool(GameObject origin)
	{
		Poolable poolable = origin.GetComponent<Poolable>();
		int poolCount = (poolable != null) ? poolable.PoolCount : DefaultPoolCount;

		Pool pool = new Pool(origin, poolCount);
		pool.Parent.SetParent(Root);
		_poolDic.Add(origin.name, pool);

		return pool;
	}

	/// <summary>
	/// 입력받은 오브젝트를 생성합니다. 
	/// 이미 풀링된 오브젝트면 활성화하고, 아닐 경우 오브젝트 풀 생성 후 하나를 활성화합니다.
	/// </summary>
	public Poolable ActivePool(GameObject origin, Transform parent = null)
	{
		Pool pool;
		if(!_poolDic.TryGetValue(origin.name, out pool))
		{
			pool = CreatePool(origin);
		}

		return pool.SetActive(parent);
	}

	/// <summary>
	/// 입력된 개체를 비활성화하고 오브젝트 풀에 보관합니다. 오브젝트 풀이 없는 개체라면 그냥 파괴합니다.
	/// </summary>
	public void DeactivePool(Poolable poolable)
	{
		string name = poolable.gameObject.name;

		if(!_poolDic.ContainsKey(name))
		{
			GameObject.Destroy(poolable.gameObject);
			return;
		}

		_poolDic[name].SetDeactive(poolable);
	}

	/// <summary>
	/// 입력된 이름의 오브젝트 풀이 있다면 원본 오브젝트를 반환합니다. 없을 경우 null이 반환됩니다.
	/// </summary>
	public GameObject GetOrigin(string name)
	{
		return (_poolDic.ContainsKey(name)) ? _poolDic[name].Origin : null;
	}
}
