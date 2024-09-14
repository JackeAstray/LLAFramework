using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GameLogic
{
    public class UIText_TMP : LanguageObserver
    {
        [SerializeField]
        private int key;

        private TMP_Text textComponent;

        void Start()
        {
            textComponent = GetComponent<TMP_Text>();

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
                Log.Debug("GetTextLanguage() " + key + "是空的");
            }
        }

        public override void UpdateData()
        {
            GetTextLanguage();
        }
    }
}