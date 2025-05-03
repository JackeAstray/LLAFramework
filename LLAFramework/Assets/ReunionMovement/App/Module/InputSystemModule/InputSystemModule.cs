using LLAFramework.Download;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LLAFramework
{
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

            InitializeDefaultBindings();

            yield return null;
            initProgress = 100;
            IsInited = true;
            Log.Debug("InputSystemModule 初始化完成");
        }

        public void ClearData()
        {
            Log.Debug("InputSystemModule 清除数据");
        }

        public void UpdateTime(float elapseSeconds, float realElapseSeconds)
        {
            // 如果处于按键绑定模式，监听键盘输入
            if (isRebinding)
            {
                ListenForRebind();
            }
        }

        private void InitializeDefaultBindings()
        {
            // 示例：添加默认按键绑定
            var moveAction = new InputAction("Move", InputActionType.Value, "<Keyboard>/w");
            moveAction.AddBinding("<Keyboard>/a");
            moveAction.AddBinding("<Keyboard>/s");
            moveAction.AddBinding("<Keyboard>/d");
            moveAction.Enable();

            var jumpAction = new InputAction("Jump", InputActionType.Button, "<Keyboard>/space");
            jumpAction.Enable();

            inputActions["Move"] = moveAction;
            inputActions["Jump"] = jumpAction;

            Log.Debug("默认按键绑定已初始化");
        }

        public void RebindAction(string actionName, string newBinding)
        {
            if (inputActions.TryGetValue(actionName, out var action))
            {
                action.ApplyBindingOverride(newBinding);
                Log.Debug($"Action {actionName} 绑定已更新为 {newBinding}");
            }
            else
            {
                Log.Warning($"未找到名为 {actionName} 的按键绑定");
            }
        }

        private void ListenForRebind()
        {
            // 捕获任意按键输入
            if (Keyboard.current.anyKey.wasPressedThisFrame)
            {
                foreach (var key in Keyboard.current.allKeys)
                {
                    if (key.wasPressedThisFrame)
                    {
                        ApplyRebind(key.path);
                        break;
                    }
                }
            }
        }

        private void ApplyRebind(string newBinding)
        {
            if (inputActions.TryGetValue(actionToRebind, out var action))
            {
                action.ApplyBindingOverride(newBinding);
                Log.Debug($"Action {actionToRebind} 绑定已更新为 {newBinding}");
            }
            else
            {
                Log.Warning($"未找到名为 {actionToRebind} 的按键绑定");
            }

            // 退出绑定模式
            isRebinding = false;
            actionToRebind = null;
        }
    }
}
