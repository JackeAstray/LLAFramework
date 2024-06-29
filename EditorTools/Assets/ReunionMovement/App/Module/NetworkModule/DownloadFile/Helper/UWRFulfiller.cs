using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace GameLogic.Download
{
    /// <summary>
    /// 
    /// </summary>
    public class UWRFulfiller : IDownloadFulfiller
    {

        protected int ExpectedSize = 0;
        protected int ChunkSize = 0;

        internal int startTime = 0;
        internal int endTime = 0;
        internal int bytesDownloaded;
        internal float progress = 0f;
        internal int timeout = 6;
        internal string uri = null;
        internal string downloadPath = Application.persistentDataPath;
        //隐式设置post-head请求
        internal bool multipartDownload = false;
        internal bool abandonOnFailure = true;
        internal bool paused = false;

        public event Action OnDownloadSuccess, OnCancel, OnDownloadChunkedSucces;

        public new bool Completed => Progress == 1.0f;

        public override float Progress
        {
            get => progress;
        }

        public override int BytesDownloaded => bytesDownloaded;

        public override string Uri
        {
            get => uri;
            set
            {
                uri = value;
            }
        }

        public override string DownloadPath
        {
            get => downloadPath;
            set
            {
                downloadPath = value;
            }
        }

        public override bool MultipartDownload
        {
            get => multipartDownload;
            set
            {
                multipartDownload = value;
            }
        }

        public override bool AbandonOnFailure
        {
            get => abandonOnFailure;
            set
            {
                abandonOnFailure = value;
            }
        }

        public override int Timeout
        {
            get => timeout;
            set
            {
                timeout = value;
            }
        }

        public override bool Paused => paused;

        internal bool didError = false;
        public override bool DidError => didError;

        public override int StartTime => startTime;
        public override int EndTime => endTime;

        public event Action<int, string> OnDownloadError;

        /// <summary>
        /// 取消
        /// </summary>
        /// <returns></returns>
        public override bool Cancel()
        {
            OnCancel?.Invoke();
            if (abandonOnFailure && File.Exists(DownloadResultPath))
            {
                File.Delete(DownloadResultPath);
            }
            return false;
        }

        /// <summary>
        /// 检查是否支持分段下载
        /// </summary>
        /// <returns></returns>
        public UnityWebRequestAsyncOperation HeadRequest()
        {
            // case: didn't submit a head request yet, and we should
            UnityWebRequest uwr = null;
            UnityWebRequestAsyncOperation hreq = HTTPHelper.Head(ref uwr, uri, RequestHeaders, timeout);
            didHeadReq = true;
            hreq.completed += (resp) =>
            {
                if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
                {
                    UnityEngine.Debug.LogError($"URI {uri} 不支持HEAD请求，因此不支持分段下载");
                    MultipartDownload = false;
                    return;
                }
                if (!uwr.GetResponseHeaders().ContainsKey("Content-Length") || (!uwr.GetResponseHeaders().ContainsKey("Accept-Ranges") ? true : uwr.GetResponseHeaders()["Accept-Ranges"] != "bytes"))
                {
                    UnityEngine.Debug.LogError($"URI {uri} 不支持分段下载");
                    return;
                }
                try
                {
                    ExpectedSize = Int32.Parse(uwr.GetResponseHeaders()["Content-Length"]);
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogError($"URI {uri} 不支持分段下载");
                    return;
                }
                ChunkSize = intitialChunkSize;
                // 如果大小低于块大小，则不分段下载
                MultipartDownload = ExpectedSize > ChunkSize;
            };
            return hreq;
        }

        /// <summary>
        /// 下载
        /// </summary>
        /// <returns></returns>
        public override UnityWebRequestAsyncOperation Download()
        {
            if (completedMultipartDownload) return null;
            if (uri == null) return null;
            if (downloadPath == null) return null;
            startTime = DateTime.Now.Millisecond;
            UnityWebRequestAsyncOperation resp = null;
            UnityWebRequest uwr = null;
            if (!MultipartDownload)
            {
                resp = HTTPHelper.Download(ref uwr, Uri, downloadPath, AbandonOnFailure, false, RequestHeaders, timeout);
                resp.completed += (obj) =>
                {
                    if (!File.Exists(DownloadResultPath)) return;
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
                            Debug.LogError(ex.ToString());
                            return null;
                        }
                    }
                    int remaining = ExpectedSize - fileSize;

                    //无需下载
                    if (remaining == 0)
                    {
                        return null;
                    }

                    int reqChunkSize = ChunkSize >= remaining ? remaining : ChunkSize;

                    //ChunkSize 小于剩余但大于所需
                    if (fileSize + reqChunkSize >= ExpectedSize)
                    {
                        reqChunkSize = remaining;
                    }

                    if (RequestHeaders == null)
                    {
                        RequestHeaders = new Dictionary<string, string>();
                    }

                    //"Range: bytes=0-1023"
                    if (RequestHeaders.ContainsKey("Range")) RequestHeaders.Remove("Range");
                    string str = "";
                    RequestHeaders.Add("Range", str = $"bytes={fileSize}-{fileSize + reqChunkSize}");
                    resp = HTTPHelper.Download(ref uwr, Uri, downloadPath, AbandonOnFailure, true, RequestHeaders, timeout);
                    resp.completed -= OnCompleteMulti;
                    resp.completed += OnCompleteMulti;
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                }
            }
            resp.completed += (obj) =>
            {
                if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
                {
                    didError = true;
                    OnDownloadError?.Invoke(0, uwr.error);
                    Cancel();
                }
            };
            return resp;
        }


        /// <summary>
        /// 分段下载完成（只有分段调用）
        /// </summary>
        /// <param name="obj"></param>
        internal void OnCompleteMulti(AsyncOperation obj)
        {
            if (!File.Exists(DownloadResultPath)) return;
            int fileSize = 0;
            if (File.Exists(DownloadResultPath)) fileSize = (int)(new FileInfo(DownloadResultPath).Length);
            OnDownloadChunkedSucces?.Invoke();
            int remaining = ExpectedSize - fileSize;
            int reqChunkSize = ChunkSize > remaining ? remaining : ChunkSize;
            progress = reqChunkSize / ExpectedSize;
            bytesDownloaded = (int)new FileInfo(DownloadResultPath).Length;

            if (new FileInfo(DownloadResultPath).Length == ExpectedSize)
            {
                OnDownloadSuccess?.Invoke();
                endTime = DateTime.Now.Millisecond;
                completedMultipartDownload = true;
            }
            else
            {
                // case: not complete!
                // 下载以递归方式调用
            }
        }
    }

}
