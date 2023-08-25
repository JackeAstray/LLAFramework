using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace GameLogic
{
    /// <summary>
    /// 资源扩展
    /// </summary>
    public static class ResourcesExtensions
    {
        public static ResourceRequestAwaiter GetAwaiter(this ResourceRequest request) => new ResourceRequestAwaiter(request);

        public static async Task<T> LoadAsync<T>(string assetPath, UnityAction callback) where T : UnityEngine.Object
        {
            var request = Resources.LoadAsync<T>(assetPath);

            await request;

            if (callback != null)
            {
                callback.Invoke();
            }

            if (request.asset != null)
            {
                return request.asset as T;
            }
            else
            {
                return null;
            }
        }
    }
}