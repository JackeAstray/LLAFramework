using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic.Base
{
    /// <summary>
    /// 被观察者
    /// </summary>
    public abstract class Observed
    {
        private List<Observer> observers = new List<Observer>();

        public void SetState()
        {
            NotifyAllObservers();
        }

        public void Attach(Observer observer)
        {
            observers.Add(observer);
        }

        public void NotifyAllObservers()
        {
            foreach (Observer observer in observers)
            {
                if (observer != null)
                {
                    observer.UpdateData();
                }
            }
        }

        public void Clear()
        {
            observers.Clear();
        }
    }
}