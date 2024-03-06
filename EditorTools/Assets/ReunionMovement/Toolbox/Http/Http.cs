using System;
using System.Collections;
using System.Collections.Generic;
using GameLogic.HttpModule.Service;
using GameLogic.HttpModule.Service.Unity;
using UnityEngine;
using UnityEngine.Networking;

namespace GameLogic.HttpModule
{
    public sealed class Http : MonoBehaviour
    {
        public static Http Instance
        {
            get
            {
                if (instance != null) return instance;
                Init(new UnityHttpService());
                return instance;
            }
        }

        private static Http instance;

        private IHttpService service;
        private Dictionary<string, string> superHeaders;
        private Dictionary<IHttpRequest, Coroutine> httpRequests;

        /// <summary>
        /// 初始化Http
        /// </summary>
        /// <param name="service"></param>
        public static void Init(IHttpService service)
        {
            if (instance) return;

            instance = new GameObject(typeof(Http).Name).AddComponent<Http>();
            instance.gameObject.hideFlags = HideFlags.HideInHierarchy;
            instance.superHeaders = new Dictionary<string, string>();
            instance.httpRequests = new Dictionary<IHttpRequest, Coroutine>();
            instance.service = service;
            DontDestroyOnLoad(instance.gameObject);
        }

        #region Super Headers

        /// <summary>
        /// SuperHeaders是键值对，将被添加到每个后续的HttpRequest中。
        /// </summary>
        /// <returns>A dictionary of super-headers.</returns>
        public static Dictionary<string, string> GetSuperHeaders()
        {
            return new Dictionary<string, string>(Instance.superHeaders);
        }

        /// <summary>
        /// 将标头设置为SuperHeaders键值对，如果标头键已存在，则该值将被替换。
        /// </summary>
        /// <param name="key">要设置的标题键</param>
        /// <param name="value">要分配的标头值</param>
        public static void SetSuperHeader(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("密钥不能为null或为空");
            }

            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("值不能为null或空，如果要删除该值，请使用RemoveSuperHeader（）方法。");
            }

            Instance.superHeaders[key] = value;
        }

        /// <summary>
        /// 从“SuperHeaders”列表中删除标头
        /// </summary>
        /// <param name="key">要删除的标题键</param>
        /// <returns>如果元素移除成功</returns>
        public static bool RemoveSuperHeader(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("密钥不能为null或为空");
            }

            return Instance.superHeaders.Remove(key);
        }

        #endregion

        #region 静态请求

        /// <see cref="GameLogic.HttpModule.Service.IHttpService.Get"/>
        public static IHttpRequest Get(string uri)
        {
            return Instance.service.Get(uri);
        }

        /// <see cref="GameLogic.HttpModule.Service.IHttpService.GetTexture"/>
        public static IHttpRequest GetTexture(string uri)
        {
            return Instance.service.GetTexture(uri);
        }

        /// <see cref="GameLogic.HttpModule.Service.IHttpService.Post(string, string)"/>
        public static IHttpRequest Post(string uri, string postData)
        {
            return Instance.service.Post(uri, postData);
        }

        /// <see cref="GameLogic.HttpModule.Service.IHttpService.Post(string, WWWForm)"/>
        public static IHttpRequest Post(string uri, WWWForm formData)
        {
            return Instance.service.Post(uri, formData);
        }

        /// <see cref="GameLogic.HttpModule.Service.IHttpService.Post(string, Dictionary&lt;string, string&gt;)"/>
        public static IHttpRequest Post(string uri, Dictionary<string, string> formData)
        {
            return Instance.service.Post(uri, formData);
        }

        /// <see cref="GameLogic.HttpModule.Service.IHttpService.Post(string, List&lt;IMultipartFormSection&gt;)"/>
        public static IHttpRequest Post(string uri, List<IMultipartFormSection> multipartForm)
        {
            return Instance.service.Post(uri, multipartForm);
        }

        /// <see cref="GameLogic.HttpModule.Service.IHttpService.Post(string, byte[], string)"/>
        public static IHttpRequest Post(string uri, byte[] bytes, string contentType)
        {
            return Instance.service.Post(uri, bytes, contentType);
        }

        /// <see cref="GameLogic.HttpModule.Service.IHttpService.PostJson"/>
        public static IHttpRequest PostJson(string uri, string json)
        {
            return Instance.service.PostJson(uri, json);
        }

        /// <see cref="GameLogic.HttpModule.Service.IHttpService.PostJson{T}(string, T)"/>
        public static IHttpRequest PostJson<T>(string uri, T payload) where T : class
        {
            return Instance.service.PostJson(uri, payload);
        }

        /// <see cref="GameLogic.HttpModule.Service.IHttpService.Put(string, byte[])"/>
        public static IHttpRequest Put(string uri, byte[] bodyData)
        {
            return Instance.service.Put(uri, bodyData);
        }

        /// <see cref="GameLogic.HttpModule.Service.IHttpService.Put(string, string)"/>
        public static IHttpRequest Put(string uri, string bodyData)
        {
            return Instance.service.Put(uri, bodyData);
        }

        /// <see cref="GameLogic.HttpModule.Service.IHttpService.Delete"/>
        public static IHttpRequest Delete(string uri)
        {
            return Instance.service.Delete(uri);
        }

        /// <see cref="GameLogic.HttpModule.Service.IHttpService.Head"/>
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
        internal void Send(IHttpRequest request, Action<HttpResponse> onSuccess = null,
            Action<HttpResponse> onError = null, Action<HttpResponse> onNetworkError = null)
        {
            var enumerator = SendCoroutine(request, onSuccess, onError, onNetworkError);
            var coroutine = StartCoroutine(enumerator);
            httpRequests.Add(request, coroutine);
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
            Instance.httpRequests.Remove(request);
        }

        /// <summary>
        /// 中止请求并将其从活动请求列表中删除
        /// </summary>
        /// <param name="request"></param>
        internal void Abort(IHttpRequest request)
        {
            Instance.service.Abort(request);

            if (httpRequests.TryGetValue(request, out Coroutine coroutine))
            {
                StopCoroutine(coroutine);
                Instance.httpRequests.Remove(request);
            }
        }

        private void Update()
        {
            var keys = new List<IHttpRequest>(httpRequests.Keys);
            foreach (var httpRequest in keys)
            {
                (httpRequest as IUpdateProgress)?.UpdateProgress();
            }
        }
    }
}
