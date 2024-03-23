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

        public IEnumerator LoadTexture2D(string url, Action<Texture2D> action)
        {
            if (string.IsNullOrEmpty(url))
            {
                Log.Error("Url不能为空！");
                yield break;
            }

            yield return LoadImage(url, tex => { action(tex); });
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
        /// <param name="action"></param>
        /// <returns></returns>
        private IEnumerator LoadImage(string url, Action<Texture2D> action)
        {
            yield return LoadImageMgr.instance.LoadImage(url, action);
        }
    }
}

