using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 引擎扩展
    /// </summary>
    public static class EngineExtensions
    {
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
        #endregion
    }
}