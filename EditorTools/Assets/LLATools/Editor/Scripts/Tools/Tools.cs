using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

namespace GameLogic.Editor
{
    public static class Tools
    {
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
    }
}