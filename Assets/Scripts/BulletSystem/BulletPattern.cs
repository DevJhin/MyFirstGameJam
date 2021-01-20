using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Bullet 공격 패턴 오브젝트를 관리하는 클래스. 
/// 각 Pattern의 세부 기능을 구현할 때에는 PatternObject를 상속받아 사용하는 것을 권장.
/// </summary>
public class BulletPattern : FieldObject, ISpawnable
{
    public FieldObject SpawnOwner { get => spawnOwner; set => spawnOwner = value; }

    /// <summary>
    /// 이 BulletPattern을 Spawn한 FieldObject. 
    /// </summary>
    [SerializeField]
    private FieldObject spawnOwner;

    /// <summary>
    /// 이 BulletPattern에 의해서 관리되는 PatternObject 배열.
    /// </summary>
    private PatternObject[] patternObjects;

    /// <summary>
    /// 유니티 OnEnalbe 이벤트에 실행하는가?
    /// </summary>
    public bool PlayOnEnable = true;

    /// <summary>
    /// ISpawnable이 호출될 때 실행하는가?
    /// </summary>
    public bool PlayOnSpawn = true;

    /// <summary>
    /// 현재 이 패턴이 실행되고 있는 상태인가?
    /// </summary>
    private bool IsPlaying;

    private void Awake()
    {
        patternObjects = GetComponentsInChildren<PatternObject>();

        foreach (var patternObject in patternObjects)
        {
            patternObject.OwnerPattern = this;
        }

    }


    private void OnEnable()
    {
        if (PlayOnEnable)
        {
            Play();
        }
    }


    private void OnDisable()
    {
        Stop();
    }


    /// <summary>
    /// 패턴 동작을 시작합니다.
    /// </summary>
    public void Play()
    {
        if (IsPlaying) return;
        IsPlaying = true;

        foreach (var patternObject in patternObjects)
        {
            patternObject.OnPatternStart(this);
        }

    }


    /// <summary>
    /// 패턴 동작을 종료합니다.
    /// </summary>
    public void Stop()
    {
        if (!IsPlaying) return;
        IsPlaying = false;
        foreach (var patternObject in patternObjects)
        {
            patternObject.OnPatternFinish(this);
        }
    }


    /// <summary>
    /// 이 패턴이 Spawn 되었을 때 실행하는 동작.
    /// </summary>
    void ISpawnable.OnSpawn(FieldObject owner)
    {
        this.spawnOwner = owner;
        if (PlayOnSpawn)
        {
            Play();
        }

    }


    /// <summary>
    /// 이 패턴이 실행을 마치고 Despawn 되었을 때 실행하는 동작.
    /// </summary>
    void ISpawnable.OnDespawn()
    {
        this.spawnOwner = null;

        if (IsPlaying)
        {
            Stop();
        }

    }
}
