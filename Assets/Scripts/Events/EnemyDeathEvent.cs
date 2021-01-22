using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enemy가 죽었을 때 Publish되는 이벤트.
/// </summary>
public class EnemyDeathEvent : IEvent
{
    public Enemy Sender;


}
