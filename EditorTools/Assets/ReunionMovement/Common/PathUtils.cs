using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static GameLogic.Download.DownloadFileModule;

namespace GameLogic
{
    /// <summary>
    /// 路径工具类
    /// </summary>
    public static class PathUtils
    {
        public static readonly string[] PathHeadDefine = { "jar://", "jar:file://", "file://", "file:///", "http://", "https://" };

        public static string GetPathHead(string path)
        {
            for (int i = 0; i < PathHeadDefine.Length; i++)
            {
                if (path.StartsWith(PathHeadDefine[i]))
                {
                    return PathHeadDefine[i];
                }
            }

            return "";
        }

        /// <summary>
        /// 获取文件协议
        /// </summary>
        public static string GetFileProtocol
        {
            get
            {
                string fileProtocol = "file://";
                return fileProtocol;
            }
        }

        /// <summary>
        /// 获取规范的路径
        /// </summary>
        public static string GetRegularPath(string path)
        {
            return path.Replace('\\', '/');
        }

        /// <summary>
        /// 验证路径（是否为真路径）
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsSureDir(string path)
        {
            return path.Contains("/") || path.Contains("\\");
        }

        /// <summary>
        /// 验证路径（是否为全路径）
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsFullPath(string path)
        {
            return path.Contains(":/") || path.Contains(":\\");
        }

        /// <summary>
        /// 获取文件名
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetFileName(string path, bool withSuffix = true)
        {
            if (withSuffix)
            {
                return Path.GetFileName(path);
            }
            else
            {
                return Path.GetFileNameWithoutExtension(path);
            }
        }

        /// <summary>
        /// 获取路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetParentDir(string path)
        {
            return Path.GetDirectoryName(path);
        }

        /// <summary>
        /// app的数据目录，有读写权限
        /// </summary>
        /// <returns></returns>
        public static string AppDataPath()
        {
            return Application.persistentDataPath + "/";
        }

        /// <summary>
        /// 获取完整路径
        /// </summary>
        /// <param name="url"></param>
        /// <param name="newPath"></param>
        /// <returns></returns>
        public static bool GetFullPath(string url, out string newPath)
        {
            newPath = Path.GetFullPath(AppDataPath() + url);
            return File.Exists(newPath);
        }

        /// <summary>
        /// 获取在只读区下的完整路径
        /// </summary>
        public static string GetReadOnlyPath(string path, bool isUwrPath = false)
        {
            string result = GetRegularPath(Path.Combine(Application.streamingAssetsPath, path));

            if (isUwrPath && !path.Contains("file://"))
            {
                //使用UnityWebRequest访问 统一加file://头
                result = "file://" + result;
            }

            return result;
        }

        /// <summary>
        /// 获取在读写区下的完整路径
        /// </summary>
        public static string GetReadWritePath(string path, bool isUwrPath = false)
        {
            string result = GetRegularPath(Path.Combine(Application.persistentDataPath, path));

            if (isUwrPath && !path.Contains("file://"))
            {
                //使用UnityWebRequest访问 统一加file://头
                result = "file://" + result;
            }

            return result;
        }

        public static string GetUrlByPC(string path)
        {
            if (!path.Contains("://"))
            {
                path = GetFileProtocol + path;
            }

            return path;
        }



        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string ReadFile(string filePath, string fileName)
        {
            string fullPath;
            bool exists = GetFullPath(filePath + fileName, out fullPath);

            if (exists)
            {
                return File.ReadAllText(fullPath);
            }

            return string.Empty;
        }

        /// <summary>
        /// 写入文件
        /// </summary>
        /// <param name="fileContent"></param>
        /// <param name="filePath"></param>
        /// <param name="fileName"></param>
        public static void WriteFile(string fileContent, string filePath, string fileName)
        {
            string fullPath;
            GetFullPath(filePath, out fullPath);

            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }

            File.WriteAllText(fullPath + fileName, fileContent);
        }

        /// <summary>
        /// 获取要保存的路径
        /// </summary>
        /// <returns></returns>
        public static string GetLocalPath(DownloadType downloadType = DownloadType.PersistentFile)
        {
            string savePath = Application.persistentDataPath + "/Download";

            switch (downloadType)
            {
                case DownloadType.PersistentAssets:
                    savePath = Application.persistentDataPath + "/Assets";
                    break;
                case DownloadType.PersistentFile:
                    savePath = Application.persistentDataPath + "/File";
                    break;
                case DownloadType.PersistentImage:
                    savePath = Application.persistentDataPath + "/Picture";
                    break;

                case DownloadType.StreamingAssets:
                    savePath = Application.streamingAssetsPath + "/Download";
                    break;
                case DownloadType.StreamingAssetsFile:
                    savePath = Application.streamingAssetsPath + "/File";
                    break;
                case DownloadType.StreamingAssetsImage:
                    savePath = Application.streamingAssetsPath + "/Picture";
                    break;
            }

            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            return savePath;
        }
    }
}