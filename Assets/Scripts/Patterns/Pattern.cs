using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

/// <summary>
/// Pattern별로 실행되는 Action들을 관리하는 클래스.
/// </summary>
[CreateAssetMenu(menuName = "MyFirstGameJam/Pattern")]
public class Pattern : ScriptableObject
{
    /// <summary>
    /// Pattern에서 관리하는 각 Action의 Schedule 정보 구조체.
    /// </summary>
    [System.Serializable]
    public struct PatternActionSchedule
    { 
        /// <summary>
        /// 실행할 액션.
        /// </summary>
        public PatternAction Action;

        /// <summary>
        /// 액션 실행 전 지연 시간.
        /// </summary>
        [HorizontalGroup("Delay"), LabelText("Pre"), LabelWidth(25)] [MinValue(0)]
        public float PreDelay;

        /// <summary>
        /// 액션 실행 후 지연 시간.
        /// </summary>
        [HorizontalGroup("Delay"), LabelText("Post"), LabelWidth(25)] [MinValue(0)] 
        public float PostDelay;

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
    public List<PatternActionSchedule> ActionSchedules;



    
}
