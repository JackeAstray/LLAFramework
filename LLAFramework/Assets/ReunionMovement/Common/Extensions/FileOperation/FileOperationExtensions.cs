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
    /// �ļ�������չ
    /// </summary>
    public class FileOperationExtensions
    {
        /// <summary>
        /// �������ļ���ֱ�Ӷ�bytes  ��ȡ�����أ�����
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
        /// �����ļ�
        /// </summary>
        /// <param name="fullpath">����·��</param>
        /// <param name="content">����</param>
        /// <returns></returns>
        public static async Task SaveFile(string fullpath, string content)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(content);
            await SaveFileAsync(fullpath, buffer);
        }

        /// <summary>
        /// �����ļ�
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

                    string dir = Path.GetDirectoryName(fullpath);

                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }

                    File.WriteAllBytes(fullpath, content);
                    return content.Length;
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex + " SaveFile");
                throw;
            }
        }

        /// <summary>
        /// ����Json
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
        /// ����Json
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
        /// ��Ϸ��ʼ��StreamingAssets�ļ����Ƶ��־û�Ŀ¼
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
                //�����ļ���
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
                            Debug.Log("�����ļ�ʧ�ܣ�" + www.error);
                        }
                        else
                        {
                            //Debug.Log("���Ƴɹ�");
                            File.WriteAllBytes(TargetPath + "/" + fileName, www.downloadHandler.data);
                        }
                    }
                    break;
                case RuntimePlatform.IPhonePlayer:
                    //IOS��StreamingAssetsĿ¼
                    OriginalPath = Application.dataPath + "/Raw/" + filePath + "/" + fileName;
                    if (!File.Exists(TargetPath + "/" + fileName))
                    {
                        //���浽�־û�Ŀ¼
                        File.Copy(OriginalPath, TargetPath + "/" + fileName);
                    }
                    break;
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.OSXPlayer:
                    if (!File.Exists(TargetPath + "/" + fileName))
                    {
                        //���浽�־û�Ŀ¼
                        File.Copy(OriginalPath, TargetPath + "/" + fileName);
                    }
                    break;
            }
            yield return null;
        }

        #region ��ȡӲ����Ϣ
        /// <summary>
        /// ��ȡ����IP
        /// </summary>
        /// <returns></returns>
        public static string GetLocalIP()
        {
            try
            {
                //��ȡ������
                string hostName = Dns.GetHostName();
                //��ȡ����IP
                IPHostEntry ipEntry = Dns.GetHostEntry(hostName);
                var ipAddress = ipEntry.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
                return ipAddress?.ToString() ?? "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// �첽��ȡIP��ַ
        /// </summary>
        /// <param name="host"></param>
        /// <param name="callback"></param>
        public static async Task<IPAddress> GetIpAddress(string host, Action<IPAddress> callback = null)
        {
            if (IPAddress.TryParse(host, out IPAddress ipAddress))
            {
                return ipAddress;
            }
            else
            {
                var addresses = await Dns.GetHostAddressesAsync(host);
                return addresses.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
            }
        }
        #endregion
    }
}