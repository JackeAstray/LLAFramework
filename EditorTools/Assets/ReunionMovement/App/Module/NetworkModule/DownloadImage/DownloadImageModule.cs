using GameLogic.Http;
using GameLogic.Http.Service;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GameLogic.Download
{
    internal class DownloadImageModule : CustommModuleInitialize
    {
        #region 实例与初始化
        public static DownloadImageModule Instance = new DownloadImageModule();
        public bool IsInited { get; private set; }
        private double initProgress = 0;
        public double InitProgress { get { return initProgress; } }
        #endregion

        #region 数据
        private string currentUrl;
        private string urlHash;
        private string suffix;
        private string localFilePath;
        private Dictionary<string, Texture2D> imageDic = new Dictionary<string, Texture2D>();

        Action<Texture2D> loadEnd;
        #endregion

        public IEnumerator Init()
        {
            initProgress = 0;
            yield return null;
            initProgress = 100;
            IsInited = true;

            Log.Debug("DownloadImageModule 初始化完成");
        }

        public void ClearData()
        {
            Log.Debug("DownloadImageModule 清除数据");
        }

        /// <summary>
        /// 下载图片
        /// </summary>
        /// <param name="url"></param>
        public void DownloadImage(string url, Action<Texture2D> loadEnd, string suffix = ".png")
        {
            try
            {
                if (imageDic.TryGetValue(url, out Texture2D texture))
                {
                    loadEnd?.Invoke(texture);
                    return;
                }

                this.suffix = suffix;
                this.currentUrl = url;
                this.urlHash = EngineExtensions.MD5Encrypt(url);
                this.localFilePath = $"{PathUtils.GetLocalPath(DownloadType.PersistentImage)}/{urlHash}{suffix}";

                if (File.Exists(localFilePath))
                {
                    byte[] imageBytes = File.ReadAllBytes(localFilePath);
                    Texture2D localTexture = new Texture2D(2, 2);
                    localTexture.LoadImage(imageBytes);
                    imageDic[url] = localTexture;
                    loadEnd?.Invoke(imageDic[url]);
                }
                else
                {
                    this.loadEnd += loadEnd;
                    var request = HttpModule.GetTexture(url).
                                        OnSuccess(GetImage).
                                        OnDownloadProgress(GetImageProgress).
                                        OnError((error) => Debug.LogError(error)).
                                        Send();

                }
            }
            catch (Exception ex)
            {
                Log.Error("下载文件时发生异常：" + ex.Message);
            }
        }

        /// <summary>
        /// 获取图片
        /// </summary>
        /// <param name="httpResponse"></param>
        public void GetImage(HttpResponse httpResponse)
        {
            Texture2D tempTexture = httpResponse.Texture;
            imageDic[currentUrl] = tempTexture;
            loadEnd?.Invoke(imageDic[currentUrl]);
            loadEnd = null;
            SaveToLocalPath(imageDic[currentUrl]);
        }

        /// <summary>
        /// 获取图片下载进度
        /// </summary>
        /// <param name="progress"></param>
        public void GetImageProgress(float progress)
        {
            Log.Debug(progress);
        }

        /// <summary>
        /// 保存图片到本地
        /// </summary>
        /// <param name="url"></param>
        /// <param name="texture"></param>
        private void SaveToLocalPath(Texture2D texture)
        {
            byte[] bytes = texture.EncodeToPNG();

            try
            {
                File.WriteAllBytes(this.localFilePath, bytes);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

        public void UpdateTime(float elapseSeconds, float realElapseSeconds)
        {

        }
    }
}