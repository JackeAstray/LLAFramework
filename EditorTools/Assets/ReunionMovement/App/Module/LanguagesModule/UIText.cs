using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic
{
    /// <summary>
    /// UI 文本
    /// </summary>
    public delegate void DelegateGetText();
    public class UIText : LanguageObserver
    {
        [SerializeField]
        private int key;

        void Start()
        {
            if (subject == null && LanguagesModule.Instance != null)
            {
                subject = LanguagesModule.Instance.GetLanguageSubject();
                subject.Attach(this);
            }

            GetTextLanguage();
        }
        
        public void GetTextLanguage()
        {
            if (LanguagesModule.Instance == null)
            {
                return;
            }

            string value = LanguagesModule.Instance.GetTextByIndex(key);

            if (!string.IsNullOrEmpty(value))
            {
                GetComponent<Text>().text = value;
            }
            else
            {
                Log.Debug("GetTextLanguage() "+ key + "是空的");
            }
        }

        public override void UpdateData()
        {
            GetTextLanguage();
        }
    }
}