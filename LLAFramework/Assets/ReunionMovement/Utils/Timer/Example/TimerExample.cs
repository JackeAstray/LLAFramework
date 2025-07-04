using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LLAFramework.Example
{
    public class TimerExample : MonoBehaviour
    {
        private Timer stopwatchTimer;
        private Timer countdownTimer;

        // Start is called before the first frame update
        void Start()
        {
            // 正计时示例
            stopwatchTimer = Timer.Register(100, OnStopwatchComplete, OnStopwatchUpdate, false, false, this);
            stopwatchTimer.StartTimer(true, 100);

            // 倒计时示例
            countdownTimer = Timer.Register(100, OnCountdownComplete, OnCountdownUpdate, false, false, this);
            countdownTimer.StartTimer(false, 100);
        }

        // Update is called once per frame
        void Update()
        {
            // 这里可以添加其他逻辑
        }

        private void OnStopwatchUpdate(float elapsed)
        {
            Debug.Log($"正计时运行时间: {elapsed}");
        }

        private void OnStopwatchComplete()
        {
            Debug.Log("正计时完成!");
        }

        private void OnCountdownUpdate(float elapsed)
        {
            Debug.Log($"倒计时剩余时间: {countdownTimer.GetTimeRemaining()}");
        }

        private void OnCountdownComplete()
        {
            Debug.Log("倒计时完成!");
        }
    }
}