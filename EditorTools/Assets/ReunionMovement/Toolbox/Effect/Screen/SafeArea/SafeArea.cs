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
        private RectTransform safeArea;

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            safeArea = GetComponent<RectTransform>();
        }

        public void Update()
        {
            AdaptAnchorsValue();
        }

        private void AdaptAnchorsValue()
        {
            Rect safeAreaRect = Screen.safeArea;
            Vector2 anchorMin = safeAreaRect.position;
            Vector2 anchorMax = safeAreaRect.position + safeAreaRect.size;
            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;
            safeArea.anchorMin = anchorMin;
            safeArea.anchorMax = anchorMax;
        }
    }
}