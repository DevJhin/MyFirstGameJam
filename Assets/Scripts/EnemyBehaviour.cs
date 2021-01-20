using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Enemy의 동작을 정의한 Behaivour 클래스.
/// </summary>
public class EnemyBehaviour : FieldObjectBehaviour
{
    private Enemy enemy;

    public EnemyBehaviour(Enemy enemy)
    {
        this.enemy = enemy;
    }


    /// <summary>
    /// BattleAction을 실행합니다.
    /// </summary>
    public void ExecuteBattleAction(BattleAction battleAction)
    {
        battleAction.Execute(enemy);
    }
}
