using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic.Download
{
    /// <summary>
    /// 加载图片
    /// </summary>
    public class LoadImageHelper : MonoBehaviour
    {
        private Coroutine loadTextureCoroutine;

        public Texture2D LoadTexture2D(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                Log.Error("Url不能为空！");
                return null;
            }

            Texture2D tex2d = new Texture2D(2,2);

            StartCoroutine(LoadImage(url, tex => tex2d = tex));

            return tex2d;
        }

        /// <summary>
        /// 加载图片
        /// </summary>
        /// <param name="rawImage"></param>
        /// <param name="url"></param>
        public void LoadImage(RawImage rawImage, string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                Log.Error("Url不能为空！");
                return;
            }

            if (loadTextureCoroutine != null)
            {
                StopCoroutine(loadTextureCoroutine);
            }

            loadTextureCoroutine = StartCoroutine(LoadImage(url, tex => rawImage.texture = tex));
        }

        /// <summary>
        /// 加载图片
        /// </summary>
        /// <param name="url"></param>
        /// <param name="cb"></param>
        /// <returns></returns>
        private IEnumerator LoadImage(string url, Action<Texture2D> cb)
        {
            yield return LoadImageMgr.instance.LoadImage(url, cb);
        }
    }
}

