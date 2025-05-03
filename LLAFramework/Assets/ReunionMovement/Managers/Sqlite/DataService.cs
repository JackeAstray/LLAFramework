using SQLite;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using LLAFramework.Download;
using LLAFramework.Http;
using LLAFramework.Http.Service;
using System;
using SqlCipher4Unity3D;

namespace LLAFramework.Sqlite
{
    public class DataService
    {
        private SQLiteConnection connection;

        string databaseName;
        string filepath;
        string password;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="databaseName"></param>
        public DataService(string databaseName, string password = null)
        {
            this.databaseName = databaseName;
            this.password = password;
            string dbPath = string.Format(@"Assets/StreamingAssets/{0}", databaseName);
            filepath = PathUtils.GetReadWritePath($"/DB/{databaseName}");

            switch (Application.platform)
            {
                case RuntimePlatform.LinuxEditor:
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.WindowsEditor:
                    InitializeConnection(dbPath, password);
                    break;

                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.OSXPlayer:
                case RuntimePlatform.IPhonePlayer:
                    HandlePlayerPlatform();
                    break;

                case RuntimePlatform.Android:
                    HandleAndroidPlatform();
                    break;

                case RuntimePlatform.WebGLPlayer:
                    Log.Error("WebGLPlayer不能直接操作Sqlite");
                    return;

                default:
                    break;
            }
        }

        /// <summary>
        /// 初始化连接
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="dbPath"></param>
        /// <param name="password"></param>
        private void InitializeConnection(string dbPath, string password)
        {
            connection = new SQLiteConnection(dbPath, password);
        }

        /// <summary>
        /// 处理平台
        /// </summary>
        private void HandlePlayerPlatform()
        {
            if (!File.Exists(filepath))
            {
                string loadDb = GetStreamingAssetsPath();
                File.Copy(loadDb, filepath);
            }
            InitializeConnection(filepath, password);
        }

        /// <summary>
        /// 获取StreamingAssets路径
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        private string GetStreamingAssetsPath()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsPlayer:
                    return Application.dataPath + "/StreamingAssets/" + databaseName;
                case RuntimePlatform.OSXPlayer:
                    return Application.dataPath + "/Resources/Data/StreamingAssets/" + databaseName;
                case RuntimePlatform.IPhonePlayer:
                    return Application.dataPath + "/Raw/" + databaseName;
                default:
                    throw new System.Exception("Unsupported platform");
            }
        }

        /// <summary>
        /// 处理安卓平台
        /// </summary>
        private void HandleAndroidPlatform()
        {
            if (!File.Exists(filepath))
            {
                string path = PathUtils.GetReadOnlyPath($"/{databaseName}", true);
                var request = HttpModule.Get(path)
                    .OnSuccess(GetDB)
                    .OnError(response => Debug.LogError(response.StatusCode))
                    .Send();
            }
            else
            {
                InitializeConnection(filepath, password);
            }
        }

        /// <summary>
        /// 获取数据库
        /// </summary>
        /// <param name="response"></param>
        private void GetDB(HttpResponse response)
        {
            string filepath = PathUtils.GetReadWritePath($"/DB/{this.databaseName}");
            File.WriteAllBytes(filepath, response.Bytes);
            InitializeConnection(filepath, password);
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public void Close()
        {
            connection.Close();
        }

        #region 增删改查
        /// <summary>
        /// 创建表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void CreateTable<T>()
        {
            connection.CreateTable<T>();
        }

        /// <summary>
        /// 删除表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void DropTable<T>()
        {
            connection.DropTable<T>();
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        public void Insert<T>(T obj)
        {
            connection.Insert(obj);
        }

        /// <summary>
        /// 批量插入数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objs"></param>
        public void InsertAll<T>(IEnumerable<T> objs)
        {
            connection.InsertAll(objs);
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        public void Delete<T>(T obj)
        {
            connection.Delete(obj);
        }

        /// <summary>
        /// 删除所有数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void DeleteAll<T>()
        {
            connection.DeleteAll<T>();
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        public void Update<T>(T obj)
        {
            connection.Update(obj);
        }

        /// <summary>
        /// 批量更新数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objs"></param>
        public void UpdateAll<T>(IEnumerable<T> objs)
        {
            connection.UpdateAll(objs);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public IEnumerable<T> Query<T>(string query, params object[] args) where T : new()
        {
            return connection.Query<T>(query, args);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public IEnumerable<T> Query<T>(string query) where T : new()
        {
            return connection.Query<T>(query);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> Query<T>() where T : new()
        {
            return connection.Table<T>();
        }

        /// <summary>
        /// 执行 - 命令 - 无参数
        /// </summary>
        /// <param name="query"></param>
        public void Execute(string query)
        {
            connection.Execute(query);
        }

        /// <summary>
        /// 执行 - 命令 - 带参数
        /// </summary>
        /// <param name="query"></param>
        /// <param name="args"></param>
        public void Execute(string query, params object[] args)
        {
            connection.Execute(query, args);
        }
        #endregion
    }
}