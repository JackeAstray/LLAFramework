using GameLogic.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    public interface IAppEntry
    {
        IEnumerator OnBeforeInit();
        IEnumerator OnGameStart();
    }

    /// <summary>
    /// App引擎
    /// </summary>
    public class AppEngine : SingleToneMgr<AppEngine>
    {
        /// <summary>
        /// 所有自定义游戏逻辑模块
        /// </summary>
        public IList<CustommModuleInitialize> GameModules { get; private set; }

        /// <summary>
        /// 进入
        /// </summary>
        public IAppEntry AppEntry { get; private set; }

        /// <summary>
        /// 用于标识是否从Unity添加
        /// </summary>
        private bool _isNewByStatic = false;

        /// <summary>
        /// 是否初始化完成
        /// </summary>
        public bool IsInited { get; private set; }
        /// <summary>
        /// 在初始化之前
        /// </summary>
        public bool IsBeforeInit { get; private set; }
        /// <summary>
        /// 正在初始化
        /// </summary>
        public bool IsOnInit { get; private set; }
        /// <summary>
        /// 是否开始游戏
        /// </summary>
        public bool IsStartGame { get; private set; }

        //应用程序退出
        public static bool IsApplicationQuit = false;
        //应用程序焦点（是否被压入后台）
        public static bool IsApplicationFocus = true;
        //应用程序正在运行
        public static bool IsAppPlaying = false;
        //更新事件
        public static Action UpdateEvent;
        //每300ms事件更新一次
        public static Action UpdatePer300msEvent;
        //每1s事件更新一次
        public static Action UpdatePer1sEvent;

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
            appEngine._isNewByStatic = true;
            appEngine.GameModules = modules;
            appEngine.AppEntry = entry;

            return appEngine;
        }

        private void Awake()
        {
            Instance = this;
            Init();
        }

        void Start()
        {
            IsAppPlaying = true;
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
            IsBeforeInit = true;
            if (AppEntry != null)
            {
                yield return StartCoroutine(AppEntry.OnBeforeInit());
            }


            IsOnInit = true;
            yield return StartCoroutine(DoInitModules(GameModules));

            IsStartGame = true;
            if (AppEntry != null)
            {
                yield return StartCoroutine(AppEntry.OnGameStart());

            }

            IsInited = true;
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
                if (Application.platform == RuntimePlatform.WindowsEditor)
                {
                    startInitTime = Time.time;
                    startMem = GC.GetTotalMemory(false);
                }

                yield return StartCoroutine(initModule.Init());

                if (Application.platform == RuntimePlatform.WindowsEditor)
                {
                    var nowMem = GC.GetTotalMemory(false);
                }
            }
        }

        private float time_update_per1s, time_update_per300ms;
        protected virtual void Update()
        {
            UpdateEvent?.Invoke();
            float time = Time.time;
            if (time > time_update_per1s)
            {
                time_update_per1s = time + 1.0f;
                UpdatePer1sEvent?.Invoke();
            }
            if (time > time_update_per300ms)
            {
                time_update_per300ms = time + 0.3f;
                UpdatePer300msEvent?.Invoke();
            }

            if (GameModules.Count > 0)
            {
                foreach (CustommModuleInitialize module in GameModules)
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
            IsApplicationQuit = true;
            IsAppPlaying = false;
        }
        /// <summary>
        /// 焦点处理
        /// </summary>
        /// <param name="focus"></param>
        void OnApplicationFocus(bool focus)
        {
            IsApplicationFocus = focus;
        }

        /// <summary>
        /// 清除数据，比如切换帐号/低内存等清空缓存数据
        /// </summary>
        public void ClearModuleData()
        {
            var modules = GameModules;
            foreach (CustommModuleInitialize initModule in modules)
            {
                initModule.ClearData();
            }
        }
    }
}