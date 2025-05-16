using LLAFramework.Http;
using LLAFramework.Http.Service;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Security.Policy;
using UnityEngine;

namespace LLAFramework.Download
{
    public class DownloadManagerModule : CustommModuleInitialize
    {
        #region 实例与初始化
        public static DownloadManagerModule Instance = new DownloadManagerModule();
        public bool IsInited { get; private set; }
        private double initProgress = 0;
        public double InitProgress { get { return initProgress; } }
        #endregion

        #region 数据
        private readonly Dictionary<string, Texture2D> imageCache = new Dictionary<string, Texture2D>();
        private readonly Dictionary<string, string> mimeTypeToExtension = new Dictionary<string, string>
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
            Log.Debug("DownloadManagerModule 初始化完成");
        }

        public void ClearData()
        {
            imageCache.Clear();
            Log.Debug("DownloadManagerModule 清除数据");
        }

        #region 下载图片
        public void DownloadImage_Web(string url, Action<float> onProgress, Action<Texture2D> onComplete, string suffix = ".png")
        {
            if (imageCache.TryGetValue(url, out Texture2D cachedTexture))
            {
                onComplete?.Invoke(cachedTexture);
                return;
            }

            string localPath = GetLocalFilePath(url, suffix);
            if (TryLoadFromLocal(localPath, out Texture2D texture))
            {
                imageCache[url] = texture;
                onComplete?.Invoke(texture);
            }
            else
            {
                HttpModule.GetTexture(url)
                    .OnDownloadProgress(onProgress)
                    .OnSuccess(response =>
                    {
                        if (response.Texture != null)
                        {
                            imageCache[url] = response.Texture;
                            SaveToLocal(response.Texture, localPath);
                            onComplete?.Invoke(response.Texture);
                        }
                    })
                    .OnError(error => Log.Error($"下载图片失败: {error}"))
                    .Send();
            }
        }
        #endregion

        #region 下载文件
        public void DownloadFile_Web(string url, Action<float> onProgress, Action<HttpResponse> onComplete)
        {
            string fileName = PathUtils.GetFileName(url);
            string localPath = Path.Combine(PathUtils.GetLocalPath(DownloadType.PersistentFile), fileName);

            HttpModule.Get(url)
                .OnDownloadProgress(onProgress)
                .OnSuccess(onComplete)
                .OnError(error => Log.Error($"下载文件失败: {error}"))
                .Send();
        }
        #endregion

        #region 多文件下载
        public async void DownloadFiles(List<string> url, string savePath, string uiPlane, string set, Action action = null)
        {
            UnityFileDownloader ufd = new UnityFileDownloader(savePath, false, 3, true, false, true, url);

            ufd.OnDownloadSuccess += (string uri) =>
            {
                Log.Debug("OnDownloadSuccess = " + ufd.Progress);
                UIModule.Instance.SetWindow("StartAppUIPlane", "SetProgress", ufd.Progress);
            };

            ufd.OnDownloadChunkedSucces += (string uri) =>
            {
                Log.Debug("Progress for " + uri + " is " + ufd.GetProgress(uri));
            };

            ufd.OnDownloadsSuccess += () =>
            {
                Log.Debug("已下载所有文件。");
                action?.Invoke();
            };

            ufd.OnDownloadError += (string uri, int errorCode, string errorMsg) =>
            {
                Log.Error($"错误代码={errorCode}, EM={errorMsg}, URU={uri}");
            };

            await ufd.Download();
        }
        #endregion

        #region 工具方法
        /// <summary>
        /// 获取本地文件路径
        /// </summary>
        /// <param name="url"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        private string GetLocalFilePath(string url, string suffix)
        {
            string urlHash = StringExtensions.CreateMD5(url);
            return $"{PathUtils.GetLocalPath(DownloadType.PersistentFile)}/{urlHash}{suffix}";
        }

        /// <summary>
        /// 尝试从本地加载图片
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="texture"></param>
        /// <returns></returns>
        private bool TryLoadFromLocal(string filePath, out Texture2D texture)
        {
            texture = null;
            if (!File.Exists(filePath)) return false;

            try
            {
                byte[] imageBytes = File.ReadAllBytes(filePath);
                texture = new Texture2D(2, 2);
                texture.LoadImage(imageBytes);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"从本地加载失败: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 保存图片到本地
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="filePath"></param>
        private void SaveToLocal(Texture2D texture, string filePath)
        {
            try
            {
                byte[] bytes = texture.EncodeToPNG();
                File.WriteAllBytes(filePath, bytes);
                Log.Debug($"保存到本地成功: {filePath}");
            }
            catch (Exception ex)
            {
                Log.Error($"保存失败: {ex.Message}");
            }
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

        public void UpdateTime(float elapseSeconds, float realElapseSeconds)
        {
            // 可扩展逻辑
        }
    }
}