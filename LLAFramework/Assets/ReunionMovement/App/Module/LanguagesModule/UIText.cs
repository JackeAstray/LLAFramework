using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LLAFramework
{
    /// <summary>
    /// UI 文本
    /// </summary>
    public delegate void DelegateGetText();
    public class UIText : LanguageObserver
    {
        [SerializeField]
        private int key;

        private Text textComponent;

        void Start()
        {
            if (subject == null && LanguagesModule.Instance != null)
            {
                subject = LanguagesModule.Instance.GetLanguageSubject();
            }

            GetTextLanguage();
        }

        void OnEnable()
        {
            if (subject != null)
            {
                subject.Attach(this);
            }
        }

        void OnDisable()
        {
            if (subject != null)
            {
                subject.Detach(this);
            }
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
                textComponent.text = value;
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