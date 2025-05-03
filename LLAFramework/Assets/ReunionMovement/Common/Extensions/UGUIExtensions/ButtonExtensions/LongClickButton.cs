using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LLAFramework
{
    /// <summary>
    /// 长按按钮
    /// </summary>
    public class LongClickButton : Button
    {
        [SerializeField]
        private ButtonClickEvent longClick = new ButtonClickEvent();

        public ButtonClickEvent onLongClick
        {
            get { return longClick; }
            set { longClick = value; }
        }

        // 新增的按钮抬起事件
        [SerializeField]
        private ButtonClickEvent buttonUp = new ButtonClickEvent();

        public ButtonClickEvent onButtonUp
        {
            get { return buttonUp; }
            set { buttonUp = value; }
        }

        // 新增的长按未抬起事件
        [SerializeField]
        private ButtonClickEvent longPressing = new ButtonClickEvent();

        public ButtonClickEvent onLongPressing
        {
            get { return longPressing; }
            set { longPressing = value; }
        }

        //按下时间
        private DateTime pressStartTime;
        //长按取消令牌
        private CancellationTokenSource longPressCts;

        /// <summary>
        /// 长按
        /// </summary>
        private void TriggerLongClick()
        {
            onLongClick?.Invoke();
            ResetPressTime();
        }

        /// <summary>
        /// 按下
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            if (pressStartTime == default)
            {
                pressStartTime = DateTime.Now;
                longPressCts = new CancellationTokenSource();
                StartLongPressingCoroutine(longPressCts.Token);
            }
        }

        /// <summary>
        /// 抬起
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            longPressCts?.Cancel();
            longPressCts = null;

            if (pressStartTime != default)
            {
                var pressDuration = DateTime.Now - pressStartTime;
                if (pressDuration.TotalMilliseconds > 600)
                {
                    TriggerLongClick();
                }
                else
                {
                    ResetPressTime();
                }
            }

            // 触发按钮抬起事件
            onButtonUp?.Invoke();
        }

        /// <summary>
        /// 离开
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            longPressCts?.Cancel();
            longPressCts = null;
            ResetPressTime();
        }

        /// <summary>
        /// 重置时间
        /// </summary>
        private void ResetPressTime()
        {
            pressStartTime = default;
        }

        /// <summary>
        /// 长按未抬起协程
        /// </summary>
        private async void StartLongPressingCoroutine(CancellationToken token)
        {
            try
            {
                await Task.Delay(600, token);
                onLongPressing?.Invoke();
            }
            catch (TaskCanceledException)
            {
                // Ignore the exception if the task is canceled
            }
        }
    }
}