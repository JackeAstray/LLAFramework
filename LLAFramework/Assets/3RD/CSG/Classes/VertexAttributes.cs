namespace LLAFramework
{
    /// <summary>
    /// Mesh attributes bitmask.
    /// </summary>
    [System.Flags]
    public enum VertexAttributes
    {
        /// <summary>
        /// 定点位置
        /// </summary>
        Position = 0x1,
        /// <summary>
        /// 顶点UV0
        /// </summary>
        Texture0 = 0x2,
        /// <summary>
        /// 顶点UV1
        /// </summary>
        Texture1 = 0x4,
        /// <summary>
        /// 顶点UV2. 光照贴图
        /// </summary>
        Lightmap = 0x4,
        /// <summary>
        /// 顶点UV3
        /// </summary>
        Texture2 = 0x8,
        /// <summary>
        /// 顶点UV4
        /// </summary>
        Texture3 = 0x10,
        /// <summary>
        /// 顶点颜色
        /// </summary>
        Color = 0x20,
        /// <summary>
        /// 顶点法线
        /// </summary>
        Normal = 0x40,
        /// <summary>
        /// 顶点切线
        /// </summary>
        Tangent = 0x80,
        /// <summary>
        /// 所有存储的网格属性
        /// </summary>
        All = 0xFF
    };
}