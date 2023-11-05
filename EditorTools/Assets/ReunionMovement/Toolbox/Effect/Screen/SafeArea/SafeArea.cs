using GameLogic.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameLogic
{
    /// <summary>
    /// 安全区管理
    /// </summary>
    public class SafeArea : MonoBehaviour
    {
        private RectTransform rectTf;

        private Rect lastSafeArea;
        private int lastScreenWidth;
        private int lastScreenHeight;

        private void Awake()
        {
            rectTf = GetComponent<RectTransform>();
            UpdateRect();
        }

        private void Update()
        {
            UpdateRect();
        }

        /// <summary>
        /// 更新矩阵
        /// </summary>
        private void UpdateRect()
        {
            var safeArea = Screen.safeArea;
            var screenWidth = Screen.width;
            var screenHeight = Screen.height;

            // is same values
            if (safeArea.Equals(lastSafeArea) && lastScreenWidth == screenWidth && lastScreenHeight == screenHeight)
            {
                return;
            }

            ApplySafeArea(safeArea, screenWidth, screenHeight);

            lastSafeArea = safeArea;
            lastScreenWidth = screenWidth;
            lastScreenHeight = screenHeight;
        }

        /// <summary>
        /// 应用安全区域
        /// </summary>
        /// <param name="safeArea"></param>
        /// <param name="screenWidth"></param>
        /// <param name="screenHeight"></param>
        private void ApplySafeArea(Rect safeArea, int screenWidth, int screenHeight)
        {
            var anchorMin = safeArea.position;
            var anchorMax = safeArea.position + safeArea.size;
            anchorMin.x /= screenWidth;
            anchorMin.y /= screenHeight;
            anchorMax.x /= screenWidth;
            anchorMax.y /= screenHeight;

            rectTf.anchoredPosition = Vector2.zero;
            rectTf.sizeDelta = Vector2.zero;
            rectTf.anchorMin = anchorMin.IsFinite() ? anchorMin : Vector2.zero;
            rectTf.anchorMax = anchorMax.IsFinite() ? anchorMax : Vector2.one;
        }
    }


    /// <summary>
    /// Vector2扩展
    /// </summary>
    public static class Vector2Extensions
    {
        public static bool IsFinite(this Vector2 v)
        {
            return v.x.IsFinite() && v.y.IsFinite();
        }

        private static bool IsFinite(this float f)
        {
            return !float.IsNaN(f) && !float.IsInfinity(f);
        }
    }
}