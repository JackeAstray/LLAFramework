using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameLogic.Sqlite;
using System;

namespace GameLogic.Example
{
    public class CreateDB : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
#if UNITY_EDITOR
            Invoke("StartSync", 3);
#endif
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void StartSync()
        {
            DataService ds = new DataService(SqliteConfig.UserDBName);
            ds.CreateDB();

            IEnumerable<UserInfo> userInfos = ds.Query<UserInfo>();
            
            foreach (var userInfo in userInfos)
            {
                Debug.Log(userInfo.ToString());
            }

            var users = ds.Query<UserInfo>("SELECT * FROM UserInfo WHERE Account = ?", "admin");
            foreach (var user in users)
            {
                Debug.Log($"Account: {user.Account}, Name: {user.Name}");
            }

            users = ds.Query<UserInfo>("SELECT * FROM UserInfo WHERE Name LIKE '%admin%'");
            foreach (var user in users)
            {
                Debug.Log($"Account: {user.Account}, Name: {user.Name}");
            }

            ds.Execute("INSERT INTO UserInfo (Account, Password, Name, Nickname, UniqueID, Avatar) VALUES (?, ?, ?, ?, ?, ?)",
                       "user1", "password1", "User One", "user1", Guid.NewGuid().ToString(), "");

            ds.Execute("INSERT INTO UserInfo (Account, Password, Name, Nickname, UniqueID, Avatar) VALUES (?, ?, ?, ?, ?, ?)",
                       "user2", "password2", "User One", "user2", Guid.NewGuid().ToString(), "");

            ds.Execute("INSERT INTO UserInfo (Account, Password, Name, Nickname, UniqueID, Avatar) VALUES (?, ?, ?, ?, ?, ?)",
                "user3", "password3", "User One", "user3", Guid.NewGuid().ToString(), "");

            ds.Execute("UPDATE UserInfo SET Password = ? WHERE Account = ?", "newpassword", "user2");

            ds.Execute("DELETE FROM UserInfo WHERE Account = ?", "user3");
        }
    }
}