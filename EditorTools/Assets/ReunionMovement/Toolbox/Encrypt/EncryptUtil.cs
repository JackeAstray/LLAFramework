using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using GameLogic;

namespace GameLogic
{
    /// <summary>
    /// 加密解密
    /// </summary>
    public static class EncryptUtil
    {
        /// <summary>
        /// 加密字节长度
        /// </summary>
        public const int EncryptBytesLength = 64;

        /// <summary>
        /// 偏移加密的头部字节值
        /// </summary>
        private const byte encryptOffsetHead = 64;

        /// <summary>
        /// 异或加密Key
        /// </summary>
        private const byte encryptXOrKey = 64;

        private static Queue<byte[]> cachedBytesQueue = new Queue<byte[]>();

        private static byte[] GetCachedBytes(int length)
        {
            if (cachedBytesQueue.Count == 0)
            {
                return new byte[length];
            }

            byte[] bytes = cachedBytesQueue.Peek();

            if (bytes.Length < length)
            {
                return new byte[length];
            }

            return cachedBytesQueue.Dequeue();
        }

        private static void ReleaseCachedBytes(byte[] bytes)
        {
            cachedBytesQueue.Enqueue(bytes);
        }

        /// <summary>
        /// 偏移加密
        /// </summary>
        public static void EncryptOffset(string filePath)
        {
            byte[] bytes = File.ReadAllBytes(filePath);
            int newLength = bytes.Length + EncryptBytesLength;

            byte[] cachedBytes = GetCachedBytes(newLength);

            //写入额外的头部数据
            for (int i = 0; i < EncryptBytesLength; i++)
            {
                cachedBytes[i] = encryptOffsetHead;
            }

            //写入原始数据
            Array.Copy(bytes, 0, cachedBytes, EncryptBytesLength, bytes.Length);
            using (FileStream fs = File.OpenWrite(filePath))
            {
                fs.Position = 0;
                fs.Write(cachedBytes, 0, newLength);
            }

            Array.Clear(cachedBytes, 0, newLength);
            ReleaseCachedBytes(cachedBytes);
        }

        /// <summary>
        /// 使用Stream进行异或加密
        /// </summary>
        public static void EncryptXOr(string filePath)
        {
            byte[] cachedBytes = GetCachedBytes(EncryptBytesLength);

            using (FileStream fs = File.Open(filePath, FileMode.Open))
            {
                int _ = fs.Read(cachedBytes, 0, EncryptBytesLength);
                EncryptXOr(cachedBytes);
                fs.Position = 0;
                fs.Write(cachedBytes, 0, encryptXOrKey);
            }

            Array.Clear(cachedBytes, 0, cachedBytes.Length);
            ReleaseCachedBytes(cachedBytes);
        }

        /// <summary>
        /// 使用二进制数据进行异或加密/解密
        /// </summary>
        public static void EncryptXOr(byte[] bytes, long length = EncryptBytesLength)
        {
            for (long i = 0; i < length; i++)
            {
                bytes[i] ^= encryptXOrKey;
            }
        }

        #region Base64与字符串转换
        public static string Base64Encode(string text)
        {
            byte[] data = Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(data);
        }

        public static string Base64Decode(string base64Text)
        {
            byte[] data = Convert.FromBase64String(base64Text);
            return Encoding.UTF8.GetString(data);
        }
        /// <summary>
        /// 判断是否是Base64
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsBase64String(string input)
        {
            try
            {
                // 尝试将字符串解码为字节数组
                byte[] bytes = Convert.FromBase64String(input);
                return true; // 如果成功解码，说明是有效的Base64编码
            }
            catch (FormatException)
            {
                return false; // 解码失败，不是有效的Base64编码
            }
        }
        #endregion

        #region 压缩解压字符串
        public static string CompressString(string text)
        {
            byte[] data = Encoding.UTF8.GetBytes(text);
            using (var compressedStream = new MemoryStream())
            using (var zipStream = new GZipStream(compressedStream, CompressionMode.Compress))
            {
                zipStream.Write(data, 0, data.Length);
                zipStream.Close();
                return Convert.ToBase64String(compressedStream.ToArray());
            }
        }

        public static string DecompressString(string compressedText)
        {
            byte[] data = Convert.FromBase64String(compressedText);
            using (var compressedStream = new MemoryStream(data))
            using (var zipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
            using (var resultStream = new MemoryStream())
            {
                zipStream.CopyTo(resultStream);
                return Encoding.UTF8.GetString(resultStream.ToArray());
            }
        }
        #endregion
    }
}