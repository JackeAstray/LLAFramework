using GameLogic.Http;
using GameLogic.Http.Service;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

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
        public string url;
        public List<string> urls = new List<string>();
        public int downloadCount;
        public int downloadCountMax;
        private string savePath;

        public float downloadProgress;
        public float DownloadProgress { get { return downloadProgress; } }

        public float downloadAllProgress;
        public float DownloadAllProgress { get { return downloadAllProgress; } }

        Action DownloadCompleted;
        Action DownloadAllCompleted;

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

            savePath = GetLocalPath();
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
            var request = HttpModule.Head(url).
                OnSuccess(GetFileSizeCompleted).
                OnError(GetFileSizeError).
                Send();
        }

        public void GetFileSizeCompleted(HttpResponse httpResponse)
        {
            Log.Debug("查询文件大小完成");

            if (httpResponse.IsSuccessful)
            {
                float fileSize = float.Parse(httpResponse.ResponseHeaders["Content-Length"]);
                Log.Debug("文件大小：" + fileSize);
            }
        }

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
            downloadProgress = 0;

            if (!multiple)
            {
                downloadCount = 0;
                downloadCountMax = 1;

                if (onDownloadsComplete != null)
                {
                    DownloadCompleted += onDownloadsComplete;
                }
            }

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

                string contentType = httpResponse.ResponseHeaders["Content-Type"];
                string extension = GetExtensionFromMimeType(contentType);

                string urlHash = EngineExtensions.MD5Encrypt(url);
                string fileName = urlHash + extension;
                string filePath = Path.Combine(savePath, fileName);
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
        /// 获取要保存的路径
        /// </summary>
        /// <returns></returns>
        private string GetLocalPath()
        {
            string savePath = Application.persistentDataPath + "/Download";

            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            return savePath;
        }

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