using GameLogic.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameLogic
{
    public class TerminalModule : CustommModuleInitialize
    {
        #region 实例与初始化
        public static TerminalModule Instance = new TerminalModule();
        public bool IsInited { get; private set; }
        private double _initProgress = 0;
        public double InitProgress { get { return _initProgress; } }
        #endregion

        Keyboard keyboard;
        
        public TerminalRequest terminalRequest { get; private set; }

        public IEnumerator Init()
        {
            Log.Debug("TerminalModule 初始化");
            _initProgress = 0;

            keyboard = Keyboard.current;
            terminalRequest = new TerminalRequest();

            yield return null;
            _initProgress = 100;
            IsInited = true;
        }

        public void ClearData()
        {
            Log.Debug("TerminalModule 清除数据");
        }

        /// <summary>
        /// 场景管理器轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        public void UpdateTime(float elapseSeconds, float realElapseSeconds)
        {
            if (keyboard == null)
            {
                return;
            }

            if (keyboard[Key.Backquote].wasPressedThisFrame)
            {
                if (UIModule.Instance != null)
                {
                    if (!UIModule.Instance.IsOpen("TerminalUIPlane"))
                    {
                        UIModule.Instance.OpenWindow("TerminalUIPlane");
                    }
                    else
                    {
                        UIModule.Instance.CloseWindow("TerminalUIPlane");
                    }
                }
            }

            if (keyboard[Key.Escape].wasPressedThisFrame)
            {
                if (UIModule.Instance.IsOpen("TerminalUIPlane"))
                {
                    UIModule.Instance.CloseWindow("TerminalUIPlane");
                }
            }
        }

        [RegisterCommand(Help = "TestTerminal 2 String", MinArgCount = 2, MaxArgCount = 2)]
        static void TestTerminal(CommandArg[] args)
        {
            if (args.Length >= 2)
            {
                string str = "TestTerminal " + args[0].String + " | " + args[0].String;

                Log.Debug("TestTerminal " + args[0].String + " | " + args[0].String);

                if (UIModule.Instance.IsOpen("TerminalUIPlane"))
                {
                    UIModule.Instance.SetWindow("TerminalUIPlane", "CreateItem", str);
                }
            }
        }
    }
}