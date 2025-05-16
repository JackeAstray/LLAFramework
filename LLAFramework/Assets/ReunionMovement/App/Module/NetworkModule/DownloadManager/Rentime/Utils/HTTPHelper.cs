using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Policy;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace LLAFramework.Download
{
    /// <summary>
    /// HTTP响应类
    /// </summary>
    public class HTTPResponse
    {
        public string responseText;
        public int responseCode;
        public int downloadedBytes = 0;
        public Dictionary<string, string> headers;
        public bool didError;

        public override string ToString()
        {
            return $"ResponseText={responseText}, ResponseCode={responseCode}, DidError={didError}";
        }
    }

    /// <summary>
    /// HTTP请求帮助类
    /// </summary>
    public static class HTTPHelper
    {
        /// <summary>
        /// 发送GET请求
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="headers"></param>
        /// <param name="timeoutSeconds"></param>
        /// <returns></returns>
        public static async Task<HTTPResponse> Get(string uri, Dictionary<string, string> headers = null, int timeoutSeconds = 3)
        {
            var resp = new HTTPResponse();

            using (var req = UnityWebRequest.Get(uri))
            {
                req.timeout = timeoutSeconds;
                if (headers != null)
                {
                    foreach (var kvp in headers)
                    {
                        req.SetRequestHeader(kvp.Key, kvp.Value);
                    }
                }

                await req.SendWebRequest();

                resp.responseText = req.result == UnityWebRequest.Result.Success
                    ? req.downloadHandler.text
                    : req.error;
                resp.didError = req.result != UnityWebRequest.Result.Success;
                resp.responseCode = (int)req.responseCode;
                resp.headers = req.GetResponseHeaders();
            }
            return resp;
        }

        /// <summary>
        /// 从Uri获取相对路径
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static string GetRelativePathFromUri(string uri)
        {
            if (string.IsNullOrEmpty(uri))
            {
                return string.Empty;
            }

            try
            {
                var u = new Uri(uri);
                // 去掉开头的斜杠
                return Uri.UnescapeDataString(u.AbsolutePath.TrimStart('/'));
            }
            catch
            {
                return PathUtils.GetFileName(uri);
            }
        }

        /// <summary>
        /// 从Uri获取文件名
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static string GetFilenameFromUriNaively(string uri)
        {
            if (string.IsNullOrEmpty(uri))
            {
                return string.Empty;
            }

            var arr = uri.Split('/');
            var v = arr[^1];
            if (v.Contains("%"))
            {
                var arr2 = v.Split('%');
                v = arr2[^1];
            }
            return v;
        }

        /// <summary>
        /// 发送HEAD请求
        /// </summary>
        /// <param name="req"></param>
        /// <param name="uri"></param>
        /// <param name="headers"></param>
        /// <param name="timeoutSeconds"></param>
        /// <returns></returns>
        public static UnityWebRequestAsyncOperation Head(ref UnityWebRequest req, string uri, Dictionary<string, string> headers = null, int timeoutSeconds = 3)
        {
            req = new UnityWebRequest(uri, UnityWebRequest.kHttpVerbHEAD)
            {
                timeout = timeoutSeconds
            };

            if (headers != null)
            {
                foreach (var kvp in headers)
                {
                    req.SetRequestHeader(kvp.Key, kvp.Value);
                }
            }

            Log.Debug($"Head URI={uri}");
            if (headers != null)
            {
                foreach (var str in headers)
                {
                    Log.Debug($"[{str.Key}={str.Value}]");
                }
            }

            return req.SendWebRequest();
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="req"></param>
        /// <param name="uri"></param>
        /// <param name="path"></param>
        /// <param name="abandonOnFailure"></param>
        /// <param name="append"></param>
        /// <param name="headers"></param>
        /// <param name="timeoutSeconds"></param>
        /// <returns></returns>
        public static UnityWebRequestAsyncOperation Download(
            ref UnityWebRequest req,
            string uri,
            string path = null,
            bool downloadToRoot = false,
            bool abandonOnFailure = false,
            bool append = false,
            Dictionary<string, string> headers = null,
            int timeoutSeconds = 3
        )
        {
            path ??= Application.persistentDataPath;

            if (headers != null)
            {
                foreach (var str in headers)
                {
                    Log.Debug($"[{str.Key}={str.Value}]");
                }
            }

            req = new UnityWebRequest(uri, UnityWebRequest.kHttpVerbGET);

            string filename = GetFilenameFromUriNaively(uri);
            string tempPath;

            if (downloadToRoot)
            {
                tempPath = Path.Combine(path, filename).Replace("/", Path.DirectorySeparatorChar.ToString());
                if (tempPath.Contains("%"))
                {
                    var arr = tempPath.Split('%');
                    tempPath = arr[^1];
                }
            }
            else
            {
                string relativePath = GetRelativePathFromUri(uri);
                tempPath = Path.Combine(path, relativePath).Replace("/", Path.DirectorySeparatorChar.ToString());
            }

            req.downloadHandler = new DownloadHandlerFile(tempPath, append)
            {
                removeFileOnAbort = abandonOnFailure
            };

            req.timeout = timeoutSeconds;

            if (headers != null)
            {
                foreach (var kvp in headers)
                {
                    req.SetRequestHeader(kvp.Key, kvp.Value);
                }
            }
            return req.SendWebRequest();
        }
    }
}
