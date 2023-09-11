using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GameLogic.EditorTools
{
    public class ColliderGizmoScriptingDefineSymbols
    {
        private const string EnabledAI = "UNITY_AI_ENABLED";
        private const string EnabledPHYSICS2D = "UNITY_PHYSICS2D_ENABLED";
        private const string EnabledPHYSICS = "UNITY_PHYSICS_ENABLED";
        
        //脚本定义符号
        private static readonly string[] AllDefineSymbols = new string[]
        {
            EnabledAI,
            EnabledPHYSICS2D,
            EnabledPHYSICS,
        };

        /// <summary>
        /// 禁用所有日志脚本宏定义。
        /// </summary>
        [MenuItem("工具箱/碰撞器线框宏/禁用所有宏", false, 1)]
        public static void DisableAllColliderGizmo()
        {
            foreach (string specifyLogScriptingDefineSymbol in AllDefineSymbols)
            {
                ScriptingDefineSymbols.RemoveScriptingDefineSymbol(specifyLogScriptingDefineSymbol);
            }
        }

        /// <summary>
        /// 启用所有日志脚本宏定义。
        /// </summary>
        [MenuItem("工具箱/碰撞器线框宏/启用所有宏", false, 1)]
        public static void EnableAllColliderGizmo()
        {
            DisableAllColliderGizmo();
            foreach (string specifyLogScriptingDefineSymbol in AllDefineSymbols)
            {
                ScriptingDefineSymbols.AddScriptingDefineSymbol(specifyLogScriptingDefineSymbol);
            }
        }

        /// <summary>
        /// 启用AI宏。
        /// </summary>
        [MenuItem("工具箱/碰撞器线框宏/启用AI宏", false, 1)]
        public static void EnableAI()
        {
            SetAboveLogScriptingDefineSymbol(EnabledAI);
        }

        /// <summary>
        /// 启用PHYSICS2D宏。
        /// </summary>
        [MenuItem("工具箱/碰撞器线框宏/启用PHYSICS2D宏", false, 2)]
        public static void EnableEnabledPHYSICS2D()
        {
            SetAboveLogScriptingDefineSymbol(EnabledPHYSICS2D);
        }

        /// <summary>
        /// 启用PHYSICS宏。
        /// </summary>
        [MenuItem("工具箱/碰撞器线框宏/启用PHYSICS宏", false, 3)]
        public static void EnableEnabledPHYSICS()
        {
            SetAboveLogScriptingDefineSymbol(EnabledPHYSICS);
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

            foreach (string i in AllDefineSymbols)
            {
                if (i == aboveLogScriptingDefineSymbol)
                {
                    DisableAllColliderGizmo();
                    ScriptingDefineSymbols.AddScriptingDefineSymbol(aboveLogScriptingDefineSymbol);
                    return;
                }
            }
        }
    }
}