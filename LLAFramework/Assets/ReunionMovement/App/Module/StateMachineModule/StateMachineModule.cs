using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 状态机模块
    /// </summary>
    public class StateMachineModule : CustommModuleInitialize
    {
        #region 实例与初始化
        public static StateMachineModule Instance = new StateMachineModule();
        public bool IsInited { get; private set; }
        private double initProgress = 0;
        public double InitProgress { get { return initProgress; } }
        #endregion

        #region 数据
        private StateMachine<string> stateMachine;
        #endregion

        public IEnumerator Init()
        {
            initProgress = 0;

            // 初始化状态机
            stateMachine = new StateMachine<string>();
            stateMachine.AddState("Idle", OnIdleStart, OnIdleUpdate, OnIdleStop);
            stateMachine.AddState("Running", OnRunningStart, OnRunningUpdate, OnRunningStop);

            ChangeState("Idle");

            initProgress = 100;
            IsInited = true;
            Log.Debug("StateModule 初始化完成");
            yield return null;
        }

        public void ClearData()
        {
            Log.Debug("StateModule 清除数据");
            stateMachine.Reset();
        }

        /// <summary>
        /// 切换状态
        /// </summary>
        /// <param name="stateLabel"></param>
        public void ChangeState(string stateLabel)
        {
            stateMachine.CurrentState = stateLabel;
        }

        /// <summary>
        /// 更新状态机
        /// </summary>
        public void UpdateTime(float elapseSeconds, float realElapseSeconds)
        {
            if (!IsInited)
            {
                Log.Warning("StateMachineModule 未初始化");
                return;
            }

            stateMachine.Update();
        }

        #region 状态回调
        private void OnIdleStart()
        {
            Log.Debug("进入 Idle 状态");
        }

        private void OnIdleUpdate()
        {
            //Log.Debug("更新 Idle 状态");
        }

        private void OnIdleStop()
        {
            Log.Debug("退出 Idle 状态");
        }

        private void OnRunningStart()
        {
            Log.Debug("进入 Running 状态");
        }

        private void OnRunningUpdate()
        {
            //Log.Debug("更新 Running 状态");
        }

        private void OnRunningStop()
        {
            Log.Debug("退出 Running 状态");
        }
        #endregion
    }
}
