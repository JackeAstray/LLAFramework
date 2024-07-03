using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameLogic.AssetsModule
{
    public class Utility
    {
        public const string AssetBundlesOutputPath = "AssetBundles";

        public static string GetPlatformName()
        {
#if UNITY_EDITOR
            return GetPlatformForAssetBundles(EditorUserBuildSettings.activeBuildTarget);
#else
			return GetPlatformForAssetBundles(Application.platform);
#endif
        }

#if UNITY_EDITOR
        /// <summary>
        /// 获取资源包平台
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static string GetPlatformForAssetBundles(BuildTarget target)
        {
            switch (target)
            {
                case BuildTarget.Android:
                    return "Android";
                case BuildTarget.iOS:
                    return "iOS";
                case BuildTarget.tvOS:
                    return "tvOS";
                case BuildTarget.WebGL:
                    return "WebGL";
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return "StandaloneWindows";
                case BuildTarget.StandaloneOSX:
                    return "StandaloneOSX";
                // 为您自己的添加更多构建目标
                // 如果添加更多目标，请不要忘记将相同的平台添加到下面的函数中。
                case BuildTarget.StandaloneLinux64:
#if !UNITY_2019_1_OR_NEWER
                case BuildTarget.StandaloneLinuxUniversal:
                case BuildTarget.StandaloneLinux:
#endif
                    return "StandaloneLinux";
                case BuildTarget.Switch:
                    return "Switch";
                default:
                    Debug.Log("未知的构建目标：使用默认枚举名称: " + target);
                    return target.ToString();
            }
        }
#endif

        public static string GetPlatformForAssetBundles(RuntimePlatform platform)
        {
            switch (platform)
            {
                case RuntimePlatform.Android:
                    return "Android";
                case RuntimePlatform.IPhonePlayer:
                    return "iOS";
                case RuntimePlatform.tvOS:
                    return "tvOS";
                case RuntimePlatform.WebGLPlayer:
                    return "WebGL";
                case RuntimePlatform.WindowsPlayer:
                    return "StandaloneWindows";
                case RuntimePlatform.OSXPlayer:
                    return "StandaloneOSX";
                case RuntimePlatform.LinuxPlayer:
                    return "StandaloneLinux";
                case RuntimePlatform.Switch:
                    return "Switch";
                // 为您自己的添加更多构建目标。
                // 如果您添加更多目标，请不要忘记将相同的平台添加到上面的函数中。
                default:
                    Debug.Log("未知的构建目标：使用默认枚举名称: " + platform);
                    return platform.ToString();
            }
        }
    }
}
