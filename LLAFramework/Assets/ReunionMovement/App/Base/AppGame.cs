using GameLogic.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    public abstract class AppGame : SingletonMgr<AppGame>, IAppEntry
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

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(this.gameObject);
            AppEngine.New(gameObject, this, CreateModules());
        }

        public abstract IEnumerator OnBeforeInit();

        public abstract IEnumerator OnGameStart();
    }
}