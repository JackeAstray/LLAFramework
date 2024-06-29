using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace GameLogic.Download
{
    /// <summary>
    ///包含以下与UnityWebRequest相关的实用程序：
    ///     -提供“SendRequest”函数的可激活性
    ///     -为调试目的提供字符串化功能
    /// </summary>
    public static class UnityWebRequestExtensions
    {
        /// <summary>
        /// UnityWebRequest转字符串
        /// </summary>
        /// <param name="uwr"></param>
        /// <returns></returns>
        public static string ToString(this UnityWebRequest uwr)
        {
            string fstr = "No form data";
            if (uwr.uploadHandler is UploadHandler uploadHandler)
            {
                // 尝试将表单数据转换为字符串
                fstr = Encoding.UTF8.GetString(uploadHandler.data);
            }
            // 对于其他类型的UploadHandler，你可能需要根据具体情况来处理

            return $"TYPE: {uwr.method}\nURL: {uwr.url}\nURI: {uwr.uri}\nFORM: {fstr}";
        }

        /// <summary>
        /// 让UnityWebRequest支持Task
        /// 为以异步方式使用的UnityWebRequest提供一个提示器
        /// </summary>
        /// <param name="reqOp"></param>
        /// <returns></returns>
        public static TaskAwaiter<UnityWebRequest.Result> GetAwaiter(this UnityWebRequestAsyncOperation reqOp)
        {
            TaskCompletionSource<UnityWebRequest.Result> tsc = new();
            reqOp.completed += asyncOp => tsc.TrySetResult(reqOp.webRequest.result);

            if (reqOp.isDone)
            {
                tsc.TrySetResult(reqOp.webRequest.result);
            }

            return tsc.Task.GetAwaiter();
        }
    }
}
