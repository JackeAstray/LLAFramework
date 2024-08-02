using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Globalization;
using LitJson;
using System.IO.Compression;
using System.IO;

namespace GameLogic
{
    /// <summary>
    /// 字符串扩展方法
    /// </summary>
    public static class StringExtensions
    {
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

        public static string Uid(string uid)
        {
            int position = uid.LastIndexOf('_');
            return uid.Remove(0, position + 1);
        }

        /// <summary>
        /// 得到字符串长度，一个汉字长度为2
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        public static int StringLength(this string inputString)
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
        /// 获取内容在UTF8编码下的字节长度；
        /// </summary>
        /// <param name="context">需要检测的内容</param>
        /// <returns>字节长度</returns>
        public static int GetUTF8Length(this string context)
        {
            return Encoding.UTF8.GetBytes(context).Length;
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
        /// 分割字符串
        /// </summary>
        /// <param name="fullString">完整字段</param>
        /// <param name="separator">new string[]{"."}</param>
        /// <param name="removeEmptyEntries">是否返回分割后数组中的空元素</param>
        /// <param name="subStringIndex">分割后数组的序号</param>
        /// <returns>分割后的字段</returns>
        public static string StringSplit(string fullString, string[] separator, bool removeEmptyEntries, int subStringIndex)
        {
            string[] stringArray = null;
            if (removeEmptyEntries)
            {
                stringArray = fullString.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            }
            else
            {
                stringArray = fullString.Split(separator, StringSplitOptions.None);
            }
            string subString = stringArray[subStringIndex];
            return subString;
        }

        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <param name="fullString">完整字段</param>
        /// <param name="separator">new string[]{"."}</param>
        /// <param name="count">要返回的子字符串的最大数量</param>
        /// <param name="removeEmptyEntries">是否移除空实体</param>
        /// <returns>分割后的字段</returns>
        public static string StringSplit(string fullString, string[] separator, int count, bool removeEmptyEntries)
        {
            string[] stringArray = null;
            if (removeEmptyEntries)
            {
                stringArray = fullString.Split(separator, count, StringSplitOptions.RemoveEmptyEntries);
            }
            else
            {
                stringArray = fullString.Split(separator, count, StringSplitOptions.None);
            }
            return stringArray.ToString();
        }

        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <param name="fullString">分割字符串</param>
        /// <param name="separator">new string[]{"."}</param>
        /// <returns>分割后的字段数组</returns>
        public static string[] StringSplit(string fullString, string[] separator)
        {
            string[] stringArray = null;
            stringArray = fullString.Split(separator, StringSplitOptions.None);
            return stringArray;
        }

        /// <summary>
        /// 截断字符串变成数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static List<T> StringSplit<T>(string str, params char[] args)
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
        /// 多字符替换；
        /// </summary>
        /// <param name="context">需要修改的内容</param>
        /// <param name="oldContext">需要修改的内容</param>
        /// <param name="newContext">修改的新内容</param>
        /// <returns>修改后的内容</returns>
        public static string Replace(string context, string[] oldContext, string newContext)
        {
            if (string.IsNullOrEmpty(context))
            {
                throw new ArgumentNullException("上下文无效");
            }
            if (oldContext == null)
            {
                throw new ArgumentNullException("旧上下文无效");
            }
            if (string.IsNullOrEmpty(newContext))
            {
                throw new ArgumentNullException("新上下文无效");
            }
            var length = oldContext.Length;
            for (int i = 0; i < length; i++)
            {
                context = context.Replace(oldContext[i], newContext);
            }

            return context;
        }

        /// <summary>
        /// 判断字符串有效
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static bool IsStringValid(this string context)
        {
            if (string.IsNullOrEmpty(context))
            {
                return false;
            }
            return true;
        }

        #region MD5
        /// <summary>
        /// 获得32位的MD5加密
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetMD5_32(string input)
        {
            MD5 md5 = MD5.Create();
            byte[] data = md5.ComputeHash(Encoding.Default.GetBytes(input));
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sb.AppendFormat("{0:X2}", data[i]);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 获得16位的MD5加密
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetMD5_16(string input)
        {
            return GetMD5_32(input).Substring(8, 16);
        }
        /// <summary>
        /// 获得8位的MD5加密
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetMD5_8(string input)
        {
            return GetMD5_32(input).Substring(8, 8);
        }
        /// <summary>
        /// 获得4位的MD5加密
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetMD5_4(string input)
        {
            return GetMD5_32(input).Substring(8, 4);
        }
        /// <summary>
        /// 计算字符串的MD5值
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string CreateMD5(string source)
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
        /// 计算文件的MD5值
        /// </summary>
        /// <param name="MD5File">MD5签名文件字符数组</param>
        /// <param name="index">计算起始位置</param>
        /// <param name="count">计算终止位置</param>
        /// <returns>计算结果</returns>
        private static string MD5Buffer(byte[] MD5File, int index, int count)
        {
            MD5CryptoServiceProvider get_md5 = new MD5CryptoServiceProvider();
            byte[] hash_byte = get_md5.ComputeHash(MD5File, index, count);
            string result = System.BitConverter.ToString(hash_byte);

            result = result.Replace("-", "");
            return result;
        }
        #endregion

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
        /// Base64转图片
        /// </summary>
        /// <param name="imageData"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static Texture2D Base64ToTexture2D(this string imageData, int offset = 0)
        {
            Texture2D tex2D = new Texture2D(2, 2);
            imageData = imageData.Substring(offset);
            byte[] data = Convert.FromBase64String(imageData);
            tex2D.LoadImage(data);
            return tex2D;
        }

        /// <summary>
        /// 图片转Base64
        /// </summary>
        /// <param name="bytesArr"></param>
        /// <returns></returns>
        public static string Texture2DToBase64(this byte[] bytesArr)
        {
            return Convert.ToBase64String(bytesArr);
        }

        /// <summary>
        /// 加入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="sp"></param>
        /// <returns></returns>
        public static string Join<T>(this IEnumerable<T> source, string sp)
        {
            return string.Join(sp, source);
        }

        /// <summary>
        /// 字典转到字符串A:1|B:2|C:3这类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="dict"></param>
        /// <param name="delimeter1"></param>
        /// <param name="delimeter2"></param>
        /// <returns></returns>
        public static string DictToSplitStr<T, K>(Dictionary<T, K> dict, char delimeter1 = '|', char delimeter2 = ':')
        {
            if (dict == null || dict.Count == 0)
            {
                return string.Empty;
            }

            StringBuilder sb = new StringBuilder();
            foreach (var kvp in dict)
            {
                sb.Append(kvp.Key);
                sb.Append(delimeter2);
                sb.Append(kvp.Value);
                sb.Append(delimeter1);
            }
            sb.Remove(sb.Length - 1, 1); // 移除最后一个分隔符
            return sb.ToString();
        }

        /// <summary>
        /// A:1|B:2|C:3这类字符串转成字典
        /// </summary>
        /// <typeparam name="T">string</typeparam>
        /// <typeparam name="K">string</typeparam>
        /// <param name="str">原始字符串</param>
        /// <param name="delimeter1">分隔符1</param>
        /// <param name="delimeter2">分隔符2</param>
        /// <returns></returns>
        public static Dictionary<T, K> SplitToDict<T, K>(string str, char delimeter1 = '|', char delimeter2 = ':')
        {
            var dict = new Dictionary<T, K>();
            if (string.IsNullOrEmpty(str)) return dict;

            var pairs = str.Split(delimeter1);
            foreach (var pair in pairs)
            {
                var keyValue = pair.Split(delimeter2);

                // 跳过无效或不完整的键值对
                if (keyValue.Length != 2)
                {
                    continue;
                }
                T key = (T)Convert.ChangeType(keyValue[0], typeof(T));
                K value = (K)Convert.ChangeType(keyValue[1], typeof(K));
                // 使用索引器添加或更新字典项
                dict[key] = value; 
            }

            return dict;
        }

        /// <summary>
        /// 字符串转枚举
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        /// <returns></returns>
        public static T ToEnum<T>(this string e)
        {
            return (T)Enum.Parse(typeof(T), e);
        }

        #region 压缩/解压缩
        /// <summary>
        /// 字符串压缩
        /// </summary>
        /// <param name="strSource"></param>
        /// <returns></returns>
        public static byte[] Compress(byte[] data)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                GZipStream zip = new GZipStream(ms, CompressionMode.Compress, true);
                zip.Write(data, 0, data.Length);
                zip.Close();
                byte[] buffer = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(buffer, 0, buffer.Length);
                ms.Close();
                return buffer;

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// 字符串解压缩
        /// </summary>
        /// <param name="strSource"></param>
        /// <returns></returns>
        public static byte[] Decompress(byte[] data)
        {
            try
            {
                MemoryStream ms = new MemoryStream(data);
                GZipStream zip = new GZipStream(ms, CompressionMode.Decompress, true);
                MemoryStream msreader = new MemoryStream();
                byte[] buffer = new byte[0x1000];
                while (true)
                {
                    int reader = zip.Read(buffer, 0, buffer.Length);
                    if (reader <= 0)
                    {
                        break;
                    }
                    msreader.Write(buffer, 0, reader);
                }
                zip.Close();
                ms.Close();
                msreader.Position = 0;
                buffer = msreader.ToArray();
                msreader.Close();
                return buffer;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        /// <summary>
        /// 压缩字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string CompressString(string str)
        {
            string compressString = "";
            byte[] compressBeforeByte = Encoding.GetEncoding("UTF-8").GetBytes(str);
            byte[] compressAfterByte = Compress(compressBeforeByte);
            compressString = Convert.ToBase64String(compressAfterByte);
            return compressString;

        }

        /// <summary>
        /// 解压字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string DecompressString(string str)
        {
            string compressString = "";
            byte[] compressBeforeByte = Convert.FromBase64String(str);
            byte[] compressAfterByte = Decompress(compressBeforeByte);
            compressString = Encoding.GetEncoding("UTF-8").GetString(compressAfterByte);
            return compressString;
        }
        #endregion

        #region StringHelper
        public static IEnumerable<byte> ToBytes(this string str)
        {
            byte[] byteArray = Encoding.Default.GetBytes(str);
            return byteArray;
        }

        public static byte[] ToByteArray(this string str)
        {
            byte[] byteArray = Encoding.Default.GetBytes(str);
            return byteArray;
        }

        public static byte[] ToUtf8(this string str)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(str);
            return byteArray;
        }

        public static byte[] HexToBytes(this string hexString)
        {
            if (hexString.Length % 2 != 0)
            {
                Debug.Log(String.Format(CultureInfo.InvariantCulture, "二进制密钥不能有奇数位数: {0}", hexString));
                //throw new ArgumentException();
            }

            var hexAsBytes = new byte[hexString.Length / 2];
            for (int index = 0; index < hexAsBytes.Length; index++)
            {
                string byteValue = "";
                byteValue += hexString[index * 2];
                byteValue += hexString[index * 2 + 1];
                hexAsBytes[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }
            return hexAsBytes;
        }

        public static string Fmt(this string text, params object[] args)
        {
            return string.Format(text, args);
        }

        public static string ListToString<T>(this List<T> list)
        {
            StringBuilder sb = new StringBuilder();
            foreach (T t in list)
            {
                sb.Append(t);
                sb.Append(",");
            }
            return sb.ToString();
        }
        #endregion

        #region ByteHelper
        public static string ToHex(this byte b)
        {
            return b.ToString("X2");
        }

        public static string ToHex(this byte[] bytes)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte b in bytes)
            {
                stringBuilder.Append(b.ToString("X2"));
            }
            return stringBuilder.ToString();
        }

        public static string ToHex(this byte[] bytes, string format)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte b in bytes)
            {
                stringBuilder.Append(b.ToString(format));
            }
            return stringBuilder.ToString();
        }

        public static string ToHex(this byte[] bytes, int offset, int count)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = offset; i < offset + count; ++i)
            {
                stringBuilder.Append(bytes[i].ToString("X2"));
            }
            return stringBuilder.ToString();
        }

        public static string ToStr(this byte[] bytes)
        {
            return Encoding.Default.GetString(bytes);
        }

        public static string ToStr(this byte[] bytes, int index, int count)
        {
            return Encoding.Default.GetString(bytes, index, count);
        }

        public static string Utf8ToStr(this byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }

        public static string Utf8ToStr(this byte[] bytes, int index, int count)
        {
            return Encoding.UTF8.GetString(bytes, index, count);
        }

        public static void WriteTo(this byte[] bytes, int offset, uint num)
        {
            bytes[offset] = (byte)(num & 0xff);
            bytes[offset + 1] = (byte)((num & 0xff00) >> 8);
            bytes[offset + 2] = (byte)((num & 0xff0000) >> 16);
            bytes[offset + 3] = (byte)((num & 0xff000000) >> 24);
        }

        public static void WriteTo(this byte[] bytes, int offset, int num)
        {
            bytes[offset] = (byte)(num & 0xff);
            bytes[offset + 1] = (byte)((num & 0xff00) >> 8);
            bytes[offset + 2] = (byte)((num & 0xff0000) >> 16);
            bytes[offset + 3] = (byte)((num & 0xff000000) >> 24);
        }

        public static void WriteTo(this byte[] bytes, int offset, byte num)
        {
            bytes[offset] = num;
        }

        public static void WriteTo(this byte[] bytes, int offset, short num)
        {
            bytes[offset] = (byte)(num & 0xff);
            bytes[offset + 1] = (byte)((num & 0xff00) >> 8);
        }

        public static void WriteTo(this byte[] bytes, int offset, ushort num)
        {
            bytes[offset] = (byte)(num & 0xff);
            bytes[offset + 1] = (byte)((num & 0xff00) >> 8);
        }
        #endregion
    }
}