using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLogic.Download
{
    /// <summary>
    /// 任务扩展
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        /// 任务链式调用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TV"></typeparam>
        /// <param name="task"></param>
        /// <param name="then"></param>
        /// <returns></returns>
        public static async Task<TV> Then<T, TV>(this Task<T> task, Func<T, TV> then)
        {
            var result = await task;
            return then(result);
        }
    }

    public static class IDownloadFulfillerExtensions
    {

        public static IDownloadFulfiller[] Add(this IDownloadFulfiller[] arr1, IDownloadFulfiller[] arr2)
        {
            if (arr1 == null) arr1 = new IDownloadFulfiller[0];
            if (arr2 == null) arr2 = new IDownloadFulfiller[0];
            var pp = arr1.ToList();
            pp.AddRange(arr2.ToList());
            return pp.ToArray();
        }

        public static IDownloadFulfiller[] Dequeue(this IDownloadFulfiller[] arr)
        {
            if (arr == null) arr = new IDownloadFulfiller[0];
            List<IDownloadFulfiller> strs = new List<IDownloadFulfiller>();
            int i = 0;
            foreach (var str in arr)
            {
                if (i++ == 0) continue;
                strs.Add(str);
            }
            arr = strs.ToArray();
            return arr;
        }

    }


    /// <summary>
    /// 提供将字符串数组添加到一起（使用Linq）和取消数组队列的实用程序
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// 将字符串数组添加到一起
        /// </summary>
        /// <param name="arr1"></param>
        /// <param name="arr2"></param>
        /// <returns></returns>
        public static string[] Add(this string[] arr1, string arr2)
        {
            return Add(arr1, new string[] { arr2 });
        }

        /// <summary>
        /// 将字符串数组添加到一起
        /// </summary>
        /// <param name="arr1"></param>
        /// <param name="arr2"></param>
        /// <returns></returns>
        public static string[] Add(this string[] arr1, string[] arr2)
        {
            if (arr1 == null) arr1 = new string[0];
            if (arr2 == null) arr2 = new string[0];
            arr1.ToList().AddRange(arr2.ToList());
            return arr1.ToArray();
        }

        /// <summary>
        /// 取消阵列的第一个元素的队列
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static string[] Dequeue(this string[] arr)
        {
            if (arr == null) arr = new string[0];
            List<string> strs = new List<string>();
            string v = arr[0];
            int i = 0;
            foreach (var str in arr)
            {
                if (i++ == 0) continue;
                strs.Add(str);
            }
            arr = strs.ToArray();
            return arr;
        }
    }
}
