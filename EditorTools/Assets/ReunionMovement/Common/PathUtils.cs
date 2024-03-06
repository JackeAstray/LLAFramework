using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 路径工具类
    /// </summary>
    public static class PathUtils
    {
        public static readonly string[] PathHeadDefine = { "jar://", "jar:file:///", "file:///", "http://", "https://" };

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
        public static string GetFileName(string path)
        {
            return Path.GetFileNameWithoutExtension(path);
        }

        /// <summary>
        /// 获取父路径
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
    }
}