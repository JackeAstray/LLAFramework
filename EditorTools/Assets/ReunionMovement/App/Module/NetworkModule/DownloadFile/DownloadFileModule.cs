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
        private string savePath;

        private Dictionary<string, string> mimeTypeToExtension = new Dictionary<string, string>
        {
            {"text/html",".html"},
            {"text/plain",".txt"},
            {"text/xml",".xml"},

            {"image/gif", ".gif"},
            {"image/jpeg", ".jpg"},
            {"image/png", ".png"},

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
        public void GetFileSize()
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
                //float fileSize = float.Parse(httpResponse.ResponseHeaders["Content-Length"]);
                //Log.Debug("文件大小：" + fileSize);
            }
        }

        public void GetFileSizeError(HttpResponse httpResponse)
        {
            Log.Error("错误：" + httpResponse.Error);
        }
        #endregion

        #region 下载

        public void DownloadFile()
        {
            var request = HttpModule.Get(url).
                OnDownloadProgress(DownloadFileProgress).
                OnSuccess(DownloadFileCompleted).
                OnError(DownloadFileError).
                Send();
        }

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
            }
        }

        public void DownloadFileError(HttpResponse httpResponse)
        {
            Log.Error("错误：" + httpResponse.Error);
        }

        public void DownloadFileProgress(float progress)
        {
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