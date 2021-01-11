﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 전반적인 게임의 흐름을 모두 제어하는 클래스 (게임이 켜지면 무조건 실행되는 녀석)
/// </summary>
public class Game : MonoBehaviour
{
    public static Game Instance { get; private set; }

    public static BulletSystem BulletSystem { get; } = new BulletSystem();

    /// <summary>
    /// 특정 맵 바로 호출해서 플레이 해보기 위한 기능들
    /// </summary>
    public bool LoadMapDirectly = false;

    /// <summary>
    /// 바로 호출할 맵 이름
    /// </summary>
    public string LoadMapName = "test";

    /// <summary>
    /// 현재 실행 중인 스테이지
    /// </summary>
    private Stage currentStage = null;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // 스테이지를 바로 불러오는 역할
        if (LoadMapDirectly)
        {
            LoadStage(LoadMapName);
        }
    }

    void OnDestroy()
    {
        if (currentStage != null)
        {
            UnloadCurrentStage();
            currentStage.Dispose();
        }
    }

    public void LoadStage(string stageName)
    {
        try
        {
            // FIXME: Async하게 불러오는건 나중에
            var loadedMap = Resources.Load("Maps/" + stageName) as GameObject;
            currentStage = new Stage(stageName, Instantiate(loadedMap).GetComponent<StageData>());

            // 맵이 다 로드되면 이벤트 재생
            MessageSystem.Instance.Publish(typeof(OnStageLoadEvent));
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Couldn't load Map: {LoadMapName}.\n" + e.ToString());
            throw;
        }
    }

    public void UnloadCurrentStage()
    {
        if (currentStage != null)
        {
            MessageSystem.Instance.PublishImmediate(typeof(OnStageUnloadEvent));
        }
    }
}
