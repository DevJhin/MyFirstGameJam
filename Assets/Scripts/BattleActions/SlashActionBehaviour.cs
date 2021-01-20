using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashActionBehaviour : BattleActionBehaviour
{
    // FIXME: 데이터로 받아오게 하던지 알아서 처리할 것 임시로 만들었음.
    private float duration = 0.3f;

    private float currentTime = 0f;

    /// <summary>
    /// BattleLayer에 이 이름을 가진 State가 있어야 함.
    /// </summary>
    private string SlashAnimName = "BA_Slash";

    /// <summary>
    /// 생성 부분에서 배틀액션 데이터 넘겨줘서 제작
    /// </summary>
    public SlashActionBehaviour(BattleAction ba, FieldObject owner) : base(ba, owner)
    {
        var data = ba as SlashAction;
    }


    public override void Start()
    {
        currentTime = 0f;
        IsActive = true;

        // BA 레이어 On.
        owner.AnimController.SetLayerWeight(2, 0.75f);

        // Slash 애니메이션(State) 실행.
        owner.AnimController.Play(SlashAnimName, 2, 0f);
    }


    public override void Update()
    {
        currentTime += Time.deltaTime;

        if (currentTime > duration)
        {
            Finish();
        }

    }
    

    public override void Finish()
    {
        // BA 레이어 Off.
        owner.AnimController.SetLayerWeight(2, 0f);
        IsActive = false;
    }
}
