using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextSetWidth : MonoBehaviour
{
    public Text text;
    public RectTransform rect;

    void Start()
    {
        text = GetComponent<Text>();
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
