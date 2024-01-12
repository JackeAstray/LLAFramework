using GameLogic.Base;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 事件管理器
    /// </summary>
    public class EventModule : CustommModuleInitialize
    {
        #region 实例与初始化
        //实例
        public static EventModule Instance = new EventModule();
        //是否初始化完成
        public bool IsInited { get; private set; }
        //初始化进度
        private double initProgress = 0;
        public double InitProgress { get { return initProgress; } }
        #endregion

        /// <summary>
        /// 事件监听池
        /// </summary>
        private Dictionary<EventModuleType, DelegateEvent> eventTypeListeners = new Dictionary<EventModuleType, DelegateEvent>();
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public IEnumerator Init()
        {
            Log.Debug("EventModule 初始化");
            initProgress = 0;
            yield return null;
            initProgress = 100;
            IsInited = true;
        }

        /// <summary>
        /// 清除数据
        /// </summary>
        public void ClearData()
        {
            Log.Debug("EventModule 清除数据");
        }

        /// <summary>
        /// 添加事件
        /// </summary>
        /// <param name="type">事件类型</param>
        /// <param name="listenerFunc">监听函数</param>
        public void AddEventListener(EventModuleType type, DelegateEvent.EventHandler listenerFunc)
        {
            DelegateEvent delegateEvent;
            if (eventTypeListeners.ContainsKey(type))
            {
                delegateEvent = eventTypeListeners[type];
            }
            else
            {
                delegateEvent = new DelegateEvent();
                eventTypeListeners[type] = delegateEvent;
            }
            delegateEvent.AddListener(listenerFunc);
        }

        /// <summary>
        /// 删除事件
        /// </summary>
        /// <param name="type">事件类型</param>
        /// <param name="listenerFunc">监听函数</param>
        public void RemoveEventListener(EventModuleType type, DelegateEvent.EventHandler listenerFunc)
        {
            if (listenerFunc == null)
            {
                return;
            }
            if (!eventTypeListeners.ContainsKey(type))
            {
                return;
            }
            DelegateEvent delegateEvent = eventTypeListeners[type];
            delegateEvent.RemoveListener(listenerFunc);
        }

        /// <summary>
        /// 触发某一类型的事件  并传递数据
        /// </summary>
        /// <param name="type">事件类型</param>
        /// <param name="data">事件的数据(可为null)</param>
        public void DispatchEvent(EventModuleType type, object data)
        {
            if (!eventTypeListeners.ContainsKey(type))
            {
                return;
            }
            //创建事件数据
            EventData eventData = new EventData();
            eventData.type = type;
            eventData.data = data;

            DelegateEvent delegateEvent = eventTypeListeners[type];
            delegateEvent.Handle(eventData);
        }

        /// <summary>
        /// 场景管理器轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        public void UpdateTime(float elapseSeconds, float realElapseSeconds)
        {

        }
    }
}

