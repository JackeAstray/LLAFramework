using LLAFramework.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace LLAFramework
{
    /// <summary>
    /// 安全区管理
    /// </summary>
    public class SafeArea : MonoBehaviour
    {
        private RectTransform rectTf;

        private Rect lastSafeArea;
        private Vector2 lastScreenSize;

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
            var screenSize = new Vector2(Screen.width, Screen.height);

            if (safeArea.Equals(lastSafeArea) && screenSize.Equals(lastScreenSize))
            {
                return;
            }

            ApplySafeArea(safeArea, screenSize);

            lastSafeArea = safeArea;
            lastScreenSize = screenSize;
        }

        /// <summary>
        /// 应用安全区域
        /// </summary>
        /// <param name="safeArea"></param>
        /// <param name="screenSize"></param>
        private void ApplySafeArea(Rect safeArea, Vector2 screenSize)
        {
            var anchorMin = safeArea.position;
            var anchorMax = safeArea.position + safeArea.size;
            anchorMin.x /= screenSize.x;
            anchorMin.y /= screenSize.y;
            anchorMax.x /= screenSize.x;
            anchorMax.y /= screenSize.y;

            rectTf.anchoredPosition = Vector2.zero;
            rectTf.sizeDelta = Vector2.zero;
            rectTf.anchorMin = anchorMin.IsFinite() ? anchorMin : Vector2.zero;
            rectTf.anchorMax = anchorMax.IsFinite() ? anchorMax : Vector2.one;
        }
    }
}