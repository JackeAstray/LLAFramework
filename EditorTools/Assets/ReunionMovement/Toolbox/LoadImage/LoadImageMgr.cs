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
        public static LoadImageMgr instance { get; private set; } = new LoadImageMgr();

        /// <summary>
        /// 从网络或硬盘下载
        /// </summary>
        /// <param name="url"></param>
        /// <param name="loadEnd">callback</param>
        /// <returns></returns>
        public IEnumerator LoadImage(string url, Action<Texture2D> loadEnd)
        {
            Texture2D texture = null;
            //先从内存加载
            if (imageDic.TryGetValue(url,out texture))
            {
                loadEnd.Invoke(texture);
                yield break;
            }
            string savePath = GetLocalPath();
            string filePath = string.Format("file://{0}/{1}.png", savePath, EngineExtensions.MD5Encrypt(url));
            //来自硬盘
            bool hasLoad = false;
            if (Directory.Exists(filePath))
                yield return DownloadImage(filePath, (state, localTexture) =>
                { 
                    hasLoad = state;
                    if (state)
                    {
                        loadEnd.Invoke(localTexture);
                        if (!imageDic.ContainsKey(url))
                            imageDic.Add(url, localTexture);
                    }
                });
            if (hasLoad) yield break; //loaded
            //来自网络
            yield return DownloadImage(url, (state, downloadTexture) =>
            {
                hasLoad = state;
                if (state)
                {
                    loadEnd.Invoke(downloadTexture);
                    if (!imageDic.ContainsKey(url))
                        imageDic.Add(url, downloadTexture);
                    Save2LocalPath(url, downloadTexture);
                }
            });
        }

        /// <summary>
        /// 下载图片
        /// </summary>
        /// <param name="url"></param>
        /// <param name="downloadEnd"></param>
        /// <returns></returns>
        public IEnumerator DownloadImage(string url, Action<bool, Texture2D> downloadEnd)
        {
            using (UnityWebRequest request = new UnityWebRequest(url))
            {
                DownloadHandlerTexture downloadHandlerTexture = new DownloadHandlerTexture(true);
                request.downloadHandler = downloadHandlerTexture;
                yield return request.Send();
                if (string.IsNullOrEmpty(request.error))
                {
                    Texture2D localTexture = downloadHandlerTexture.texture;
                    downloadEnd.Invoke(true, localTexture);
                    request.Dispose();
                }
                else
                {
                    downloadEnd.Invoke(false, null);
                    Debug.Log(request.error);
                }
            }
        }
        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="url"></param>
        /// <param name="texture"></param>
        private void Save2LocalPath(string url, Texture2D texture)
        {
            byte[] bytes = texture.EncodeToPNG();
            string savePath = GetLocalPath();
            try
            {
                File.WriteAllBytes( string.Format("{0}/{1}.png", savePath , EngineExtensions.MD5Encrypt(url)), bytes);
            }
            catch(Exception ex)
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
    #if UNITY_EDITOR
            savePath = Application.dataPath + "/Picture";
    #endif
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            return savePath;
        }

    }
}