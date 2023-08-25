using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 事件类
    /// </summary>
    public class DelegateEvent
    {
        /// <summary>
        /// 定义委托函数
        /// </summary>
        /// <param name="data"></param>
        public delegate void EventHandler(EventData data);
        /// <summary>
        /// 定义基于委托函数的事件
        /// </summary>
        public event EventHandler eventHandle;

        /// <summary>
        /// 触发监听事件
        /// </summary>
        /// <param name="data"></param>
        public void Handle(EventData data)
        {
            if (eventHandle != null)
                eventHandle(data);
        }

        /// <summary>
        /// 删除监听函数
        /// </summary>
        /// <param name="removeHandle"></param>
        public void RemoveListener(EventHandler removeHandle)
        {
            if (eventHandle != null)
                eventHandle -= removeHandle;
        }

        /// <summary>
        /// 添加监听函数
        /// </summary>
        /// <param name="addHandle"></param>
        public void AddListener(EventHandler addHandle)
        {
            eventHandle += addHandle;
        }
    }
}