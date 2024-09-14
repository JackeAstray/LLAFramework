using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace GameLogic.Http.Service.Unity
{
	public class UnityHttpService : IHttpService
	{
		public IHttpRequest Get(string uri)
		{
			return new UnityHttpRequest(UnityWebRequest.Get(uri));
		}

		public IHttpRequest GetTexture(string uri)
		{
			return new UnityHttpRequest(UnityWebRequestTexture.GetTexture(uri));
		}

		public IHttpRequest Post(string uri, string postData)
		{
			return new UnityHttpRequest(UnityWebRequest.PostWwwForm(uri, postData));
		}

		public IHttpRequest Post(string uri, WWWForm formData)
		{
			return new UnityHttpRequest(UnityWebRequest.Post(uri, formData));
		}

		public IHttpRequest Post(string uri, Dictionary<string, string> formData)
		{
			return new UnityHttpRequest(UnityWebRequest.Post(uri, formData));
		}

		public IHttpRequest Post(string uri, List<IMultipartFormSection> multipartForm)
		{
			return new UnityHttpRequest(UnityWebRequest.Post(uri, multipartForm));
		}

		public IHttpRequest Post(string uri, byte[] bytes, string contentType)
		{
            UnityWebRequest unityWebRequest = new UnityWebRequest(uri, UnityWebRequest.kHttpVerbPOST);
            unityWebRequest.uploadHandler = new UploadHandlerRaw(bytes);
            unityWebRequest.downloadHandler = new DownloadHandlerBuffer();
            unityWebRequest.SetRequestHeader("Content-Type", contentType);
            return new UnityHttpRequest(unityWebRequest);
		}

		public IHttpRequest PostJson(string uri, string json)
		{
			return Post(uri, Encoding.UTF8.GetBytes(json), "application/json");
		}

		public IHttpRequest PostJson<T>(string uri, T payload) where T : class
		{
            var json = JsonMapper.ToJson(payload);
            return PostJson(uri, json);
		}

		public IHttpRequest Put(string uri, byte[] bodyData)
		{
			return new UnityHttpRequest(UnityWebRequest.Put(uri, bodyData));
		}

		public IHttpRequest Put(string uri, string bodyData)
		{
			return new UnityHttpRequest(UnityWebRequest.Put(uri, bodyData));
		}

		public IHttpRequest Delete(string uri)
		{
			return new UnityHttpRequest(UnityWebRequest.Delete(uri));
		}

		public IHttpRequest Head(string uri)
		{
			return new UnityHttpRequest(UnityWebRequest.Head(uri));
		}

		public IEnumerator Send(IHttpRequest request, Action<HttpResponse> onSuccess = null,
			Action<HttpResponse> onError = null, Action<HttpResponse> onNetworkError = null)
		{
            var unityHttpRequest = (UnityHttpRequest)request;
            using (UnityWebRequest unityWebRequest = unityHttpRequest.UnityWebRequest)
            {
                yield return unityWebRequest.SendWebRequest();

                var response = new HttpResponse
                {
                    Url = unityWebRequest.url,
                    Bytes = unityWebRequest.downloadHandler?.data,
                    Text = unityWebRequest.downloadHandler?.text,
                    IsSuccessful = unityWebRequest.result != UnityWebRequest.Result.ConnectionError && unityWebRequest.result != UnityWebRequest.Result.ProtocolError,
                    IsHttpError = unityWebRequest.result == UnityWebRequest.Result.ProtocolError,
                    IsNetworkError = unityWebRequest.result == UnityWebRequest.Result.ConnectionError,
                    Error = unityWebRequest.error,
                    StatusCode = unityWebRequest.responseCode,
                    ResponseHeaders = unityWebRequest.GetResponseHeaders(),
                    Texture = (unityWebRequest.downloadHandler as DownloadHandlerTexture)?.texture
                };

                if (response.IsNetworkError) // 使用修正后的条件
                {
                    onNetworkError?.Invoke(response);
                }
                else if (response.IsHttpError) // 使用修正后的条件
                {
                    onError?.Invoke(response);
                }
                else if (response.IsSuccessful) // 检查请求是否成功
                {
                    onSuccess?.Invoke(response);
                }
            }
        }

		public void Abort(IHttpRequest request)
		{
            var unityHttpRequest = request as UnityHttpRequest;
			if (unityHttpRequest?.UnityWebRequest != null && !unityHttpRequest.UnityWebRequest.isDone)
			{
				unityHttpRequest.UnityWebRequest.Abort();
			}
		}
	}
}