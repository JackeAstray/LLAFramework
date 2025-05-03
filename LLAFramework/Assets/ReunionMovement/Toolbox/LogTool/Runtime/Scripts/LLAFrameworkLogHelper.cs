using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LLAFramework
{
    /// <summary>
    /// Log帮助
    /// </summary>
    public class LLAFrameworkLogHelper : LLAFrameworkLog.ILogHelper
    {
        /// <summary>
        /// 记录日志。
        /// </summary>
        /// <param name="level">日志等级。</param>
        /// <param name="message">日志内容。</param>
        public void Log(LLAFrameworkLogLevel level, object message)
        {
            switch (level)
            {
                case LLAFrameworkLogLevel.Debug:
                    string msg = string.Format("<color=#80FF00>{0}</color>", message);
                    Debug.Log(msg);
                    break;

                case LLAFrameworkLogLevel.Info:
                    msg = string.Format("<color=#80FF00>{0}</color>", message);
                    Debug.Log(msg);
                    break;

                case LLAFrameworkLogLevel.Warning:
                    msg = string.Format("<color=#FFCC00>{0}</color>", message);
                    Debug.LogWarning(msg);
                    break;

                case LLAFrameworkLogLevel.Error:
                    msg = string.Format("<color=#FF0040>{0}</color>", message);
                    Debug.LogError(msg);
                    break;

                default:
                    throw new System.Exception(message.ToString());
            }
        }
    }
}