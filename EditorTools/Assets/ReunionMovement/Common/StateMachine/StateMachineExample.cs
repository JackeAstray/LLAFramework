using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;

namespace GameLogic
{
    public enum PlayerStateTest
    {
        Idle,
        Running,
        Jumping,
        Attacking
    }

    /// <summary>
    /// 状态机示例
    /// </summary>
    public class StateMachineExample : MonoBehaviour
    {
        private StateMachine<PlayerStateTest> stateMachine;

        void Start()
        {
            Invoke("Init", 3);
        }

        public void Init()
        {
            stateMachine = new StateMachine<PlayerStateTest>();

            stateMachine.AddState(PlayerStateTest.Idle, OnIdleEnter, OnIdleUpdate, OnIdleExit);
            stateMachine.AddState(PlayerStateTest.Running, OnRunningEnter, OnRunningUpdate, OnRunningExit);
            stateMachine.AddState(PlayerStateTest.Jumping, OnJumpingEnter, OnJumpingUpdate, OnJumpingExit);
            stateMachine.AddState(PlayerStateTest.Attacking, OnAttackingEnter, OnAttackingUpdate, OnAttackingExit);

            stateMachine.CurrentState = PlayerStateTest.Idle;

            stateMachine.AddTransitionCondition(PlayerStateTest.Idle, PlayerStateTest.Running, () => Input.GetKey(KeyCode.W));
            stateMachine.AddTransitionCondition(PlayerStateTest.Running, PlayerStateTest.Idle, () => !Input.GetKey(KeyCode.W));
            stateMachine.AddTransitionCondition(PlayerStateTest.Running, PlayerStateTest.Jumping, () => Input.GetKeyDown(KeyCode.Space));
            //stateMachine.AddTransitionCondition(PlayerStateTest.Jumping, PlayerStateTest.Idle, () => /* 检查是否着地 */);
            stateMachine.AddTransitionCondition(PlayerStateTest.Idle, PlayerStateTest.Attacking, () => Input.GetMouseButtonDown(0));
            //stateMachine.AddTransitionCondition(PlayerStateTest.Attacking, PlayerStateTest.Idle, () => /* 攻击动画结束 */);
        }

        void Update()
        {
            if (stateMachine != null)
            {
                stateMachine.Update();
            }
            

            if (Input.GetKeyDown(KeyCode.W))
            {
                stateMachine.CurrentState = PlayerStateTest.Running;
            }

            if (Input.GetKeyUp(KeyCode.W))
            {
                stateMachine.CurrentState = PlayerStateTest.Idle;
            }

            if (Input.GetKey(KeyCode.Space))
            {

            }
        }

        private void OnIdleEnter() { Debug.Log("进入 Idle 状态"); }
        private void OnIdleUpdate() { Debug.Log("更新 Idle 状态"); }
        private void OnIdleExit() { Debug.Log("退出 Idle 状态"); }

        private void OnRunningEnter() { Debug.Log("进入 Running 状态"); }
        private void OnRunningUpdate() { Debug.Log("更新 Running 状态"); }
        private void OnRunningExit() { Debug.Log("退出 Running 状态"); }

        private void OnJumpingEnter() { Debug.Log("进入 Jumping 状态"); }
        private void OnJumpingUpdate() { Debug.Log("更新 Jumping 状态"); }
        private void OnJumpingExit() { Debug.Log("退出 Jumping 状态"); }

        private void OnAttackingEnter() { Debug.Log("进入 Attacking 状态"); }
        private void OnAttackingUpdate() { Debug.Log("更新 Attacking 状态"); }
        private void OnAttackingExit() { Debug.Log("退出 Attacking 状态"); }
    }
}
