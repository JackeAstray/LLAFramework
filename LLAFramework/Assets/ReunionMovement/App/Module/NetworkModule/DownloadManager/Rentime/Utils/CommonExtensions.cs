using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLAFramework.Download
{
    /// <summary>
    /// 任务扩展
    /// </summary>
    public static class TaskExtensions
    {
        public static async Task<TV> Then<T, TV>(this Task<T> task, Func<T, TV> then)
        {
            var result = await task;
            return then(result);
        }
    }

    /// <summary>
    /// 下载执行器扩展
    /// </summary>
    public static class IDownloadExecutorExtensions
    {
        /// <summary>
        /// 将两个IDownloadExecutor数组合并
        /// </summary>
        public static IDownloadExecutor[] Add(this IDownloadExecutor[] arr1, IDownloadExecutor[] arr2)
        {
            arr1 ??= Array.Empty<IDownloadExecutor>();
            arr2 ??= Array.Empty<IDownloadExecutor>();
            var result = new IDownloadExecutor[arr1.Length + arr2.Length];
            Array.Copy(arr1, 0, result, 0, arr1.Length);
            Array.Copy(arr2, 0, result, arr1.Length, arr2.Length);
            return result;
        }

        /// <summary>
        /// 出队列（移除第一个元素）
        /// </summary>
        public static IDownloadExecutor[] Dequeue(this IDownloadExecutor[] arr)
        {
            arr ??= Array.Empty<IDownloadExecutor>();
            if (arr.Length <= 1) return Array.Empty<IDownloadExecutor>();
            var result = new IDownloadExecutor[arr.Length - 1];
            Array.Copy(arr, 1, result, 0, arr.Length - 1);
            return result;
        }
    }

    /// <summary>
    /// string[]扩展
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// 添加单个元素
        /// </summary>
        public static string[] Add(this string[] arr1, string arr2)
        {
            return arr1.Add(new[] { arr2 });
        }

        /// <summary>
        /// 合并两个string数组
        /// </summary>
        public static string[] Add(this string[] arr1, string[] arr2)
        {
            arr1 ??= Array.Empty<string>();
            arr2 ??= Array.Empty<string>();
            var result = new string[arr1.Length + arr2.Length];
            Array.Copy(arr1, 0, result, 0, arr1.Length);
            Array.Copy(arr2, 0, result, arr1.Length, arr2.Length);
            return result;
        }

        /// <summary>
        /// 出队列（移除第一个元素）
        /// </summary>
        public static string[] Dequeue(this string[] arr)
        {
            arr ??= Array.Empty<string>();
            if (arr.Length <= 1) return Array.Empty<string>();
            var result = new string[arr.Length - 1];
            Array.Copy(arr, 1, result, 0, arr.Length - 1);
            return result;
        }
    }
}
