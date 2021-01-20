using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

/// <summary>
/// Pattern별로 실행되는 Action들을 관리하는 클래스.
/// </summary>
[CreateAssetMenu(menuName = "MyFirstGameJam/Action Schedule")]
public class ActionSchedule : ScriptableObject
{
    public bool Loop;

    /// <summary>
    /// Pattern에서 관리하는 각 Action의 Schedule 정보 구조체.
    /// </summary>
    [System.Serializable]
    public struct ActionScheduleData
    { 
        /// <summary>
        /// 실행할 액션.
        /// </summary>
        public BattleAction Action;

        /// <summary>
        /// 액션 실행 전 지연 시간.
        /// </summary>
        [HorizontalGroup("Delay"), LabelText("Delay"), LabelWidth(40)] [MinValue(0)]
        public float Delay;

        /// <summary>
        /// 액션의 실행(Update) 시간.
        /// </summary>
        [HorizontalGroup("Delay"), LabelText("Time"), LabelWidth(40)] [MinValue(0)]
        public float Playtime;

        /// <summary>
        /// 액션 반복 실행 횟수.
        /// </summary>
        [HorizontalGroup("Repeat"), LabelText("Count"), LabelWidth(40)] [MinValue(1)] 
        public int RepeatCount;

        /// <summary>
        /// 액션 반복 실행 간격.
        /// </summary>
        [DisableIf("@this.RepeatCount < 2")][HorizontalGroup("Repeat"), LabelText("Interval"), LabelWidth(60)] 
        public float RepeatInterval;


    }

    /// <summary>
    /// 실행할 Action들의 Scheudle정보 목록.
    /// </summary>
    [TableList]
    public List<ActionScheduleData> TimeTable;

    
}
