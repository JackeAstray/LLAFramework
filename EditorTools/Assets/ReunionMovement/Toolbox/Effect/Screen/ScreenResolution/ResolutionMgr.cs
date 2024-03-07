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
            AspectRatio_4_3,
            AspectRatio_16_9,
            AspectRatio_16_10,
            AspectRatio_21_9,
            AspectRatio_32_9,
        }

        public AspectRatio aspectRatio;
        static public bool FixedAspectRatio = true;
        static public float TargetAspectRatio = 4f / 3f;
        static public float WindowedAspectRatio = 4f / 3f;
        int[] resolutions = new int[] { 600, 800, 1024, 1280, 1400, 1600, 1920, 2048, 2560, 2880, 3840 };

        public Resolution DisplayResolution;
        public List<Vector2> WindowedResolutions, FullscreenResolutions;

        int currWindowedRes;
        int currFullscreenRes;

        void Start()
        {
            switch (aspectRatio)
            {
                case AspectRatio.AspectRatio_4_3:
                    TargetAspectRatio = 4f / 3f;
                    WindowedAspectRatio = 4f / 3f;
                    break;
                case AspectRatio.AspectRatio_16_9:
                    TargetAspectRatio = 16f / 9f;
                    WindowedAspectRatio = 16f / 9f;
                    break;
                case AspectRatio.AspectRatio_16_10:
                    TargetAspectRatio = 16f / 10f;
                    WindowedAspectRatio = 16f / 10f;
                    break;
                case AspectRatio.AspectRatio_21_9:
                    TargetAspectRatio = 21f / 9f;
                    WindowedAspectRatio = 21f / 9f;
                    break;
                case AspectRatio.AspectRatio_32_9:
                    TargetAspectRatio = 32f / 9f;
                    WindowedAspectRatio = 32f / 9f;
                    break;
            }

            StartCoroutine(StartRoutine());
        }

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

        private void InitResolutions()
        {
            float screenAspect = (float)DisplayResolution.width / DisplayResolution.height;

            WindowedResolutions = new List<Vector2>();
            FullscreenResolutions = new List<Vector2>();

            foreach (int w in resolutions)
            {
                if (w < DisplayResolution.width)
                {
                    if (w < DisplayResolution.width * 0.8f)
                    {
                        Vector2 windowedResolution = new Vector2(w, Mathf.Round(w / (FixedAspectRatio ? TargetAspectRatio : WindowedAspectRatio)));
                        if (windowedResolution.y < DisplayResolution.height * 0.8f)
                            WindowedResolutions.Add(windowedResolution);

                        FullscreenResolutions.Add(new Vector2(w, Mathf.Round(w / screenAspect)));
                    }
                }
            }

            FullscreenResolutions.Add(new Vector2(DisplayResolution.width, DisplayResolution.height));

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

        public void SetResolution(int index, bool fullscreen)
        {
            Vector2 r = fullscreen ? FullscreenResolutions[currFullscreenRes = index] : WindowedResolutions[currWindowedRes = index];

            bool fullscreen2windowed = Screen.fullScreen & !fullscreen;

            Screen.SetResolution((int)r.x, (int)r.y, fullscreen);

            if (Application.platform == RuntimePlatform.OSXPlayer)
            {
                StopAllCoroutines();

                if (fullscreen2windowed) StartCoroutine(SetResolutionAfterResize(r));
            }
        }

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

        public void ToggleFullscreen()
        {
            SetResolution(
                Screen.fullScreen ? currWindowedRes : currFullscreenRes,
                !Screen.fullScreen);
        }
    }
}
