using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PlayerCharacter가 수행하는 Action.
/// </summary>
public class BattleAction : ScriptableObject
{

    /// <summary>
    /// 이 BattleAction에서 실행할 동작.
    /// </summary>
    /// <param name="actor">동작의 주체가 되는 FieldObject.</param>
    public virtual void Execute(FieldObject actor)
    { 
        
        
    }
}
