using GameLogic.Http;
using GameLogic.Http.Service;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace GameLogic.Download
{
    public class DownloadFileModule : CustommModuleInitialize
    {
        #region 实例与初始化
        public static DownloadFileModule Instance = new DownloadFileModule();
        public bool IsInited { get; private set; }
        private double initProgress = 0;
        public double InitProgress { get { return initProgress; } }
        #endregion

        #region 数据
        private string currentFilename;
        private string url;
        private List<string> urls = new List<string>();
        private int downloadCount;
        private int downloadCountMax;

        // 进度
        private float downloadProgress;
        public float DownloadProgress { get { return downloadProgress; } }

        private float downloadAllProgress;
        public float DownloadAllProgress { get { return downloadAllProgress; } }

        Action DownloadCompleted;
        Action DownloadAllCompleted;

        // MIME类型对应的文件后缀
        private Dictionary<string, string> mimeTypeToExtension = new Dictionary<string, string>
        {
            {"text/html",".html"},
            {"text/plain",".txt"},
            {"text/xml",".xml"},

            {"image/gif", ".gif"},
            {"image/jpeg", ".jpg"},
            {"image/png", ".png"},
            {"image/webp", ".webp"},

            {"audio/mp3",".mp3"},
            {"audio/wav",".wav"},
            {"audio/ogg",".ogg"},
            {"audio/mid",".mid"},

            {"video/mpeg4",".mp4"},
            {"video/avi",".avi"},

            {"application/pdf",".pdf"},
            {"application/msword",".doc"},
            {"application/json",".json"},
            {"application/javascript",".js"},
            {"application/xml",".xml"},
            {"application/zip",".zip"},
            {"application/7z",".7z"},
        };
        #endregion

        public IEnumerator Init()
        {
            initProgress = 0;
            yield return null;
            initProgress = 100;
            IsInited = true;
            Log.Debug("DownloadFileModule 初始化完成");
        }

        public void ClearData()
        {
            Log.Debug("DownloadFileModule 清除数据");
        }

        #region 查询文件大小
        /// <summary>
        /// 查询文件大小
        /// </summary>
        /// <returns></returns>
        public void GetFileSize(string url)
        {
            string haed = PathUtils.GetPathHead(url);
            if (haed != "http://" && haed != "https://")
            {
                Log.Warning("不是http或https协议");
                return;
            }

            var request = HttpModule.Head(url).
                                     OnSuccess(GetFileSizeCompleted).
                                     OnError(GetFileSizeError).
                                     Send();
        }

        /// <summary>
        /// 查询文件大小完成
        /// </summary>
        /// <param name="httpResponse"></param>
        public void GetFileSizeCompleted(HttpResponse httpResponse)
        {
            Log.Debug("查询文件大小完成");

            if (httpResponse.IsSuccessful)
            {
                float fileSize = float.Parse(httpResponse.ResponseHeaders["Content-Length"]);
                Log.Debug("文件大小：" + fileSize);
            }
        }

        /// <summary>
        /// 查询文件大小错误
        /// </summary>
        /// <param name="httpResponse"></param>
        public void GetFileSizeError(HttpResponse httpResponse)
        {
            Log.Error("错误：" + httpResponse.Error);
        }
        #endregion

        #region 下载
        /// <summary>
        /// 下载文件
        /// </summary>
        public void DownloadFile(string url, Action onDownloadsComplete = null, bool multiple = false)
        {
            if (string.IsNullOrEmpty(url))
            {
                Log.Error("下载地址为空");
                return;
            }
            currentFilename = PathUtils.GetFileName(url);

            downloadProgress = 0;

            if (!multiple)
            {
                downloadCount = 0;
                downloadCountMax = 1;
                DownloadCompleted = onDownloadsComplete;
            }

            string urlHash = StringExtensions.CreateMD5(url);

            this.url = url;
            var request = HttpModule.Get(url).
                                     OnDownloadProgress(DownloadFileProgress).
                                     OnSuccess(DownloadFileCompleted).
                                     OnError(DownloadFileError).
                                     Send();
        }

        public void DownloadFiles(List<string> fileUrls, Action onDownloadsAllComplete = null)
        {
            urls.Clear();
            downloadCount = 0;
            downloadCountMax = fileUrls.Count;
            downloadAllProgress = 0;

            if (onDownloadsAllComplete != null)
            {
                DownloadAllCompleted += onDownloadsAllComplete;
            }

            urls.AddRange(fileUrls);

            DownloadFile(urls[downloadCount] , null, true);
        }

        /// <summary>
        /// 下载完成
        /// </summary>
        /// <param name="httpResponse"></param>
        public void DownloadFileCompleted(HttpResponse httpResponse)
        {
            Log.Debug("下载完成");
            if (httpResponse.IsSuccessful)
            {
                byte[] bytes = httpResponse.Bytes;

                string urlHash = StringExtensions.CreateMD5(url);
                string fileName = currentFilename;
                string filePath = Path.Combine(PathUtils.GetLocalPath(DownloadType.PersistentFile), fileName);
                File.WriteAllBytes(filePath, bytes);

                DownloadCompleted?.Invoke();
                DownloadCompleted = null;

                if (downloadCountMax > 1)
                {
                    downloadCount++;
                    Log.Debug("下载进度：" + downloadCount + "/" + downloadCountMax);

                    if (downloadCount == downloadCountMax)
                    {
                        DownloadAllCompleted?.Invoke();
                        DownloadAllCompleted = null;
                    }
                    else
                    {
                        DownloadFile(urls[downloadCount], null, true);
                    }

                    downloadAllProgress = (float)downloadCount / downloadCountMax;
                }
            }
        }

        public void DownloadFileError(HttpResponse httpResponse)
        {
            Log.Error("错误：" + httpResponse.Error);
        }

        public void DownloadFileProgress(float progress)
        {
            downloadProgress = progress;
            Log.Debug("下载进度：" + progress);
        }

        /// <summary>
        /// 通过MIME类型获取文件后缀
        /// </summary>
        /// <param name="mimeType"></param>
        /// <returns></returns>
        private string GetExtensionFromMimeType(string mimeType)
        {
            if (mimeTypeToExtension.TryGetValue(mimeType, out string extension))
            {
                return extension;
            }
            return ".asset";
        }
        #endregion

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="elapseSeconds"></param>
        /// <param name="realElapseSeconds"></param>
        public void UpdateTime(float elapseSeconds, float realElapseSeconds)
        {

        }
    }
}