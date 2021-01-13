using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

/// <summary>
/// Pattern���� ����Ǵ� Action���� �����ϴ� Ŭ����.
/// </summary>
[CreateAssetMenu(menuName = "MyFirstGameJam/Pattern")]
public class Pattern : ScriptableObject
{
    /// <summary>
    /// Pattern���� �����ϴ� �� Action�� Schedule ���� ����ü.
    /// </summary>
    [System.Serializable]
    public struct PatternActionSchedule
    {
        /// <summary>
        /// ������ �׼�.
        /// </summary>
        public PatternAction Action;

        /// <summary>
        /// �׼� ���� �� ���� �ð�.
        /// </summary>
        [HorizontalGroup("Delay"), LabelText("Pre"), LabelWidth(25)] [MinValue(0)]
        public float PreDelay;

        /// <summary>
        /// �׼� ���� �� ���� �ð�.
        /// </summary>
        [HorizontalGroup("Delay"), LabelText("Post"), LabelWidth(25)] [MinValue(0)] 
        public float PostDelay;

        /// <summary>
        /// �׼� �ݺ� ���� Ƚ��.
        /// </summary>
        [HorizontalGroup("Repeat"), LabelText("Count"), LabelWidth(40)] [MinValue(1)] 
        public int RepeatCount;

        /// <summary>
        /// �׼� �ݺ� ���� ����.
        /// </summary>
        [DisableIf("@this.RepeatCount < 2")][HorizontalGroup("Repeat"), LabelText("Interval"), LabelWidth(60)] 
        public float RepeatInterval;

    }

    /// <summary>
    /// ������ Action���� Scheudle���� ���.
    /// </summary>
    [TableList]
    public List<PatternActionSchedule> ActionSchedules;



    
}
