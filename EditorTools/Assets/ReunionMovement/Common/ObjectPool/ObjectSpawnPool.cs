using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 对象生成池
    /// </summary>
    public sealed class ObjectSpawnPool
    {
        private GameObject spawnTem;
        // 限制对象数量
        private int limit = 100;
        // 当前生成的对象数量
        private int currentCount = 0;
        private Queue<GameObject> objectQueue = new Queue<GameObject>();
        private Action<GameObject> onSpawn;
        private Action<GameObject> onDespawn;

        /// <summary>
        /// 对象数量
        /// </summary>
        public int Count
        {
            get
            {
                return objectQueue.Count;
            }
        }

        public ObjectSpawnPool(GameObject spawnTem, int limit, Action<GameObject> onSpawn, Action<GameObject> onDespawn)
        {
            this.spawnTem = spawnTem;
            this.limit = limit;
            this.onSpawn = onSpawn;
            this.onDespawn = onDespawn;
        }

        /// <summary>
        /// 生成对象
        /// </summary>
        /// <returns>对象</returns>
        public GameObject Spawn()
        {
            GameObject obj;
            if (currentCount >= limit && objectQueue.Count > 0)
            {
                obj = objectQueue.Dequeue();
            }
            else if(currentCount >= limit && objectQueue.Count <= 0)
            {
                Debug.LogWarning("已达到对象池限制。无法生成更多对象。");
                return null;
            }
            else if (objectQueue.Count > 0)
            {
                obj = objectQueue.Dequeue();
            }
            else
            {
                obj = ObjectPoolMgr.CloneGameObject(spawnTem);
                currentCount++; // 增加当前生成的对象数量
            }

            obj.SetActive(true);

            onSpawn?.Invoke(obj);

            return obj;
        }
        /// <summary>
        /// 回收对象
        /// </summary>
        /// <param name="obj">对象</param>
        public void Despawn(GameObject obj)
        {
            if (obj == null)
            {
                return;
            }

            if (objectQueue.Count >= limit)
            {
                onDespawn?.Invoke(obj);
                ObjectPoolMgr.Kill(obj);
                currentCount--; // 减少当前生成的对象数量
            }
            else
            {
                obj.SetActive(false);
                onDespawn?.Invoke(obj);
                objectQueue.Enqueue(obj);
            }
        }
        /// <summary>
        /// 清空所有对象
        /// </summary>
        public void Clear()
        {
            while (objectQueue.Count > 0)
            {
                GameObject obj = objectQueue.Dequeue();
                if (obj)
                {
                    ObjectPoolMgr.Kill(obj);
                    currentCount--; // 减少当前生成的对象数量
                }
            }
        }
    }
}
