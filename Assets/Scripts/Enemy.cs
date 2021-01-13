﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 적 개체 FieldObject.
/// </summary>
public class Enemy : FieldObject
{
    public EnemyPatternAI Controller
    {
        get; private set;
    }

    public EnemyBehaviour Behaviour
    {
        get; private set;
    }

    /// <summary>
    /// 실행중인 Pattern.
    /// </summary>
    public Pattern MainPattern;

    void Awake()
    {
        Controller = new EnemyPatternAI(this);
        Behaviour = new EnemyBehaviour(this);
    }


    void Update()
    {
        Controller.OnUpdate();
    }
}