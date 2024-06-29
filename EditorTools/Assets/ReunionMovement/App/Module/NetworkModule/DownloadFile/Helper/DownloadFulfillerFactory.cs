using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLogic.Download
{
    /// <summary>
    /// 包含根据其某个子类名创建“IDownloadFulfiller”子类的函数
    /// </summary>
    public static class DownloadFulfillerFactory
    {
        /// <summary>
        /// 存储所有“IDownloadFulfiller”子类型
        /// </summary>
        /// <typeparam name="Singleton"></typeparam>
        /// <returns></returns>
        internal static List<Type> types = new List<Type>();

        /// <summary>
        /// 搜索程序集以查找“IDownloadFulfiller”的所有子项并将其存储在“types”中
        /// </summary>
        static DownloadFulfillerFactory()
        {
            types = typeof(IDownloadFulfiller).Assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(IDownloadFulfiller)) && !t.IsAbstract).Select(t => 
            {
                //忽略基类型
                if (t == typeof(IDownloadFulfiller))
                {
                    return null;
                }
                return t; 
            }).ToList();
        }

        /// <summary>
        /// 给定一个与某个“IDownloadFulfiller”子类的完整类名匹配的字符串，返回该类的新实例。
        /// </summary>
        /// <param name="classname"></param>
        /// <returns></returns>
        public static IDownloadFulfiller CreateFromClassName(string classname)
        {
            return (IDownloadFulfiller)Activator.CreateInstance(types.Find(t => t.Name == classname));
        }
    }
}
