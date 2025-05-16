using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Tilemaps;

namespace LLAFramework.Download
{
    /// <summary>
    /// 下载执行器工厂
    /// </summary>
    public static class DownloadExecutorFactory
    {
        /// <summary>
        /// 下载执行器类型映射（类名->类型）
        /// </summary>
        private static Dictionary<string, Type> typeMap = new Dictionary<string, Type>();

        /// <summary>
        /// 静态构造函数
        /// </summary>
        static DownloadExecutorFactory()
        {
            typeMap = typeof(IDownloadExecutor).Assembly
                      .GetTypes()
                      .Where(t => t.IsSubclassOf(typeof(IDownloadExecutor)) && !t.IsAbstract)
                      .ToDictionary(t => t.Name, t => t);
        }

        /// <summary>
        /// 创建下载执行器
        /// </summary>
        /// <param name="classname"></param>
        /// <returns></returns>
        public static IDownloadExecutor CreateFromClassName(string classname)
        {
            if (string.IsNullOrEmpty(classname))
            {
                Log.Error($"{classname} 是Null 或 Empty");
                return null;
            }

            if (!typeMap.TryGetValue(classname, out var type))
            {
                Log.Error($"未找到名为 {classname} 的下载执行器类型");
                return null;
            }

            return (IDownloadExecutor)Activator.CreateInstance(type);
        }
    }
}
