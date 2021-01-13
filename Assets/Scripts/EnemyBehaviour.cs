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
    /// PatternAction을 실행합니다.
    /// </summary>
    /// <param name="pattern"></param>
    public void ExecutePatternAction(PatternAction pattern)
    {
        // 지금은 Enemy객체를 전달하기 때문에, Enemy의 Transform 기준으로 총알이 발사됨.
        // 하지만 나중에 class BulletShooter : IFieldObject 이런식으로 하나 만들어주면,
        // 발사 중심점을 다양하게 설정할 수 있을 듯.
        pattern.Execute(enemy);
    }
}
