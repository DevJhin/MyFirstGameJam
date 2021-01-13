using System;
using UnityEngine;

public abstract class DrawCommand
{
    public float StartTime;
    public float LifeTime;
      

    /// <summary>
    /// 커맨드 실행
    /// </summary>
    public abstract void Execute();

    public DrawCommand()
    {
        StartTime = Time.time;
    }

    public bool IsTimeOut()
    {
        return Time.time - StartTime > LifeTime;
    }
}