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
        private RectTransform rectTrans;

        private void Start()
        {
            Init();
            AdaptAnchorsValue();
        }

        private void Init()
        {
            rectTrans = GetComponent<RectTransform>();

            rectTrans.anchorMin = Vector2.zero;
            rectTrans.anchorMax = Vector2.one;
            rectTrans.anchoredPosition = Vector2.zero;
            rectTrans.sizeDelta = Vector2.zero;

        }

        private void AdaptAnchorsValue()
        {
            var maxWidth = Display.main.systemWidth;
            var maxHeight = Display.main.systemHeight;
            var safeArea = Screen.safeArea;
            var anchorMin = safeArea.position;
            var anchorMax = safeArea.position + safeArea.size;
            anchorMin.x /= maxWidth;
            anchorMin.y /= maxHeight;
            anchorMax.x /= maxWidth;
            anchorMax.y /= maxHeight;

            rectTrans.anchorMin = anchorMin;
            rectTrans.anchorMax = anchorMax;
        }
    }
}