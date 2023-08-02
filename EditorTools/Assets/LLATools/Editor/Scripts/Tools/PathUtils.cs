using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GameLogic
{
    public class PathUtils
    {
        public static readonly string[] PathHeadDefine = { "jar://", "jar:file:///", "file:///", "http://", "https://" };

        /// <summary>
        /// 验证路径（是否为真路径）
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsSureDir(string path)
        {
            int i = path.LastIndexOf("/");
            if (i >= 0)
            {
                return true;
            }

            i = path.LastIndexOf("\\");
            if (i >= 0)
            {
                return true;
            }

            return false;
        }
        /// <summary>
        /// 验证路径（是否为全路径）
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsFullPath(string path)
        {
            int i = path.IndexOf(":/");
            if (i >= 0)
            {
                return true;
            }

            i = path.IndexOf(":\\");
            if (i >= 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 获取文件名
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetFileName(string path)
        {
            string parent = "", child = "";
            SplitPath(path, ref parent, ref child, true);
            return child;
        }

        /// <summary>
        /// 获取父路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetParentDir(string path)
        {
            string parent = "", child = "";
            SplitPath(path, ref parent, ref child, true);
            return parent;
        }

        /// <summary>
        /// 路径拆分
        /// </summary>
        /// <param name="path"></param>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        /// <param name="bSplitExt"></param>
        /// <returns></returns>
        public static string SplitPath(string path, ref string parent, ref string child, bool bSplitExt = false)
        {
            string ext = "";
            string head = SplitPath(path, ref parent, ref child, ref ext);
            if (bSplitExt)
            {
                return head;
            }

            if (!string.IsNullOrEmpty(ext))
            {
                child = child + "." + ext;
            }

            return head;
        }

        /// <summary>
        /// 路径拆分
        /// </summary>
        /// <param name="path"></param>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        /// <param name="ext"></param>
        /// <returns></returns>
        public static string SplitPath(string path, ref string parent, ref string child, ref string ext)
        {
            string head = GetPathHead(path);

            int index = path.LastIndexOf("/");
            int index2 = path.LastIndexOf("\\");
            index = System.Math.Max(index, index2);

            if (index == head.Length - 1)
            {
                parent = "";
                child = path;
            }
            else
            {
                parent = path.Substring(0, index);
                child = path.Substring(index + 1);
            }

            index = child.LastIndexOf(".");
            if (index >= 0)
            {
                ext = child.Substring(index + 1);
                child = child.Substring(0, index);
            }

            return head;
        }

        /// <summary>
        /// 获取路径头
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
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
        /// app的数据目录，有读写权限，实际是Application.persistentDataPath，以/结尾
        /// </summary>
        public static string AppDataPath()
        {
            return Application.persistentDataPath + "/";
        }

        /// <summary>
        /// 获得完整路径
        /// </summary>
        /// <param name="url"></param>
        /// <param name="withFileProtocol">是否带有file://前缀</param>
        /// <param name="newUrl"></param>
        /// <returns></returns>
        public static bool GetFullPath(string url, out string newPath)
        {
            string path = "";
            path = Path.GetFullPath(AppDataPath() + url);
            newPath = path;
            return File.Exists(Path.GetFullPath(path));
        }

        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string ReadFile(string filePath, string fileName)
        {
            //获取完整路径
            string fullPath;
            string fileContent = "";
            bool exists = GetFullPath(filePath + fileName, out fullPath);

            if (exists)
            {
                FileStream fs = new FileStream(fullPath, FileMode.Open);
                StreamReader sr = new StreamReader(fs);
                fileContent = sr.ReadToEnd();

                sr.Close();
                if (fs != null)
                {
                    fs.Close();
                }
            }

            return fileContent;
        }

        /// <summary>
        /// 写入文件
        /// </summary>
        /// <param name="fileContent"></param>
        /// <param name="filePath"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static void WriteFile(string fileContent, string filePath, string fileName)
        {
            //获取完整路径
            string fullPath;
            GetFullPath(filePath , out fullPath);

            //创建路径
            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }

            byte[] bts = System.Text.Encoding.UTF8.GetBytes(fileContent);
            //写入文件，文件存在则覆盖
            File.WriteAllBytes(fullPath + fileName, bts);
        }
    }
}