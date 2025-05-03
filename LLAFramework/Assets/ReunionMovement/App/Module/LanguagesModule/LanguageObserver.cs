using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LLAFramework
{
    /// <summary>
    /// 观察者
    /// </summary>
    public abstract class LanguageObserver : MonoBehaviour
    {
        protected LanguageSubject subject;
        public abstract void UpdateData();
    }
}
