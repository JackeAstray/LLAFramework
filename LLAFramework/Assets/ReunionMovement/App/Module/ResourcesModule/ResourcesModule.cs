using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.U2D;
using Object = UnityEngine.Object;

namespace GameLogic
{
    /// <summary>
    /// 资源管理器
    /// </summary>
    public class ResourcesModule : CustommModuleInitialize
    {
        #region 实例与初始化
        public static ResourcesModule Instance = new ResourcesModule();
        public bool IsInited { get; private set; }
        private double initProgress = 0;
        public double InitProgress { get { return initProgress; } }
        #endregion

        //缓存从Resource中加载的资源
        private Dictionary<string, Object> resourceTable = new Dictionary<string, Object>();

        public IEnumerator Init()
        {
            initProgress = 0;
            yield return null;
            initProgress = 100;
            IsInited = true;
            Log.Debug("ResourcesModule 初始化完成");
        }

        public void ClearData()
        {
            Log.Debug("ResourcesModule 清除数据");
        }

        #region 加载
        /// <summary>
        /// 同步加载Resources下资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetPath">路径</param>
        /// <param name="isCache">是否缓存</param>
        /// <returns></returns>
        public T Load<T>(string assetPath, bool isCache = true) where T : UnityEngine.Object
        {
            if (resourceTable.TryGetValue(assetPath, out var asset))
            {
                return asset as T;
            }

            var assets = Resources.Load<T>(assetPath);
            if (assets is null)
            {
                Log.Error($"资源没有找到,路径为:{assetPath}");
                return null;
            }
            if (isCache && assets is null)
            {
                resourceTable.Add(assetPath, assets);
            }
            return assets;
        }

        /// <summary>
        /// 异步加载Resources下资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetPath"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public async Task<T> LoadAsync<T>(string assetPath, bool isCache = true, UnityAction callback = null) where T : UnityEngine.Object
        {
            var assets = await ResourcesExtensions.LoadAsync<T>(assetPath, callback);
            if (assets is null)
            {
                Log.Error($"资源没有找到,路径为:{assetPath}");
                return null;
            }

            if (isCache && assets is null)
            {
                resourceTable.Add(assetPath, assets);
            }
            return assets;
        }
        #endregion

        /// <summary>
        /// 从图集加载精灵
        /// </summary>
        /// <param name="atlasName">图集路径名称</param>
        /// <param name="spriteName">精灵路径名称 </param>
        /// <returns></returns>
        public Sprite GetAtlasSprite(string atlasName, string spriteName)
        {
            var atlas = Resources.Load<SpriteAtlas>(atlasName);
            Sprite sprite = atlas.GetSprite(spriteName);

            if (atlas is null)
            {
                Log.Error($"图集：{nameof(atlasName)}不存在，请检查！");
            }

            if (sprite is null)
            {
                Log.Error($"{atlasName} 图集中Sprite:" + spriteName + " 不存在，请检查！");
            }

            return sprite;
        }

        /// <summary>
        /// 实例化资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public T InstantiateAsset<T>(string path) where T : Object
        {
            var obj = Load<T>(path);
            var go = GameObject.Instantiate<T>(obj);
            if (go == null)
                Log.Error(string.Format("实例化 {0} 失败!", obj));
            return go;
        }

        /// <summary>
        /// 移除单个数据缓存
        /// </summary>
        /// <param name="path"></param>
        public void DeleteAssetCache(string path)
        {
            if (resourceTable.ContainsKey(path))
            {
                resourceTable.Remove(path);
            }
        }

        /// <summary>
        /// 清除资源缓存
        /// </summary>
        public void ClearAssetsCache()
        {
            foreach (KeyValuePair<string, Object> item in resourceTable)
            {
                if (item.Value is Object obj)
                {
#if UNITY_EDITOR
                    Object.DestroyImmediate(obj, true);
#else
                    Object.Destroy(obj);
#endif
                }
            }
            resourceTable.Clear();
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        public void UpdateTime(float elapseSeconds, float realElapseSeconds)
        {

        }
    }

    /// <summary>
    /// 资源请求等待器
    /// </summary>
    public class ResourceRequestAwaiter : INotifyCompletion
    {
        public Action Continuation;
        public ResourceRequest resourceRequest;
        public bool IsCompleted => resourceRequest.isDone;
        public ResourceRequestAwaiter(ResourceRequest resourceRequest)
        {
            this.resourceRequest = resourceRequest;

            //注册完成时的回调
            this.resourceRequest.completed += Accomplish;
        }

        //awati 后面的代码包装成 continuation ，保存在类中方便完成是调用
        public void OnCompleted(Action continuation) => this.Continuation = continuation;

        public void Accomplish(AsyncOperation asyncOperation) => Continuation?.Invoke();

        public void GetResult() { }
    }
}