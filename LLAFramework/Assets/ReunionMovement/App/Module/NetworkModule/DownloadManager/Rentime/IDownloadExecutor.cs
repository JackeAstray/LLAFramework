using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace LLAFramework.Download
{
    public abstract class IDownloadExecutor
    {
        /// <summary>
        /// 初始块大小
        /// </summary>
        public int IntitialChunkSize = 200000;

        /// <summary>
        /// 已完成分块下载
        /// </summary>
        public bool CompletedMultipartDownload = false;
        public bool DidHeadReq = false;

        /// <summary>
        /// 是否完成下载
        /// </summary>
        public bool completed => Progress == 1.0f;

        /// <summary>
        /// 下载进度
        /// </summary>
        public abstract float Progress { get; }

        /// <summary>
        /// 下载的uri
        /// </summary>
        public abstract string Uri { get; set; }

        /// <summary>
        /// 下载的文件大小
        /// </summary>
        public abstract int BytesDownloaded { get; }

        /// <summary>
        /// 请求头
        /// </summary>
        public Dictionary<string, string> RequestHeaders { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// 下载的路径
        /// </summary>
        public abstract string DownloadPath { get; set; }

        public abstract bool DownloadToRoot { get; set; }

        /// <summary>
        /// 下载的结果路径
        /// </summary>
        public string DownloadResultPath
        {
            get
            {
                if (string.IsNullOrEmpty(Uri) || string.IsNullOrEmpty(DownloadPath))
                {
                    return null;
                }

                var relativePath = DownloadToRoot
                    ? HTTPHelper.GetFilenameFromUriNaively(Uri)
                    : HTTPHelper.GetRelativePathFromUri(Uri);

                return Path.Combine(DownloadPath, relativePath)
                    .Replace("/", Path.DirectorySeparatorChar.ToString());
            }
        }

        /// <summary>
        /// 分块下载
        /// </summary>
        public abstract bool MultipartDownload { get; set; }

        /// <summary>
        /// 是否支持分块下载
        /// </summary>
        public bool TryMultipartDownload { get; set; } = true;

        /// <summary>
        /// 是否暂停下载
        /// </summary>
        public abstract bool Paused { get; }

        /// <summary>
        /// 是否在下载失败时放弃
        /// </summary>
        public abstract bool AbandonOnFailure { get; set; }

        /// <summary>
        /// 下载的错误
        /// </summary>
        public abstract bool DidError { get; set; }

        /// <summary>
        /// 下载的开始时间
        /// </summary>
        public abstract int StartTime { get; }

        /// <summary>
        /// 是否正在下载
        /// </summary>
        public bool Downloading => StartTime != 0;

        /// <summary>
        /// 下载的结束时间
        /// </summary>
        public abstract int EndTime { get; }

        /// <summary>
        /// 下载的超时时间
        /// </summary>
        public abstract int Timeout { get; set; }

        /// <summary>
        /// 下载的时间
        /// </summary>
        public int ElapsedTime
        {
            get
            {
                if (StartTime == 0)
                {
                    return 0;
                }

                if (EndTime == 0)
                {
                    return Math.Abs(Environment.TickCount - StartTime);
                }

                return Math.Abs(EndTime - StartTime);
            }
        }

        /// <summary>
        /// 下载的字节数
        /// </summary>
        public float MegabytesDownloadedPerSecond
        {
            get
            {
                var seconds = ElapsedTime / 1000f;
                return seconds > 0 ? (BytesDownloaded / 1000f) / seconds : 0f;
            }
        }

        /// <summary>
        /// 取消下载
        /// </summary>
        /// <returns></returns>
        public abstract bool Cancel();

        /// <summary>
        /// 根据现有的“URI”属性启动下载
        /// </summary>
        /// <returns></returns>
        public abstract UnityWebRequestAsyncOperation Download();
    }
}
