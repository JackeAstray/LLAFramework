using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace LLAFramework
{
    /// <summary>
    /// 文件操作扩展
    /// </summary>
    public static class FileOperation
    {
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
                return await Task.Run(async () =>
                {
                    if (content == null)
                    {
                        content = new byte[0];
                    }

                    string dir = Path.GetDirectoryName(fullpath);

                    if (!Directory.Exists(dir))
                    {
                        try
                        {
                            Directory.CreateDirectory(dir);
                        }
                        catch (Exception e)
                        {
                            Log.Error($"SaveFile() 创建目录错误! 目录:{dir}, 错误:{e.Message}");
                            return -1;
                        }
                    }

                    try
                    {
                        using (FileStream fs = new FileStream(fullpath, FileMode.Create, FileAccess.Write))
                        {
                            await fs.WriteAsync(content, 0, content.Length);
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error($"SaveFile() 路径:{fullpath}, 错误:{e.Message}");
                        return -1;
                    }

                    return content.Length;
                });
            }
            catch (Exception ex)
            {
                Debug.LogError($"{ex} SaveFile");
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


        /// <summary>
        /// 检查类名是否合法
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool CheckClassName(string str)
        {
            if (string.IsNullOrEmpty(str) || !char.IsUpper(str[0]))
            {
                return false;
            }

            for (int i = 1; i < str.Length; i++)
            {
                if (!char.IsLetterOrDigit(str[i]) && str[i] != '_')
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 检查字段名是否合法
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool CheckFieldName(string name)
        {
            if (string.IsNullOrEmpty(name) || (!char.IsLetter(name[0]) && name[0] != '_'))
            {
                return false;
            }

            for (int i = 1; i < name.Length; i++)
            {
                if (!char.IsLetterOrDigit(name[i]) && name[i] != '_')
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 将字符串的首字母大写
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string CapitalFirstChar(string str)
        {
            return char.ToUpper(str[0]) + str.Substring(1);
        }
    }
}