using GameLogic.Base;
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
    }
}