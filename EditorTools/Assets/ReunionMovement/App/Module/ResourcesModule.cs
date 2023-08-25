using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.U2D;
using Unity.VisualScripting.Antlr3.Runtime.Tree;

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
        private double _initProgress = 0;
        public double InitProgress { get { return _initProgress; } }
        #endregion

        //缓存从Resource中加载的资源
        private Hashtable _resourceTable;

        public IEnumerator Init()
        {
            Log.Debug("ResourcesModule 初始化");

            _resourceTable = new Hashtable();

            yield return null;
            IsInited = true;
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
            if (_resourceTable.Contains(assetPath))
            {
                return _resourceTable[assetPath] as T;
            }
            var assets = Resources.Load<T>(assetPath);
            if (assets == null)
            {
                Log.Error(string.Format("资源没有找到,路径为:{0}", assetPath));
                return null;
            }
            if (isCache)
            {
                _resourceTable.Add(assetPath, assets);
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
            if (assets == null)
            {
                Log.Error(string.Format("资源没有找到,路径为:{0}", assetPath));
                return null;
            }
            if (isCache)
            {
                _resourceTable.Add(assetPath, assets);
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
            SpriteAtlas atlas = Resources.Load<SpriteAtlas>(atlasName);
            Sprite sprite = atlas.GetSprite(spriteName);

            if (atlas == null)
            {
                Log.Error("图集：" + atlasName + "不存在，请检查！");
            }

            if (sprite == null)
            {
                Log.Error(atlasName + " 图集中sprite" + spriteName + "不存在，请检查！");
            }

            return sprite;
        }

        public void Update(float elapseSeconds, float realElapseSeconds)
        {

        }
    }

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