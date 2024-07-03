using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEditor;
using UnityEngine;

namespace GameLogic.AssetsModule
{
    /// <summary>
    /// AB包管理器(该模块拷贝的 https://github.com/SadPandaStudios/AssetBundleManager )
    /// 目前没有测试
    /// </summary>
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

        #region 数据
        private AssetBundleManager bundleManager;
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

            bundleManager = new AssetBundleManager();
        }

        /// <summary>
        /// 清除数据
        /// </summary>
        public void ClearData()
        {
            Log.Debug("AssetBundleModule 清除数据");
        }

        public void SetBaseUri(string uri)
        {
            bundleManager.SetBaseUri(uri);
            bundleManager.Initialize(OnAssetBundleManagerInitialized);
        }

        private void OnAssetBundleManagerInitialized(bool success)
        {
            if (success)
            {
                bundleManager.GetBundle("BundleNameHere", OnAssetBundleDownloaded, OnProgress);
            }
            else
            {
                Debug.LogError("Error initializing ABM.");
            }
        }

        private void OnAssetBundleDownloaded(AssetBundle bundle)
        {
            if (bundle != null)
            {
                // Do something with the bundle
                bundleManager.UnloadBundle(bundle);
            }

            bundleManager.Dispose();
        }

        private void OnProgress(float progress)
        {
            Debug.Log("Current Progress: " + Math.Round(progress * 100, 2) + "%");
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