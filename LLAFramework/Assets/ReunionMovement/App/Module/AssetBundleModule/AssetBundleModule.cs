using CatAsset.Runtime;
using GameLogic.Http;
using GameLogic.Http.Service;
using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Burst.Intrinsics;
using UnityEditor;
using UnityEngine;

namespace GameLogic.AssetsModule
{
    /// <summary>
    /// AB包版本
    /// </summary>
    public class ABVersion
    {
        public int ManifestVersion;
        public bool IsUpdate;
        public string StartLimitTime;
        public string EndLimitTime;
    }

    /// <summary>
    /// AB包管理器
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
        public string assetServerIP;
        private bool isChcked;
        string versionTxtUri;
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
            //VerifyVersionNumber();
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
        /// 验证版本号
        /// </summary>
        public void VerifyVersionNumber()
        {
            isChcked = false;
            assetServerIP = "192.168.18.37:8081";
            versionTxtUri = assetServerIP + "/version.txt";

            var request = HttpModule.Get(versionTxtUri)
                .OnSuccess(GetVersion)
                .OnError(response => Debug.LogError(response.StatusCode))
                .Send();
        }

        /// <summary>
        /// 获取版本
        /// </summary>
        /// <param name="response"></param>
        public void GetVersion(HttpResponse response)
        {
            ABVersion webVersion = JsonMapper.ToObject<ABVersion>(response.Text);
            int manifestVersion = webVersion.ManifestVersion;
            DateTime start = DateTimeExtensions.GetDateTimeByString(webVersion.StartLimitTime);
            DateTime end = DateTimeExtensions.GetDateTimeByString(webVersion.EndLimitTime);
            bool isUpdate = webVersion.IsUpdate;

            string uriPrefix = $"{assetServerIP}/StandaloneWindows64/{manifestVersion}";
            switch (Application.platform)
            {
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.LinuxEditor:
                case RuntimePlatform.WindowsEditor:
                    uriPrefix = $"{assetServerIP}/StandaloneWindows64/{manifestVersion}";
                    break;

                case RuntimePlatform.OSXPlayer:
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.LinuxPlayer:
                    uriPrefix = $"{assetServerIP}/StandaloneWindows64/{manifestVersion}";
                    break;

                case RuntimePlatform.IPhonePlayer:
                    uriPrefix = $"{assetServerIP}/Android/{manifestVersion}";
                    break;

                case RuntimePlatform.Android:
                    uriPrefix = $"{assetServerIP}/iOS/{manifestVersion}";
                    break;

                case RuntimePlatform.WebGLPlayer:
                    uriPrefix = $"{assetServerIP}/WebGL/{manifestVersion}";
                    break;
            }

            if (isUpdate)
            {
                //检查资源版本
                CatAssetManager.CheckVersion(OnVersionChecked);
            }
        }

        /// <summary>
        /// 资源版本检查回调
        /// </summary>
        /// <param name="result"></param>
        private void OnVersionChecked(VersionCheckResult result)
        {
            if (!string.IsNullOrEmpty(result.Error))
            {
                Debug.LogError($"资源版本检查失败：{result.Error}");
                return;
            }

            Debug.Log($"资源版本检查成功：{result}");
            isChcked = true;

            //遍历所有资源组的更新器
            List<GroupInfo> groups = CatAssetManager.GetAllGroupInfo();
            foreach (GroupInfo groupInfo in groups)
            {
                GroupUpdater updater = CatAssetManager.GetGroupUpdater(groupInfo.GroupName);
                if (updater != null)
                {
                    //存在资源组对应的更新器 就说明此资源组有需要更新的资源
                    Debug.Log($"{groupInfo.GroupName}组的资源需要更新，请按A更新");
                }
            }
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