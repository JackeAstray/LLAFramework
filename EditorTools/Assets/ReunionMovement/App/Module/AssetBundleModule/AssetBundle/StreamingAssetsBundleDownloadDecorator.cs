using System;
using System.Collections;
using UnityEngine;

namespace GameLogic.AssetsModule
{
    /// <summary>
    /// AssetBundleDownloader 的装饰器，在移动到链中的下一个处理程序之前尝试使用 StreamingAssets 文件夹中的资源。
    /// </summary>
    public class StreamingAssetsBundleDownloadDecorator : ICommandHandler<AssetBundleDownloadCommand>
    {
        private string fullBundlePath;
        private ICommandHandler<AssetBundleDownloadCommand> decorated;
        private string remoteManifestName;

        private AssetBundleManifest manifest;
        private PrioritizationStrategy currentStrategy;
        private Action<IEnumerator> coroutineHandler;
        private string currentPlatform;

        /// <param name="remoteManifestName">
        /// 远程清单的文件名，因此该装饰器知道它应该被忽略。
        /// </param>
        /// <param name="platformName">要使用的平台的名称</param>
        /// <param name="decorated">当 StreamingAssets 中没有可用 bundle 时使用的 CommandHandler</param>
        /// <param name="strategy">
        /// 使用的策略。如果哈希值不同，则默认让远程包覆盖 StreamingAssets 包
        /// </param>
        public StreamingAssetsBundleDownloadDecorator(string remoteManifestName, string platformName, ICommandHandler<AssetBundleDownloadCommand> decorated, PrioritizationStrategy strategy)
        {
            this.decorated = decorated;
            this.remoteManifestName = remoteManifestName;
            currentStrategy = strategy;
            currentPlatform = platformName;

#if UNITY_EDITOR
            if (!Application.isPlaying)
                coroutineHandler = EditorCoroutine.Start;
            else
#endif
                coroutineHandler = AssetBundleDownloaderMonobehaviour.Instance.HandleCoroutine;

            fullBundlePath = Application.streamingAssetsPath + "/" + currentPlatform;
            var fullManifestPath = fullBundlePath + "/" + currentPlatform;
            var manifestBundle = AssetBundle.LoadFromFile(fullManifestPath);

            if (manifestBundle == null) {
                Debug.LogWarningFormat("无法从 StreamingAssets 中检索清单文件 [{0}]，正在禁用 StreamingAssetsBundleDownloadDecorator。", fullManifestPath);
            } else {
                manifest = manifestBundle.LoadAsset<AssetBundleManifest>("assetbundlemanifest");
                manifestBundle.Unload(false);
            }
        }

        /// <summary>
        /// 处理下载命令
        /// </summary>
        public void Handle(AssetBundleDownloadCommand cmd)
        {
            coroutineHandler(InternalHandle(cmd));
        }

        /// <summary>
        /// 返回 StreamingAssets 文件夹中的清单
        /// </summary>
        public AssetBundleManifest GetManifest()
        {
            return manifest;
        }

        private IEnumerator InternalHandle(AssetBundleDownloadCommand cmd)
        {
            // 切勿将 StreamingAssets 用于清单包，始终尝试将其用于具有匹配哈希值的包（除非策略另有说明）
            if (BundleAvailableInStreamingAssets(cmd.BundleName, cmd.Hash)) {
                if (AssetBundleManager.debugLoggingEnabled) Debug.LogFormat("使用 StreamingAssets 的 bundle [{0}]", cmd.BundleName);
                var request = AssetBundle.LoadFromFileAsync(fullBundlePath + "/" + cmd.BundleName);

                while (request.isDone == false)
                    yield return null;

                if (request.assetBundle != null) {
                    cmd.OnComplete(request.assetBundle);
                    yield break;
                }

                Debug.LogWarningFormat("捆绑包 [{0}] 的 StreamingAssets 下载失败，请切换到标准下载。", cmd.BundleName);
            }

            decorated.Handle(cmd);
        }

        /// <summary>
        /// 检查 StreamingAssets 文件夹中是否有可用的包
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        private bool BundleAvailableInStreamingAssets(string bundleName, Hash128 hash)
        {
            // 何时应从 StreamingAssets 中检索包的规则
            // #) StreamingAssets 文件夹中有一个清单
            // #) 我们不会尝试检索远程清单
            // #) 该文件存在于 StreamingAssets 文件夹中
            // #) 以下之一：
            // - 我们优先考虑 StreamingAssets 包而不是远程包
            // - StreamingAssets 中包的哈希值与请求的哈希值匹配

            if (manifest == null) {
                if (AssetBundleManager.debugLoggingEnabled) Debug.Log("StreamingAssets 清单为空，使用标准下载");
                return false;
            }

            if (bundleName == remoteManifestName) {
                if (AssetBundleManager.debugLoggingEnabled) Debug.Log("尝试使用标准下载来下载清单文件");
                return false;
            }

            if (manifest.GetAssetBundleHash(bundleName) != hash && currentStrategy != PrioritizationStrategy.PrioritizeStreamingAssets) {
                if (AssetBundleManager.debugLoggingEnabled) Debug.LogFormat("使用标准下载时，[{0}] 的哈希与 StreamingAssets 中的哈希不匹配", bundleName);
                return false;
            }

            return true;
        }
    }
}