using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace LLAFramework.Download
{
    public class UWRExecutor : IDownloadExecutor
    {
        protected int expectedSize = 0;
        protected int chunkSize = 0;

        internal int startTime = 0;
        internal int endTime = 0;

        internal int bytesDownloaded;
        internal float progress = 0f;
        internal int timeout = 6;
        internal string uri = null;

        internal string downloadPath;
        internal bool downloadToRoot;

        internal bool multipartDownload = false;
        internal bool abandonOnFailure = true;
        internal bool paused = false;

        public event Action OnDownloadSuccess;
        public event Action OnCancel;
        public event Action OnDownloadChunkedSucces;
        public event Action<int, string> OnDownloadError;

        public bool Completed => Progress == 1.0f;
        public override float Progress => progress;
        public override int BytesDownloaded => bytesDownloaded;

        public override string Uri
        {
            get => uri;
            set => uri = value;
        }

        public override string DownloadPath
        {
            get => downloadPath;
            set => downloadPath = value;
        }

        public override bool DownloadToRoot
        {
            get => downloadToRoot;
            set => downloadToRoot = value;
        }

        public override bool MultipartDownload
        {
            get => multipartDownload;
            set => multipartDownload = value;
        }

        public override bool AbandonOnFailure
        {
            get => abandonOnFailure;
            set => abandonOnFailure = value;
        }

        public override int Timeout
        {
            get => timeout;
            set => timeout = value;
        }

        public override bool Paused => paused;

        internal bool didError = false;
        public override bool DidError
        {
            get => didError;
            set => didError = value;
        }

        public override int StartTime => startTime;
        public override int EndTime => endTime;

        /// <summary>
        /// TODO: Implement
        /// </summary>
        /// <returns></returns>
        public override bool Cancel()
        {
            OnCancel?.Invoke();
            if (abandonOnFailure && !string.IsNullOrEmpty(DownloadResultPath) && File.Exists(DownloadResultPath))
            {
                try
                {
                    File.Delete(DownloadResultPath);
                }
                catch (Exception ex)
                {
                    Log.Error($"删除文件失败: {ex}");
                }
            }
            return false;
        }

        /// <summary>
        /// 向URI提交head请求，以确定是否可以进行分块下载。
        /// </summary>
        /// <returns></returns>
        public UnityWebRequestAsyncOperation HeadRequest()
        {
            UnityWebRequest uwr = null;
            UnityWebRequestAsyncOperation hreq = HTTPHelper.Head(ref uwr, Uri, RequestHeaders, Timeout);
            DidHeadReq = true;
            hreq.completed += (resp) =>
            {
                if (uwr.result != UnityWebRequest.Result.Success)
                {
                    Log.Debug($"URI {Uri} 不支持HEAD请求，因此不支持分块下载。 错误: {uwr.error}");
                    MultipartDownload = false;
                    return;
                }

                var headers = uwr.GetResponseHeaders();

                if (headers == null ||
                    !headers.ContainsKey("Content-Length") ||
                    !headers.TryGetValue("Accept-Ranges", out var acceptRanges) ||
                    !string.Equals(acceptRanges, "bytes", StringComparison.OrdinalIgnoreCase))
                {
                    Log.Debug($"URI {Uri} 不支持分块下载。");
                    MultipartDownload = false;
                    return;
                }

                if (!int.TryParse(headers["Content-Length"], out expectedSize))
                {
                    Log.Debug($"URI {Uri} 不支持分块下载。Content-Length 解析失败。");
                    MultipartDownload = false;
                    return;
                }

                chunkSize = IntitialChunkSize;
                MultipartDownload = expectedSize > chunkSize;
            };
            return hreq;
        }

        /// <summary>
        /// 根据当前的 URI 和配置，发起文件下载请求，并处理单个或分块下载的流程
        /// </summary>
        /// <returns></returns>
        public override UnityWebRequestAsyncOperation Download()
        {
            if (CompletedMultipartDownload || string.IsNullOrEmpty(Uri) || string.IsNullOrEmpty(DownloadPath))
            {
                return null;
            }

            startTime = DateTime.Now.Millisecond;

            UnityWebRequestAsyncOperation resp = null;
            UnityWebRequest uwr = null;

            if (!MultipartDownload)
            {
                resp = HTTPHelper.Download(ref uwr, Uri, DownloadPath, DownloadToRoot, AbandonOnFailure, false, RequestHeaders, Timeout);
                resp.completed += (obj) =>
                {
                    if (!File.Exists(DownloadResultPath))
                    {
                        return;
                    }
                    progress = 1.0f;
                    OnDownloadSuccess?.Invoke();
                    endTime = DateTime.Now.Millisecond;
                    bytesDownloaded = (int)new FileInfo(DownloadResultPath).Length;
                };
            }
            else
            {
                try
                {
                    int fileSize = 0;
                    if (File.Exists(DownloadResultPath))
                    {
                        try
                        {
                            fileSize = (int)(new FileInfo(DownloadResultPath).Length);
                        }
                        catch (Exception ex)
                        {
                            Log.Debug(ex.ToString());
                            return null;
                        }
                    }
                    int remaining = expectedSize - fileSize;
                    if (remaining <= 0)
                    {
                        return null;
                    }

                    int reqChunkSize = Math.Min(chunkSize, remaining);

                    if (RequestHeaders == null)
                    {
                        RequestHeaders = new Dictionary<string, string>();
                    }
                    RequestHeaders.Remove("Range");
                    RequestHeaders.Add("Range", $"bytes={fileSize}-{fileSize + reqChunkSize - 1}");

                    resp = HTTPHelper.Download(ref uwr, Uri, DownloadPath, DownloadToRoot, AbandonOnFailure, true, RequestHeaders, Timeout);
                    resp.completed -= OnCompleteMulti;
                    resp.completed += OnCompleteMulti;
                }
                catch (Exception e)
                {
                    Log.Error(e.ToString());
                }
            }

            resp.completed += (obj) =>
            {
                if (uwr.result != UnityWebRequest.Result.Success)
                {
                    DidError = true;
                    OnDownloadError?.Invoke(0, uwr.error);
                    Cancel();
                }
            };
            return resp;
        }


        /// <summary>
        /// 当分块请求完成时调用。
        /// </summary>
        /// <param name="obj"></param>
        internal void OnCompleteMulti(AsyncOperation obj)
        {
            if (!File.Exists(DownloadResultPath))
            {
                return;
            }
            int fileSize = (int)(new FileInfo(DownloadResultPath).Length);
            OnDownloadChunkedSucces?.Invoke();
            int remaining = expectedSize - fileSize;
            progress = expectedSize > 0 ? (float)fileSize / expectedSize : 0f;
            bytesDownloaded = fileSize;

            if (fileSize == expectedSize)
            {
                OnDownloadSuccess?.Invoke();
                endTime = DateTime.Now.Millisecond;
                CompletedMultipartDownload = true;
            }
        }
    }
}
