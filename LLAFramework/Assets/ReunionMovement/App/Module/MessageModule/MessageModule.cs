using LLAFramework;
using LLAFramework.Download;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LLAFramework
{
    /// <summary>
    /// 提示、警告、错误等消息的显示模块
    /// </summary>
    public class MessageModule : CustommModuleInitialize
    {
        #region 实例与初始化
        public static MessageModule Instance = new MessageModule();
        public bool IsInited { get; private set; }
        private double initProgress = 0;
        public double InitProgress { get { return initProgress; } }
        #endregion

        #region 数据
        // 消息监听器
        private Dictionary<MessageTipType, Action<Message>> listeners = new Dictionary<MessageTipType, Action<Message>>();
        // 消息队列
        private Queue<Message> messageQueue = new Queue<Message>();
        #endregion

        public IEnumerator Init()
        {
            initProgress = 0;
            yield return null;
            initProgress = 100;
            IsInited = true;
            Log.Debug("MessageModule 初始化完成");
        }

        public void ClearData()
        {
            Log.Debug("MessageModule 清除数据");
        }

        /// <summary>
        /// 注册消息监听器
        /// </summary>
        /// <param name="messageType"></param>
        /// <param name="listener"></param>
        public void RegisterListener(MessageTipType messageType, Action<Message> listener)
        {
            if (!listeners.ContainsKey(messageType))
            {
                listeners[messageType] = listener;
            }
            else
            {
                listeners[messageType] += listener;
            }
        }

        /// <summary>
        /// 注销消息监听器
        /// </summary>
        /// <param name="messageType"></param>
        /// <param name="listener"></param>
        public void UnregisterListener(MessageTipType messageType, Action<Message> listener)
        {
            if (listeners.ContainsKey(messageType))
            {
                listeners[messageType] -= listener;
            }
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="message"></param>
        public void SendMessage(Message message)
        {
            if (listeners.ContainsKey(message.Type))
            {
                listeners[message.Type]?.Invoke(message);
            }
        }

        /// <summary>
        /// 添加消息到队列
        /// </summary>
        /// <param name="message"></param>
        public void AddMessageToQueue(Message message, float delayed = 3f)
        {
            messageQueue.Enqueue(message);
            if (messageQueue.Count == 1)
            {
                CoroutinerMgr.Instance.AddRoutine(ShowMessage(messageQueue.Dequeue(), delayed), null);
            }
        }

        /// <summary>
        /// 显示消息
        /// </summary>
        /// <param name="message"></param>
        private IEnumerator ShowMessage(Message message, float delayed)
        {
            // 显示消息的逻辑（未完成）
            Debug.Log($"Displaying message: {message.Content}");

            yield return new WaitForSeconds(delayed);

            // 显示消息后的逻辑（未完成）

            MessageDisplayed(delayed);
        }

        /// <summary>
        /// 消息显示完毕
        /// </summary>
        private void MessageDisplayed(float delayed)
        {
            if (messageQueue.Count > 0)
            {
                CoroutinerMgr.Instance.AddRoutine(ShowMessage(messageQueue.Dequeue(), delayed), null);
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="elapseSeconds"></param>
        /// <param name="realElapseSeconds"></param>
        public void UpdateTime(float elapseSeconds, float realElapseSeconds)
        {

        }
    }
}