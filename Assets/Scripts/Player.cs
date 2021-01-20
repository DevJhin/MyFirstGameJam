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
            DamageEvent damageEvent = e as DamageEvent;
            OnDamaged(damageEvent);
            return true;
        }

        return false;
    }


    /// <summary>
    /// 테스트: 캐릭턱 현재 무적 상태인가?
    /// </summary>
    public bool test_isInvinsible = false;

    /// <summary>
    /// 테스트: 피격 후 무적 상태 지속 시간.
    /// </summary>
    public float test_invinsibleTime = 1f;

    /// <summary>
    /// 테스트: 캐릭터가 죽은 상태인가?
    /// </summary>
    public bool test_isDead = false;


    void OnDamaged(DamageEvent damageEvent)
    {
        if (test_isDead) return;
        if (test_isInvinsible) return;

        // TODO: 데미지 받았을 때 처리 추가
        CurrentHP -= damageEvent.damageInfo.amount;

        if (CurrentHP <= 0)
        {
            CurrentHP = 0;

            
            test_isDead = true;
            test_isInvinsible = true;

            //피격 레이어 ON.
            AnimController.SetLayerWeight(1, 1f);
            
            // 죽은 상태도 업데이트 필요.
            AnimController.SetBool("IsDead", true);

            //죽으면 Controller Input을 Disable 해준다.
            Controller.DisableInput();
        }
        else
        {
            test_isInvinsible = true;
            //피격 레이어 On.
            AnimController.SetLayerWeight(1, 1f);

            //FIXME: 귀찮아서 Invoke 쓰긴 했는데 나중엔 바꿔야 할 듯.
            Invoke(nameof(Test_OnInvinsibleTimeEnd), test_invinsibleTime);
        }
    }

 
    /// <summary>
    /// Invinsible 시간 종료 시 실행됩니다.
    /// </summary>
    public void Test_OnInvinsibleTimeEnd()
    {
        if (test_isInvinsible)
        {
            test_isInvinsible = false;
            //피격 레이어 Off.
            AnimController.SetLayerWeight(1, 0f);
        }
    }


    /// <summary>
    /// 플레이어 GameObject가 Destroy 되었을 때 실행합니다.
    /// </summary>
    private void OnDestroy()
    {
        Controller.Dispose();
    }
}
