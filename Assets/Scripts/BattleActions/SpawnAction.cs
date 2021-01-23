using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 부채꼴 영역 내의 적을 공격하는 Action.
/// </summary>
[CreateAssetMenu(menuName = "MyFirstGameJam/BattleActions/SpawnAction", fileName = "New SpawnAction")]
public class SpawnAction : BattleAction
{
    /// <summary>
    /// Spawn할 Prefab의 경로(이름).
    /// </summary>
    public string PrefabName;

    /// <summary>
    /// 스폰 위치를 Owner의 Transform에 상대적으로 배치하는가?(False일 경우, WorldPosition 기준으로 스폰).
    /// </summary>
    public bool RelativeToOwner;

    /// <summary>
    /// Spawn 위치.
    /// </summary>
    public Vector2 Position;

    /// <summary>
    /// Spawn할 각도.
    /// </summary>
    public float Angle;

}
