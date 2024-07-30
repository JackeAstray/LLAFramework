using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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

            public State(TLabel label, Action onStart, Action onUpdate, Action onStop)
            {
                this.label = label;
                this.onStart = onStart;
                this.onUpdate = onUpdate;
                this.onStop = onStop;
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
        public void AddParallelState(TLabel label, Action onStart = null, Action onUpdate = null, Action onStop = null)
        {
            parallelStates.Add(new State(label, onStart, onUpdate, onStop));
        }

        /// <summary>
        /// 改变状态
        /// </summary>
        /// <param name="newState"></param>
        private void ChangeState(TLabel newState)
        {
            if (currentState != null && transitionConditions.TryGetValue((currentState.label, newState), out var condition) && !condition())
            {
                Debug.LogError($"无法从状态 {currentState.label} 转换到 {newState}，条件未满足。");
                return;
            }

            Debug.Log($"状态从 {currentState.label} 转换到 {newState}");

            currentState?.onStop?.Invoke();
            stateHistory.Push(currentState);
            currentState = stateDictionary[newState];
            currentState?.onStart?.Invoke();
            onStateChanged?.Invoke(stateHistory.Peek().label, newState);
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
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return CurrentState?.ToString();
        }
    }
}