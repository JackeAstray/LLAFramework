using LLAFramework.Download;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace LLAFramework
{
    /// <summary>
    /// 输入系统模块 (该模块为创建阶段)
    /// </summary>
    public class InputSystemModule : CustommModuleInitialize
    {
        #region 实例与初始化
        public static InputSystemModule Instance = new InputSystemModule();
        public bool IsInited { get; private set; }
        private double initProgress = 0;
        public double InitProgress { get { return initProgress; } }
        #endregion

        private Dictionary<string, InputAction> inputActions = new Dictionary<string, InputAction>();

        // 按键绑定模式状态
        private bool isRebinding = false;
        private string actionToRebind;

        public IEnumerator Init()
        {
            initProgress = 0;

            InitializeDefaultBindings(JsonDatabaseModule.Instance.GetInputSystemConfig().Values.ToList());

            yield return null;
            initProgress = 100;
            IsInited = true;

            // 注册输入监听
            RegisterInputListeners();

            Log.Debug("InputSystemModule 初始化完成");
        }

        public void ClearData()
        {
            UnregisterInputListeners();
            Log.Debug("InputSystemModule 清除数据");
        }

        public void UpdateTime(float elapseSeconds, float realElapseSeconds)
        {

        }

        /// <summary>
        /// 初始化默认按键绑定
        /// </summary>
        /// <param name="configs"></param>
        private void InitializeDefaultBindings(List<InputSystemConfig> configs)
        {
            if (configs == null || configs.Count == 0)
            {
                Log.Warning("未提供任何按键绑定配置");
                return;
            }

            foreach (var config in configs)
            {
                if (string.IsNullOrEmpty(config.ActionName) || string.IsNullOrEmpty(config.BindingPath))
                {
                    Log.Warning($"配置无效: Index={config.Number}, ActionName={config.ActionName}, BindingPath={config.BindingPath}");
                    continue;
                }

                // 验证 BindingPath 格式
                if (!IsValidBindingPath(config.BindingPath))
                {
                    Log.Warning($"无效的绑定路径: {config.BindingPath}");
                    continue;
                }

                if (!inputActions.TryGetValue(config.ActionName, out var inputAction))
                {
                    //config.Interactions, config.Processors, config.Groups
                    inputAction = new InputAction(config.ActionName, InputActionType.Button);
                    inputActions[config.ActionName] = inputAction;
                }

                var binding = inputAction.AddBinding(config.BindingPath);

                if (!string.IsNullOrEmpty(config.Interactions))
                {
                    binding.WithInteractions(config.Interactions);
                }

                if (!string.IsNullOrEmpty(config.Processors))
                {
                    binding.WithProcessors(config.Processors);
                }

                if (!string.IsNullOrEmpty(config.Groups))
                {
                    binding.WithGroup(config.Groups);
                }

                inputAction.Enable();
            }
        }

        private bool IsValidBindingPath(string bindingPath)
        {
            // 检查路径是否包含无效的修饰符
            return !bindingPath.Contains("[Default]");
        }

        #region 监听
        /// <summary>
        /// 注册输入监听
        /// </summary>
        public void RegisterInputListeners()
        {
            foreach (var action in inputActions.Values)
            {
                action.performed += OnInputPerformed;
                action.started += OnInputStarted;
                action.canceled += OnInputCanceled;
            }
        }

        /// <summary>
        /// 移除输入监听
        /// </summary>
        public void UnregisterInputListeners()
        {
            foreach (var action in inputActions.Values)
            {
                action.performed -= OnInputPerformed;
                action.started -= OnInputStarted;
                action.canceled -= OnInputCanceled;
            }
        }

        /// <summary>
        /// 延时触发调用
        /// </summary>
        private void OnInputPerformed(InputAction.CallbackContext context)
        {
            // 获取触发输入的控制器
            var control = context.control;

            switch (context.action.name)
            {
                case "Move":
                    break;
                case "Return":
                    break;
                case "Confirm":
                    break;
                case "Select":
                    break;
                case "Switch":
                    break;
                case "OpenConsole":
                    if (UIModule.Instance != null)
                    {
                        if (!UIModule.Instance.IsOpen("TerminalUIPlane"))
                        {
                            UIModule.Instance.OpenWindow("TerminalUIPlane");
                        }
                    }
                    break;
                case "Arrow":
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 按下-立即调用
        /// </summary>
        private void OnInputStarted(InputAction.CallbackContext context)
        {
            // 获取触发输入的控制器
            var control = context.control;

            switch (context.action.name)
            {
                case "Move":
                    break;
                case "Return":
                    break;
                case "Confirm":
                    break;
                case "Select":
                    break;
                case "Switch":
                    break;
                case "OpenConsole":
                    if (UIModule.Instance != null)
                    {
                        if (UIModule.Instance.IsOpen("TerminalUIPlane"))
                        {
                            UIModule.Instance.CloseWindow("TerminalUIPlane");
                        }
                    }
                    break;
                case "Arrow":
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 抬起-立即调用
        /// </summary>
        private void OnInputCanceled(InputAction.CallbackContext context)
        {
            // 获取触发输入的控制器
            var control = context.control;

            switch (context.action.name)
            {
                case "Move":
                    break;
                case "Return":
                    break;
                case "Confirm":
                    break;
                case "Select":
                    break;
                case "Switch":
                    break;
                case "OpenConsole":
                    break;
                case "Arrow":
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 重新绑定按键
        /// </summary>
        /// <param name="actionName">动作名称，例如 "Move"</param>
        /// <param name="newBindingPath">新的按键绑定路径，例如 "<Keyboard>/upArrow"</param>
        public void RebindAction(string actionName, string oldBindingPath, string newBindingPath)
        {
            if (string.IsNullOrEmpty(actionName) || string.IsNullOrEmpty(newBindingPath))
            {
                Log.Warning("动作名称或绑定路径不能为空");
                return;
            }

            if (!inputActions.TryGetValue(actionName, out var inputAction))
            {
                Log.Warning($"未找到动作: {actionName}");
                return;
            }

            var bindingIndex = inputAction.GetBindingIndex(oldBindingPath);
            if (bindingIndex == -1)
            {
                Log.Warning($"未找到动作 {actionName} 的绑定路径: {oldBindingPath}");
                return;
            }

            // 应用新的绑定路径
            inputAction.ApplyBindingOverride(bindingIndex, newBindingPath);
            Log.Debug($"动作 {actionName} 的绑定已更新为 {newBindingPath}");
        }
        #endregion
    }
}
