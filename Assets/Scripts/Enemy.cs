using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 적 개체 FieldObject.
/// </summary>
public class Enemy : FieldObject, IEventListener
{
    public EnemyPatternAI Controller { get; private set; }

    public EnemyBehaviour Behaviour { get; private set; }

    /// <summary>
    /// 실행중인 ActionSchedule.
    /// </summary>
    public ActionSchedule MainActionSchedule;

    /// <summary>
    /// 이 Enemy 객체가 Boss 인가?
    /// (주의: 현재로써는 Boss로 설정된 객체가 죽은 경우, Stage 쪽에서 StageClear 조건으로 처리하기 때문에
    /// Stage당 Boss는 1명으로 두어야 한다)
    /// </summary>
    public bool IsBoss;

    /// <summary>
    /// 이 객체가 죽어서 비활성화 된 상태인가?
    /// </summary>
    public bool IsDead = false;

    /// <summary>
    /// 이 개체가 현재 무적 상태인가?
    /// </summary>
    public bool IsInvinsible = false;


    void Awake()
    {
        Controller = new EnemyPatternAI(this);
        Behaviour = new EnemyBehaviour(this);
    }


    void Update()
    {
        Controller.OnUpdate();
    }


    public bool OnEvent(IEvent e)
    {
        if (e is DamageEvent)
        {
            if (IsDead) return false;

            DamageEvent damageEvent = e as DamageEvent;
            OnDamaged(damageEvent);
            return true;
        }

        return false;
    }

    /// <summary>
    /// 적이 데미지 이벤트를 수신 받았을 때 실행.
    /// </summary>
    /// <param name="damageEvent"></param>

    void OnDamaged(DamageEvent damageEvent)
    {
        // 적이 무적상태라면 데미지 처리 무시.
        if (IsInvinsible)
        {

            SoundManager.Instance.PlayClipAtPoint("PlayerShieldAttack", transform.position);
            Debug.Log($"나 {gameObject.name}, 무적이지롱.");
            return;
        }

        //무적 상태가 아닌 경우, 최종적으로 계산한 데미지를 적용.

        //TODO: 데이터 바인딩이 구현된다면, 여기서 데미지 값 수정할 때
        //자동으로 UI가 업데이트가 되는 지 확인할 것.
        int finalDamage = damageEvent.damageInfo.amount;
        CurrentHP -= finalDamage;

        //사망 조건 확인.
        if (CurrentHP <= 0)
        {
            CurrentHP = 0;
            IsDead = true;
        }

        
        if (IsDead)
        {
            OnDeath();
        }
        //
        else
        {
            //TODO: 적의 피격 사운드, VFX 등 작업 구현할 수 있을 것.

            //AnimController.SetBool("IsHurting", true);
            SoundManager.Instance.PlayClipAtPoint("EnemyHurt", transform.position);
            Debug.Log($"나 {gameObject.name}, 체력 {CurrentHP + finalDamage} => {CurrentHP}.");
        }


    }
       
    
    /// <summary>
    /// Enemy 사망 시 1회 호출되는 함수.
    /// </summary>
    void OnDeath()
    {
        //사망 애니메이션 업데이트
        AnimController.SetBool("IsDead", true);
        
        //컨트롤러 비활성화.
        Controller.Stop();

        ///사망 이벤트 전달.
        var deathEvent = new EnemyDeathEvent()
        {
            Sender = this
        };

        SoundManager.Instance.PlayClipAtPoint("EnemyDeath1", transform.position);
        
        MessageSystem.Instance.Publish(deathEvent);

        //GameObject 비활성화.
        gameObject.SetActive(false);
        Debug.Log($"나 {gameObject.name}, 여기서 죽다.");
    }


    public override void Dispose()
    {
        base.Dispose();

        Controller.Dispose();
        Behaviour = null;

    }
}
