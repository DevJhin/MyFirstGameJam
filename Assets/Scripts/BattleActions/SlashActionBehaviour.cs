using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashActionBehaviour : BattleActionBehaviour
{
    // FIXME: 데이터로 받아오게 하던지 알아서 처리할 것 임시로 만들었음.
    private float duration = 0.3f;

    private float currentTime = 0f;

    /// <summary>
    /// 생성 부분에서 배틀액션 데이터 넘겨줘서 제작
    /// </summary>
    public SlashActionBehaviour(BattleAction ba, FieldObject owner) : base(ba, owner)
    {
        var data = ba as SlashAction;
    }

    // Start is called before the first frame update
    public override void Start()
    {
        Debug.Log("Started");

        currentTime = 0f;
        IsActive = true;
    }

    // Update is called once per frame
    public override void Update()
    {
        Debug.Log("Update");

        currentTime += Time.deltaTime;

        if (currentTime > duration)
        {
            Finish();
        }
    }

    public override void Finish()
    {
        Debug.Log("Finished");

        IsActive = false;
    }
}
