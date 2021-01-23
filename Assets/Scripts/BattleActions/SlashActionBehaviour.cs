using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashActionBehaviour : BattleActionBehaviour
{

    /// <summary>
    /// SlashAction의 데이터.
    /// </summary>
    private SlashAction data;


    /// <summary>
    /// Start 실행 후 동작 시간.
    /// </summary>
    private float currentTime = 0f;


    /// <summary>
    /// BattleLayer에 이 이름을 가진 State가 있어야 함.
    /// </summary>
    private string SlashAnimName = "BA_Slash";


    private PooledObject vfxPooledObject;


    /// <summary>
    /// 이번 실행에서 데미지를 전달한 FieldObject 리스트. 
    /// </summary> 이번 실행에서 데미지를 전달한 FieldObject 리스트. 
    ///<remarks>한 번 공격에 1회 이상 피격당하는 상황을 방지하기 위함.</remarks>
    private List<FieldObject> attackedEntityList = new List<FieldObject>();
    /// <summary>
    /// 생성 부분에서 배틀액션 데이터 넘겨줘서 제작
    /// </summary>
    public SlashActionBehaviour(BattleAction ba, FieldObject owner) : base(ba, owner)
    {
        data = ba as SlashAction;
        IsActive = false;
    }


    public override void Start()
    {
        if (IsActive) return;
        currentTime = 0f;
        IsActive = true;

        // BA 레이어 On.
        owner.AnimController.SetLayerWeight(2, 0.75f);

        // Slash 애니메이션(State) 실행.
        owner.AnimController.Play(SlashAnimName, 2, 0f);


        Vector3 vfxOffset = owner.transform.position + data.vfxOffset;
        Quaternion vfxRotation = Quaternion.Euler(0, 0, data.vfxAngle);

        //VFX 생성.
        vfxPooledObject = PoolManager.GetOrCreate("SlashVFX", 5).Instantiate(vfxOffset, vfxRotation);

        SoundManager.Instance.PlayClipAtPoint("PlayerAttack2", owner.transform.position);


        attackedEntityList.Clear();

    }


    public override void Update()
    {
        if (!IsActive) return;
        currentTime += Time.deltaTime;

        ProcessCollision();

        Vector3 vfxOffset = owner.transform.position + owner.transform.rotation * data.vfxOffset;
        Quaternion vfxRotation = owner.transform.rotation * Quaternion.Euler(0, 0, data.vfxAngle);

        //VFX 생성.
        vfxPooledObject.transform.position = vfxOffset;
        vfxPooledObject.transform.rotation = vfxRotation;

        if (currentTime > data.duration)
        {
            Finish();
        }

    }


    public override void Finish()
    {
        if (!IsActive) return;

        // BA 레이어 Off.
        owner.AnimController.SetLayerWeight(2, 0f);
        IsActive = false;
        PoolManager.GetOrCreate("SlashVFX", 5).Dispose(vfxPooledObject);

        attackedEntityList.Clear();
    }


    /// <summary>
    /// 충돌 및 데미지 이벤트 전달을 처리합니다.
    /// </summary>
    public void ProcessCollision()
    {
        Vector2 point = owner.transform.position;
        Vector2 forward = owner.transform.right;

        var hitInfos = Physics2D.CircleCastAll(point, data.Radius, forward, data.CircleCastDistance, data.CollideLayerMask.value);
        float halfAngle = data.Angle * 0.5f;

        foreach (var hitInfo in hitInfos)
        {
            
            //범위(Angle) 내에서 충돌이 발생한 Collider만 충돌로 판정해야 한다.
            Vector2 dir = ((Vector2)hitInfo.transform.position - point).normalized;
            float angle = Vector2.Angle(forward, dir);

            bool withinRange = angle < halfAngle;
            

            if (withinRange)
            {
                //범위 내 Collider인 경우, 충돌 이벤트 처리.
                FieldObject target = hitInfo.collider.GetComponent<FieldObject>();
                if (target == null)
                    target = hitInfo.collider.GetComponentInParent<FieldObject>();

                if (target != null && !attackedEntityList.Contains(target))
                {
                    // 적이면 데미지 가함
                    if (owner.EntityGroup.IsHostileTo(target.EntityGroup))
                    {
                        var info = new DamageInfo
                        {
                            Sender = owner,
                            Target = target,
                            amount = data.Damage
                        };
                        var cmd = DamageCommand.Create(info);
                        cmd.Execute();

                        attackedEntityList.Add(target);

                    }
                }

            }
        }

        //GLDebug.DrawSector(new Circle(point, Radius, forward), Angle, DebugColor, 1f);
    }

}

