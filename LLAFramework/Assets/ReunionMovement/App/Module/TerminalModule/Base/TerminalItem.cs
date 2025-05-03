using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace LLAFramework
{
    public class TerminalItem : MonoBehaviour
    {
        public TextMeshProUGUI text;

        public void SetText(string str)
        {
            text.text = str;
        }
    }
}