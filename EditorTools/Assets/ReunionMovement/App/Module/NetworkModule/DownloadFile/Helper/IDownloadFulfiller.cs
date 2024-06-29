using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace GameLogic.Download
{
    /// <summary>
    /// 表示能够从公共端点下载文件的类
    /// </summary>
    public abstract class IDownloadFulfiller
    {
        /// <summary>
        /// 每个请求的分块字节数
        /// </summary>
        public int intitialChunkSize = 200000;

        public bool completedMultipartDownload = false;
        public bool didHeadReq = false;


        /// <summary>
        /// 当此履行程序完成后，返回 true
        /// </summary>
        public bool Completed => Progress == 1.0f;

        /// <summary>
        /// 返回此下载的进度（如果有），从 0f 到 1f。
        /// </summary>
        /// <value></value>
        public abstract float Progress
        {
            get;
        }

        /// <summary>
        /// 如果未在“下载”时设置，则返回与此履行程序关联的 URI。
        /// </summary>
        /// <value></value>
        public abstract string Uri
        {
            get; set;
        }

        public abstract int BytesDownloaded
        {
            get;
        }

        /// <summary>
        /// 用于任何已调度请求的标头
        /// </summary>
        public Dictionary<string, string> RequestHeaders = null;

        /// <summary>
        /// 下载到的下载路径，不包括文件名
        /// </summary>
        /// <value></value>
        public abstract string DownloadPath
        {
            get; set;
        }

        public string DownloadResultPath => (Uri == null || DownloadPath == null) ? null : Path.Combine(DownloadPath, HTTPHelper.GetFilenameFromUriNaively(Uri)).Replace("/", Path.DirectorySeparatorChar.ToString());

        /// <summary>
        /// 如果此履行程序可以分块下载此文件，则返回 true。
        /// 此字段是在根据“ChunkSize”下载文件时隐式确定的。
        /// </summary>
        /// <value></value>
        public abstract bool MultipartDownload
        {
            get; set;
        }

        /// <summary>
        /// 仅当此布尔值为真时才明确检查分段下载
        /// </summary>
        public bool TryMultipartDownload = true;

        /// <summary>
        /// 通过“暂停”功能设置为 true，通过“下载”功能设置为 false。
        /// </summary>
        /// <value></value>
        public abstract bool Paused
        {
            get;
        }

        /// <summary>
        /// 如果发生网络错误，则删除整个文件，即使文件只是部分下载。
        /// </summary>
        /// <value></value>
        public abstract bool AbandonOnFailure
        {
            get; set;
        }

        public abstract bool DidError
        {
            get;
        }

        public abstract int StartTime
        {
            get;
        }

        public bool Downloading => StartTime != 0;

        public abstract int EndTime
        {
            get;
        }

        public abstract int Timeout
        {
            get; set;
        }

        public int ElapsedTime => Math.Abs(StartTime == 0 ? 0 : (EndTime == 0 ? DateTime.Now.Millisecond - StartTime : EndTime - StartTime));

        public float MegabytesDownloadedPerSecond => (BytesDownloaded / 1000) / ((ElapsedTime / 1000) == 0 ? 1 : (ElapsedTime / 1000));

        /// <summary>
        /// 如果此履行程序将“MultipartDownload”设置为true，则暂停下载。
        /// 如果此下载程序成功暂停，则返回true。
        /// </summary>
        /// <returns></returns>
        public abstract bool Cancel();


        /// <summary>
        /// 根据现有的“URI”属性启动下载（或取消暂停）
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public abstract UnityWebRequestAsyncOperation Download();
    }
}
