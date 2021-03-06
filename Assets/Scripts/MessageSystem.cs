﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public delegate void SubscribedCallback(IEvent e);

public class MessageSystem : MonoBehaviour
{
    public static MessageSystem Instance {get; private set;}

    /// <summary>
    /// 현재 타입에서 구독하고 있는 콜백들
    /// </summary>
    private Dictionary<Type, List<SubscribedCallback>> callbackDict;

    /// <summary>
    /// 이번 프레임에서 이벤트 발동할 것들
    /// </summary>
    private List<IEvent> publishedOnThisFrame;

    /// <summary>
    /// 이벤트 추가 요청 관리
    /// </summary>
    private Queue<KeyValuePair<Type, SubscribedCallback>> subscribeRequestQueue;

    /// <summary>
    /// 이벤트 제거 요청 관리
    /// </summary>
    private Queue<KeyValuePair<Type, SubscribedCallback>> unsubscribeRequestQueue;

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

        callbackDict = new Dictionary<Type, List<SubscribedCallback>>();

        publishedOnThisFrame = new List<IEvent>();

        subscribeRequestQueue = new Queue<KeyValuePair<Type, SubscribedCallback>>();
        unsubscribeRequestQueue = new Queue<KeyValuePair<Type, SubscribedCallback>>();
    }

    void OnDestroy()
    {
        callbackDict.Clear();

        publishedOnThisFrame.Clear();

        subscribeRequestQueue.Clear();
        subscribeRequestQueue.Clear();
        unsubscribeRequestQueue.Clear();
    }

    void Update()
    {
        int subscribeRequestQueueCount = subscribeRequestQueue.Count;
        // 이벤트들은 마지막 틱에 일괄 추가 및 제거가 되게함.
        for (int i = 0; i < subscribeRequestQueueCount; ++i)
        {
            // Type이 있으면 기존 리스트에 추가
            var req = subscribeRequestQueue.Dequeue();
            if (callbackDict.TryGetValue(req.Key, out var callbacks))
            {
                if (!callbacks.Contains(req.Value))
                {
                    callbacks.Add(req.Value);
                }
            }
            // Type이 키에 없으면 새로 추가
            else
            {
                callbackDict.Add(req.Key, new List<SubscribedCallback> {req.Value});
            }
        }

        int unsubscribeRequestQueueCount = unsubscribeRequestQueue.Count;
        for (int i = 0; i < unsubscribeRequestQueueCount; ++i)
        {
            // Type이 있으면 기존 리스트에서 제거
            var req = unsubscribeRequestQueue.Dequeue();
            if (callbackDict.TryGetValue(req.Key, out var callbacks))
            {
                if (callbacks.Contains(req.Value))
                {
                    callbacks.Remove(req.Value);
                }
            }
        }

        for (int i = 0; i < publishedOnThisFrame.Count; ++i)
        {
            if (callbackDict.TryGetValue(publishedOnThisFrame[i].GetType(), out var callbacks))
            {
                foreach (var callback in callbacks)
                {
                    callback.Invoke(publishedOnThisFrame[i]);
                }
            }
        }

        publishedOnThisFrame.Clear();
    }

    public void Subscribe<T>(SubscribedCallback cb) where T : IEvent
    {
        subscribeRequestQueue.Enqueue(new KeyValuePair<Type, SubscribedCallback>(typeof(T), cb));
    }

    public void Unsubscribe<T>(SubscribedCallback cb) where T : IEvent
    {
        unsubscribeRequestQueue.Enqueue(new KeyValuePair<Type, SubscribedCallback>(typeof(T), cb));
    }

    /// <summary>
    /// 구독한 전체 대상에게 이벤트를 발생시키는 것
    /// </summary>
    public void Publish(IEvent e)
    {
        publishedOnThisFrame.Add(e);
    }

    /// <summary>
    /// 구독한 전체 대상에게 바로 이벤트 발생
    /// </summary>
    public void PublishImmediate(IEvent e)
    {
        if (callbackDict.TryGetValue(e.GetType(), out var callbacks))
        {
            foreach (var callback in callbacks)
            {
                callback.Invoke(e);
            }
        }
    }

    /// <summary>
    /// 대상에게 직접 이벤트를 발생 시키는 것
    /// </summary>
    /// <returns>false일 경우에는 이벤트를 받지 않게 구현</returns>
    public bool Send(IEvent e, IEventListener listener)
    {
        return listener.OnEvent(e);
    }
}
