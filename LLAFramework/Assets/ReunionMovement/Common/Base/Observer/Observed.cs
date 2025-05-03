using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LLAFramework.Base
{
    /// <summary>
    /// 被观察者
    /// </summary>
    public abstract class Observed
    {
        private List<Observer> observers = new List<Observer>();

        public void SetState(params object[] args)
        {
            NotifyAllObservers(args);
        }

        public void Attach(Observer observer)
        {
            observers.Add(observer);
        }

        public void NotifyAllObservers(params object[] args)
        {
            foreach (Observer observer in observers)
            {
                if (observer != null)
                {
                    observer.UpdateData(args);
                }
            }
        }

        public void Clear()
        {
            observers.Clear();
        }
    }
}