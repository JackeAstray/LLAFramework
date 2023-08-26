using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    public class SoundModule : CustommModuleInitialize
    {
        #region 实例与初始化
        public static SoundModule Instance = new SoundModule();
        public bool IsInited { get; private set; }
        private double _initProgress = 0;
        public double InitProgress { get { return _initProgress; } }
        #endregion

        public IEnumerator Init()
        {
            Log.Debug("SoundModule 初始化");
            _initProgress = 0;
            yield return null;
            _initProgress = 100;
            IsInited = true;
        }

        public void ClearData()
        {
            Log.Debug("SoundModule 清除数据");
        }

        /// <summary>
        /// 场景管理器轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        public void Update(float elapseSeconds, float realElapseSeconds)
        {

        }
    }
}