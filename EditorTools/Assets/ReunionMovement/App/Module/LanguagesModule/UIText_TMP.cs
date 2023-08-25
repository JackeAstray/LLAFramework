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
        // Use this for initialization

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
                GetComponent<TMP_Text>().text = value;
            }
            else
            {
                //Debug.Log("GetTextLanguage() "+ key + "是空的");
            }
        }

        public override void UpdateData()
        {
            GetTextLanguage();
        }
    }
}