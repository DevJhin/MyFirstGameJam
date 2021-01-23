using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : FieldObject, IEventListener
{
    public PlayerBehavior Behavior { get; private set; }

    public PlayerController Controller { get; private set; }

    [Header("REFERENCES")] public LayerMask groundLayerMask;

    //public Rigidbody2D rb;
    public BoxCollider2D col;

    [Header("MOVEMENT")] public float moveSpeed;
    public float airDrag;
    public float jumpPower;

    /// <summary>
    /// 천장에 부딪히면 부딪힌 속도의 일정 비율로 튕겨나오는 비율을 결정합니다.
    /// 0일 경우 얼마나 빠르게 부딪히든 동일하게 낙하하며, 1일 경우 부딪힐 당시의 속도와 동일합니다.
    /// </summary>
    public float ceilingReactionForceRatio;

    public float fallMultiplier;
    public float lowJumpFallMultiplier;
    public float jumpCheckTimer;
    public float raycastLength;

    /// <summary>
    /// Attack 커맨드 입력시, 실행할 BattleAction.
    /// </summary>
    public BattleAction AttackBattleAction;

    /// <summary>
    /// Interact 커맨드 입력시, 실행할 BattleAction.
    /// </summary>
    public BattleAction InteractBattleAction;

    public BattleActionBehaviour AttackBattleActionBehaviour;
    public BattleActionBehaviour InteractActionBehaviour;

    /// <summary>
    /// 캐릭터가 현재 무적 상태인가?
    /// </summary>
    public bool IsInvinsible = false;

    /// <summary>
    /// 피격 후 무적 상태 지속 시간.
    /// </summary>
    public float test_invinsibleTime = 1f;

    /// <summary>
    /// 캐릭터가 죽은 상태인가?
    /// </summary>
    public bool IsDead = false;

    /// <summary>
    /// 캐릭터 전체 바운드 (충돌 판정과는 무관한 땅 밟는 쪽과 연관)
    /// </summary>
    public Bounds bounds;

    void Awake()
    {
        Behavior = new PlayerBehavior(this);
        Controller = new PlayerController(this);

        AttackBattleActionBehaviour = BattleActionBehaviourFactory.Create(AttackBattleAction, this);
        InteractActionBehaviour = BattleActionBehaviourFactory.Create(InteractBattleAction, this);
    }

    private void Update()
    {
        Behavior.OnPlayerUpdate();

        if (AttackBattleActionBehaviour.IsActive)
        {
            AttackBattleActionBehaviour.Update();
        }
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
    /// 플레이어가 데미지 이벤트를 받았을 때 실행.
    /// </summary>
    /// <param name="damageEvent"></param>

    void OnDamaged(DamageEvent damageEvent)
    {
        // 무적상태라면 데미지 처리 무시.
        if (IsInvinsible) return;

        // TODO: 데미지 받았을 때 처리 추가
        CurrentHP -= damageEvent.damageInfo.amount;

        if (CurrentHP <= 0)
        {
            CurrentHP = 0;

            CurrentHP = 0;
            IsDead = true;
        }


        if (IsDead)
        {
            OnDeath();
        }
        else
        {
            //피격후 무적 샃태로으로 설정.
            IsInvinsible = true;

            //피격 애니메이션 실행.
            AnimController.SetLayerWeight(1, 1f);

            //일정 시간후, 무적상태 OFF시킨다.
            //FIXME: 귀찮아서 Invoke 쓰긴 했는데 나중엔 바꿔야 할 듯.
            Invoke(nameof(Test_OnInvinsibleTimeEnd), test_invinsibleTime);
        }
    }


    /// <summary>
    /// 플레이어 사망시 1회 호출되는 함수.
    /// </summary>
    private void OnDeath()
    {
        // 사망상태 애니메이션 업데이트.
        AnimController.SetLayerWeight(1, 1f);
        AnimController.SetBool("IsDead", true);

        //죽으면 Controller Input을 Disable 해준다.
        Controller.DisableInput();

        //플레이어 사망 이벤트 전달.
        var deathEvent = new PlayerDeathEvent()
        {
        };

        ///사망 이벤트 전달.
        MessageSystem.Instance.Publish(deathEvent);
    }


    /// <summary>
    /// Invinsible 시간 종료 시 실행됩니다.
    /// </summary>
    public void Test_OnInvinsibleTimeEnd()
    {
        if (IsInvinsible)
        {
            IsInvinsible = false;
            //피격 레이어 Off.
            AnimController.SetLayerWeight(1, 0f);
        }
    }


    /// <summary>
    /// 플레이어 GameObject가 Destroy 되었을 때 실행합니다.
    /// </summary>
    public override void Dispose()
    {
        base.Dispose();
        Controller.Dispose();

        AttackBattleActionBehaviour.Dispose();
        InteractActionBehaviour.Dispose();
    }
}
