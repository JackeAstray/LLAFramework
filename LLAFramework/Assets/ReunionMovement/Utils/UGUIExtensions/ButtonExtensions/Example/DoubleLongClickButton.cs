using LLAFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleLongClickButton : MonoBehaviour
{
    public DoubleClickButton DoubleClickButton = null;
    public LongClickButton LongClickButton = null;

    public void Start()
    {
        DoubleClickButton.onDoubleClick.AddListener(() =>
        {
            Debug.Log("双击按钮");
        });
        LongClickButton.onLongClick.AddListener(() =>
        {
            Debug.Log("长按按钮");
        });
    }
}
