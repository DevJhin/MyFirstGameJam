using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SpawnAction에 의해 Spawn되려면, 이 인터페이스를 구현해야 합니다.
/// </summary>
public interface ISpawnable
{
    /// <summary>
    /// ISpawnable을 Spawn한 주체.
    /// </summary>
    public FieldObject SpawnOwner { get; set; }


    /// <summary>
    /// Spawn 시작 시에 실행해야 하는 동작 구현.
    /// </summary>
    /// <param name="owner"></param>
    void OnSpawn(FieldObject owner);


    /// <summary>
    /// Spawn 종료시에 실행해야 하는 동작 구현.
    /// </summary>
    void OnDespawn();


}
