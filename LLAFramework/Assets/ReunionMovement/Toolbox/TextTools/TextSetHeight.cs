using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LLAFramework
{
    /// <summary>
    /// 设置文本框高度
    /// </summary>
    public class TextSetHeight : MonoBehaviour
    {
        private Text text;
        private RectTransform rect;
        private Vector2 previousSize;

        void Start()
        {
            text = GetComponent<Text>();
            rect = GetComponent<RectTransform>();
            previousSize = rect.rect.size;
        }

        void Update()
        {
            Vector2 currentSize = rect.rect.size;
            if (currentSize != previousSize)
            {
                SetSize();
                previousSize = currentSize;
            }
        }

        public void SetSize()
        {
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, text.preferredHeight);
        }
    }
}
