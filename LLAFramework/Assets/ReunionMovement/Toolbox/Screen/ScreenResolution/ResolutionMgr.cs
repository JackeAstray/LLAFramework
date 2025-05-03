using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using LLAFramework.Base;

namespace LLAFramework
{
    /// <summary>
    /// 分辨率管理
    /// </summary>
    public class ResolutionMgr : SingletonMgr<ResolutionMgr>
    {
        /// <summary>
        /// 纵横比
        /// </summary>
        public enum AspectRatio
        {
            AspectRatio_4_3,
            AspectRatio_16_9,
            AspectRatio_16_10,
            AspectRatio_21_9,
            AspectRatio_32_9,
        }

        // 纵横比
        public AspectRatio aspectRatio;
        // 固定纵横比
        public static bool FixedAspectRatio = true;
        // 目标纵横比
        public static float TargetAspectRatio = 4f / 3f;
        // 窗口纵横比
        public static float WindowedAspectRatio = 4f / 3f;
        // 分辨率
        private readonly int[] resolutions = { 600, 800, 1024, 1280, 1400, 1600, 1920, 2048, 2560, 2880, 3840 };

        // 显示分辨率
        public Resolution DisplayResolution { get; private set; }
        // 窗口分辨率
        public List<Vector2> WindowedResolutions { get; private set; }
        // 全屏分辨率
        public List<Vector2> FullscreenResolutions { get; private set; }

        // 当前窗口分辨率
        private int currWindowedRes;
        // 当前全屏分辨率
        private int currFullscreenRes;

        private void Start()
        {
            SetAspectRatio(aspectRatio);
            StartCoroutine(StartRoutine());
        }

        /// <summary>
        /// 设置纵横比
        /// </summary>
        /// <param name="aspectRatio"></param>
        private void SetAspectRatio(AspectRatio aspectRatio)
        {
            var aspectRatios = new Dictionary<AspectRatio, float>
            {
                { AspectRatio.AspectRatio_4_3, 4f / 3f },
                { AspectRatio.AspectRatio_16_9, 16f / 9f },
                { AspectRatio.AspectRatio_16_10, 16f / 10f },
                { AspectRatio.AspectRatio_21_9, 21f / 9f },
                { AspectRatio.AspectRatio_32_9, 32f / 9f }
            };

            if (aspectRatios.TryGetValue(aspectRatio, out float ratio))
            {
                TargetAspectRatio = ratio;
                WindowedAspectRatio = ratio;
            }
        }

        /// <summary>
        /// 开始协程
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
            // 获取当前显示分辨率的纵横比
            float screenAspect = (float)DisplayResolution.width / DisplayResolution.height;

            // 初始化窗口和全屏分辨率列表
            WindowedResolutions = new List<Vector2>();
            FullscreenResolutions = new List<Vector2>();

            // 遍历预定义的分辨率
            foreach (int w in resolutions)
            {
                // 仅处理小于当前显示分辨率80%的分辨率
                if (w < DisplayResolution.width * 0.8f)
                {
                    AddResolution(w, screenAspect);
                }
            }

            // 添加当前显示分辨率到全屏分辨率列表中
            FullscreenResolutions.Add(new Vector2(DisplayResolution.width, DisplayResolution.height));

            // 计算当前显示分辨率的一半并添加到全屏分辨率列表中
            Vector2 halfNative = new Vector2(DisplayResolution.width * 0.5f, DisplayResolution.height * 0.5f);
            if (halfNative.x > resolutions[0] && FullscreenResolutions.IndexOf(halfNative) == -1)
            {
                FullscreenResolutions.Add(halfNative);
            }

            // 按宽度对全屏分辨率列表进行排序
            FullscreenResolutions = FullscreenResolutions.OrderBy(resolution => resolution.x).ToList();

            bool found = false;

            // 如果当前是全屏模式
            if (Screen.fullScreen)
            {
                currWindowedRes = WindowedResolutions.Count - 1;

                // 查找当前屏幕分辨率在全屏分辨率列表中的索引
                for (int i = 0; i < FullscreenResolutions.Count; i++)
                {
                    if (FullscreenResolutions[i].x == Screen.width && FullscreenResolutions[i].y == Screen.height)
                    {
                        currFullscreenRes = i;
                        found = true;
                        break;
                    }
                }

                // 如果未找到，设置为全屏分辨率列表中的最后一个分辨率
                if (!found)
                {
                    SetResolution(FullscreenResolutions.Count - 1, true);
                }
            }
            else
            {
                currFullscreenRes = FullscreenResolutions.Count - 1;

                // 查找当前屏幕分辨率在窗口分辨率列表中的索引
                for (int i = 0; i < WindowedResolutions.Count; i++)
                {
                    if (WindowedResolutions[i].x == Screen.width && WindowedResolutions[i].y == Screen.height)
                    {
                        found = true;
                        currWindowedRes = i;
                        break;
                    }
                }

                // 如果未找到，设置为窗口分辨率列表中的最后一个分辨率
                if (!found)
                {
                    SetResolution(WindowedResolutions.Count - 1, false);
                }
            }
        }

        /// <summary>
        /// 添加分辨率
        /// </summary>
        /// <param name="width">宽度</param>
        /// <param name="screenAspect">屏幕纵横比</param>
        private void AddResolution(int width, float screenAspect)
        {
            Vector2 windowedResolution = new Vector2(width, Mathf.Round(width / (FixedAspectRatio ? TargetAspectRatio : WindowedAspectRatio)));
            if (windowedResolution.y < DisplayResolution.height * 0.8f)
            {
                WindowedResolutions.Add(windowedResolution);
            }

            FullscreenResolutions.Add(new Vector2(width, Mathf.Round(width / screenAspect)));
        }

        /// <summary>
        /// 设置分辨率
        /// </summary>
        /// <param name="index"></param>
        /// <param name="fullscreen"></param>
        public void SetResolution(int index, bool fullscreen)
        {
            Vector2 r = fullscreen ? FullscreenResolutions[currFullscreenRes = index] : WindowedResolutions[currWindowedRes = index];

            bool fullscreen2windowed = Screen.fullScreen && !fullscreen;

            Screen.SetResolution((int)r.x, (int)r.y, fullscreen);

            if (Application.platform == RuntimePlatform.OSXPlayer)
            {
                StopAllCoroutines();

                if (fullscreen2windowed)
                {
                    StartCoroutine(SetResolutionAfterResize(r));
                }
            }
        }

        /// <summary>
        /// 设置分辨率
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        private IEnumerator SetResolutionAfterResize(Vector2 r)
        {
            int maxTime = 5;
            float time = Time.time;

            yield return null;
            yield return null;

            int lastW = Screen.width;
            int lastH = Screen.height;

            while (Time.time - time < maxTime)
            {
                if (lastW != Screen.width || lastH != Screen.height)
                {
                    Screen.SetResolution((int)r.x, (int)r.y, Screen.fullScreen);
                    yield break;
                }

                yield return null;
            }
        }

        /// <summary>
        /// 切换全屏
        /// </summary>
        public void ToggleFullscreen()
        {
            SetResolution(
                Screen.fullScreen ? currWindowedRes : currFullscreenRes,
                !Screen.fullScreen
            );
        }

        /// <summary>
        /// 获取当前分辨率
        /// </summary>
        /// <returns>当前分辨率</returns>
        public Vector2 GetCurrentResolution()
        {
            return new Vector2(Screen.width, Screen.height);
        }

        /// <summary>
        /// 获取当前纵横比
        /// </summary>
        /// <returns>当前纵横比</returns>
        public float GetCurrentAspectRatio()
        {
            return (float)Screen.width / Screen.height;
        }
    }
}