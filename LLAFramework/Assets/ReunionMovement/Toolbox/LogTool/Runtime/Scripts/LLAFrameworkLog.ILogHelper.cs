using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LLAFramework
{
    public static partial class LLAFrameworkLog
    {
        /// <summary>
        /// 日志辅助器接口。
        /// </summary>
        public interface ILogHelper
        {
            /// <summary>
            /// 记录日志。
            /// </summary>
            /// <param name="level">游戏框架日志等级。</param>
            /// <param name="message">日志内容。</param>
            void Log(LLAFrameworkLogLevel level, object message);
        }
    }
}