using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameLogic.Base;

namespace GameLogic.Sqlite
{
    public class SqliteMgr : SingletonMgr<SqliteMgr>
    {
        private DataService dataService;

        // 初始化数据库
        public void Initialize(string dbName, string password = null)
        {
            dataService = new DataService(dbName, password);
        }


        //// 查询所有用户信息
        //public IEnumerable<T> GetAllUsers()
        //{
        //    return dataService.Query<T>();
        //}

        //// 根据账户查询用户信息
        //public IEnumerable<UserInfo> GetUsersByAccount(string account)
        //{
        //    return dataService.Query<UserInfo>("SELECT * FROM UserInfo WHERE Account = ?", account);
        //}

        //// 根据名称查询用户信息
        //public IEnumerable<UserInfo> GetUsersByName(string name)
        //{
        //    return dataService.Query<UserInfo>("SELECT * FROM UserInfo WHERE Name LIKE ?", $"%{name}%");
        //}

        //// 插入用户信息
        //public void InsertUser(UserInfo user)
        //{
        //    dataService.Insert(user);
        //}

        //// 更新用户信息
        //public void UpdateUser(UserInfo user)
        //{
        //    dataService.Update(user);
        //}

        //// 删除用户信息
        //public void DeleteUser(UserInfo user)
        //{
        //    dataService.Delete(user);
        //}

        // 关闭数据库连接
        public void Close()
        {
            dataService.Close();
        }
    }
}