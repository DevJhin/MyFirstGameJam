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
    /// Enemy에 정의된 PatternAction을 순차적으로 실행합니다. 
    /// </summary>
    /// <returns></returns>
    public IEnumerator ExecutePatternRoutine()
    {
        isExecutingPattern = true;

        var patterns = enemy.Patterns;

        foreach (var pattern in patterns)
        {
            yield return new WaitForSeconds(pattern.PreDelayTime);

            int repeatCount = pattern.RepeatCount;

            for (int i = 0; i < repeatCount; i++)
            { 
                enemy.Behaviour.ExecutePatternAction(pattern);

                yield return new WaitForSeconds(pattern.RepeatIntervalTime);
            }

            yield return new WaitForSeconds(pattern.PostDelayTime);
        }

        isExecutingPattern = false;
    }

}
