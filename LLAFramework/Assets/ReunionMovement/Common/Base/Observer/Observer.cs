using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LLAFramework.Base
{
    /// <summary>
    /// 观察者
    /// </summary>
    public abstract class Observer : MonoBehaviour
    {
        protected Observed subject;
        public abstract void UpdateData(params object[] args);
    }
}