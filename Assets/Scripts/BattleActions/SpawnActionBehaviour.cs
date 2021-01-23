using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SpawnAction에 대한 Behaviour 클래스.
/// </summary>
public class SpawnActionBehaviour : BattleActionBehaviour
{
    /// <summary>
    /// Owner와 상대적으로 배치하는가?
    /// </summary>
    public bool RelativeToOwner;

    /// <summary>
    /// Spawn할 위치.
    /// </summary>
    public Vector3 SpawnPosition = Vector3.zero;

    /// <summary>
    /// Spawn할 Rotation.
    /// </summary>
    public Quaternion SpawnRotation = Quaternion.identity;

    /// <summary>
    /// Spawn할 Prefab 경로(이름).
    /// </summary>
    public string PrefabName;

    /// <summary>
    /// Spawn한 객체의 PooledObject 레퍼런스.
    /// </summary>
    private PooledObject asPooledObject;

    /// <summary>
    /// Spawn한 객체의 ISpawnable 레퍼런스.
    /// </summary>
    private ISpawnable asSpawnedObject;
    

    /// <summary>
    /// 생성 부분에서 배틀액션 데이터 넘겨줘서 제작
    /// </summary>
    public SpawnActionBehaviour(BattleAction ba, FieldObject owner) : base(ba, owner)
    {
        var spawnAction = ba as SpawnAction;

        PrefabName = spawnAction.PrefabName;
        RelativeToOwner = spawnAction.RelativeToOwner;
        SpawnPosition = spawnAction.Position;
        SpawnRotation = Quaternion.Euler(0, 0, spawnAction.Angle);
    }


    /// <summary>
    /// 1회 Spawn 수행. 
    /// </summary>
    public override void Start()
    {
        var pool = PoolManager.GetOrCreate(PrefabName);

        Vector3 position = SpawnPosition;
        Quaternion rotation = SpawnRotation;

        if (RelativeToOwner)
        {
            position += owner.transform.position;
            rotation *= owner.transform.rotation;
        }

        //풀에서 ISpawnable 객체를 가져온다.
        asPooledObject = pool.Instantiate(position, rotation);
        asSpawnedObject = asPooledObject.gameObject.GetComponent<FieldObject>() as ISpawnable;

        if (asSpawnedObject == null)
        {
            Debug.LogError("ISpawnable을 구현하지 않은 FieldObject는 Spawn할 수 없습니다 ");
            pool.Dispose(asPooledObject);

            asPooledObject = null;
            return;
        }

        asSpawnedObject.SpawnOwner = owner;
        asSpawnedObject.OnSpawn(owner);

        IsActive = true;

    }


    public override void Update()
    {
      
    }


    /// <summary>
    /// Spawn동작 종료시 호출. 리소스 반환을 위해 반드시 호출하는 것을 권장.
    /// </summary>
    public override void Finish()
    {
        if (asSpawnedObject != null)
        { 
            asSpawnedObject.OnDespawn();
            asSpawnedObject = null;
        }

        if (asPooledObject != null)
        {
            PoolManager.GetOrCreate(PrefabName).Dispose(asPooledObject);
            asPooledObject = null;
            return;
        }
        
        IsActive = false;
    }

    public override void Dispose()
    {
        base.Dispose();

        if (IsActive)
        {
            Finish();
        }
    }

}
