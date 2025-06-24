using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

namespace LLAFramework
{
    /// <summary>
    /// 文件工具类
    /// </summary>
    public static class FileTool
    {
        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="fullpath"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static async Task SaveFile(string fullpath, string content)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(content);
            await SaveFileAsync(fullpath, buffer);
        }

        /// <summary>
        /// 异步保存文件
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