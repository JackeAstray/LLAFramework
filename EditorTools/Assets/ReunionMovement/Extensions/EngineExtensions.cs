using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Concurrent;
using System.Linq;
using System.Globalization;

namespace GameLogic
{
    /// <summary>
    /// 引擎扩展
    /// </summary>
    public static class EngineExtensions
    {
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

        #region Find
        private static ConcurrentDictionary<string, Type> typeCache = new ConcurrentDictionary<string, Type>();
        /// <summary>
        /// 查找类型
        /// </summary>
        /// <param name="qualifiedTypeName"></param>
        /// <returns></returns>
        public static Type FindType(string qualifiedTypeName)
        {
            if (typeCache.TryGetValue(qualifiedTypeName, out Type t))
            {
                return t;
            }

            t = Type.GetType(qualifiedTypeName);

            if (t != null)
            {
                typeCache[qualifiedTypeName] = t;
                return t;
            }

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            t = assemblies.AsParallel().Select(asm => asm.GetType(qualifiedTypeName)).FirstOrDefault(t => t != null);

            if (t != null)
            {
                typeCache[qualifiedTypeName] = t;
            }

            return t;
        }
        #endregion

        #region object转换
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
        public enum TimeType
        {
            Seconds = 1,
            Minutes = 2,
            Hours = 3,
            Days = 4
        }

        /// <summary>
        /// 获取当前时间
        /// </summary>
        /// <returns></returns>
        public static DateTime GetCurrentTime()
        {
            return DateTime.Now;
        }

        /// <summary>
        /// 获得增加时间后的时间
        /// </summary>
        /// <param name="type">1、秒 2、分钟 3、小时 4、天</param>
        /// <param name="value">索要添加的数值</param>
        /// <returns></returns>
        public static DateTime GetAddTime(TimeType type, int value)
        {
            DateTime time = DateTime.Now;
            DateTime result = DateTime.Now;

            switch (type)
            {
                case TimeType.Seconds:
                    return time.AddSeconds(value);
                case TimeType.Minutes:
                    return time.AddMinutes(value);
                case TimeType.Hours:
                    return time.AddHours(value);
                case TimeType.Days:
                    return time.AddDays(value);
                default:
                    throw new ArgumentException("无效的时间类型.");
            }
        }

        /// <summary>
        /// 获取目标时间与当前时间的时间差
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static TimeSpan GetTime(DateTime target)
        {
            //当前时间减去目标时间
            return DateTimeOffset.Now - target;
        }
        private static readonly DateTimeOffset UnixEpoch = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);
        
        /// <summary>
        /// 秒数据换算为日期
        /// </summary>
        /// <param name="tick"></param>
        /// <param name="totalMilliseconds"></param>
        /// <returns></returns>
        public static DateTime GetDateTime(long tick, bool totalMilliseconds = false)
        {
            var dateTimeOffset = totalMilliseconds ? UnixEpoch.AddMilliseconds(tick) : UnixEpoch.AddSeconds(tick);
            return dateTimeOffset.LocalDateTime;
        }

        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="totalMilliseconds"></param>
        /// <returns></returns>
        public static long GetTimestamp(DateTime dateTime, bool totalMilliseconds = false)
        {
            var dateTimeOffset = new DateTimeOffset(dateTime);
            return totalMilliseconds ? dateTimeOffset.ToUnixTimeMilliseconds() : dateTimeOffset.ToUnixTimeSeconds();
        }

        /// <summary>
        /// 获取当前时间戳
        /// </summary>
        /// <param name="totalMilliseconds"></param>
        /// <returns></returns>
        public static long GetCurrentTimestamp(bool totalMilliseconds = false)
        {
            var now = DateTimeOffset.Now;
            return totalMilliseconds ? now.ToUnixTimeMilliseconds() : now.ToUnixTimeSeconds();
        }

        /// <summary>
        /// 格式的日期string 转换为 DateTime
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        /// <exception cref="FormatException"></exception>
        public static DateTime GetDateTimeByString(string dateTime)
        {
            if (DateTime.TryParseExact(dateTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTime result))
            {
                return result;
            }
            else
            {
                throw new FormatException("无效的日期时间格式。");
            }
        }
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

        #region 字符串
        /// <summary>
        /// 字符串转byte
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static byte ToByte(this string val)
        {
            if (byte.TryParse(val, out byte result))
            {
                return result;
            }
            else
            {
                return 0;
            }
        }
        /// <summary>
        /// 字符串转int64
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static long ToInt64(this string val)
        {
            if (long.TryParse(val, out long result))
            {
                return result;
            }
            else
            {
                return 0;
            }
        }
        /// <summary>
        /// 字符串转float
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static float ToFloat(this string val)
        {
            if (float.TryParse(val, out float result))
            {
                return result;
            }
            else
            {
                return 0;
            }
        }
        /// <summary>
        /// 字符串转int32
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        static public Int32 ToInt32(this string str)
        {
            if (Int32.TryParse(str, out Int32 result))
            {
                return result;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 得到字符串长度，一个汉字长度为2
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        public static int StrLength(string inputString)
        {
            int tempLen = 0;
            byte[] s = Encoding.ASCII.GetBytes(inputString);
            foreach (byte b in s)
            {
                tempLen += (b == 63) ? 2 : 1;
            }
            return tempLen;
        }

        /// <summary>
        /// 计算字符串的MD5值
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string MD5(string source)
        {
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                byte[] data = Encoding.UTF8.GetBytes(source);
                byte[] md5Data = md5.ComputeHash(data, 0, data.Length);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < md5Data.Length; i++)
                {
                    sb.Append(md5Data[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }

        /// <summary>
        /// 检查类名
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool CheckClassName(string str)
        {
            return Regex.IsMatch(str, @"^[A-Z][A-Za-z0-9_]*$");
        }

        /// <summary>
        /// 检查字段名
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool CheckFieldName(string name)
        {
            return Regex.IsMatch(name, @"^[A-Za-z_][A-Za-z0-9_]*$");
        }

        /// <summary>
        /// 第一个字符大写，不改变其他字符
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string CapitalFirstChar(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }

            return char.ToUpper(str[0]) + str.Substring(1);
        }

        /// <summary>
        /// 首字母大写，其他小写
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static string ToTitleCase(string word)
        {
            if (string.IsNullOrEmpty(word))
            {
                return word;
            }

            return char.ToUpper(word[0]) + (word.Length > 1 ? word.Substring(1).ToLower() : "");
        }

        /// <summary>
        /// 驼峰命名转下划线命名
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToSentenceCase(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }

            str = char.ToLower(str[0]) + str.Substring(1);
            return Regex.Replace(str, "[a-z][A-Z]", m => char.ToLower(m.Value[0]) + "_" + char.ToLower(m.Value[1]));
        }

        /// <summary>
        /// 截断字符串变成数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static List<T> Split<T>(string str, params char[] args)
        {
            if (args.Length == 0)
            {
                args = new[] { '|' }; // 默认
            }

            if (string.IsNullOrEmpty(str))
            {
                return new List<T>();
            }

            return str.Split(args)
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Select(s => (T)Convert.ChangeType(s.Trim(), typeof(T)))
                    .ToList();
        }

        /// <summary>
        /// 判断字符串是否是数字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNumber(string str)
        {
            return double.TryParse(str, out _);
        }

        /// <summary>
        /// 人性化数字显示，百万，千万，亿
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string HumanizeNumber(int number)
        {
            if (number > 100000000)
            {
                return $"{number / 100000000}亿";
            }
            else if (number > 10000000)
            {
                return $"{number / 10000000}千万";
            }
            else if (number > 1000000)
            {
                return $"{number / 1000000}百万";
            }
            else if (number > 10000)
            {
                return $"{number / 10000}万";
            }

            return number.ToString();
        }
        #endregion

        #region File
        /// <summary>
        /// MD5Encrypt
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string MD5Encrypt(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");
            }

            return byte2String;
        }
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
        #endregion

        #region 计算公式
        /// <summary>
        /// 计算最大公约数
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int CalculateMaximumCommonDivisor(int a, int b)
        {
            a = Math.Abs(a);
            b = Math.Abs(b);
            while (b != 0)
            {
                int t = b;
                b = a % b;
                a = t;
            }
            return a;
        }

        /// <summary>
        /// 计算中心点
        /// </summary>
        /// <param name="Points"></param>
        /// <returns></returns>
        public static Vector3 CalculateCenterPoint(List<Transform> Points)
        {
            return Points.Aggregate(Vector3.zero, (acc, p) => acc + p.position) / Points.Count;
        }

        /// <summary>
        /// 计算AB与CD两条线段的交点.
        /// </summary>
        /// <param name="a">A点</param>
        /// <param name="b">B点</param>
        /// <param name="c">C点</param>
        /// <param name="d">D点</param>
        /// <returns>是否相交 true:相交 false:未相交 | AB与CD的交点</returns>
        public static (bool, Vector3) CalculateIntersectionPoint_AB_And_CD_LineSegments(
            Vector3 a,
            Vector3 b,
            Vector3 c,
            Vector3 d)
        {
            Vector3 ab = b - a;
            Vector3 ca = a - c;
            Vector3 cd = d - c;

            Vector3 v1 = Vector3.Cross(ca, cd);

            if (Mathf.Abs(Vector3.Dot(v1, ab)) > 1e-6)
            {
                // 不共面
                return (false, Vector3.zero);
            }

            if (Vector3.Cross(ab, cd).sqrMagnitude <= 1e-6)
            {
                // 平行
                return (false, Vector3.zero);
            }

            Vector3 ad = d - a;
            Vector3 cb = b - c;
            // 快速排斥
            if (Mathf.Min(a.x, b.x) > Mathf.Max(c.x, d.x) || Mathf.Max(a.x, b.x) < Mathf.Min(c.x, d.x)
            || Mathf.Min(a.y, b.y) > Mathf.Max(c.y, d.y) || Mathf.Max(a.y, b.y) < Mathf.Min(c.y, d.y)
            || Mathf.Min(a.z, b.z) > Mathf.Max(c.z, d.z) || Mathf.Max(a.z, b.z) < Mathf.Min(c.z, d.z)
            )
                return (false, Vector3.zero);

            // 跨立试验
            if (Vector3.Dot(Vector3.Cross(-ca, ab), Vector3.Cross(ab, ad)) > 0
                && Vector3.Dot(Vector3.Cross(ca, cd), Vector3.Cross(cd, cb)) > 0)
            {
                Vector3 v2 = Vector3.Cross(cd, ab);
                float ratio = Vector3.Dot(v1, v2) / v2.sqrMagnitude;
                Vector3 intersectPos = a + ab * ratio;
                return (true, intersectPos);
            }

            return (false, Vector3.zero);
        }

        /// <summary>
        /// 计算两条线段的交点
        /// </summary>
        /// <param name="ps1"></param>
        /// <param name="pe1"></param>
        /// <param name="ps2"></param>
        /// <param name="pe2"></param>
        /// <returns></returns>
        public static (bool, Vector2) LineIntersectionPoint(Vector2 ps1, Vector2 pe1, Vector2 ps2, Vector2 pe2)
        {
            // 获取第一行的A、B、C-点：ps1到pe1
            float A1 = pe1.y - ps1.y;
            float B1 = ps1.x - pe1.x;
            float C1 = A1 * ps1.x + B1 * ps1.y;

            // 获取第二行的A、B、C点：ps2到pe2
            float A2 = pe2.y - ps2.y;
            float B2 = ps2.x - pe2.x;
            float C2 = A2 * ps2.x + B2 * ps2.y;

            // 获取delta并检查直线是否平行
            float delta = A1 * B2 - A2 * B1;
            if (delta == 0)
            {
                return (false, Vector2.zero);
            }

            // 现在返回Vector2的交点
            Vector2 intersectPoint = new Vector2(
                (B2 * C1 - B1 * C2) / delta,
                (A1 * C2 - A2 * C1) / delta
            );
            return (true, intersectPoint);
        }

        /// <summary>
        /// 获取两点之间距离一定百分比的一个点
        /// </summary>
        /// <param name="start">起始点</param>
        /// <param name="end">结束点</param>
        /// <param name="distance">起始点到目标点距离百分比</param>
        /// <returns></returns>
        public static Vector3 GetBetweenPoint1(Vector3 start, Vector3 end, float percent)
        {
            // old
            // Vector3 normal = (end - start).normalized;
            // float distance = Vector3.Distance(start, end);
            // return normal * (distance * percent) + start;
            // new
            return Vector3.Lerp(start, end, percent);
        }

        /// <summary>
        /// 获取两点之间一定距离的点
        /// </summary>
        /// <param name="start">起始点</param>
        /// <param name="end">结束点</param>
        /// <param name="distance">距离</param>
        /// <returns></returns>
        public static Vector3 GetBetweenPoint2(Vector3 start, Vector3 end, float distance)
        {
            return start + (end - start).normalized * distance;
        }

        /// <summary>
        /// 获取椭圆上的某一点，相对坐标
        /// </summary>
        /// <param name="longHalfAxis">长半轴即目标距离</param>
        /// <param name="shortHalfAxis">短半轴</param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Vector2 GetRelativePositionOfEllipse(float longHalfAxis, float shortHalfAxis, float angle)
        {
            var rad = angle * Mathf.Deg2Rad; // 弧度
            var newPos = Vector2.right * longHalfAxis * Mathf.Cos(rad) + Vector2.up * shortHalfAxis * Mathf.Sin(rad);
            return newPos;
        }

        /// <summary>
        /// 计算两个向量之间的角度 3D
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static float Angel(Transform value1, Transform value2)
        {
            return Vector3.Angle(value1.forward, value2.forward);
        }

        /// <summary>
        /// 计算两个向量之间的角度 2D
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static float Angle(Vector2 value1, Vector2 value2)
        {
            return Vector2.Angle(value1, value2);
        }

        public enum RotationDirection
        {
            None,
            Right,
            Left
        }
        /// <summary>
        /// 判断物体左右转转
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static RotationDirection GetRotationDirection(Transform value1, Transform value2)
        {
            Vector2 v1 = new Vector2(value1.forward.x, value1.forward.z); //旋转前的前方
            Vector2 v2 = new Vector2(value2.forward.x, value2.forward.z); //旋转后的前方

            float rightFloat = v1.x * v2.y - v2.x * v1.y;

            if (rightFloat < 0)
            {
                return RotationDirection.Right;
            }
            else if (rightFloat > 0)
            {
                return RotationDirection.Left;
            }
            else
            {
                return RotationDirection.None;
            }
        }

        /// <summary>
        /// 文件大小格式化显示成KB，MB,GB
        /// </summary>
        /// <param name="size">字节</param>
        public static String FormatFileSize(long size)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            int order = 0;
            while (size >= 1024 && order < sizes.Length - 1)
            {
                order++;
                size = size / 1024;
            }
            return String.Format("{0:0.##} {1}", size, sizes[order]);
        }

        /// <summary>
        /// 数组值比较
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        /// <returns></returns>
        public static bool ArraysEqual<T>(T[] a1, T[] a2)
        {
            return Enumerable.SequenceEqual(a1, a2);
        }

        /// <summary>
        /// 概率，百分比 FLOAT
        /// 注意，0的时候当是100%
        /// </summary>
        /// <param name="chancePercent"></param>
        /// <returns></returns>
        public static bool Probability(float chancePercent)
        {
            return UnityEngine.Random.Range(0f, 100f) <= chancePercent;
        }

        /// <summary>
        /// 概率，百分比 BYTE
        /// </summary>
        /// <param name="chancePercent"></param>
        /// <returns></returns>
        public static bool Probability(byte chancePercent)
        {
            return UnityEngine.Random.Range(1, 101) <= chancePercent;
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
    }
}