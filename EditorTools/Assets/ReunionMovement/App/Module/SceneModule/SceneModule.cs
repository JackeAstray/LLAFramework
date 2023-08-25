using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 场景模块
    /// </summary>
    public class SceneModule : CustommModuleInitialize
    {
        #region 实例与初始化
        public static LanguagesModule Instance = new LanguagesModule();
        public bool IsInited { get; private set; }
        private double _initProgress = 0;
        public double InitProgress { get { return _initProgress; } }
        #endregion

        public IEnumerator Init()
        {
            Log.Debug("LanguagesModule 初始化");

            yield return null;
            IsInited = true;
        }

        public void ClearData()
        {
            Log.Debug("LanguagesModule 清除数据");
        }

        public void Update(float elapseSeconds, float realElapseSeconds)
        {

        }
    }
}