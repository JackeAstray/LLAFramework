using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 显示Build结果，并控制版本号
    /// </summary>
    public class AppBuild : IPreprocessBuildWithReport
    {
        public int callbackOrder { get; private set; }

        public void OnPreprocessBuild(BuildReport report)
        {
            // 获取当前的Bundle版本号
            string currentVersion = PlayerSettings.bundleVersion;

            Log.Debug("构建开始 - " + report.summary.buildStartedAt);
            Log.Debug("构建平台 - " + report.summary.platform);
            Log.Debug("构建路径 - " + report.summary.outputPath);
            Log.Debug("构建结束 - " + report.summary.buildEndedAt);
            Log.Debug("构建结果 - " + report.summary.result);
            Log.Debug("构建总大小 - " + report.summary.totalSize);
            Log.Debug("构建总时长 - " + report.summary.totalTime);
            Log.Debug("构建总时长 - " + report.summary.totalTime);
            Log.Debug("当前版本 - " + currentVersion);

            // 解析版本号
            Version version = new Version(currentVersion);

            // 目标版本
            Version targetVersion = new Version(version.Major, version.Minor);
            
            // 增加版本号
            version = new Version(targetVersion.Major, targetVersion.Minor, version.Build + 1);

            // 将新的版本号设置为Bundle版本号
            PlayerSettings.bundleVersion = version.ToString();
        }
    }
}