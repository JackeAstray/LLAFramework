using GameLogic.HttpModule;
using GameLogic.HttpModule.Service;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using UnityEngine.Rendering;

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
        public string currentUrl;
        private string savePath;
        string urlHash;
        string suffix;
        private Dictionary<string, Texture2D> imageDic = new Dictionary<string, Texture2D>();

        Action<Texture2D> loadEnd;
        #endregion

        public IEnumerator Init()
        {
            initProgress = 0;
            yield return null;
            initProgress = 100;
            IsInited = true;

            savePath = GetLocalPath();

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
            if (imageDic.TryGetValue(url, out Texture2D texture))
            {
                loadEnd.Invoke(texture);
                return;
            }

            this.suffix = suffix;
            currentUrl = url;
            urlHash = EngineExtensions.MD5Encrypt(url);
            string localFilePath = $"{savePath}/{urlHash}{suffix}";

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

                var request = Http.GetTexture(url).
                                    OnSuccess(GetImage).
                                    OnDownloadProgress(GetImageProgress).
                                    OnError((error) => Debug.LogError(error)).
                                    Send();
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
            SaveToLocalPath(urlHash, imageDic[currentUrl], suffix);
        }

        /// <summary>
        /// 获取图片下载进度
        /// </summary>
        /// <param name="progress"></param>
        public void GetImageProgress(float progress)
        {
            Debug.Log(progress);
        }

        /// <summary>
        /// 保存图片到本地
        /// </summary>
        /// <param name="url"></param>
        /// <param name="texture"></param>
        private void SaveToLocalPath(string urlHash, Texture2D texture, string suffix = ".png")
        {
            byte[] bytes = texture.EncodeToPNG();
            string localFilePath = $"{savePath}/{urlHash}{suffix}";

            try
            {
                File.WriteAllBytes(localFilePath, bytes);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.ToString());
            }
        }

        /// <summary>
        /// 获取要保存的路径
        /// </summary>
        /// <returns></returns>
        private string GetLocalPath()
        {
            string savePath = Application.persistentDataPath + "/Picture";

            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            return savePath;
        }

        public void UpdateTime(float elapseSeconds, float realElapseSeconds)
        {

        }
    }
}