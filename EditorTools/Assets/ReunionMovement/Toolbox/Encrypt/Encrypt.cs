using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using GameLogic;

/// <summary>
/// 加密解密
/// </summary>
public class Encrypt
{
    #region AES加密解密
    public string GenerateAESKey()
    {
        using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
        {
            aes.GenerateKey();
            return Convert.ToBase64String(aes.Key);
        }
    }

    public string AESEncrypt(string text, string key)
    {
        try
        {
            Aes aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = new byte[aes.BlockSize / 8];
            ICryptoTransform encryptor = aes.CreateEncryptor();

            byte[] data = Encoding.UTF8.GetBytes(text);
            byte[] result = encryptor.TransformFinalBlock(data, 0, data.Length);
            return Convert.ToBase64String(result);
        }
        catch (Exception ex)
        {
            // 处理异常
            Log.Error("Encryption failed: " + ex.Message);
            return null;
        }
    }

    public string AESDecrypt(string text, string key)
    {
        Aes aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(key);
        aes.IV = new byte[aes.BlockSize / 8];
        ICryptoTransform decryptor = aes.CreateDecryptor();

        byte[] data = Convert.FromBase64String(text);
        byte[] result = decryptor.TransformFinalBlock(data, 0, data.Length);
        return Encoding.UTF8.GetString(result);
    }
    #endregion

    #region RSA加密解密
    public void GenerateRSAKey(out string publicKey, out string privateKey)
    {
        RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048); //2048是密钥大小
        publicKey = rsa.ToXmlString(false); //false以获取公钥
        privateKey = rsa.ToXmlString(true); //true以获取私钥
    }

    public string RSAEncrypt(string text, string publicKey)
    {
        RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
        rsa.FromXmlString(publicKey);

        byte[] data = Encoding.UTF8.GetBytes(text);
        byte[] result = rsa.Encrypt(data, false);
        return Convert.ToBase64String(result);
    }

    public string RSADecrypt(string text, string privateKey)
    {
        RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
        rsa.FromXmlString(privateKey);

        byte[] data = Convert.FromBase64String(text);
        byte[] result = rsa.Decrypt(data, false);
        return Encoding.UTF8.GetString(result);
    }
    #endregion

    #region Base64与字符串转换
    public string Base64Encode(string text)
    {
        byte[] data = Encoding.UTF8.GetBytes(text);
        return Convert.ToBase64String(data);
    }

    public string Base64Decode(string base64Text)
    {
        byte[] data = Convert.FromBase64String(base64Text);
        return Encoding.UTF8.GetString(data);
    }
    #endregion

    #region 压缩解压字符串
    public string CompressString(string text)
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

    public string DecompressString(string compressedText)
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