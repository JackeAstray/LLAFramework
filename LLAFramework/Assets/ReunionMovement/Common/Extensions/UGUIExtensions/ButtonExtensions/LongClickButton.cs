using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameLogic
{
    /// <summary>
    /// ������ť
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

        //����ʱ��
        private DateTime firstTime = default(DateTime);
        //̧��ʱ��
        private DateTime secondTime = default(DateTime);

        /// <summary>
        /// ����
        /// </summary>
        private void Press()
        {
            if (onLongClick != null)
            {
                onLongClick.Invoke();
            }
            resetTime();
        }

        /// <summary>
        /// ����
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            if (firstTime.Equals(default(DateTime)))
            {
                firstTime = DateTime.Now;
            }
        }

        /// <summary>
        /// ̧��
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            if (!firstTime.Equals(default(DateTime)))
            {
                secondTime = DateTime.Now;
            }

            if (!firstTime.Equals(default(DateTime)) && !secondTime.Equals(default(DateTime)))
            {
                var intervalTime = secondTime - firstTime;
                int milliSeconds = intervalTime.Seconds * 1000 + intervalTime.Milliseconds;
                if (milliSeconds > 600)
                {
                    Press();
                }
                else
                {
                    resetTime();
                }
            }
        }

        /// <summary>
        /// �뿪
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            resetTime();
        }

        /// <summary>
        /// ����ʱ��
        /// </summary>
        private void resetTime()
        {
            firstTime = default(DateTime);
            secondTime = default(DateTime);
        }
    }
}