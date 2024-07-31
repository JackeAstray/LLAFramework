using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

namespace GameLogic
{
    /// <summary>
    /// 状态机
    /// </summary>
    /// <typeparam name="TLabel"></typeparam>
    public class StateMachine<TLabel>
    {
        private class State
        {
            public readonly TLabel label;       // 状态标签
            public readonly Action onStart;     // 开始时的回调
            public readonly Action onStop;      // 结束时的回调
            public readonly Action onUpdate;    // 更新时的回调

            public readonly int priority;       // 优先级

            public readonly float timeout;      // 超时时间
            public float elapsedTime;           // 已经过去的时间

            public State(TLabel label, Action onStart, Action onUpdate, Action onStop, float timeout = float.MaxValue, int priority = 0)
            {
                this.label = label;
                this.onStart = onStart;
                this.onUpdate = onUpdate;
                this.onStop = onStop;

                this.priority = priority;
                
                this.timeout = timeout;
                this.elapsedTime = 0f;
            }
        }

        // 状态字典
        private readonly Dictionary<TLabel, State> stateDictionary;
        // 当前状态
        private State currentState;
        // 全局更新
        private Action globalUpdate;
        // 历史状态
        private Stack<State> stateHistory;
        // 并行状态
        private List<State> parallelStates;
        // 状态改变事件
        public event Action<TLabel, TLabel> onStateChanged;
        // 状态进入事件
        public event Action<TLabel> onStateEnter;
        // 状态退出事件
        public event Action<TLabel> onStateExit;
        // 状态转换条件
        private readonly Dictionary<(TLabel, TLabel), Func<bool>> transitionConditions;
        // 状态机是否暂停
        private bool isPaused;

        public TLabel CurrentState
        {
            get => currentState.label;
            set => ChangeState(value);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public StateMachine()
        {
            stateDictionary = new Dictionary<TLabel, State>();
            stateHistory = new Stack<State>();
            transitionConditions = new Dictionary<(TLabel, TLabel), Func<bool>>();
            parallelStates = new List<State>();
            isPaused = false;
        }

        /// <summary>
        /// 设置全局更新
        /// </summary>
        /// <param name="globalUpdate"></param>
        public void SetGlobalUpdate(Action globalUpdate)
        {
            this.globalUpdate = globalUpdate;
        }

        /// <summary>
        /// 更新状态
        /// </summary>
        public void Update()
        {
            if (isPaused)
            {
                return;
            }
            globalUpdate?.Invoke();
            currentState?.onUpdate?.Invoke();
            currentState.elapsedTime += Time.deltaTime;
            if (currentState.elapsedTime >= currentState.timeout)
            {
                HandleStateTimeout();
            }
            foreach (var state in parallelStates)
            {
                state.onUpdate?.Invoke();
            }
        }
        /// <summary>
        /// 添加状态
        /// </summary>
        /// <typeparam name="TSubStateLabel"></typeparam>
        /// <param name="label"></param>
        /// <param name="subMachine"></param>
        /// <param name="subMachineStartState"></param>
        public void AddState<TSubStateLabel>(TLabel label, StateMachine<TSubStateLabel> subMachine,TSubStateLabel subMachineStartState)
        {
            AddState(label, () => subMachine.ChangeState(subMachineStartState), subMachine.Update);
        }

        /// <summary>
        /// 添加状态
        /// </summary>
        /// <param name="label"></param>
        /// <param name="onStart"></param>
        /// <param name="onUpdate"></param>
        /// <param name="onStop"></param>
        public void AddState(TLabel label, Action onStart = null, Action onUpdate = null, Action onStop = null)
        {
            stateDictionary[label] = new State(label, onStart, onUpdate, onStop);
        }

        /// <summary>
        /// 添加状态转换条件
        /// </summary>
        /// <param name="fromState"></param>
        /// <param name="toState"></param>
        /// <param name="condition"></param>
        public void AddTransitionCondition(TLabel fromState, TLabel toState, Func<bool> condition)
        {
            transitionConditions[(fromState, toState)] = condition;
        }

        /// <summary>
        /// 添加并行状态
        /// </summary>
        /// <param name="label"></param>
        /// <param name="onStart"></param>
        /// <param name="onUpdate"></param>
        /// <param name="onStop"></param>
        public void AddParallelState(TLabel label, Action onStart = null, Action onUpdate = null, Action onStop = null, float timeout = float.MaxValue, int priority = 0)
        {
            parallelStates.Add(new State(label, onStart, onUpdate, onStop, timeout, priority));
            parallelStates = parallelStates.OrderByDescending(s => s.priority).ToList(); // 按优先级排序
        }

        /// <summary>
        /// 改变状态
        /// </summary>
        /// <param name="newState"></param>
        private void ChangeState(TLabel newState)
        {
            try
            {
                if (currentState != null && IsTransitionConditions(newState))
                {
                    Debug.LogError($"无法从状态 {currentState.label} 转换到 {newState}，条件未满足。");
                    return;
                }

                Debug.Log($"状态从 {currentState.label} 转换到 {newState}");

                currentState?.onStop?.Invoke();
                onStateExit?.Invoke(currentState.label);
                stateHistory.Push(currentState);
                currentState = stateDictionary[newState];
                currentState?.onStart?.Invoke();
                onStateEnter?.Invoke(newState);
                onStateChanged?.Invoke(stateHistory.Peek().label, newState);
            }
            catch (Exception ex)
            {
                Debug.LogError($"状态转换时发生异常: {ex.Message}");
            }
        }

        /// <summary>
        /// 处理状态超时
        /// </summary>
        private void HandleStateTimeout()
        {
            // 超时后的状态转换逻辑
            Debug.Log($"状态 {currentState.label} 超时，切换到默认状态");
            //ChangeState(defaultStateLabel); // 假设有一个默认状态
        }

        /// <summary>
        /// 是否满足状态转换条件
        /// </summary>
        /// <param name="newState"></param>
        /// <returns></returns>
        private bool IsTransitionConditions(TLabel newState)
        {
            return transitionConditions.TryGetValue((currentState.label, newState), out var condition) && !condition();
        }

        /// <summary>
        /// 并行状态的移除方法
        /// </summary>
        /// <param name="label"></param>
        public void RemoveParallelState(TLabel label)
        {
            var state = parallelStates.FirstOrDefault(s => s.label.Equals(label));
            if (state != null)
            {
                parallelStates.Remove(state);
            }
        }

        /// <summary>
        /// 回退到上一个状态
        /// </summary>
        public void RevertToPreviousState()
        {
            if (stateHistory.Count > 0)
            {
                currentState?.onStop?.Invoke();
                currentState = stateHistory.Pop();
                currentState?.onStart?.Invoke();
            }
        }


        /// <summary>
        /// 暂停状态机
        /// </summary>
        public void Pause()
        {
            isPaused = true;
        }

        /// <summary>
        /// 恢复状态机
        /// </summary>
        public void Resume()
        {
            isPaused = false;
        }

        /// <summary>
        /// 重置状态机
        /// </summary>
        public void Reset()
        {
            currentState?.onStop?.Invoke();
            currentState = null;
            stateHistory.Clear();
        }

        /// <summary>
        /// 状态机的序列化方法
        /// </summary>
        /// <returns></returns>
        public string Serialize()
        {
            // 序列化当前状态、历史状态等信息
            return JsonMapper.ToJson(this);
        }

        /// <summary>
        /// 状态机的反序列化方法
        /// </summary>
        /// <param name="json"></param>
        public StateMachine<TLabel> Deserialize(string json)
        {
            return JsonMapper.ToObject<StateMachine<TLabel>>(json);
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return CurrentState?.ToString();
        }
    }
}