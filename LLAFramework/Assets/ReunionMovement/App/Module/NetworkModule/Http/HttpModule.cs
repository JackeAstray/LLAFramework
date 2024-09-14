using GameLogic;
using GameLogic.Http.Service.Unity;
using GameLogic.Http.Service;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;

namespace GameLogic.Http
{
    public class HttpModule : CustommModuleInitialize
    {
        #region 实例与初始化
        public static HttpModule Instance = new HttpModule();
        public bool IsInited { get; private set; }
        private double initProgress = 0;
        public double InitProgress { get { return initProgress; } }
        #endregion

        #region 数据
        private IHttpService service;
        private IHttpRequest sendRequest;
        private Dictionary<string, string> superHeaders;
        private Dictionary<IHttpRequest, Coroutine> httpRequests;
        #endregion

        public IEnumerator Init()
        {
            initProgress = 0;
            yield return null;
            initProgress = 100;
            IsInited = true;

            Log.Debug("HttpModule 初始化完成");

            Init(new UnityHttpService());
        }

        public void ClearData()
        {
            Log.Debug("HttpModule 清除数据");
        }

        /// <summary>
        /// 初始化Http
        /// </summary>
        /// <param name="service"></param>
        public void Init(IHttpService service)
        {
            superHeaders = new Dictionary<string, string>();
            httpRequests = new Dictionary<IHttpRequest, Coroutine>();
            this.service = service;
        }

        #region Super Headers

        /// <summary>
        /// SuperHeaders是键值对，将被添加到每个后续的HttpRequest中。
        /// </summary>
        /// <returns>A dictionary of super-headers.</returns>
        public Dictionary<string, string> GetSuperHeaders()
        {
            return new Dictionary<string, string>(superHeaders);
        }

        /// <summary>
        /// 将标头设置为SuperHeaders键值对，如果标头键已存在，则该值将被替换。
        /// </summary>
        /// <param name="key">要设置的标题键</param>
        /// <param name="value">要分配的标头值</param>
        public void SetSuperHeader(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("密钥不能为null或为空");
            }

            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("值不能为null或空，如果要删除该值，请使用RemoveSuperHeader（）方法。");
            }

            superHeaders[key] = value;
        }

        /// <summary>
        /// 从“SuperHeaders”列表中删除标头
        /// </summary>
        /// <param name="key">要删除的标题键</param>
        /// <returns>如果元素移除成功</returns>
        public bool RemoveSuperHeader(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("密钥不能为null或为空");
            }

            return superHeaders.Remove(key);
        }

        #endregion

        #region 静态请求

        /// <see cref="GameLogic.Http.Service.IHttpService.Get"/>
        public static IHttpRequest Get(string uri)
        {
            return Instance.service.Get(uri);
        }

        /// <see cref="GameLogic.Http.Service.IHttpService.GetTexture"/>
        public static IHttpRequest GetTexture(string uri)
        {
            return Instance.service.GetTexture(uri);
        }

        /// <see cref="GameLogic.Http.Service.IHttpService.Post(string, string)"/>
        public static IHttpRequest Post(string uri, string postData)
        {
            return Instance.service.Post(uri, postData);
        }

        /// <see cref="GameLogic.Http.Service.IHttpService.Post(string, WWWForm)"/>
        public static IHttpRequest Post(string uri, WWWForm formData)
        {
            return Instance.service.Post(uri, formData);
        }

        /// <see cref="GameLogic.Http.Service.IHttpService.Post(string, Dictionary&lt;string, string&gt;)"/>
        public static IHttpRequest Post(string uri, Dictionary<string, string> formData)
        {
            return Instance.service.Post(uri, formData);
        }

        /// <see cref="GameLogic.Http.Service.IHttpService.Post(string, List&lt;IMultipartFormSection&gt;)"/>
        public static IHttpRequest Post(string uri, List<IMultipartFormSection> multipartForm)
        {
            return Instance.service.Post(uri, multipartForm);
        }

        /// <see cref="GameLogic.Http.Service.IHttpService.Post(string, byte[], string)"/>
        public static IHttpRequest Post(string uri, byte[] bytes, string contentType)
        {
            return Instance.service.Post(uri, bytes, contentType);
        }

        /// <see cref="GameLogic.Http.Service.IHttpService.PostJson"/>
        public static IHttpRequest PostJson(string uri, string json)
        {
            return Instance.service.PostJson(uri, json);
        }

        /// <see cref="GameLogic.Http.Service.IHttpService.PostJson{T}(string, T)"/>
        public static IHttpRequest PostJson<T>(string uri, T payload) where T : class
        {
            return Instance.service.PostJson(uri, payload);
        }

        /// <see cref="GameLogic.Http.Service.IHttpService.Put(string, byte[])"/>
        public static IHttpRequest Put(string uri, byte[] bodyData)
        {
            return Instance.service.Put(uri, bodyData);
        }

        /// <see cref="GameLogic.Http.Service.IHttpService.Put(string, string)"/>
        public static IHttpRequest Put(string uri, string bodyData)
        {
            return Instance.service.Put(uri, bodyData);
        }

        /// <see cref="GameLogic.Http.Service.IHttpService.Delete"/>
        public static IHttpRequest Delete(string uri)
        {
            return Instance.service.Delete(uri);
        }

        /// <see cref="GameLogic.Http.Service.IHttpService.Head"/>
        public static IHttpRequest Head(string uri)
        {
            return Instance.service.Head(uri);
        }

        #endregion
        /// <summary>
        /// 发送请求并处理响应
        /// </summary>
        /// <param name="request"></param>
        /// <param name="onSuccess"></param>
        /// <param name="onError"></param>
        /// <param name="onNetworkError"></param>
        internal void Send(IHttpRequest request,
            Action<HttpResponse> onSuccess = null,
            Action<HttpResponse> onError = null,
            Action<HttpResponse> onNetworkError = null
         )
        {
            sendRequest = request;
            CoroutinerMgr.Instance.AddRoutine(SendCoroutine(sendRequest, onSuccess, onError, onNetworkError), SendCallback);
        }

        private void SendCallback(Coroutine coroutine)
        {
            if (!httpRequests.ContainsKey(sendRequest))
            {
                httpRequests.Add(sendRequest, coroutine);
            }
        }

        /// <summary>
        /// 用于发送请求和处理响应的协程
        /// </summary>
        /// <param name="request"></param>
        /// <param name="onSuccess"></param>
        /// <param name="onError"></param>
        /// <param name="onNetworkError"></param>
        /// <returns></returns>
        private IEnumerator SendCoroutine(IHttpRequest request, Action<HttpResponse> onSuccess = null,
            Action<HttpResponse> onError = null, Action<HttpResponse> onNetworkError = null)
        {
            yield return service.Send(request, onSuccess, onError, onNetworkError);
            httpRequests.Remove(request);
        }

        /// <summary>
        /// 中止请求并将其从活动请求列表中删除
        /// </summary>
        /// <param name="request"></param>
        internal void Abort(IHttpRequest request)
        {
            service.Abort(request);

            if (httpRequests.TryGetValue(request, out Coroutine coroutine))
            {
                CoroutinerMgr.Instance.StopTargetRoutine(coroutine);
                httpRequests.Remove(request);
            }
        }
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="elapseSeconds"></param>
        /// <param name="realElapseSeconds"></param>
        public void UpdateTime(float elapseSeconds, float realElapseSeconds)
        {
            if (httpRequests != null && httpRequests.Count > 0)
            {
                var keys = new List<IHttpRequest>(httpRequests.Keys);
                foreach (var httpRequest in keys)
                {
                    (httpRequest as IUpdateProgress)?.UpdateProgress();
                }
            }
        }
    }
}