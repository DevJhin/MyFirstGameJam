using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Enemy의 Pattern을 순차적으로 실행하여 동작하는 AIController.
/// </summary>
public class EnemyPatternAI: FieldObjectAI
{
    private Enemy enemy;

    private Coroutine coroutine;

    /// <summary>
    /// 현재 패턴을 실행중인가?
    /// </summary>
    private bool isExecutingPattern;

    public EnemyPatternAI(Enemy enemy)
    {
        this.enemy = enemy;
        isExecutingPattern = false;
    }

    public override void OnUpdate()
    {
        if (!isExecutingPattern)
        {
            coroutine = enemy.StartCoroutine(ExecutePatternRoutine());
        }
    }

    /// <summary>
    /// Enemy에 정의된 Pattern을 실행합니다. 
    /// </summary>
    /// <returns></returns>
    public IEnumerator ExecutePatternRoutine()
    {
        isExecutingPattern = true;

        var actionSchedules = enemy.MainPattern.ActionSchedules;

        foreach (var actionSchedule in actionSchedules)
        {
            //하나의 PatternAction을 처리합니다.

            yield return new WaitForSeconds(actionSchedule.PreDelay);

            int repeatCount = actionSchedule.RepeatCount;

            for (int i = 0; i < repeatCount; i++)
            { 
                enemy.Behaviour.ExecutePatternAction(actionSchedule.Action);

                if (i != repeatCount - 1)
                { 
                    yield return new WaitForSeconds(actionSchedule.RepeatInterval);
                }
            }

            yield return new WaitForSeconds(actionSchedule.PostDelay);
        }

        isExecutingPattern = false;
    }

}
