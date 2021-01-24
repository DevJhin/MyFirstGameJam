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

    /// <summary>
    /// 생성된 플레이어 객체.
    /// </summary>
    public FieldObject PlayerFieldObject;

    /// <summary>
    /// 생성된 플레이어 외 객체.
    /// </summary>
    public List<FieldObject> StageFieldObjects = new List<FieldObject>();

    public Stage(string stageName, StageData map)
    {
        StageName = stageName;
        LoadedMap = map;

        MessageSystem.Instance.Subscribe<StageLoadEvent>(OnStageLoaded);
        MessageSystem.Instance.Subscribe<StageUnloadEvent>(OnStageUnloaded);
        MessageSystem.Instance.Subscribe<EnemyDeathEvent>(OnEnemyDeathEvent);
        MessageSystem.Instance.Subscribe<PlayerDeathEvent>(OnPlayerDeathEvent);
    }


    // 변수 등 모두 여기서 해제
    public void Dispose()
    {
        MessageSystem.Instance.Unsubscribe<StageLoadEvent>(OnStageLoaded);
        MessageSystem.Instance.Unsubscribe<StageUnloadEvent>(OnStageUnloaded);
        MessageSystem.Instance.Unsubscribe<EnemyDeathEvent>(OnEnemyDeathEvent);
        MessageSystem.Instance.Unsubscribe<PlayerDeathEvent>(OnPlayerDeathEvent);

        //Dispose Player FieldObject.
        if (PlayerFieldObject != null)
        {
            PlayerFieldObject.Dispose();
            GameObject.Destroy(PlayerFieldObject.gameObject);
            PlayerFieldObject = null;
        }

        //Dispose all StageFieldObjects.
        foreach (var stageFieldObject in StageFieldObjects)
        {
            if (stageFieldObject == null) continue;

            stageFieldObject.Dispose();
            GameObject.Destroy(stageFieldObject.gameObject);
        }
        StageFieldObjects.Clear();
        StageFieldObjects = null;

        if (LoadedMap != null)
        {
            GameObject.Destroy(LoadedMap.gameObject);
            LoadedMap = null;
        }
    }

    /// <summary>
    /// 스테이지가 로드 되었을 때
    /// </summary>
    void OnStageLoaded(IEvent e)
    {
        Debug.Log("Stage Loaded");
        CameraManager.Instance.StartTransition(1, 1f);

        foreach(var data in LoadedMap.spawnDatas)
        {
            if (data.FieldObjectName == "Player")
            {
                var playerResource = Resources.Load("Player");
                var player = GameObject.Instantiate(playerResource, data.transform.position, Quaternion.identity) as GameObject;
                PlayerFieldObject = player?.GetComponent<FieldObject>();

                var hpBar = UIManager.Instance.LoadUI<UIFieldObjectHpBar>("UIPlayerHpBar");
                hpBar.SetFieldObject(PlayerFieldObject);

                CameraManager.Instance.enabled = true;
                CameraManager.Instance.Setup(player?.transform);
            }
            else
            {
                var fieldObjectResource = Resources.Load(data.FieldObjectName);
                var spawnedStageGameObject = GameObject.Instantiate(fieldObjectResource, data.transform.position, data.transform.rotation) as GameObject;

                if (spawnedStageGameObject.GetComponent<FieldObject>() is Enemy enemy)
                {
                    var hpBar = UIManager.Instance.LoadUI<UIFieldObjectHpBar>("UIEnemyHpBar");
                    hpBar.SetFieldObject(enemy);
                }

                StageFieldObjects.Add(spawnedStageGameObject.GetComponent<FieldObject>());
            }

        }

        SoundManager.Instance.ChangeBgm("BgmGame1");

    }

    /// <summary>
    /// 스테이지가 해제되었을 때
    /// </summary>
    void OnStageUnloaded(IEvent e)
    {
        Debug.Log("Stage Unloaded");

        CameraManager.Instance.StartTransition(0, 1f);
        CameraManager.Instance.FollowTarget = null;

    }


    void OnEnemyDeathEvent(IEvent e)
    {
        var deathEvent = e as EnemyDeathEvent;

        var deadEnemy = deathEvent.Sender;

        // 죽은 적이 보스일 경우, 스테이지 클리어 이벤트 전달.
        if (deadEnemy.IsBoss)
        {
            MessageSystem.Instance.Publish(new StageClearEvent());


        }
        else
        {

        }

    }


    /// <summary>
    /// 플레이어 사망시 전달되는 이벤트.
    /// </summary>
    /// <param name="e"></param>
    void OnPlayerDeathEvent(IEvent e)
    {
        var deathEvent = e as PlayerDeathEvent;

        SoundManager.Instance.StopBgm();
        //TODO: 플레이어 사망시 처리해야 하는 작업 수행.

    }
}
