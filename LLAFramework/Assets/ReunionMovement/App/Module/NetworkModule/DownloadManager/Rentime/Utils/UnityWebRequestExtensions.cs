using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace LLAFramework.Download
{
    /// <summary>
    /// UnityWebRequest扩展
    /// </summary>
    public static class UnityWebRequestExtensions
    {
        /// <summary>
        /// 将UnityWebRequest转换为字符串
        /// </summary>
        /// <param name="uwr"></param>
        /// <returns></returns>
        public static string ToString(this UnityWebRequest uwr)
        {
            if (uwr == null)
            {
                return "UnityWebRequest: null";
            }

            return $"TYPE: {uwr.method}\nURL: {uwr.url}\nURI: {uwr.uri}\nResponseCode: {uwr.responseCode}\nError: {uwr.error}";
        }

        /// <summary>
        /// 将UnityWebRequestAsyncOperation转换为TaskAwaiter
        /// </summary>
        /// <param name="reqOp"></param>
        /// <returns></returns>
        public static TaskAwaiter<UnityWebRequest.Result> GetAwaiter(this UnityWebRequestAsyncOperation reqOp)
        {
            if (reqOp == null || reqOp.webRequest == null)
            {
                throw new ArgumentNullException(nameof(reqOp), "UnityWebRequestAsyncOperation 或其 webRequest 不能为 null");
            }

            var tsc = new TaskCompletionSource<UnityWebRequest.Result>();

            if (reqOp.isDone)
            {
                tsc.TrySetResult(reqOp.webRequest.result);
            }
            else
            {
                reqOp.completed += _ => tsc.TrySetResult(reqOp.webRequest.result);
            }

            return tsc.Task.GetAwaiter();
        }
    }
}
