using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GameLogic.Editor
{
    /// <summary>
    /// 日志脚本宏定义
    /// </summary>
    public static class LogScriptingDefineSymbols
    {
        //启用日志
        private const string EnableLogScriptingDefineSymbol = "ENABLE_LOG";
        //启用Debug及以上日志
        private const string EnableDebugAndAboveLogScriptingDefineSymbol = "ENABLE_DEBUG_AND_ABOVE_LOG";
        //启用信息及以上日志
        private const string EnableInfoAndAboveLogScriptingDefineSymbol = "ENABLE_INFO_AND_ABOVE_LOG";
        //启用警告及以上日志
        private const string EnableWarningAndAboveLogScriptingDefineSymbol = "ENABLE_WARNING_AND_ABOVE_LOG";
        //启用错误及以上日志
        private const string EnableErrorAndAboveLogScriptingDefineSymbol = "ENABLE_ERROR_AND_ABOVE_LOG";
        //启用致命及以上日志
        private const string EnableFatalAndAboveLogScriptingDefineSymbol = "ENABLE_FATAL_AND_ABOVE_LOG";
        //启用Debug日志
        private const string EnableDebugLogScriptingDefineSymbol = "ENABLE_DEBUG_LOG";
        //启用信息日志
        private const string EnableInfoLogScriptingDefineSymbol = "ENABLE_INFO_LOG";
        //启用警告日志
        private const string EnableWarningLogScriptingDefineSymbol = "ENABLE_WARNING_LOG";
        //启用错误日志
        private const string EnableErrorLogScriptingDefineSymbol = "ENABLE_ERROR_LOG";
        //启用致命日志
        private const string EnableFatalLogScriptingDefineSymbol = "ENABLE_FATAL_LOG";

        //日志以上脚本定义符号
        private static readonly string[] AboveLogScriptingDefineSymbols = new string[]
        {
            EnableDebugAndAboveLogScriptingDefineSymbol,
            EnableInfoAndAboveLogScriptingDefineSymbol,
            EnableWarningAndAboveLogScriptingDefineSymbol,
            EnableErrorAndAboveLogScriptingDefineSymbol,
            EnableFatalAndAboveLogScriptingDefineSymbol
        };

        //指定日志脚本定义符号
        private static readonly string[] SpecifyLogScriptingDefineSymbols = new string[]
        {
            EnableDebugLogScriptingDefineSymbol,
            EnableInfoLogScriptingDefineSymbol,
            EnableWarningLogScriptingDefineSymbol,
            EnableErrorLogScriptingDefineSymbol,
            EnableFatalLogScriptingDefineSymbol
        };

        /// <summary>
        /// 禁用所有日志脚本宏定义。
        /// </summary>
        [MenuItem("工具箱/日志/禁用所有日志", false, 1)]
        public static void DisableAllLogs()
        {
            ScriptingDefineSymbols.RemoveScriptingDefineSymbol(EnableLogScriptingDefineSymbol);

            foreach (string specifyLogScriptingDefineSymbol in SpecifyLogScriptingDefineSymbols)
            {
                ScriptingDefineSymbols.RemoveScriptingDefineSymbol(specifyLogScriptingDefineSymbol);
            }

            foreach (string aboveLogScriptingDefineSymbol in AboveLogScriptingDefineSymbols)
            {
                ScriptingDefineSymbols.RemoveScriptingDefineSymbol(aboveLogScriptingDefineSymbol);
            }
        }

        /// <summary>
        /// 启用所有日志脚本宏定义。
        /// </summary>
        [MenuItem("工具箱/日志/启用所有日志", false, 2)]
        public static void EnableAllLogs()
        {
            DisableAllLogs();
            ScriptingDefineSymbols.AddScriptingDefineSymbol(EnableLogScriptingDefineSymbol);
        }

        /// <summary>
        /// 启用调试及以上级别的日志脚本宏定义。
        /// </summary>
        [MenuItem("工具箱/日志/启用调试及以上级别的日志", false, 3)]
        public static void EnableDebugAndAboveLogs()
        {
            SetAboveLogScriptingDefineSymbol(EnableDebugAndAboveLogScriptingDefineSymbol);
        }

        /// <summary>
        /// 启用信息及以上级别的日志脚本宏定义。
        /// </summary>
        [MenuItem("工具箱/日志/启用信息及以上级别的日志", false, 4)]
        public static void EnableInfoAndAboveLogs()
        {
            SetAboveLogScriptingDefineSymbol(EnableInfoAndAboveLogScriptingDefineSymbol);
        }

        /// <summary>
        /// 启用警告及以上级别的日志脚本宏定义。
        /// </summary>
        [MenuItem("工具箱/日志/启用警告及以上级别的日志", false, 5)]
        public static void EnableWarningAndAboveLogs()
        {
            SetAboveLogScriptingDefineSymbol(EnableWarningAndAboveLogScriptingDefineSymbol);
        }

        /// <summary>
        /// 启用错误及以上级别的日志脚本宏定义。
        /// </summary>
        [MenuItem("工具箱/日志/启用错误及以上级别的日志", false, 6)]
        public static void EnableErrorAndAboveLogs()
        {
            SetAboveLogScriptingDefineSymbol(EnableErrorAndAboveLogScriptingDefineSymbol);
        }

        /// <summary>
        /// 启用严重错误及以上级别的日志脚本宏定义。
        /// </summary>
        [MenuItem("工具箱/日志/启用严重错误及以上级别的日志", false, 7)]
        public static void EnableFatalAndAboveLogs()
        {
            SetAboveLogScriptingDefineSymbol(EnableFatalAndAboveLogScriptingDefineSymbol);
        }

        //----------------------------------------------------------

        /// <summary>
        /// 启用调试日志。
        /// </summary>
        [MenuItem("工具箱/日志/启用调试日志", false, 8)]
        public static void EnableDebugLog()
        {
            SetAboveLogScriptingDefineSymbol(EnableDebugLogScriptingDefineSymbol);
        }

        /// <summary>
        /// 启用信息日志。
        /// </summary>
        [MenuItem("工具箱/日志/启用信息日志", false, 9)]
        public static void EnableInfoLog()
        {
            SetAboveLogScriptingDefineSymbol(EnableInfoLogScriptingDefineSymbol);
        }

        /// <summary>
        /// 启用警告及日志。
        /// </summary>
        [MenuItem("工具箱/日志/启用警告日志", false, 10)]
        public static void EnableWarningLog()
        {
            SetAboveLogScriptingDefineSymbol(EnableWarningLogScriptingDefineSymbol);
        }

        /// <summary>
        /// 启用错误及日志。
        /// </summary>
        [MenuItem("工具箱/日志/启用错误及日志", false, 11)]
        public static void EnableErrorLog()
        {
            SetAboveLogScriptingDefineSymbol(EnableErrorLogScriptingDefineSymbol);
        }

        /// <summary>
        /// 启用严重错误日志。
        /// </summary>
        [MenuItem("工具箱/日志/启用严重错误日志", false, 12)]
        public static void EnableFatalLog()
        {
            SetAboveLogScriptingDefineSymbol(EnableFatalLogScriptingDefineSymbol);
        }

        /// <summary>
        /// 设置日志脚本宏定义。
        /// </summary>
        /// <param name="aboveLogScriptingDefineSymbol">要设置的日志脚本宏定义。</param>
        public static void SetAboveLogScriptingDefineSymbol(string aboveLogScriptingDefineSymbol)
        {
            if (string.IsNullOrEmpty(aboveLogScriptingDefineSymbol))
            {
                return;
            }

            foreach (string i in AboveLogScriptingDefineSymbols)
            {
                if (i == aboveLogScriptingDefineSymbol)
                {
                    DisableAllLogs();
                    ScriptingDefineSymbols.AddScriptingDefineSymbol(aboveLogScriptingDefineSymbol);
                    return;
                }
            }
        }

        /// <summary>
        /// 设置日志脚本宏定义。
        /// </summary>
        /// <param name="specifyLogScriptingDefineSymbols">要设置的日志脚本宏定义。</param>
        public static void SetSpecifyLogScriptingDefineSymbols(string[] specifyLogScriptingDefineSymbols)
        {
            if (specifyLogScriptingDefineSymbols == null || specifyLogScriptingDefineSymbols.Length <= 0)
            {
                return;
            }

            bool removed = false;
            foreach (string specifyLogScriptingDefineSymbol in specifyLogScriptingDefineSymbols)
            {
                if (string.IsNullOrEmpty(specifyLogScriptingDefineSymbol))
                {
                    continue;
                }

                foreach (string i in SpecifyLogScriptingDefineSymbols)
                {
                    if (i == specifyLogScriptingDefineSymbol)
                    {
                        if (!removed)
                        {
                            removed = true;
                            DisableAllLogs();
                        }

                        ScriptingDefineSymbols.AddScriptingDefineSymbol(specifyLogScriptingDefineSymbol);
                        break;
                    }
                }
            }
        }
    }
}