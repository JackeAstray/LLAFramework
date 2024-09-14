using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic.Base
{
    /// <summary>
    /// 单例模式管理器
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    public class SingletonMgr<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;
        public static T Instance
        {
            get { return instance; }
            set
            {
                if (instance == null)
                {
                    instance = value;
                }
                else if (instance != value)
                {
                    Destroy(value.gameObject);
                }
            }
        }

        void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Instance = this as T;
            }
        }
    }
}