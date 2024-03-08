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
            BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;
            
            // 导出路径
            string exportPath = Path.Combine(Application.dataPath, "../AssetBundles/"+ buildTarget);

            if (!Directory.Exists(exportPath))
            {
                Directory.CreateDirectory(exportPath);
            }

            Log.Debug("导出路径: " + exportPath);
        }
    }
}