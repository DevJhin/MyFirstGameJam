using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 전체 오브젝트 풀들을 관리 및 담당 합니다.
/// </summary>
public static class PoolManager
{
    /// <summary>
    /// 오브젝트 풀들을 관리하는 Dictionary
    /// </summary>
    private static Dictionary<string, Pool> poolDict = new Dictionary<string, Pool>();

    /// <summary>
    /// 풀을 가져오거나 생성
    /// </summary>
    /// <remarks>count는 생성될 때만 적용된다.</remarks>
    /// <param name="objName">가져올 오브젝트 이름</param>
    /// <param name="poolCount">처음 생성할 때, 오브젝트 갯수</param>
    /// <returns></returns>
    public static Pool GetOrCreate(string objName, int poolCount = 30)
    {
        // 풀 이미 생성되었으면 그 풀을 리턴
        if (poolDict.TryGetValue(objName, out var pool))
        {
            return pool;
        }

        pool = new Pool(objName, poolCount);

        poolDict.Add(objName, pool);

        return pool;
    }
}

/// <summary>
/// 풀링된 오브젝트를 관리하는 클래스. 오브젝트 프리팹 하나당 Pool 하나가 생성됩니다.
/// </summary>
public class Pool
{
    /// <summary>
    /// 원본 게임 오브젝트
    /// </summary>
    public GameObject Origin { get; private set; }

    /// <summary>
    /// 풀링 된 오브젝트들은 전부 이 트랜스폼의 하위에 존재
    /// </summary>
    public Transform Parent { get; set; }

    /// <summary>
    /// 풀링 된 오브젝트들 관리 스택
    /// 이 방식 정말 괜찮은 것인가? PooledObject가 poolStack에서 벗어난 사이에 해지된다거나 하면 남아 있는 오브젝트들은 컨트롤이 전혀 안 된다.
    /// 적어도 그 부분들은 신경 쓰는 것을 권장
    /// </summary>
    Stack<PooledObject> poolStack = new Stack<PooledObject>();

    public Pool(string objName, int poolCount)
    {
        Origin = ResourcesHelper.LoadPoolableOrigin(objName);
        Parent = new GameObject(name: $"Pool: {objName}").transform;

        for (int i = 0; i < poolCount; i++)
        {
            var inst = Create();
            inst.gameObject.SetActive(false);
        }
    }

    ~Pool()
    {
        Origin = null;
        Parent = null;

        poolStack.Clear();
        poolStack = null;
    }

    /// <summary>
    /// 완전히 새로 생성된 풀링되는 오브젝트
    /// </summary>
    PooledObject Create()
    {
        GameObject obj = GameObject.Instantiate(Origin);
        obj.name = Origin.name;
        PooledObject pooledObject = obj.GetOrAddComponent<PooledObject>();
        pooledObject.OriginalObjectName = obj.name;
        pooledObject.transform.SetParent(Parent);

        return pooledObject;
    }

    /// <summary>
    /// 풀링 된 오브젝트 활성화
    /// </summary>
    public PooledObject Instantiate(Vector3 position, Quaternion rotation)
    {
        // 풀에 남아있는게 없으면 새로 생성
        PooledObject pooledObject = (poolStack.Count > 0 && poolStack.Peek() != null) ? poolStack.Pop() : Create();

        pooledObject.transform.SetPositionAndRotation(position, rotation);
        pooledObject.gameObject.SetActive(true);

        return pooledObject;
    }

    /// <summary>
    /// 풀링 오브젝트 해지하고, 풀에 반환
    /// </summary>
    /// <param name="pooledObject">해지할 오브젝트</param>
    /// <returns>정상적으로 해지 되었는가?</returns>
    public bool Dispose(PooledObject pooledObject)
    {
        if (pooledObject == null)
            return false;

        pooledObject.gameObject.SetActive(false);
        poolStack.Push(pooledObject);

        return true;
    }
}
