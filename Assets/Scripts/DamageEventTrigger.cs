using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 데미지를 입으면 이벤트를 발생시키는 FieldObject.
/// 이벤트를 수신받기 위해서는 <see cref = "MessageSystem.Subscribe"/>를 통해
/// <see cref = "TriggerEvent"/>를 구독해야 합니다.
/// </summary>
public class DamageEventTrigger : FieldObject, IEventListener
{
    /// <summary>
    /// 이 이벤트 트리거가 활성화되어 TriggerEvent를 Publish 할 수 있는 상태인가?
    /// </summary>
    public bool IsActive;


    /// <summary>
    /// DamageEvent를 수신하면 TriggerEvent를 Publish 합니다.
    /// </summary>
    public bool OnEvent(IEvent e)
    {
        if (!IsActive) return false;

        if (e is DamageEvent)
        {
            var triggerEvent = new TriggerEvent()
            {
                Sender = this
            };

            MessageSystem.Instance.Publish(triggerEvent);

            return true;
        }

        return false;
    }




}
