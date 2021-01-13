using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// FieldObject의 AI 관련 클래스는 이 클래스를 상속받습니다.
/// </summary>
public abstract class FieldObjectAI : FieldObjectController
{
    /// <summary>
    /// 이 컨트롤러의 Owner(FieldObject)가 Update될 때 실행됩니다.
    /// </summary>
    public abstract void OnUpdate();
}
