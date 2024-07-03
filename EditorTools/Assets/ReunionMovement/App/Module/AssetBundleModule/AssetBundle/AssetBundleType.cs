namespace GameLogic.AssetsModule
{
    /// <summary>
    /// 下载设置
    /// </summary>
    public enum DownloadSettings
    {
        // 从缓存中获取
        UseCacheIfAvailable,
        // 不使用缓存
        DoNotUseCache
    }

    /// <summary>
    /// 优先级策略
    /// </summary>
    public enum PrioritizationStrategy
    {
        // 远程优先
        PrioritizeRemote,
        // 流媒体资产优先
        PrioritizeStreamingAssets,
    }

    /// <summary>
    /// 主清单类型
    /// </summary>
    public enum PrimaryManifestType
    {
        None,
        // 远程
        Remote,
        // 远程缓存
        RemoteCached,
        // 流媒体资产
        StreamingAssets,
    }
}