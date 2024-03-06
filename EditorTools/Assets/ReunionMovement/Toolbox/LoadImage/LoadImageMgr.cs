using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.Networking;

namespace GameLogic.Download
{
    /// <summary>
    /// 加载Image管理器
    /// </summary>
    public class LoadImageMgr
    {
        private Dictionary<string, Texture2D> imageDic = new Dictionary<string, Texture2D>();
        private string savePath;
        public static LoadImageMgr instance { get; private set; } = new LoadImageMgr();

        public LoadImageMgr()
        {
            savePath = GetLocalPath();
        }

        /// <summary>
        /// 从网络或硬盘下载
        /// </summary>
        /// <param name="url"></param>
        /// <param name="loadEnd">callback</param>
        /// <returns></returns>
        public IEnumerator LoadImage(string url, Action<Texture2D> loadEnd)
        {
            if (imageDic.TryGetValue(url, out Texture2D texture))
            {
                loadEnd.Invoke(texture);
                yield break;
            }

            string urlHash = EngineExtensions.MD5Encrypt(url);
            string localFilePath = string.Format("{0}/{1}.png", savePath, urlHash);

            if (File.Exists(localFilePath))
            {
                yield return DownloadImage(localFilePath, (state, localTexture) =>
                {
                    if (state)
                    {
                        loadEnd.Invoke(localTexture);
                        imageDic[url] = localTexture;
                    }
                });
            }
            else
            {
                yield return DownloadImage(url, (state, downloadTexture) =>
                {
                    if (state)
                    {
                        loadEnd.Invoke(downloadTexture);
                        imageDic[url] = downloadTexture;
                        SaveToLocalPath(urlHash, downloadTexture);
                    }
                });
            }
        }

        /// <summary>
        /// 下载图片
        /// </summary>
        /// <param name="url"></param>
        /// <param name="downloadEnd"></param>
        /// <returns></returns>
        public IEnumerator DownloadImage(string url, Action<bool, Texture2D> downloadEnd)
        {
            using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
            {
                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    downloadEnd.Invoke(false, null);
                    Debug.LogError("下载失败: " + request.error);
                }
                else
                {
                    Texture2D localTexture = DownloadHandlerTexture.GetContent(request);
                    downloadEnd.Invoke(true, localTexture);
                }
            }
        }

        /// <summary>
        /// 保存图片到本地
        /// </summary>
        /// <param name="url"></param>
        /// <param name="texture"></param>
        private void SaveToLocalPath(string urlHash, Texture2D texture)
        {
            byte[] bytes = texture.EncodeToPNG();
            string localFilePath = string.Format("{0}/{1}.png", savePath, urlHash);

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
    }
}