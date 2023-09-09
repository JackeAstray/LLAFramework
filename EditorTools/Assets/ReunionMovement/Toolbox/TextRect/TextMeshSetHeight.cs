using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic
{
    /// <summary>
    /// 设置文本框高度
    /// </summary>
    public class TextMeshSetHeight : MonoBehaviour
    {
        public TextMeshProUGUI text;
        public RectTransform rect;

        void Start()
        {
            text = GetComponent<TextMeshProUGUI>();
            rect = GetComponent<RectTransform>();
        }

        // Update is called once per frame
        void Update()
        {
            SetSize();
        }

        /// <summary>
        /// 设置Text高度
        /// </summary>
        public void SetSize()
        {
            Vector2 v2 = rect.rect.size;
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, v2.x);
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, text.preferredHeight);
        }
    }
}
