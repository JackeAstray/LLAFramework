using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    public class AssetBundleModule : CustommModuleInitialize
    {
        #region 实例与初始化
        //实例
        public static AssetBundleModule Instance = new AssetBundleModule();
        //是否初始化完成
        public bool IsInited { get; private set; }
        //初始化进度
        private double initProgress = 0;
        public double InitProgress { get { return initProgress; } }
        #endregion

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public IEnumerator Init()
        {
            initProgress = 0;
            yield return null;
            initProgress = 100;
            IsInited = true;
            Log.Debug("AssetBundleModule 初始化完成");
        }

        /// <summary>
        /// 清除数据
        /// </summary>
        public void ClearData()
        {
            Log.Debug("AssetBundleModule 清除数据");
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        public void UpdateTime(float elapseSeconds, float realElapseSeconds)
        {

        }
    }
}