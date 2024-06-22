using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameLogic
{
    /// <summary>
    /// 对象池
    /// </summary>
    public class SoundPoolModule : CustommModuleInitialize
    {
        #region 实例与初始化
        public static SoundPoolModule Instance = new SoundPoolModule();
        public bool IsInited { get; private set; }
        private double initProgress = 0;
        public double InitProgress { get { return initProgress; } }
        #endregion

        string poolPath = "Prefabs/Pools/SoundObj";
        GameObject soundPoolRoot;
        GameObject soundPoolTempRoot;
        //启动对象池用
        public List<StartupPool> startupPools = new List<StartupPool>();
        //临时列表
        static List<GameObject> tempList = new List<GameObject>();
        //预设对象池
        Dictionary<GameObject, List<GameObject>> pooledObjects = new Dictionary<GameObject, List<GameObject>>();
        //生成对象池
        Dictionary<GameObject, GameObject> spawnedObjects = new Dictionary<GameObject, GameObject>();

        public IEnumerator Init()
        {
            initProgress = 0;
            CreateRoot();
            CreatePools();
            yield return null;
            initProgress = 100;
            IsInited = true;
            Log.Debug("ObjectPoolModule 初始化完成");
        }

        public void ClearData()
        {
            Log.Debug("ObjectPoolModule 清除数据");
        }

        /// <summary>
        /// 场景管理器轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        public void UpdateTime(float elapseSeconds, float realElapseSeconds)
        {

        }

        #region 启动对象池
        public void CreateRoot()
        {
            soundPoolRoot = new GameObject("SoundPoolRoot");
            soundPoolRoot.transform.localPosition = Vector3.zero;
            soundPoolRoot.transform.localRotation = Quaternion.identity;
            soundPoolRoot.transform.localScale = Vector3.one;

            soundPoolTempRoot = new GameObject("SoundPoolTempRoot");
            soundPoolTempRoot.transform.localPosition = Vector3.zero;
            soundPoolTempRoot.transform.localRotation = Quaternion.identity;
            soundPoolTempRoot.transform.localScale = Vector3.one;

            GameObject.DontDestroyOnLoad(soundPoolRoot);
            GameObject.DontDestroyOnLoad(soundPoolTempRoot);
        }

        public void CreatePools()
        {
            StartupPool pool = new StartupPool();
            pool.size = 20;
            pool.prefab = ResourcesModule.Instance.Load<GameObject>(poolPath);
            startupPools.Add(pool);

            if (startupPools != null && startupPools.Count > 0)
            {
                for (int i = 0; i < startupPools.Count; ++i)
                {
                    CreatePool(startupPools[i].prefab, startupPools[i].size);
                }
            }
        }

        /// <summary>
        /// 创建对象池
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="initialPoolSize"></param>
        public void CreatePool(GameObject prefab, int initialPoolSize)
        {
            if (prefab != null && !pooledObjects.ContainsKey(prefab))
            {
                var list = new List<GameObject>();
                pooledObjects.Add(prefab, list);

                if (initialPoolSize > 0)
                {
                    Transform parent = soundPoolRoot.transform;
                    while (list.Count < initialPoolSize)
                    {
                        var obj = (GameObject)Object.Instantiate(prefab);
                        obj.transform.parent = parent;
                        obj.SetActive(false);
                        list.Add(obj);
                    }
                }
            }
        }
        #endregion

        #region 生成
        /// <summary>
        /// 生成
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="parent"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public GameObject Spawn(GameObject prefab, Transform parent, Vector3 position)
        {
            return Spawn(prefab, parent, position, Quaternion.identity);
        }
        /// <summary>
        /// 生成
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            return Spawn(prefab, soundPoolTempRoot.transform, position, rotation);
        }
        /// <summary>
        /// 生成
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public GameObject Spawn(GameObject prefab, Transform parent)
        {
            return Spawn(prefab, parent, Vector3.zero, Quaternion.identity);
        }
        /// <summary>
        /// 生成
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public GameObject Spawn(GameObject prefab, Vector3 position)
        {
            return Spawn(prefab, soundPoolTempRoot.transform, position, Quaternion.identity);
        }
        /// <summary>
        /// 生成
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        public GameObject Spawn(GameObject prefab)
        {
            return Spawn(prefab, soundPoolTempRoot.transform, Vector3.zero, Quaternion.identity);
        }
        /// <summary>
        /// 生成
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="parent"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public GameObject Spawn(GameObject prefab, Transform parent, Vector3 position, Quaternion rotation)
        {
            List<GameObject> list;
            GameObject obj;
            if (pooledObjects.TryGetValue(prefab, out list))
            {
                obj = null;
                if (list.Count > 0)
                {
                    while (obj == null && list.Count > 0)
                    {
                        obj = list[0];
                        list.RemoveAt(0);
                    }
                    if (obj != null)
                    {
                        obj.transform.parent = parent;
                        obj.transform.localPosition = position;
                        obj.transform.localRotation = rotation;
                        obj.SetActive(true);
                        spawnedObjects.Add(obj, prefab);
                        return obj;
                    }
                }
                obj = GameObject.Instantiate(prefab, position, rotation, parent);
                spawnedObjects.Add(obj, prefab);
                return obj;
            }
            else
            {
                obj = GameObject.Instantiate(prefab, position, rotation, parent);
                return obj;
            }
        }
        #endregion

        #region 回收利用
        /// <summary>
        /// 回收利用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        public void Recycle<T>(T obj) where T : Component
        {
            Recycle(obj.gameObject);
        }

        /// <summary>
        /// 回收利用
        /// </summary>
        /// <param name="obj"></param>
        public void Recycle(GameObject obj)
        {
            GameObject prefab;
            if (spawnedObjects.TryGetValue(obj, out prefab))
            {
                Recycle(obj, prefab);
            }
            else
            {
                Object.Destroy(obj);
            }
        }

        /// <summary>
        /// 回收利用
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="prefab"></param>
        void Recycle(GameObject obj, GameObject prefab)
        {
            pooledObjects[prefab].Add(obj);
            spawnedObjects.Remove(obj);
            obj.transform.parent = soundPoolRoot.transform;
            obj.SetActive(false);
        }

        /// <summary>
        /// 回收利用全部
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prefab"></param>
        public void RecycleAll<T>(T prefab) where T : Component
        {
            RecycleAll(prefab.gameObject);
        }

        /// <summary>
        /// 回收利用全部
        /// </summary>
        /// <param name="prefab"></param>
        public void RecycleAll(GameObject prefab)
        {
            foreach (var item in spawnedObjects)
            {
                if (item.Value == prefab)
                {
                    tempList.Add(item.Key);
                }
            }
            for (int i = 0; i < tempList.Count; ++i)
            {
                Recycle(tempList[i]);
            }
            tempList.Clear();
        }

        /// <summary>
        /// 回收利用全部
        /// </summary>
        public void RecycleAll()
        {
            tempList.AddRange(spawnedObjects.Keys);
            for (int i = 0; i < tempList.Count; ++i)
            {
                Recycle(tempList[i]);
            }
            tempList.Clear();
        }
        #endregion

        /// <summary>
        /// 对象池计数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prefab"></param>
        /// <returns></returns>
        public int CountPooled<T>(T prefab) where T : Component
        {
            return CountPooled(prefab.gameObject);
        }

        /// <summary>
        /// 对象池计数
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        public int CountPooled(GameObject prefab)
        {
            List<GameObject> list;
            if (pooledObjects.TryGetValue(prefab, out list))
            {
                return list.Count;
            }
            return 0;
        }

        /// <summary>
        /// 对象池计数
        /// </summary>
        /// <returns></returns>
        public int CountAllPooled()
        {
            int count = 0;
            foreach (var list in pooledObjects.Values)
            {
                count += list.Count;
            }
            return count;
        }

        /// <summary>
        /// 从预设对象池中获得同类型对象
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="list"></param>
        /// <param name="appendList"></param>
        /// <returns></returns>
        public List<GameObject> GetPooled(GameObject prefab, List<GameObject> list, bool appendList)
        {
            if (list == null)
            {
                list = new List<GameObject>();
            }
            if (!appendList)
            {
                list.Clear();
            }
            List<GameObject> pooled;
            if (pooledObjects.TryGetValue(prefab, out pooled))
            {
                list.AddRange(pooled);
            }
            return list;
        }

        /// <summary>
        /// 从预设对象池中获得同类型对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prefab"></param>
        /// <param name="list"></param>
        /// <param name="appendList"></param>
        /// <returns></returns>
        public List<T> GetPooled<T>(T prefab, List<T> list, bool appendList) where T : Component
        {
            if (list == null)
            {
                list = new List<T>();
            }
            if (!appendList)
            {
                list.Clear();
            }
            List<GameObject> pooled;
            if (pooledObjects.TryGetValue(prefab.gameObject, out pooled))
            {
                for (int i = 0; i < pooled.Count; ++i)
                {
                    list.Add(pooled[i].GetComponent<T>());
                }
            }
            return list;
        }

        /// <summary>
        /// 对象是否在生成对象池中
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool IsSpawned(GameObject obj)
        {
            return spawnedObjects.ContainsKey(obj);
        }

        /// <summary>
        /// 获取生成的数量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prefab"></param>
        /// <returns></returns>
        public int CountSpawned<T>(T prefab) where T : Component
        {
            return CountSpawned(prefab.gameObject);
        }

        /// <summary>
        /// 获取生成的数量
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        public int CountSpawned(GameObject prefab)
        {
            int count = 0;
            foreach (var instancePrefab in spawnedObjects.Values)
            {
                if (prefab == instancePrefab)
                {
                    ++count;
                }
            }
            return count;
        }

        /// <summary>
        /// 从生成对象池中获得同类型对象
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="list"></param>
        /// <param name="appendList"></param>
        /// <returns></returns>
        public List<GameObject> GetSpawned(GameObject prefab, List<GameObject> list, bool appendList)
        {
            if (list == null)
            {
                list = new List<GameObject>();
            }
            if (!appendList)
            {
                list.Clear();
            }
            foreach (var item in spawnedObjects)
            {
                if (item.Value == prefab)
                {
                    list.Add(item.Key);
                }
            }
            return list;
        }

        /// <summary>
        /// 从生成对象池中获得同类型对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prefab"></param>
        /// <param name="list"></param>
        /// <param name="appendList"></param>
        /// <returns></returns>
        public List<T> GetSpawned<T>(T prefab, List<T> list, bool appendList) where T : Component
        {
            if (list == null)
            {
                list = new List<T>();
            }
            if (!appendList)
            {
                list.Clear();
            }
            var prefabObj = prefab.gameObject;
            foreach (var item in spawnedObjects)
            {
                if (item.Value == prefabObj)
                {
                    list.Add(item.Key.GetComponent<T>());
                }
            }
            return list;
        }

        #region 销毁
        /// <summary>
        /// 销毁集合
        /// </summary>
        /// <param name="prefab"></param>
        public void DestroyPooled(GameObject prefab)
        {
            List<GameObject> pooled;
            if (pooledObjects.TryGetValue(prefab, out pooled))
            {
                for (int i = 0; i < pooled.Count; ++i)
                {
                    GameObject.Destroy(pooled[i]);
                }
                pooled.Clear();
            }
        }

        public void DestroyAll(GameObject prefab)
        {
            RecycleAll(prefab);
            DestroyPooled(prefab);
        }
        #endregion
    }

    /// <summary>
    /// 启动池
    /// </summary>
    [System.Serializable]
    public class StartupPool
    {
        public int size;
        public GameObject prefab;
    }
}