using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 프리팹에서 가지고 있는 스테이지 정보들 기록
/// 따로 행동을 하지 않고, 정보만 가지고 있는다.
/// </summary>
public class StageData : MonoBehaviour
{
    /// <summary>
    /// 스테이지에서 스폰할 오브젝트들 정보
    /// </summary>
    public List<SpawnData> spawnDatas = new List<SpawnData>();

    /// <summary>
    /// 지형 게임 오브젝트
    /// </summary>
    public GameObject Ground;
}
