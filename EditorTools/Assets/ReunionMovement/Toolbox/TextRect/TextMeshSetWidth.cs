using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextMeshSetWidth : MonoBehaviour
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
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, text.preferredHeight);
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, v2.y);
    }
}
