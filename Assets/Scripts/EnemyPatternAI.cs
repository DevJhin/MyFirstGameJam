using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Enemy의 Pattern을 순차적으로 실행하여 동작하는 AIController.
/// </summary>
public class EnemyPatternAI : FieldObjectAI
{
    private Enemy enemy;

    /// <summary>
    /// Schedule부터 빌드한 명령 리스트.
    /// </summary>
    private List<ActionCommand> commandOrderList = new List<ActionCommand>();

    /// <summary>
    /// 명령 실행을 관리하는 Queue.
    /// </summary>
    private Queue<ActionCommand> commandQueue = new Queue<ActionCommand>();


    /// <summary>
    /// 이번 Routine에서의 시간.
    /// </summary>
    private float playbackTime = 0;


    /// <summary>
    /// 루틴 실행을 반복 실행하는가?
    /// </summary>
    private bool doLoop;

    /// <summary>
    /// 루틴 실행을 활성화하는가?
    /// </summary>
    private bool isActive;

    public EnemyPatternAI(Enemy enemy)
    {
        this.enemy = enemy;

        isActive = true;

        LoadActionSchedule();
        ResetQueue();
    }


    public override void OnUpdate()
    {
        if (!isActive) return;

        //이번에 처리할 커맨드.
        ActionCommand cmd = commandQueue.Peek();

        if (cmd.State == CommandState.Queued)
        {
            //playbackTime이 실행 구간 이후라면, Start 실행.
            if (playbackTime >= cmd.StartTime)
            {
                cmd.ActionBehaviour.Start();

                cmd.State = CommandState.Start;
            }
        }

        if (cmd.State == CommandState.Start)
        {
            //playbackTime이 실행 구간 내라면, Update 실행.
            if (playbackTime >= cmd.StartTime && playbackTime <= cmd.EndTime)
            {
                cmd.ActionBehaviour.Update();
            }

            //playbackTime이 실행 구간을 경과했다면, 종료 처리하고 Dequeue.
            else if (playbackTime > cmd.EndTime)
            {
                cmd.ActionBehaviour.Finish();
                commandQueue.Dequeue();

                cmd.State = CommandState.Finish;

            }
        }

        //Queue가 비면 루틴을 초기화할 필요가 있음.
        if (commandQueue.Count <= 0)
        {
            isActive = doLoop;
            ResetQueue();
        }

        //진행 시간 업데이트.
        playbackTime += Time.deltaTime;

    }

    /// <summary>
    /// 명령 큐 상태를 초기화하여 스케쥴을 재시작합니다.
    /// </summary>
    void ResetQueue()
    {
        commandOrderList.ForEach(x => x.Reset());
        commandQueue = new Queue<ActionCommand>(commandOrderList);
        playbackTime = 0;
    }


    /// <summary>
    /// Enemy의 ActionSchedule로부터 실행에 필요한 정보를 초기화합니다. 
    /// </summary>
    public void LoadActionSchedule()
    {
        BuildTimetable();
    }


    /// <summary>
    /// ActionSchedule의 명세에 따라 commandOrderList를 빌드합니다.
    /// </summary>
    public void BuildTimetable()
    {
        commandQueue.Clear();
        commandOrderList.Clear();

        var actionScheduleDataList = enemy.MainActionSchedule.TimeTable;

        //루프 여부 설정
        doLoop = enemy.MainActionSchedule.Loop;

        //ActionScheudule 데이터를 돌면서 필요한 명령 생성.
        float time = 0;
        foreach (var actionData in actionScheduleDataList)
        {
            time += actionData.Delay;

            for (int i = 0; i < actionData.RepeatCount; i++)
            {
                float playTime = actionData.Playtime;

                //명령 커맨드를 생성합니다.
                ActionCommand cmd = new ActionCommand(time, time + playTime, actionData.Action, enemy);
                commandOrderList.Add(cmd);

                time += playTime;

                if (i != actionData.RepeatCount - 1)
                {
                    time += actionData.RepeatInterval;
                }

            }

        }

    }

    /// <summary>
    /// 사용을 정지하고 Command 상태 초기화.
    /// </summary>
    public void Stop()
    {
        isActive = false;

        commandQueue.Clear();
        foreach (var cmd in commandOrderList)
        {
            cmd.Reset();
        }
    }


    /// <summary>
    /// Destroy시, 모든 Command 자원을 Dispose.
    /// </summary>
    public override void Dispose()
    {
        base.Dispose();
        isActive = false;

        commandQueue.Clear();
        foreach (var cmd in commandOrderList)
        {
            cmd.Dispose();
        }
    }

    /// <summary>
    ///  Action 실행에 필요한 정보를 관리하는 클래스.
    /// </summary>
    public class ActionCommand : System.IDisposable
    {
        /// <summary>
        /// Action 실행에 필요한 Behaviour 객체.
        /// </summary>
        public BattleActionBehaviour ActionBehaviour;

        /// <summary>
        /// 실행 시작 시간.
        /// </summary>
        public float StartTime;

        /// <summary>
        /// 실행 종료 시간.
        /// </summary>
        public float EndTime;


        /// <summary>
        /// 현재 상태.
        /// </summary>
        public CommandState State;

        public ActionCommand(float startTime, float endTime, BattleAction action, FieldObject owner)
        {
            this.StartTime = startTime;
            this.EndTime = endTime;

            ActionBehaviour = BattleActionBehaviourFactory.Create(action, owner);
        }

        public void Dispose()
        {
            ActionBehaviour.Dispose();
        }


        /// <summary>
        /// 사용 전 상태로 초기화합니다.
        /// </summary>
        public void Reset()
        {
            State = CommandState.Queued;
            
            if (ActionBehaviour.IsActive)
            { 
                ActionBehaviour.Finish();
            }
        }


    }


    public enum CommandState
    {
        /// <summary>
        /// 실행 전 대기 상태.
        /// </summary>
        Queued,

        /// <summary>
        /// 실행이 시작된 상태. Action의 Start가 호출되었으며, Update 가능한 상태.
        /// </summary>
        Start,

        /// <summary>
        /// 실행이 종료된 상태. Finish가 호출되었으며, Dequeue된 상태.
        /// </summary>
        Finish
    }

}
