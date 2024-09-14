using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GameLogic
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