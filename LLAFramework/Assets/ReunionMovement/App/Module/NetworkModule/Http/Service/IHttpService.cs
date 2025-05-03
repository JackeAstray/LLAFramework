using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace LLAFramework.Http.Service
{
    /// <summary>
    /// 服务
    /// </summary>
    public interface IHttpService
    {
        /// <summary>
        ///  创建一个配置为HTTP GET的HttpRequest
        /// </summary>
        /// <param name="uri">通过HTTP GET检索资源的URI</param>
        /// <returns>配置为从uri检索数据的HttpRequest对象</returns>
        IHttpRequest Get(string uri);

        /// <summary>
        /// 创建一个配置为HTTP GET的HttpRequest
        /// </summary>
        /// <param name="uri">通过HTTP GET检索资源的URI</param>
        /// <returns>配置为从uri检索数据的HttpRequest对象</returns>
        IHttpRequest GetTexture(string uri);

        /// <summary>
        /// 创建一个配置为通过HTTP POST向服务器发送表单数据的HttpRequest
        /// </summary>
        /// <param name="uri">将表单数据传输到的目标URI</param>
        /// <param name="postData">表单主体数据将在传输之前通过WWWTranscoder.URLEncode进行URL编码</param>
        /// <returns>配置为通过POST将表单数据发送到uri的HttpRequest</returns>
        IHttpRequest Post(string uri, string postData);

        /// <summary>
        /// 创建一个配置为通过HTTP POST向服务器发送表单数据的HttpRequest
        /// </summary>
        /// <param name="uri">将表单数据传输到的目标URI</param>
        /// <param name="formData">以WWWForm对象封装的表单字段或文件，用于格式化和传输到远程服务器</param>
        /// <returns>配置为通过POST将表单数据发送到uri的HttpRequest</returns>
        IHttpRequest Post(string uri, WWWForm formData);

        /// <summary>
        /// 创建一个配置为通过HTTP POST向服务器发送表单数据的HttpRequest
        /// </summary>
        /// <param name="uri">将表单数据传输到的目标URI</param>
        /// <param name="formData">以键值对形式的表单字段，用于格式化和传输到远程服务器</param>
        /// <returns>配置为通过POST将表单数据发送到uri的HttpRequest</returns>
        IHttpRequest Post(string uri, Dictionary<string, string> formData);

        /// <summary>
        /// 创建一个配置为通过HTTP POST向服务器发送多部分表单数据的HttpRequest
        /// </summary>
        /// <param name="uri">将表单数据传输到的目标URI</param>
        /// <param name="multipartForm">用于格式化和传输到远程服务器的MultipartForm数据</param>
        /// <returns>配置为通过POST将表单数据发送到uri的HttpRequest</returns>
        IHttpRequest Post(string uri, List<IMultipartFormSection> multipartForm);

        /// <summary>
        /// 创建一个配置为通过HTTP POST向服务器发送原始字节的HttpRequest
        /// </summary>
        /// <param name="uri">将字节发送到的目标URI</param>
        /// <param name="bytes">字节数组数据</param>
        /// <param name="contentType">表示数据的MIME类型的字符串（例如image/jpeg）</param>
        /// <returns>配置为通过POST将原始字节发送到服务器的HttpRequest</returns>
        IHttpRequest Post(string uri, byte[] bytes, string contentType);

        /// <summary>
        /// 创建一个配置为通过HTTP POST向服务器发送JSON数据的HttpRequest
        /// </summary>
        /// <param name="uri">将JSON数据发送到的目标URI</param>
        /// <param name="json">JSON主体数据</param>
        /// <returns>配置为通过POST将JSON数据发送到uri的HttpRequest</returns>
        IHttpRequest PostJson(string uri, string json);

        /// <summary>
        /// 创建一个配置为通过HTTP POST向服务器发送JSON数据的HttpRequest
        /// </summary>
        /// <param name="uri">将JSON数据发送到的目标URI</param>
        /// <param name="payload">要解析为JSON数据的对象</param>
        /// <returns>配置为通过POST将JSON数据发送到uri的HttpRequest</returns>
        IHttpRequest PostJson<T>(string uri, T payload) where T : class;

        /// <summary>
        /// 创建一个配置为通过HTTP PUT向远程服务器上传原始数据的HttpRequest
        /// </summary>
        /// <param name="uri">将数据发送到的URI</param>
        /// <param name="bodyData">要传输到远程服务器的数据</param>
        /// <returns>配置为通过HTTP PUT将bodyData传输到uri的HttpRequest</returns>
        IHttpRequest Put(string uri, byte[] bodyData);

        /// <summary>
        /// 创建一个配置为通过HTTP PUT向远程服务器上传原始数据的HttpRequest
        /// </summary>
        /// <param name="uri">将数据发送到的URI</param>
        /// <param name="bodyData">要传输到远程服务器的数据
        /// 字符串将通过System.Text.Encoding.UTF8转换为原始字节</param>
        /// <returns>配置为通过HTTP PUT将bodyData传输到uri的HttpRequest</returns>
        IHttpRequest Put(string uri, string bodyData);

        /// <summary>
        /// 创建一个配置为HTTP DELETE的HttpRequest
        /// </summary>
        /// <param name="uri">应发送DELETE请求的URI</param>
        /// <returns>配置为发送HTTP DELETE请求的HttpRequest</returns>
        IHttpRequest Delete(string uri);

        /// <summary>
        /// 创建一个配置为发送HTTP HEAD请求的HttpRequest
        /// </summary>
        /// <param name="uri">要发送HTTP HEAD请求的URI</param>
        /// <returns>配置为发送HTTP HEAD请求的HttpRequest</returns>
        IHttpRequest Head(string uri);

        IEnumerator Send(IHttpRequest request, Action<HttpResponse> onSuccess = null, Action<HttpResponse> onError = null, Action<HttpResponse> onNetworkError = null);

        void Abort(IHttpRequest request);
    }
}
