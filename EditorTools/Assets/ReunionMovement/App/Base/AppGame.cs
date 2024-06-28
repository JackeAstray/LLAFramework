using GameLogic.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    public abstract class AppGame : SingleToneMgr<AppGame>, IAppEntry
    {
        /// <summary>
        /// 创建一个模块，里面有一些新类
        /// </summary>
        /// <returns></returns>
        protected virtual IList<CustommModuleInitialize> CreateModules()
        {
            return new List<CustommModuleInitialize>
            {

            };
        }

        public void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
            Instance = this;
            AppEngine.New(gameObject, this, CreateModules());
        }

        public abstract IEnumerator OnBeforeInit();

        public abstract IEnumerator OnGameStart();

        #region 协程定义
        /// <summary>
        /// 启动协程
        /// </summary>
        /// <param name="coroutine"></param>
        /// <returns></returns>
        public abstract Coroutine StartMyCoroutine(IEnumerator coroutine);

        /// <summary>
        /// 停止协程
        /// </summary>
        /// <param name="coroutine"></param>
        public abstract void StopMyCoroutine(Coroutine coroutine);

        public long taskIndex = 0;
        /// <summary>
        /// 协程任务
        /// </summary>
        public class CoroutineTask : IReference
        {
            public IEnumerator Enumerator;
            public Action<Coroutine> Callback;
            public long TaskId;

            public void Clear()
            {
                TaskId = -1;
                Enumerator = null;
                Callback = null;
            }
        }

        public List<CoroutineTask> coroutineTasks = new List<CoroutineTask>();
        #endregion
    }
}