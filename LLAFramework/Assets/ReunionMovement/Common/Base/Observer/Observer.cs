using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic.Base
{
    /// <summary>
    /// 观察者
    /// </summary>
    public abstract class Observer : MonoBehaviour
    {
        protected Observed subject;
        public abstract void UpdateData();
    }
}