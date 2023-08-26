using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

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
        public static T Get<T>(this object[] openArgs, int offset, bool isLog = true)
        {
            T ret;
            if ((openArgs.Length - 1) >= offset)
            {
                var arrElement = openArgs[offset];
                if (arrElement == null)
                    ret = default(T);
                else
                {
                    try
                    {
                        ret = (T)Convert.ChangeType(arrElement, typeof(T));
                    }
                    catch (Exception)
                    {
                        if (arrElement is string && string.IsNullOrEmpty(arrElement as string))
                            ret = default(T);
                        else
                        {
                            Debug.LogError(string.Format("[Error get from object[],  '{0}' change to type {1}", arrElement, typeof(T)));
                            ret = default(T);
                        }
                    }
                }
            }
            else
            {
                ret = default(T);

                Debug.LogError(string.Format("[GetArg] {0} args - offset: {1}", openArgs, offset));
            }
            return ret;
        }

        #region Find
        /// <summary>
        /// 查找类型
        /// </summary>
        /// <param name="qualifiedTypeName"></param>
        /// <returns></returns>
        public static Type FindType(string qualifiedTypeName)
        {
            Type t = Type.GetType(qualifiedTypeName);

            if (t != null)
            {
                return t;
            }
            else
            {
                Assembly[] Assemblies = AppDomain.CurrentDomain.GetAssemblies();
                for (int n = 0; n < Assemblies.Length; n++)
                {
                    Assembly asm = Assemblies[n];
                    t = asm.GetType(qualifiedTypeName);
                    if (t != null)
                        return t;
                }
                return null;
            }
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
            Int32 ret = 0;
            try
            {
                if (obj != null)
                {
                    ret = Convert.ToInt32(obj);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("ToInt32 : " + ex);
            }

            return ret;
        }

        /// <summary>
        /// object转int64 | long
        /// </summary>
        /// <param name="o"></param>
        /// <returns>int</returns>
        public static long ObjToInt64(this object obj)
        {
            Int64 ret = 0;
            try
            {
                if (obj != null)
                {
                    ret = Convert.ToInt64(obj);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("ToInt64 : " + ex);
            }

            return ret;
        }

        /// <summary>
        /// object转float
        /// </summary>
        /// <param name="o"></param>
        /// <returns>float</returns>
        public static float ObjToFloat(this object obj)
        {
            float ret = 0;

            try
            {
                if (obj != null)
                {
                    ret = (float)Math.Round(Convert.ToSingle(obj), 2);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("ObjToFloat : " + ex);
            }

            return ret;
        }

        /// <summary>
        /// object转string
        /// </summary>
        /// <param name="o"></param>
        /// <returns>string</returns>
        public static string ObjToString(this object obj)
        {
            string ret = null;

            try
            {
                if (obj != null)
                {
                    ret = Convert.ToString(obj);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("ObjToString : " + ex);
            }

            return ret;
        }
        #endregion

        #region 随机
        /// <summary>
        /// 随机数（int）
        /// </summary>
        /// <param name="min">最小数</param>
        /// <param name="max">最大数</param>
        /// <returns>随机数</returns>
        public static int Random(int min, int max)
        {
            return UnityEngine.Random.Range(min, max + 1);
        }
        /// <summary>
        /// 随机数（float）
        /// </summary>
        /// <param name="min">最小数</param>
        /// <param name="max">最大数</param>
        /// <returns>随机数</returns>
        public static float Random(float min, float max)
        {
            return UnityEngine.Random.Range(min, max + 1);
        }

        /// <summary>
        /// 从一个List中随机获取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T GetRandomItemFromList<T>(IList<T> list)
        {
            if (list.Count == 0)
                return default(T);

            return list[UnityEngine.Random.Range(0, list.Count)];
        }

        /// <summary>
        /// 波浪随机数整数版
        /// </summary>
        /// <param name="waveNumberStr"></param>
        /// <returns></returns>
        public static int GetWaveRandomNumberInt(string waveNumberStr)
        {
            return Mathf.RoundToInt(GetWaveRandomNumber(waveNumberStr));
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
            if (string.IsNullOrEmpty(waveNumberStr))
                return 0;

            var strs = waveNumberStr.Split('-', '~');
            if (strs.Length == 1)
            {
                return waveNumberStr.ToFloat();
            }

            return UnityEngine.Random.Range(strs[0].ToFloat(), strs[1].ToFloat());
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
            var rangeArr = EngineExtensions.Split<float>(str, '~', '-');
            var number = new FromToNumber();
            if (rangeArr.Count > 0)
            {
                number.From = rangeArr[0];
            }
            if (rangeArr.Count > 1)
            {
                number.To = rangeArr[1];
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
            if (string.IsNullOrEmpty(waveNumberStr))
                return false;

            var strs = waveNumberStr.Split('~');
            if (strs.Length == 1)
            {
                return strs[0].ToInt32() == testNumber;
            }
            var min = strs[0].ToInt32();
            var max = strs[1].ToInt32();
            return testNumber >= min && testNumber <= max;
        }
        #endregion

        #region 时间
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
        public static DateTime GetAddTime(int type, int value)
        {
            DateTime time = DateTime.Now;
            DateTime result = DateTime.Now;

            switch (type)
            {
                case 1:
                    result = time.AddSeconds(value);
                    break;
                case 2:
                    result = time.AddMinutes(value);
                    break;
                case 3:
                    result = time.AddHours(value);
                    break;
                case 4:
                    result = time.AddDays(value);
                    break;
            }

            return result;
        }

        /// <summary>
        /// 获取目标时间与当前时间的时间差
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static TimeSpan GetTime(DateTime target)
        {
            //timeA 表示需要计算
            DateTime current = DateTime.Now;    //获取当前时间
            TimeSpan ts = current - target; //计算时间差
            return ts;
        }

        /// <summary>
        /// 秒数据换算为日期
        /// </summary>
        /// <param name="tick"></param>
        /// <returns></returns>
        public static DateTime GetDateTime(long tick, bool totalMilliseconds = false)
        {
            if (totalMilliseconds == false)
            {
                //秒
                return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(tick).ToLocalTime();
            }
            else
            {
                //毫秒
                return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(tick).ToLocalTime();
            }
        }

        /// <summary>
        /// 获取1970-01-01至dateTime0 - 毫秒
        /// </summary>
        public static long GetTimestamp(DateTime dateTime, bool totalMilliseconds = false)
        {
            if (totalMilliseconds == false)
            {
                //秒
                DateTime dt1970 = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToLocalTime();
                return (dateTime.Ticks - dt1970.Ticks) / 10000000;
            }
            else
            {
                //毫秒
                DateTime dt1970 = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToLocalTime();
                return (dateTime.Ticks - dt1970.Ticks) / 10000;
            }
        }

        /// <summary>
        /// 获取当前时间戳
        /// </summary>
        /// <returns></returns>
        public static long GetCurrentTimestamp(bool totalMilliseconds = false)
        {
            if (totalMilliseconds == false)
            {
                //秒
                DateTime dt1970 = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToLocalTime();
                return (DateTime.Now.Ticks - dt1970.Ticks) / 10000000;
            }
            else
            {
                //毫秒
                DateTime dt1970 = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToLocalTime();
                return (DateTime.Now.Ticks - dt1970.Ticks) / 10000;
            }
        }

        /// <summary>
        /// yyyy-MM-dd HH:MM:SS 格式的日期string 转换为 DateTime
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime GetDateTimeByString(string dateTime)
        {
            string[] tmp = dateTime.Split(' ');
            string[] tmpDate = tmp[0].Split('-');
            string[] tmpTime = tmp[1].Split(':');
            int year = int.Parse(tmpDate[0]);
            int month = int.Parse(tmpDate[1]);
            int date = int.Parse(tmpDate[2]);
            int hours = int.Parse(tmpTime[0]);
            int minutes = int.Parse(tmpTime[1]);
            int seconds = int.Parse(tmpTime[2]);

            return new DateTime(year, month, date, hours, minutes, seconds, 0, DateTimeKind.Utc);
        }
        #endregion

        #region 分辨率
        /// <summary>
        /// 设置屏幕分辨率
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="fullScreen"></param>
        public static void SetScreen(int width, int height, bool fullScreen)
        {
            Screen.SetResolution(/*屏幕宽度*/ width,/*屏幕高度*/ height, /*是否全屏显示*/fullScreen);
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
            byte ret = 0;
            try
            {
                if (!String.IsNullOrEmpty(val))
                {
                    ret = Convert.ToByte(val);
                }
            }
            catch (Exception)
            {
            }

            return ret;
        }
        /// <summary>
        /// 字符串转int64
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static long ToInt64(this string val)
        {
            long ret = 0;
            try
            {
                if (!String.IsNullOrEmpty(val))
                {
                    ret = Convert.ToInt64(val);
                }
            }
            catch (Exception)
            {
            }

            return ret;
        }
        /// <summary>
        /// 字符串转float
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static float ToFloat(this string val)
        {
            float ret = 0;
            try
            {
                if (!String.IsNullOrEmpty(val))
                {
                    ret = Convert.ToSingle(val);
                }
            }
            catch (Exception)
            {
            }

            return ret;
        }
        /// <summary>
        /// 字符串转int32
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        static public Int32 ToInt32(this string str)
        {
            Int32 ret = 0;
            try
            {
                if (!String.IsNullOrEmpty(str))
                {
                    ret = Convert.ToInt32(str);
                }
            }
            catch (Exception)
            {
            }

            return ret;
        }

        /// <summary>
        /// 得到字符串长度，一个汉字长度为2
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        public static int StrLength(string inputString)
        {
            System.Text.ASCIIEncoding ascii = new System.Text.ASCIIEncoding();
            int tempLen = 0;
            byte[] s = ascii.GetBytes(inputString);
            for (int i = 0; i < s.Length; i++)
            {
                if ((int)s[i] == 63)
                    tempLen += 2;
                else
                    tempLen += 1;
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
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.UTF8.GetBytes(source);
            byte[] md5Data = md5.ComputeHash(data, 0, data.Length);
            md5.Clear();

            string destString = "";
            for (int i = 0; i < md5Data.Length; i++)
            {
                destString += System.Convert.ToString(md5Data[i], 16).PadLeft(2, '0');
            }
            destString = destString.PadLeft(32, '0');
            return destString;
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
        /// 第一个字符大写
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string CapitalFirstChar(string str)
        {
            return str[0].ToString().ToUpper() + str.Substring(1);
        }

        /// <summary>
        /// 首字母大写
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static string ToTitleCase(string word)
        {
            return word.Substring(0, 1).ToUpper() + (word.Length > 1 ? word.Substring(1).ToLower() : "");
        }

        /// <summary>
        /// 首字母大写变下划线
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToSentenceCase(string str)
        {
            str = char.ToLower(str[0]) + str.Substring(1);
            return Regex.Replace(str, "[a-z][A-Z]",
                (m) => { return char.ToLower(m.Value[0]) + "_" + char.ToLower(m.Value[1]); });
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

            var retList = new List<T>();
            if (!string.IsNullOrEmpty(str))
            {
                string[] strs = str.Split(args);

                foreach (string s in strs)
                {
                    string trimS = s.Trim();
                    if (!string.IsNullOrEmpty(trimS))
                    {
                        T val = (T)Convert.ChangeType(trimS, typeof(T));
                        if (val != null)
                        {
                            retList.Add(val);
                        }
                    }
                }
            }
            return retList;
        }

        /// <summary>
        /// 判断字符串是否是数字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNumber(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                Log.Warning("传入的值为空！请检查");
                return false;
            }
            var pattern = @"^\d*$";
            return Regex.IsMatch(str, pattern);
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
                return string.Format("{0}{1}", number / 100000000, "亿");
            }
            else if (number > 10000000)
            {
                return string.Format("{0}{1}", number / 10000000, "千万");
            }
            else if (number > 1000000)
            {
                return string.Format("{0}{1}", number / 1000000, "百万");
            }
            else if (number > 10000)
            {
                return string.Format("{0}{1}", number / 10000, "万");
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
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
            byte[] hashedDataBytes;
            hashedDataBytes = md5Hasher.ComputeHash(Encoding.GetEncoding("gb2312").GetBytes(str));
            StringBuilder tmp = new StringBuilder();
            foreach (byte i in hashedDataBytes)
            {
                tmp.Append(i.ToString("x2"));
            }
            return tmp.ToString();
        }
        /// <summary>
        /// 无视锁文件，直接读bytes  读取（加载）数据
        /// </summary>
        /// <param name="resPath"></param>
        /// <returns></returns>
        public static byte[] ReadAllBytes(string resPath)
        {
            byte[] bytes;
            using (FileStream fs = File.Open(resPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                bytes = new byte[fs.Length];
                fs.Read(bytes, 0, (int)fs.Length);
            }
            return bytes;
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

                    string dir = PathUtils.GetParentDir(fullpath);

                    if (!Directory.Exists(dir))
                    {
                        try
                        {
                            Directory.CreateDirectory(dir);
                        }
                        catch (Exception e)
                        {
                            Debug.LogError(string.Format("SaveFile() CreateDirectory Error! Dir:{0}, Error:{1}", dir, e.Message));
                            return -1;
                        }
                    }

                    FileStream fs = null;
                    try
                    {
                        fs = new FileStream(fullpath, FileMode.Create, FileAccess.Write);
                        fs.Write(content, 0, content.Length);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(string.Format("SaveFile() Path:{0}, Error:{1}", fullpath, e.Message));
                        fs.Close();
                        return -1;
                    }

                    fs.Close();
                    return content.Length;
                });
            }
            catch (Exception ex)
            {
                Debug.LogError(ex + " SaveFile");
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
            string fileAbslutePath = Application.persistentDataPath + "/Json/" + fileName + ".json";
            object value = null;
            if (File.Exists(fileAbslutePath))
            {
                FileStream fs = new FileStream(fileAbslutePath, FileMode.Open);
                StreamReader sr = new StreamReader(fs);
                string tempStr = sr.ReadToEnd();
                value = JsonMapper.ToObject<T>(tempStr);

                sr.Close();
                if (fs != null)
                {
                    fs.Close();
                }
            }

            return (T)value;
        }

        /// <summary>
        /// 保存Json
        /// </summary>
        /// <param name="jsonStr"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static IEnumerator SaveJson(string jsonStr, string fileName)
        {
            string filePath = Application.persistentDataPath + "/Json";
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            string fileAbslutePath = filePath + "/" + fileName + ".json";

            byte[] bts = System.Text.Encoding.UTF8.GetBytes(jsonStr);
            File.WriteAllBytes(fileAbslutePath, bts);

            yield return null;
        }
        #endregion

        #region 计算公式
        /// <summary>
        /// 计算最大公约数
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int GetGCD(int a, int b)
        {
            if (a < b)
            {
                int t = a;
                a = b;
                b = t;
            }
            while (b > 0)
            {
                int t = a % b;
                a = b;
                b = t;
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
            Vector3 centerPoint = Vector3.zero;
            foreach (Transform p in Points)
            {
                centerPoint += p.position;
            }
            centerPoint /= Points.Count;
            return centerPoint;
        }

        /// <summary>
        /// 计算AB与CD两条线段的交点.
        /// </summary>
        /// <param name="a">A点</param>
        /// <param name="b">B点</param>
        /// <param name="c">C点</param>
        /// <param name="d">D点</param>
        /// <param name="intersectPos">AB与CD的交点</param>
        /// <returns>是否相交 true:相交 false:未相交</returns>
        public static bool TryGetIntersectPoint(Vector3 a, Vector3 b, Vector3 c, Vector3 d, out Vector3 intersectPos)
        {
            intersectPos = Vector3.zero;

            Vector3 ab = b - a;
            Vector3 ca = a - c;
            Vector3 cd = d - c;

            Vector3 v1 = Vector3.Cross(ca, cd);

            if (Mathf.Abs(Vector3.Dot(v1, ab)) > 1e-6)
            {
                // 不共面
                return false;
            }

            if (Vector3.Cross(ab, cd).sqrMagnitude <= 1e-6)
            {
                // 平行
                return false;
            }

            Vector3 ad = d - a;
            Vector3 cb = b - c;
            // 快速排斥
            if (Mathf.Min(a.x, b.x) > Mathf.Max(c.x, d.x) || Mathf.Max(a.x, b.x) < Mathf.Min(c.x, d.x)
               || Mathf.Min(a.y, b.y) > Mathf.Max(c.y, d.y) || Mathf.Max(a.y, b.y) < Mathf.Min(c.y, d.y)
               || Mathf.Min(a.z, b.z) > Mathf.Max(c.z, d.z) || Mathf.Max(a.z, b.z) < Mathf.Min(c.z, d.z)
            )
                return false;

            // 跨立试验
            if (Vector3.Dot(Vector3.Cross(-ca, ab), Vector3.Cross(ab, ad)) > 0
                && Vector3.Dot(Vector3.Cross(ca, cd), Vector3.Cross(cd, cb)) > 0)
            {
                Vector3 v2 = Vector3.Cross(cd, ab);
                float ratio = Vector3.Dot(v1, v2) / v2.sqrMagnitude;
                intersectPos = a + ab * ratio;
                return true;
            }

            return false;
        }

        /// <summary>
        /// 两线交点（忽略长度）
        /// </summary>
        /// <param name="intersectPoint"></param>
        /// <param name="ps1"></param>
        /// <param name="pe1"></param>
        /// <param name="ps2"></param>
        /// <param name="pe2"></param>
        /// <returns></returns>
        public static bool LineIntersectionPoint(out Vector2 intersectPoint, Vector2 ps1, Vector2 pe1, Vector2 ps2,
            Vector2 pe2)
        {
            intersectPoint = Vector2.zero;

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
                return false;

            // 现在返回Vector2的交点
            intersectPoint = new Vector2(
                (B2 * C1 - B1 * C2) / delta,
                (A1 * C2 - A2 * C1) / delta
                );
            return true;
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
            Vector3 normal = (end - start).normalized;
            float distance = Vector3.Distance(start, end);
            return normal * (distance * percent) + start;
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
            Vector3 normal = (end - start).normalized;
            return normal * distance + start;
        }

        /// <summary>
        /// 获取角度
        /// </summary>
        /// <param name="var1"></param>
        /// <param name="var2"></param>
        /// <returns></returns>
        public static float GetAngel(Transform var1, Transform var2)
        {
            //注意角度测量一定要用对象的正方向
            float angel = Vector3.Angle(var1.forward, var2.forward);
            return angel;
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
            var newPos = new Vector2(longHalfAxis * Mathf.Cos(rad), shortHalfAxis * Mathf.Sin(rad));
            return newPos;
        }
        public static float Angle(Vector2 from, Vector2 to)
        {
            return Quaternion.FromToRotation(from.normalized, to.normalized).eulerAngles.z;
        }

        /// <summary>
        /// 判断物体左右转转
        /// </summary>
        /// <param name="var1"></param>
        /// <param name="var2"></param>
        /// <returns></returns>
        public static int RotationDirection(Transform var1, Transform var2)
        {
            int direction = 0;

            Vector2 v1 = new Vector2(var1.forward.x, var1.forward.z); //旋转前的前方
            Vector2 v2 = new Vector2(var2.forward.x, var2.forward.z); //旋转后的前方

            float rightFloat = v1.x * v2.y - v2.x * v1.y;

            if (rightFloat < 0)
            {
                direction = 1;
                //Debug.Log("向右转了");
            }
            else if (rightFloat > 0)
            {
                direction = -1;
                //Debug.Log("向左转了");
            }
            else
            {
                direction = 0;
                //Debug.Log("没转");
            }

            return direction;
        }

        /// <summary>
        /// 文件大小格式化显示成KB，MB,GB
        /// </summary>
        /// <param name="size">字节</param>
        public static String FormatFileSize(long size)
        {
            int GB = 1024 * 1024 * 1024;
            int MB = 1024 * 1024;
            int KB = 1024;

            if (size / GB >= 1)
            {
                return Math.Round(size / (float)GB, 2) + "GB";
            }

            if (size / MB >= 1)
            {
                return Math.Round(size / (float)MB, 2) + "MB";
            }

            if (size / KB >= 1)
            {
                return Math.Round(size / (float)KB, 2) + "KB";
            }

            return size + "B";
        }

        /// <summary>
        /// 數組值比較
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        /// <returns></returns>
        public static bool ArraysEqual<T>(T[] a1, T[] a2)
        {
            if (ReferenceEquals(a1, a2))
                return true;

            if (a1 == null || a2 == null)
                return false;

            if (a1.Length != a2.Length)
                return false;

            EqualityComparer<T> comparer = EqualityComparer<T>.Default;
            for (int i = 0; i < a1.Length; i++)
            {
                if (!comparer.Equals(a1[i], a2[i])) return false;
            }
            return true;
        }

        /// <summary>
        /// 概率，百分比 FLOAT
        /// 注意，0的时候当是100%
        /// </summary>
        /// <param name="chancePercent"></param>
        /// <returns></returns>
        public static bool Probability(float chancePercent)
        {
            var chance = UnityEngine.Random.Range(0f, 100f);

            if (chance <= chancePercent) // 概率
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 概率，百分比 BYTE
        /// </summary>
        /// <param name="chancePercent"></param>
        /// <returns></returns>
        public static bool Probability(byte chancePercent)
        {
            int chance = UnityEngine.Random.Range(1, 101);

            if (chance <= chancePercent) // 概率
            {
                return true;
            }

            return false;
        }
        #endregion

        #region 获取硬件信息
        /// <summary>
        /// 取本机主机ip
        /// </summary>
        /// <returns></returns>
        public static string GetLocalIP()
        {
            try
            {
                string HostName = Dns.GetHostName(); //得到主机名
                IPHostEntry IpEntry = Dns.GetHostEntry(HostName);
                for (int i = 0; i < IpEntry.AddressList.Length; i++)
                {
                    //从IP地址列表中筛选出IPv4类型的IP地址
                    //AddressFamily.InterNetwork表示此IP为IPv4,AddressFamily.InterNetworkV6表示此地址为IPv6类型
                    if (IpEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                    {
                        return IpEntry.AddressList[i].ToString();
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 从IpHostEntry获取IP地址，配合GetIpAddress
        /// </summary>
        /// <param name="ipHostEntry"></param>
        /// <returns></returns>
        public static IPAddress GetIpAddressFromIpHostEntry(IPHostEntry ipHostEntry)
        {
            var addresses = ipHostEntry.AddressList;

            foreach (var item in addresses)
            {
                if (item.AddressFamily == AddressFamily.InterNetwork)
                {
                    return item;
                }
            }
            return null;
        }

        /// <summary>
        /// 异步获取IP地址
        /// </summary>
        /// <param name="host"></param>
        /// <param name="callback"></param>
        public static void GetIpAddress(string host, Action<IPAddress> callback = null)
        {
            IPAddress ipAddress = null;
            if (!IPAddress.TryParse(host, out ipAddress))
            {
                Dns.BeginGetHostAddresses(host, new AsyncCallback((asyncResult) =>
                {
                    IPAddress[] addrs = Dns.EndGetHostAddresses(asyncResult);
                    if (callback != null)
                    {
                        if (addrs.Length > 0)
                            ipAddress = addrs[0];
                        callback(ipAddress);
                    }
                }), null);
            }
            else
            {
                if (callback != null)
                    callback(ipAddress);
            }
        }
        #endregion
    }
}