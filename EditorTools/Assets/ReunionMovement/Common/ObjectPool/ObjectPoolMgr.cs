using GameLogic.Base;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameLogic
{
    /// <summary>
    /// 对象池管理器
    /// </summary>
    public class ObjectPoolMgr : SingleToneMgr<ObjectPoolMgr>
    {
        /// <summary>
        /// 所有对象池
        /// </summary>
        public Dictionary<string, ObjectSpawnPool> SpawnPools { get; private set; } = new Dictionary<string, ObjectSpawnPool>();

        /// <summary>
        /// 单个对象池上限【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal int Limit = 100;

        /// <summary>
        /// 注册对象池
        /// </summary>
        /// <param name="name">对象池名称</param>
        /// <param name="spawnTem">对象模板</param>
        /// <param name="onSpawn">对象生成时初始化委托</param>
        /// <param name="onDespawn">对象回收时处理委托</param>
        /// <param name="limit">对象池上限，等于0时，表示使用默认值</param>
        public void RegisterSpawnPool(string name, GameObject spawnTem, Action<GameObject> onSpawn = null, Action<GameObject> onDespawn = null, int limit = 0)
        {
            if (string.IsNullOrEmpty(name) || spawnTem == null)
            {
                return;
            }

            if (!SpawnPools.ContainsKey(name))
            {
                SpawnPools.Add(name, new ObjectSpawnPool(spawnTem, limit, onSpawn, onDespawn));
            }
            else
            {
                Log.Error($"注册对象池失败：已存在对象池 {name} ！");
            }
        }

        /// <summary>
        /// 是否存在指定名称的对象池
        /// </summary>
        /// <param name="name">对象池名称</param>
        /// <returns>是否存在</returns>
        public bool IsExistSpawnPool(string name)
        {
            return SpawnPools.ContainsKey(name);
        }

        /// <summary>
        /// 移除已注册的对象池
        /// </summary>
        /// <param name="name">对象池名称</param>
        public void UnRegisterSpawnPool(string name)
        {
            if (SpawnPools.ContainsKey(name))
            {
                SpawnPools[name].Clear();
                SpawnPools.Remove(name);
            }
            else
            {
                Log.Error($"移除对象池失败：不存在对象池 {name} ！");
            }
        }

        /// <summary>
        /// 获取对象池中对象数量
        /// </summary>
        /// <param name="name">对象池名称</param>
        /// <returns>对象数量</returns>
        public int GetPoolCount(string name)
        {
            if (SpawnPools.ContainsKey(name))
            {
                return SpawnPools[name].Count;
            }
            else
            {
                Log.Warning($"获取对象数量失败：不存在对象池 {name} ！");
                return 0;
            }
        }

        /// <summary>
        /// 生成对象
        /// </summary>
        /// <param name="name">对象池名称</param>
        /// <returns>对象</returns>
        public GameObject Spawn(string name)
        {
            if (SpawnPools.ContainsKey(name))
            {
                return SpawnPools[name].Spawn();
            }
            else
            {
                Log.Error($"生成对象失败：不存在对象池 {name} ！");
                return null;
            }
        }

        /// <summary>
        /// 回收对象
        /// </summary>
        /// <param name="name">对象池名称</param>
        /// <param name="target">对象</param>
        public void Despawn(string name, GameObject target)
        {
            if (SpawnPools.ContainsKey(name))
            {
                SpawnPools[name].Despawn(target);
            }
            else
            {
                Log.Error($"回收对象失败：不存在对象池 {name} ！");
            }
        }

        /// <summary>
        /// 批量回收对象
        /// </summary>
        /// <param name="name">对象池名称</param>
        /// <param name="targets">对象数组</param>
        public void Despawns(string name, GameObject[] targets)
        {
            if (targets == null)
                return;

            if (SpawnPools.ContainsKey(name))
            {
                for (int i = 0; i < targets.Length; i++)
                {
                    SpawnPools[name].Despawn(targets[i]);
                }
            }
            else
            {
                Log.Error($"回收对象失败：不存在对象池 {name} ！");
            }
        }

        /// <summary>
        /// 批量回收对象
        /// </summary>
        /// <param name="name">对象池名称</param>
        /// <param name="targets">对象集合</param>
        public void Despawns(string name, List<GameObject> targets)
        {
            if (targets == null)
                return;

            if (SpawnPools.ContainsKey(name))
            {
                for (int i = 0; i < targets.Count; i++)
                {
                    SpawnPools[name].Despawn(targets[i]);
                }
                targets.Clear();
            }
            else
            {
                Log.Error($"回收对象失败：不存在对象池 {name} ！");
            }
        }

        /// <summary>
        /// 清空指定的对象池
        /// </summary>
        /// <param name="name">对象池名称</param>
        public void Clear(string name)
        {
            if (SpawnPools.ContainsKey(name))
            {
                SpawnPools[name].Clear();
            }
            else
            {
                Log.Error($"清空对象池失败：不存在对象池 {name} ！");
            }
        }

        /// <summary>
        /// 清空所有对象池
        /// </summary>
        public void ClearAll()
        {
            foreach (var spawnPool in SpawnPools)
            {
                spawnPool.Value.Clear();
            }
        }

        #region 静态方法

        /// <summary>
        /// 克隆实例
        /// </summary>
        /// <typeparam name="T">实例类型</typeparam>
        /// <param name="original">初始对象</param>
        /// <returns>克隆的新对象</returns>
        public static T Clone<T>(T original) where T : Object
        {
            return Instantiate(original);
        }

        /// <summary>
        /// 克隆实例
        /// </summary>
        /// <typeparam name="T">实例类型</typeparam>
        /// <param name="original">初始对象</param>
        /// <param name="position">新对象的位置</param>
        /// <param name="rotation">新对象的旋转</param>
        /// <returns>克隆的新对象</returns>
        public static T Clone<T>(T original, Vector3 position, Quaternion rotation) where T : Object
        {
            return Instantiate(original, position, rotation);
        }

        /// <summary>
        /// 克隆实例
        /// </summary>
        /// <typeparam name="T">实例类型</typeparam>
        /// <param name="original">初始对象</param>
        /// <param name="position">新对象的位置</param>
        /// <param name="rotation">新对象的旋转</param>
        /// <param name="parent">新对象的父物体</param>
        /// <returns>克隆的新对象</returns>
        public static T Clone<T>(T original, Vector3 position, Quaternion rotation, Transform parent) where T : Object
        {
            return Instantiate(original, position, rotation, parent);
        }

        /// <summary>
        /// 克隆实例
        /// </summary>
        /// <typeparam name="T">实例类型</typeparam>
        /// <param name="original">初始对象</param>
        /// <param name="parent">新对象的父物体</param>
        /// <returns>克隆的新对象</returns>
        public static T Clone<T>(T original, Transform parent) where T : Object
        {
            return Instantiate(original, parent);
        }

        /// <summary>
        /// 克隆实例
        /// </summary>
        /// <typeparam name="T">实例类型</typeparam>
        /// <param name="original">初始对象</param>
        /// <param name="parent">新对象的父物体</param>
        /// <param name="worldPositionStays">是否保持世界位置不变</param>
        /// <returns>克隆的新对象</returns>
        public static T Clone<T>(T original, Transform parent, bool worldPositionStays) where T : Object
        {
            return Instantiate(original, parent, worldPositionStays);
        }

        /// <summary>
        /// 克隆 GameObject 实例
        /// </summary>
        /// <param name="original">初始对象</param>
        /// <param name="isUI">是否是UI对象</param>
        /// <returns>克隆的新对象</returns>
        public static GameObject CloneGameObject(GameObject original, bool isUI = false)
        {
            GameObject obj = Instantiate(original);
            obj.transform.SetParent(original.transform.parent);
            if (isUI)
            {
                RectTransform rect = obj.GetComponent<RectTransform>();
                RectTransform originalRect = original.GetComponent<RectTransform>();
                rect.anchoredPosition3D = originalRect.anchoredPosition3D;
                rect.sizeDelta = originalRect.sizeDelta;
                rect.offsetMin = originalRect.offsetMin;
                rect.offsetMax = originalRect.offsetMax;
                rect.anchorMin = originalRect.anchorMin;
                rect.anchorMax = originalRect.anchorMax;
                rect.pivot = originalRect.pivot;
            }
            else
            {
                obj.transform.localPosition = original.transform.localPosition;
            }
            obj.transform.localRotation = original.transform.localRotation;
            obj.transform.localScale = original.transform.localScale;
            obj.SetActive(true);
            return obj;
        }

        /// <summary>
        /// 杀死实例
        /// </summary>
        /// <param name="obj">实例对象</param>
        public static void Kill(Object obj)
        {
            GameObjectExtensions.DestroyObject(obj);
        }

        /// <summary>
        /// 立即杀死实例
        /// </summary>
        /// <param name="obj">实例对象</param>
        public static void KillImmediate(Object obj)
        {
            GameObjectExtensions.DestroyObjectImmediate(obj);
        }

        /// <summary>
        /// 杀死一群实例
        /// </summary>
        /// <typeparam name="T">实例类型</typeparam>
        /// <param name="objs">实例集合</param>
        public static void Kills<T>(List<T> objs) where T : Object
        {
            GameObjectExtensions.DestroyObjects(objs);
            objs.Clear();
        }

        /// <summary>
        /// 杀死一群实例
        /// </summary>
        /// <typeparam name="T">实例类型</typeparam>
        /// <param name="objs">实例数组</param>
        public static void Kills<T>(T[] objs) where T : Object
        {
            GameObjectExtensions.DestroyObjects(objs);
        }
        #endregion
    }
}