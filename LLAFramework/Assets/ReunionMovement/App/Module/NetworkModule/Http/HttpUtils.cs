using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameLogic.Http
{
    /// <summary>
    /// http的实用程序类
    /// <see cref="https://developer.mozilla.org/en-US/docs/Web/HTTP/Status"/>
    /// </summary>
    public static class HttpUtils
	{
        /// <summary>
        /// 构造带有参数的URI
        /// </summary>
        /// <param name="uri">要附加参数的URI</param>
        /// <param name="parameters">要附加的参数字典</param>
        /// <returns>带有附加参数的URI</returns>
        public static string ConstructUriWithParameters(string uri, Dictionary<string, string> parameters)
        {
            // 如果参数为空或者数量为0，直接返回原始URI
            if (parameters == null || parameters.Count == 0)
            {
                return uri;
            }

            // 使用StringBuilder来构造新的URI
            var stringBuilder = new StringBuilder(uri);

            // 使用string.Join方法和LINQ来连接参数
            stringBuilder.Append("?");
            stringBuilder.Append(string.Join("&", parameters.Select(kvp => $"{kvp.Key}={kvp.Value}")));

            // 返回构造好的URI
            return stringBuilder.ToString();
        }
    }
}
