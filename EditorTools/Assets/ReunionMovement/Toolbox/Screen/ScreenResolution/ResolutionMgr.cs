using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using GameLogic.Base;

namespace GameLogic
{
    /// <summary>
    /// 分辨率管理
    /// </summary>
    public class ResolutionMgr : SingleToneMgr<ResolutionMgr>
    {
        public enum AspectRatio
        {
            AR_4_3,
            AR_16_9,
            AR_16_10,
        }

        public AspectRatio aspectRatio;
        // 固定纵横比参数
        static public bool FixedAspectRatio = true;
        static public float TargetAspectRatio = 4f / 3f;

        //FixedAspectRatio为false时的窗口纵横比
        static public float WindowedAspectRatio = 4f / 3f;

        //要包括的水平分辨率列表
        int[] resolutions = new int[] { 600, 800, 1024, 1280, 1400, 1600, 1920 };

        public Resolution DisplayResolution;
        public List<Vector2> WindowedResolutions, FullscreenResolutions;

        //当前窗口分辨率
        int currWindowedRes;
        //当前全屏分辨率
        int currFullscreenRes;

        void Start()
        {
            switch(aspectRatio)
            {
                case AspectRatio.AR_4_3:
                    TargetAspectRatio = 4f / 3f;
                    WindowedAspectRatio = 4f / 3f;
                    break;
                case AspectRatio.AR_16_9:
                    TargetAspectRatio = 16f / 9f;
                    WindowedAspectRatio = 16f / 9f;
                    break;
                case AspectRatio.AR_16_10:
                    TargetAspectRatio = 16f / 10f;
                    WindowedAspectRatio = 16f / 10f;
                    break;
            }

            StartCoroutine(StartRoutine());
        }

        /// <summary>
        /// 打印分辨率
        /// </summary>
        private void PrintResolution()
        {
            Debug.Log("Current res: " + Screen.currentResolution.width + " x " + Screen.currentResolution.height);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private IEnumerator StartRoutine()
        {
            if (Application.platform == RuntimePlatform.OSXPlayer)
            {
                DisplayResolution = Screen.currentResolution;
            }
            else
            {
                if (Screen.fullScreen)
                {
                    Resolution r = Screen.currentResolution;
                    Screen.fullScreen = false;

                    yield return null;
                    yield return null;

                    DisplayResolution = Screen.currentResolution;

                    Screen.SetResolution(r.width, r.height, true);

                    yield return null;
                }
                else
                {
                    DisplayResolution = Screen.currentResolution;
                }
            }

            InitResolutions();
        }

        /// <summary>
        /// 初始化分辨率
        /// </summary>
        private void InitResolutions()
        {
            float screenAspect = (float)DisplayResolution.width / DisplayResolution.height;

            WindowedResolutions = new List<Vector2>();
            FullscreenResolutions = new List<Vector2>();

            foreach (int w in resolutions)
            {
                if (w < DisplayResolution.width)
                {
                    // 仅当分辨率比屏幕小20%时才添加分辨率
                    if (w < DisplayResolution.width * 0.8f)
                    {
                        Vector2 windowedResolution = new Vector2(w, Mathf.Round(w / (FixedAspectRatio ? TargetAspectRatio : WindowedAspectRatio)));
                        Debug.Log(windowedResolution.y);
                        if (windowedResolution.y < DisplayResolution.height * 0.8f)
                            WindowedResolutions.Add(windowedResolution);

                        FullscreenResolutions.Add(new Vector2(w, Mathf.Round(w / screenAspect)));
                    }
                }
            }

            // 添加全屏本机分辨率
            FullscreenResolutions.Add(new Vector2(DisplayResolution.width, DisplayResolution.height));

            // 添加半全屏本机分辨率
            Vector2 halfNative = new Vector2(DisplayResolution.width * 0.5f, DisplayResolution.height * 0.5f);
            if (halfNative.x > resolutions[0] && FullscreenResolutions.IndexOf(halfNative) == -1)
                FullscreenResolutions.Add(halfNative);

            FullscreenResolutions = FullscreenResolutions.OrderBy(resolution => resolution.x).ToList();

            bool found = false;

            if (Screen.fullScreen)
            {
                currWindowedRes = WindowedResolutions.Count - 1;

                for (int i = 0; i < FullscreenResolutions.Count; i++)
                {
                    if (FullscreenResolutions[i].x == Screen.width && FullscreenResolutions[i].y == Screen.height)
                    {
                        currFullscreenRes = i;
                        found = true;
                        break;
                    }
                }

                if (!found)
                    SetResolution(FullscreenResolutions.Count - 1, true);
            }
            else
            {
                currFullscreenRes = FullscreenResolutions.Count - 1;

                for (int i = 0; i < WindowedResolutions.Count; i++)
                {
                    if (WindowedResolutions[i].x == Screen.width && WindowedResolutions[i].y == Screen.height)
                    {
                        found = true;
                        currWindowedRes = i;
                        break;
                    }
                }

                if (!found)
                    SetResolution(WindowedResolutions.Count - 1, false);
            }
        }

        /// <summary>
        /// 设置分辨率
        /// </summary>
        /// <param name="index"></param>
        /// <param name="fullscreen"></param>
        public void SetResolution(int index, bool fullscreen)
        {
            Vector2 r = new Vector2();
            if (fullscreen)
            {
                currFullscreenRes = index;
                r = FullscreenResolutions[currFullscreenRes];
            }
            else
            {
                currWindowedRes = index;
                r = WindowedResolutions[currWindowedRes];
            }

            bool fullscreen2windowed = Screen.fullScreen & !fullscreen;

            Debug.Log("将分辨率设置为 " + (int)r.x + "x" + (int)r.y);
            Screen.SetResolution((int)r.x, (int)r.y, fullscreen);

            // 在OSX上，应用程序将通过几秒钟的动画转换从全屏切换到窗口。
            // 在此转换之后，第一次退出全屏时，必须再次调用SetResolution，以确保窗口大小正确调整。
            if (Application.platform == RuntimePlatform.OSXPlayer)
            {
                // 确保没有SetResolutionAfterResize协同程序正在运行并等待屏幕大小更改
                StopAllCoroutines();

                // 在调整大小转换结束后再次调整窗口大小
                if (fullscreen2windowed) StartCoroutine(SetResolutionAfterResize(r));
            }
        }

        /// <summary>
        /// 调整大小后设置分辨率
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        private IEnumerator SetResolutionAfterResize(Vector2 r)
        {
            int maxTime = 5; //最大等待调整大小转换结束
            float time = Time.time;

            // 跳过屏幕大小将更改的几帧
            yield return null;
            yield return null;

            int lastW = Screen.width;
            int lastH = Screen.height;

            // 在过渡动画结束时等待另一个屏幕大小更改
            while (Time.time - time < maxTime)
            {
                if (lastW != Screen.width || lastH != Screen.height)
                {
                    Debug.Log("Resize! " + Screen.width + "x" + Screen.height);

                    Screen.SetResolution((int)r.x, (int)r.y, Screen.fullScreen);
                    yield break;
                }

                yield return null;
            }

            Debug.Log("End waiting");
        }

        public void ToggleFullscreen()
        {
            SetResolution(
                Screen.fullScreen ? currWindowedRes : currFullscreenRes,
                !Screen.fullScreen);
        }
    }
}
