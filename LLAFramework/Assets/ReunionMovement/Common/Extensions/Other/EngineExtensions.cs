using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Linq;
using System.Globalization;
using UnityEngine.Networking;

namespace GameLogic
{
    /// <summary>
    /// 引擎扩展
    /// </summary>
    public static class EngineExtensions
    {
        #region Application
        public static bool IsDebug()
        {
            if (Application.isEditor)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 网络可用
        /// </summary>
        public static bool NetAvailable
        {
            get
            {
                return Application.internetReachability != NetworkReachability.NotReachable;
            }
        }

        /// <summary>
        /// 是否是无线
        /// </summary>
        public static bool IsWifi
        {
            get
            {
                return Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork;
            }
        }


        /// <summary>
        /// 退出
        /// </summary>
        public static void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
        #endregion

        #region object转换
        /// <summary>
        /// 从对象数组中获取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="openArgs"></param>
        /// <param name="offset"></param>
        /// <param name="isLog"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static T Get<T>(this object[] openArgs, int offset, bool isLog = true)
        {
            if (offset < 0 || offset >= openArgs.Length)
            {
                if (isLog)
                {
                    Log.Error($"[获取错误<object[]>],  越界: {offset}  {openArgs.Length}");
                }
                return default(T);
            }

            var arrElement = openArgs[offset];
            if (arrElement == null)
            {
                return default(T);
            }

            try
            {
                return (T)arrElement;
            }
            catch (InvalidCastException)
            {
                try
                {
                    return (T)Convert.ChangeType(arrElement, typeof(T));
                }
                catch (Exception ex)
                {
                    if (isLog)
                    {
                        Log.Error($"[获取错误<object[]>],  '{arrElement}' 无法转换为类型<{typeof(T)}>: {ex}");
                    }
                    return default(T);
                }
            }
        }
        /// <summary>
        /// object转int32
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int ObjToInt32(this object obj)
        {
            if (obj is int i)
            {
                return i;
            }

            try
            {
                return Convert.ToInt32(obj);
            }
            catch (Exception ex)
            {
                Log.Error("ToInt32 : " + ex);
                return 0;
            }
        }

        /// <summary>
        /// object转int64 | long
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static long ObjToInt64(this object obj)
        {
            if (obj is long l)
            {
                return l;
            }

            try
            {
                return Convert.ToInt64(obj);
            }
            catch (Exception ex)
            {
                Log.Error("ToInt64 : " + ex);
                return 0;
            }
        }

        /// <summary>
        /// object转float
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static float ObjToFloat(this object obj)
        {
            if (obj is float f)
            {
                return f;
            }

            try
            {
                return (float)Math.Round(Convert.ToSingle(obj), 2);
            }
            catch (Exception ex)
            {
                Log.Error("object转float失败 : " + ex);
                return 0;
            }
        }

        /// <summary>
        /// object转string
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ObjToString(this object obj)
        {
            if (obj is string s)
            {
                return s;
            }

            try
            {
                return Convert.ToString(obj);
            }
            catch (Exception ex)
            {
                Log.Error("object转string失败 : " + ex);
                return null;
            }
        }
        #endregion

        #region 随机
        /// <summary>
        /// 随机数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static T Random<T>(T min, T max) where T : IComparable<T>
        {
            if (typeof(T) == typeof(int))
            {
                int imin = Convert.ToInt32(min);
                int imax = Convert.ToInt32(max);
                return (T)(object)UnityEngine.Random.Range(imin, imax + 1);
            }
            else if (typeof(T) == typeof(float))
            {
                float fmin = Convert.ToSingle(min);
                float fmax = Convert.ToSingle(max);
                return (T)(object)UnityEngine.Random.Range(fmin, fmax);
            }
            else
            {
                throw new ArgumentException("不支持的类型");
            }
        }

        /// <summary>
        /// 从一个List中随机获取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T GetRandomItemFromList<T>(IList<T> list)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            if (list.Count == 0)
            {
                return default(T);
            }

            return list[UnityEngine.Random.Range(0, list.Count)];
        }
        #endregion

        #region 波浪数
        /// <summary>
        /// 波浪随机数整数版
        /// </summary>
        /// <param name="waveNumberStr"></param>
        /// <returns></returns>
        public static int GetWaveRandomNumberInt(string waveNumberStr)
        {
            FromToNumber from = ParseMinMaxNumber(waveNumberStr);
            return (int)UnityEngine.Random.Range(from.From, from.To + 1);
        }

        /// <summary>
        /// 获取波浪随机数,   即填“1”或填“1~2”这样的字符串中返回一个数！
        /// 如填"1"，直接返回1
        /// 如果填"1~10"这样的，那么随机返回1~10中间一个数
        /// </summary>
        /// <param name="waveNumberStr"></param>
        /// <returns></returns>
        public static float GetWaveRandomNumber(string waveNumberStr)
        {
            FromToNumber from = ParseMinMaxNumber(waveNumberStr);
            return UnityEngine.Random.Range(from.From, from.To);
        }

        public struct FromToNumber
        {
            public float From;
            public float To;
        }

        /// <summary>
        /// 获取波浪随机数的最大最小
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static FromToNumber ParseMinMaxNumber(string str)
        {
            var strs = str.Split('-', '~');
            var number = new FromToNumber();
            if (strs.Length > 0)
            {
                number.From = strs[0].ToInt32();
            }
            if (strs.Length > 1)
            {
                number.To = strs[1].ToInt32();
            }
            return number;
        }

        /// <summary>
        /// 是否在波浪数之间
        /// </summary>
        /// <param name="waveNumberStr"></param>
        /// <param name="testNumber"></param>
        /// <returns></returns>
        public static bool IsBetweenWave(string waveNumberStr, int testNumber)
        {
            FromToNumber from = ParseMinMaxNumber(waveNumberStr);
            return testNumber >= from.From && testNumber <= from.To;
        }
        #endregion

        #region 时间

        #endregion

        #region 分辨率
        /// <summary>
        /// 设置屏幕分辨率
        /// </summary>
        /// <param name="width">屏幕宽度</param>
        /// <param name="height">屏幕高度</param>
        /// <param name="fullScreen">是否全屏显示</param>
        public static void SetScreen(int width, int height, bool fullScreen)
        {
            Screen.SetResolution(width, height, fullScreen);
        }
        #endregion

        #region 文件操作
        /// <summary>
        /// 无视锁文件，直接读bytes  读取（加载）数据
        /// </summary>
        /// <param name="resPath"></param>
        /// <returns></returns>
        public static byte[] ReadAllBytes(string resPath)
        {
            using (FileStream fs = File.Open(resPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, (int)fs.Length);
                return bytes;
            }
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="fullpath">完整路径</param>
        /// <param name="content">内容</param>
        /// <returns></returns>
        public static async Task SaveFile(string fullpath, string content)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(content);
            await SaveFileAsync(fullpath, buffer);
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="fullpath"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static async Task<int> SaveFileAsync(string fullpath, byte[] content)
        {
            try
            {
                return await Task.Run(() =>
                {
                    if (content == null)
                    {
                        content = new byte[0];
                    }

                    string dir = Path.GetDirectoryName(fullpath);

                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }

                    File.WriteAllBytes(fullpath, content);
                    return content.Length;
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex + " SaveFile");
                throw;
            }
        }

        /// <summary>
        /// 加载Json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static T LoadJson<T>(string fileName)
        {
            string fileAbslutePath = Path.Combine(Application.persistentDataPath, "Json", fileName + ".json");
            if (File.Exists(fileAbslutePath))
            {
                string tempStr = File.ReadAllText(fileAbslutePath);
                return JsonMapper.ToObject<T>(tempStr);
            }

            return default(T);
        }

        /// <summary>
        /// 保存Json
        /// </summary>
        /// <param name="jsonStr"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static async Task SaveJson(string jsonStr, string fileName)
        {
            string filePath = Path.Combine(Application.persistentDataPath, "Json");
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            string fileAbslutePath = Path.Combine(filePath, fileName + ".json");

            await File.WriteAllTextAsync(fileAbslutePath, jsonStr);
        }

        /// <summary>
        /// 游戏开始把StreamingAssets文件复制到持久化目录
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static IEnumerator CopyFileToTarget(string filePath, string fileName)
        {
            string OriginalPath = Application.streamingAssetsPath + "/" + filePath + "/" + fileName;
            string TargetPath = Application.persistentDataPath + "/" + filePath;

            if (!Directory.Exists(TargetPath))
            {
                //创建文件夹
                Directory.CreateDirectory(TargetPath);
            }

            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    using (UnityWebRequest www = UnityWebRequest.Get(OriginalPath))
                    {
                        yield return www.SendWebRequest();
                        if (www.result != UnityWebRequest.Result.Success)
                        {
                            Debug.Log("复制文件失败：" + www.error);
                        }
                        else
                        {
                            //Debug.Log("复制成功");
                            File.WriteAllBytes(TargetPath + "/" + fileName, www.downloadHandler.data);
                        }
                    }
                    break;
                case RuntimePlatform.IPhonePlayer:
                    //IOS下StreamingAssets目录
                    OriginalPath = Application.dataPath + "/Raw/" + filePath + "/" + fileName;
                    if (!File.Exists(TargetPath + "/" + fileName))
                    {
                        //保存到持久化目录
                        File.Copy(OriginalPath, TargetPath + "/" + fileName);
                    }
                    break;
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.OSXPlayer:
                    if (!File.Exists(TargetPath + "/" + fileName))
                    {
                        //保存到持久化目录
                        File.Copy(OriginalPath, TargetPath + "/" + fileName);
                    }
                    break;
            }
            yield return null;
        }
        #endregion

        #region 获取硬件信息
        /// <summary>
        /// 获取本机IP
        /// </summary>
        /// <returns></returns>
        public static string GetLocalIP()
        {
            try
            {
                //获取本机名
                string hostName = Dns.GetHostName();
                //获取本机IP
                IPHostEntry ipEntry = Dns.GetHostEntry(hostName);
                var ipAddress = ipEntry.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
                return ipAddress?.ToString() ?? "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 异步获取IP地址
        /// </summary>
        /// <param name="host"></param>
        /// <param name="callback"></param>
        public static async Task<IPAddress> GetIpAddress(string host, Action<IPAddress> callback = null)
        {
            if (IPAddress.TryParse(host, out IPAddress ipAddress))
            {
                return ipAddress;
            }
            else
            {
                var addresses = await Dns.GetHostAddressesAsync(host);
                return addresses.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
            }
        }
        #endregion

        #region Texture2D
        /// <summary>
        /// texture 转换成 texture2d
        /// </summary>
        /// <param name="texture"></param>
        /// <returns></returns>
        public static Texture2D TextureToTexture2D(Texture texture)
        {
            Texture2D texture2D = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
            RenderTexture currentRT = RenderTexture.active;
            RenderTexture renderTexture = RenderTexture.GetTemporary(texture.width, texture.height, 32);
            Graphics.Blit(texture, renderTexture);

            RenderTexture.active = renderTexture;
            texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            texture2D.Apply();

            RenderTexture.active = currentRT;
            RenderTexture.ReleaseTemporary(renderTexture);

            return texture2D;
        }

        /// <summary>
        /// 解除texture锁
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Texture2D DuplicateTexture(this Texture2D source)
        {
            RenderTexture renderTex = RenderTexture.GetTemporary(
                        source.width,
                        source.height,
                        0,
                        RenderTextureFormat.Default,
                        RenderTextureReadWrite.Linear);

            Graphics.Blit(source, renderTex);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = renderTex;
            Texture2D readableText = new Texture2D(source.width, source.height);
            readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
            readableText.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(renderTex);

            return readableText;
        }

        /// <summary>
        /// 裁剪正方形
        /// </summary>
        /// <param name="texture"></param>
        /// <returns></returns>
        public static Texture2D CutForSquare(this Texture2D texture)
        {
            Texture2D tex;
            int TextureWidth = texture.width;//图片的宽
            int TextureHeight = texture.height;//图片的高

            int TextureSide = Mathf.Min(TextureWidth, TextureHeight);
            tex = new Texture2D(TextureSide, TextureSide);
            UnityEngine.Color[] col = texture.GetPixels((TextureWidth - TextureSide) / 2, (TextureHeight - TextureSide) / 2, TextureSide, TextureSide);
            tex.SetPixels(0, 0, TextureSide, TextureSide, col);
            tex.Apply();
            return tex;
        }

        /// <summary>
        /// 正方型裁剪
        /// 以图片中心为轴心，截取正方型，然后等比缩放
        /// 用于头像处理
        /// </summary>
        /// <param name="texture">要处理的图片</param>
        /// <param name="side_x">指定的边长</param>
        /// <param name="side_y">指定的边宽</param>
        /// <returns></returns>
        public static Texture2D CutForSquare(this Texture2D texture, int side_x, int side_y)
        {
            Texture2D tex;
            int TextureWidth = texture.width;//图片的宽
            int TextureHeight = texture.height;//图片的高

            //如果图片的高和宽都比side大
            if (TextureWidth > side_x && TextureHeight > side_y)
            {
                tex = new Texture2D(side_x, side_y);
                UnityEngine.Color[] col = texture.GetPixels((TextureWidth - side_x) / 2, (TextureWidth - side_y) / 2, side_x, side_y);
                tex.SetPixels(0, 0, side_x, side_y, col);
                tex.Apply();
                return tex;
            }
            //如果图片的宽或高小于side
            if (TextureWidth < side_x || TextureHeight < side_y)
            {
                int TextureSide = Mathf.Min(TextureWidth, TextureHeight);
                tex = new Texture2D(TextureSide, TextureSide);
                UnityEngine.Color[] col = texture.GetPixels((TextureWidth - TextureSide) / 2, (TextureHeight - TextureSide) / 2, TextureSide, TextureSide);
                tex.SetPixels(0, 0, TextureSide, TextureSide, col);
                tex.Apply();
                return tex;
            }
            return null;
        }

        /// <summary>
        /// byte[]转换为Texture2D
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Texture2D BytesToTexture2D(this byte[] bytes, int width, int height)
        {
            Texture2D texture2D = new Texture2D(width, height);
            texture2D.LoadImage(bytes);
            return texture2D;
        }

        /// <summary>
        /// 双线性插值法缩放图片，等比缩放 
        /// </summary>
        public static Texture2D ScaleTextureBilinear(this Texture2D originalTexture, float scaleFactor)
        {
            Texture2D newTexture = new Texture2D(Mathf.CeilToInt(originalTexture.width * scaleFactor),
                Mathf.CeilToInt(originalTexture.height * scaleFactor));
            float scale = 1.0f / scaleFactor;
            int maxX = originalTexture.width - 1;
            int maxY = originalTexture.height - 1;
            for (int y = 0; y < newTexture.height; y++)
            {
                for (int x = 0; x < newTexture.width; x++)
                {
                    float targetX = x * scale;
                    float targetY = y * scale;
                    int x1 = Mathf.Min(maxX, Mathf.FloorToInt(targetX));
                    int y1 = Mathf.Min(maxY, Mathf.FloorToInt(targetY));
                    int x2 = Mathf.Min(maxX, x1 + 1);
                    int y2 = Mathf.Min(maxY, y1 + 1);

                    float u = targetX - x1;
                    float v = targetY - y1;
                    float w1 = (1 - u) * (1 - v);
                    float w2 = u * (1 - v);
                    float w3 = (1 - u) * v;
                    float w4 = u * v;
                    Color color1 = originalTexture.GetPixel(x1, y1);
                    Color color2 = originalTexture.GetPixel(x2, y1);
                    Color color3 = originalTexture.GetPixel(x1, y2);
                    Color color4 = originalTexture.GetPixel(x2, y2);
                    Color color = new Color(
                        Mathf.Clamp01(color1.r * w1 + color2.r * w2 + color3.r * w3 + color4.r * w4),
                        Mathf.Clamp01(color1.g * w1 + color2.g * w2 + color3.g * w3 + color4.g * w4),
                        Mathf.Clamp01(color1.b * w1 + color2.b * w2 + color3.b * w3 + color4.b * w4),
                        Mathf.Clamp01(color1.a * w1 + color2.a * w2 + color3.a * w3 + color4.a * w4)
                    );
                    newTexture.SetPixel(x, y, color);
                }
            }

            newTexture.Apply();
            return newTexture;
        }

        /// <summary> 
        /// 双线性插值法缩放图片为指定尺寸 
        /// </summary>
        public static Texture2D SizeTextureBilinear(this Texture2D originalTexture, Vector2 size)
        {
            Texture2D newTexture = new Texture2D(Mathf.CeilToInt(size.x), Mathf.CeilToInt(size.y));
            float scaleX = originalTexture.width / size.x;
            float scaleY = originalTexture.height / size.y;
            int maxX = originalTexture.width - 1;
            int maxY = originalTexture.height - 1;
            for (int y = 0; y < newTexture.height; y++)
            {
                for (int x = 0; x < newTexture.width; x++)
                {
                    float targetX = x * scaleX;
                    float targetY = y * scaleY;
                    int x1 = Mathf.Min(maxX, Mathf.FloorToInt(targetX));
                    int y1 = Mathf.Min(maxY, Mathf.FloorToInt(targetY));
                    int x2 = Mathf.Min(maxX, x1 + 1);
                    int y2 = Mathf.Min(maxY, y1 + 1);

                    float u = targetX - x1;
                    float v = targetY - y1;
                    float w1 = (1 - u) * (1 - v);
                    float w2 = u * (1 - v);
                    float w3 = (1 - u) * v;
                    float w4 = u * v;
                    Color color1 = originalTexture.GetPixel(x1, y1);
                    Color color2 = originalTexture.GetPixel(x2, y1);
                    Color color3 = originalTexture.GetPixel(x1, y2);
                    Color color4 = originalTexture.GetPixel(x2, y2);
                    Color color = new Color(
                        Mathf.Clamp01(color1.r * w1 + color2.r * w2 + color3.r * w3 + color4.r * w4),
                        Mathf.Clamp01(color1.g * w1 + color2.g * w2 + color3.g * w3 + color4.g * w4),
                        Mathf.Clamp01(color1.b * w1 + color2.b * w2 + color3.b * w3 + color4.b * w4),
                        Mathf.Clamp01(color1.a * w1 + color2.a * w2 + color3.a * w3 + color4.a * w4)
                    );
                    newTexture.SetPixel(x, y, color);
                }
            }

            newTexture.Apply();
            return newTexture;
        }

        /// <summary> 
        /// Texture旋转
        /// </summary>
        public static Texture2D RotateTexture(this Texture2D texture, float eulerAngles)
        {
            int x;
            int y;
            int i;
            int j;
            float phi = eulerAngles / (180 / Mathf.PI);
            float sn = Mathf.Sin(phi);
            float cs = Mathf.Cos(phi);
            Color32[] arr = texture.GetPixels32();
            Color32[] arr2 = new Color32[arr.Length];
            int W = texture.width;
            int H = texture.height;
            int xc = W / 2;
            int yc = H / 2;

            for (j = 0; j < H; j++)
            {
                for (i = 0; i < W; i++)
                {
                    arr2[j * W + i] = new Color32(0, 0, 0, 0);

                    x = (int)(cs * (i - xc) + sn * (j - yc) + xc);
                    y = (int)(-sn * (i - xc) + cs * (j - yc) + yc);

                    if ((x > -1) && (x < W) && (y > -1) && (y < H))
                    {
                        arr2[j * W + i] = arr[y * W + x];
                    }
                }
            }

            Texture2D newImg = new Texture2D(W, H);
            newImg.SetPixels32(arr2);
            newImg.Apply();

            return newImg;
        }
        #endregion
    }
}