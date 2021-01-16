using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임이 진행되는 스테이지
/// 우리 같은 경우에는 실제 게임은 스테이지 내부에서만 진행되기 때문에 실제 게임의 객체들을 관리는 여기서 진행한다.
/// </summary>
public class Stage : IDisposable
{
    /// <summary>
    /// 스테이지 이름
    /// </summary>
    public string StageName;

    /// <summary>
    /// 생성된 맵
    /// </summary>
    public StageData LoadedMap;

    public Stage(string stageName, StageData map)
    {
        StageName = stageName;
        LoadedMap = map;

        MessageSystem.Instance.Subscribe<StageLoadEvent>(OnStageLoaded);
        MessageSystem.Instance.Subscribe<StageUnloadEvent>(OnStageUnloaded);
    }

    // 변수 등 모두 여기서 해제
    public void Dispose()
    {
        MessageSystem.Instance.Unsubscribe<StageLoadEvent>(OnStageLoaded);
        MessageSystem.Instance.Unsubscribe<StageUnloadEvent>(OnStageUnloaded);
    }

    /// <summary>
    /// 스테이지가 로드 되었을 때
    /// </summary>
    void OnStageLoaded(IEvent e)
    {
        Debug.Log("Stage Loaded");

        foreach(var data in LoadedMap.spawnDatas)
        {
            if (data.FieldObjectName == "Player")
            {
                var playerResource = Resources.Load("Player");
                var player = GameObject.Instantiate(playerResource, data.transform.position, Quaternion.identity) as GameObject;

                CameraManager.Instance.FollowTarget = player.transform;
            }
            else
            {
                var fieldObjectResource = Resources.Load(data.FieldObjectName);
                var spawnedFieldObject = GameObject.Instantiate(fieldObjectResource, data.transform.position, data.transform.rotation) as GameObject;
            }
        }
    }

    /// <summary>
    /// 스테이지가 해제되었을 때
    /// </summary>
    void OnStageUnloaded(IEvent e)
    {
        Debug.Log("Stage Unloaded");
    }
}
