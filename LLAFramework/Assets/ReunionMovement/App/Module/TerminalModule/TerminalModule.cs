using LLAFramework.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LLAFramework
{
    public class TerminalModule : CustommModuleInitialize
    {
        #region 实例与初始化
        public static TerminalModule Instance = new TerminalModule();
        public bool IsInited { get; private set; }
        private double initProgress = 0;
        public double InitProgress { get { return initProgress; } }
        #endregion

        Keyboard keyboard;

        public TerminalRequest terminalRequest { get; private set; }

        public IEnumerator Init()
        {
            initProgress = 0;
            keyboard = Keyboard.current;
            terminalRequest = new TerminalRequest();
            yield return null;
            initProgress = 100;
            IsInited = true;
            Log.Debug("TerminalModule 初始化完成");
        }

        public void ClearData()
        {
            Log.Debug("TerminalModule 清除数据");
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        public void UpdateTime(float elapseSeconds, float realElapseSeconds)
        {

        }

        #region 例子
        [RegisterCommand(Help = "TestTerminal 2 String", MinArgCount = 2, MaxArgCount = 2)]
        static void TestTerminal(CommandArg[] args)
        {
            if (args.Length >= 2)
            {
                string str = "TestTerminal " + args[0].String + " | " + args[0].String;

                Log.Debug(str);

                if (UIModule.Instance.IsOpen("TerminalUIPlane"))
                {
                    UIModule.Instance.SetWindow("TerminalUIPlane", "CreateItem", str);
                }
            }
        }
        #endregion
    }
}