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
        private static readonly object lockObj = new object();

        private static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObj)
                    {
                        if (instance == null)
                        {
                            instance = CreateInstance();
                        }
                    }
                }
                return instance;
            }
            set
            {
                if (instance == null)
                {
                    instance = value;
                    isInitialized = true;
                    OnInstanceCreated?.Invoke(instance);
                }
                else if (instance != value)
                {
                    Destroy(value.gameObject);
                }
            }
        }

        /// <summary>
        /// 是否初始化
        /// </summary>
        private static bool isInitialized = false;
        public static bool IsInitialized
        {
            get { return isInitialized; }
        }

        /// <summary>
        /// 单例实例创建事件
        /// </summary>
        public static event System.Action<T> OnInstanceCreated;

        /// <summary>
        /// 单例实例销毁事件
        /// </summary>
        public static event System.Action OnInstanceDestroyed;

        protected virtual void Awake()
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

        /// <summary>
        /// 手动销毁单例
        /// </summary>
        public static void DestroyInstance()
        {
            if (instance != null)
            {
                OnInstanceDestroyed?.Invoke();
                Destroy(instance.gameObject);
                instance = null;
                isInitialized = false;
            }
        }

        /// <summary>
        /// 创建单例实例
        /// </summary>
        private static T CreateInstance()
        {
            T foundInstance = FindObjectOfType<T>();
            if (foundInstance == null)
            {
                GameObject singletonObject = new GameObject();
                foundInstance = singletonObject.AddComponent<T>();
                singletonObject.name = typeof(T).ToString() + " (Singleton)";
            }
            isInitialized = true;
            OnInstanceCreated?.Invoke(foundInstance);
            return foundInstance;
        }
    }
}