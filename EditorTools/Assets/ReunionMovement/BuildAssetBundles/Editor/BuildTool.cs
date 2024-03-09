using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GameLogic.EditorTools
{
    public static class BuildTool
    {
        //22-40
        [MenuItem("工具箱/构建包/构建Bundles", false, 22)]
        public static void BuildBundles()
        {
            // 获取ResourcesFile文件夹下的所有子文件夹
            string[] directoryEntries = Directory.GetDirectories("Assets/ResourcesFile");
            // 获取当前的BuildTarget
            BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;
            // 导出路径
            string exportPath = Path.Combine(Application.dataPath, "../AssetBundles/"+ buildTarget);
            // 如果导出路径不存在，则创建
            if (!Directory.Exists(exportPath))
            {
                Directory.CreateDirectory(exportPath);
            }

            Log.Debug("导出路径: " + exportPath);
            // 遍历所有子文件夹
            foreach (string directoryPath in directoryEntries)
            {
                // 获取文件夹名称作为AB包的名称
                string assetBundleName = Path.GetFileName(directoryPath);
                // 设置AB包的名称
                AssetImporter.GetAtPath(directoryPath).assetBundleName = assetBundleName;
                // 打包AB包到指定路径
                BuildPipeline.BuildAssetBundles("../AssetBundles", BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
            }
        }
    }
}