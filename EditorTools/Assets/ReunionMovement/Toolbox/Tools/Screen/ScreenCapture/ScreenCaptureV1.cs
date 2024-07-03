using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 截屏
    /// </summary>
    public class ScreenCaptureV1
    {
        public static List<Camera> cameraList = new List<Camera>();

        /// <summary>
        /// 摄像机列表，注意顺序会影响渲染顺序，如果新增相机，要把相机添加到数组中
        /// </summary>
        private static string[] cameraNameList = new string[] { "Main Camera", "UICamera" };

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init()
        {
            for (int i = 0, len = cameraNameList.Length; i < len; ++i)
            {
                var camName = cameraNameList[i];
                var camObj = GameObject.Find(camName);
                if (null != camObj)
                {
                    var cam = camObj.GetComponent<Camera>();
                    cameraList.Add(cam);
                }
                else
                {
                    Log.Error("摄像头不存在, 名称: " + camName);
                }
            }
        }

        /// <summary>
        /// 截屏接口
        /// </summary>
        /// <returns></returns>
        public static Texture2D StartScreenCapture(TextureFormat textureFormat = TextureFormat.RGBA32)
        {
            int width = Screen.width;
            int height = Screen.height;
            RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 24);
            for (int i = 0, cnt = cameraList.Count; i < cnt; ++i)
            {
                var cam = cameraList[i];
                cam.targetTexture = rt;
                cam.Render();
            }
            RenderTexture.active = rt;
            Texture2D tex = new Texture2D(width, height, textureFormat, false);
            tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            RenderTexture.active = null;
            for (int i = 0, cnt = cameraList.Count; i < cnt; ++i)
            {
                var cam = cameraList[i];
                cam.targetTexture = null;
            }
            Object.Destroy(rt);
            tex.Apply();
            return tex;
        }


        /// <summary>
        /// 通过相机截取屏幕并转换为Sprite
        /// </summary>
        /// <returns>相机抓取的屏幕Texture2D</returns>
        public static Sprite CameraScreenshotAsSpriteRGBA()
        {
            var texture2D = StartScreenCapture();
            var sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), Vector2.zero);
            return sprite;
        }
    }
}