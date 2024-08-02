using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLite.Attributes;
using System.Xml;

namespace GameLogic.Sqlite
{
    [UnityEngine.Scripting.Preserve]
    public class UserInfo
    {
        [PrimaryKey][AutoIncrement] public int Id { get; set; }
        // 账户
        public string Account { get; set; }
        // 密码
        public string Password { get; set; }
        // 名称
        public string Name { get; set; }
        // 昵称
        public string Nickname { get; set; }
        // 唯一ID
        public string UniqueID { get; set; }
        // 头像
        public string Avatar { get; set; }

        public override string ToString()
        {
            return string.Format("Id:{0}, Account:{1}, Password:{2}, Name:{3}, Nickname:{4}, UniqueID:{5}, Avatar:{6}", Id, Account, Password, Name, Nickname, UniqueID, Avatar);
        }
    }
}