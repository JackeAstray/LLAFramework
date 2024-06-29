using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace GameLogic.Download
{
    public class DownloadFileModule : CustommModuleInitialize
    {
        #region 实例与初始化
        public static DownloadFileModule Instance = new DownloadFileModule();
        public bool IsInited { get; private set; }
        private double initProgress = 0;
        public double InitProgress { get { return initProgress; } }
        #endregion

        #region 数据

        #endregion

        public IEnumerator Init()
        {
            initProgress = 0;
            yield return null;
            initProgress = 100;
            IsInited = true;

            Log.Debug("DownloadFileModule 初始化完成");
        }

        public void ClearData()
        {
            Log.Debug("DownloadFileModule 清除数据");
        }


        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="elapseSeconds"></param>
        /// <param name="realElapseSeconds"></param>
        public void UpdateTime(float elapseSeconds, float realElapseSeconds)
        {

        }
    }
}