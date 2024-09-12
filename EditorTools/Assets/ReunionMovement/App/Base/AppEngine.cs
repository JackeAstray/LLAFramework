using GameLogic.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// App引擎
    /// </summary>
    public class AppEngine : SingletonMgr<AppEngine>
    {
        /// <summary>
        /// 所有自定义游戏逻辑模块
        /// </summary>
        public IList<CustommModuleInitialize> gameModules { get; private set; }

        /// <summary>
        /// 进入
        /// </summary>
        public IAppEntry appEntry { get; private set; }

        ///// <summary>
        ///// 用于标识是否从Unity添加
        ///// </summary>
        //private bool isNewByStatic = false;

        /// <summary>
        /// 是否初始化完成
        /// </summary>
        public bool isInited { get; private set; }
        /// <summary>
        /// 在初始化之前
        /// </summary>
        public bool isBeforeInit { get; private set; }
        /// <summary>
        /// 正在初始化
        /// </summary>
        public bool isOnInit { get; private set; }
        /// <summary>
        /// 是否开始游戏
        /// </summary>
        public bool isStartGame { get; private set; }

        // 应用程序退出
        public static bool isApplicationQuit = false;
        // 应用程序焦点（是否被压入后台）
        public static bool isApplicationFocus = true;
        // 应用程序正在运行
        public static bool isAppPlaying = false;
        // 更新事件
        public static Action updateEvent;
        // 每300ms事件更新一次
        public static Action updatePer300msEvent;
        // 每1s事件更新一次
        public static Action updatePer1sEvent;

        private float time_update_per1s;
        private float time_update_per300ms;

        private bool isWindowsEditor;

        /// <summary>
        /// 启动入口
        /// </summary>
        /// <param name="gameObjectToAttach"></param>
        /// <param name="entry"></param>
        /// <param name="modules"></param>
        /// <returns></returns>
        public static AppEngine New(GameObject gameObjectToAttach, IAppEntry entry, IList<CustommModuleInitialize> modules)
        {
            AppEngine appEngine = gameObjectToAttach.AddComponent<AppEngine>();
            //appEngine.isNewByStatic = true;
            appEngine.gameModules = modules;
            appEngine.appEntry = entry;

            return appEngine;
        }

        private void Awake()
        {
            Instance = this;
            Init();
        }

        void Start()
        {
            isAppPlaying = true;
            isWindowsEditor = Application.platform == RuntimePlatform.WindowsEditor;
        }

        private void Init()
        {
            StartCoroutine(DoInit());
        }

        /// <summary>
        /// 执行初始化
        /// </summary>
        /// <returns></returns>
        private IEnumerator DoInit()
        {
            yield return null;
            isBeforeInit = true;
            if (appEntry != null)
            {
                yield return StartCoroutine(appEntry.OnBeforeInit());
            }


            isOnInit = true;
            yield return StartCoroutine(DoInitModules(gameModules));

            isStartGame = true;
            if (appEntry != null)
            {
                yield return StartCoroutine(appEntry.OnGameStart());

            }

            isInited = true;
        }

        /// <summary>
        /// 执行从初始化模块
        /// </summary>
        /// <param name="modules"></param>
        /// <returns></returns>
        private IEnumerator DoInitModules(IList<CustommModuleInitialize> modules)
        {
            var startInitTime = 0f;
            var startMem = 0f;
            foreach (CustommModuleInitialize initModule in modules)
            {
                if (isWindowsEditor)
                {
                    startInitTime = Time.time;
                    startMem = GC.GetTotalMemory(false);
                }

                yield return StartCoroutine(initModule.Init());

                if (isWindowsEditor)
                {
                    var nowMem = GC.GetTotalMemory(false);
                }
            }
        }

        protected virtual void Update()
        {
            updateEvent?.Invoke();
            float time = Time.time;
            if (time > time_update_per1s)
            {
                time_update_per1s = time + 1.0f;
                updatePer1sEvent?.Invoke();
            }
            if (time > time_update_per300ms)
            {
                time_update_per300ms = time + 0.3f;
                updatePer300msEvent?.Invoke();
            }

            if (gameModules.Count > 0)
            {
                foreach (CustommModuleInitialize module in gameModules)
                {
                    module.UpdateTime(Time.deltaTime, Time.unscaledDeltaTime);
                }
            }
        }

        /// <summary>
        /// 退出处理
        /// </summary>
        void OnApplicationQuit()
        {
            isApplicationQuit = true;
            isAppPlaying = false;
        }
        /// <summary>
        /// 焦点处理
        /// </summary>
        /// <param name="focus"></param>
        void OnApplicationFocus(bool focus)
        {
            isApplicationFocus = focus;
        }

        /// <summary>
        /// 清除数据，比如切换帐号/低内存等清空缓存数据
        /// </summary>
        public void ClearModuleData()
        {
            var modules = gameModules;
            foreach (CustommModuleInitialize initModule in modules)
            {
                initModule.ClearData();
            }
        }
    }
}