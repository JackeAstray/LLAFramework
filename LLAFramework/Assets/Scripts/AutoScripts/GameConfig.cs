//此脚本为工具生成，请勿手动创建 2025-04-08 21:02:41.132 <ExcelTo>
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using SQLite.Attributes;

namespace GameLogic
{
    [Serializable]
    [UnityEngine.Scripting.Preserve]
    public class GameConfig
    {
        
        [PrimaryKey][AutoIncrement] public int Id{ get; set; }    //索引
        public int Number{ get; set; }    //编号
        public int LanguageID{ get; set; }    //语言ID
        public string Title{ get; set; }    //文本介绍
        public string Value{ get; set; }    //值

        public override string ToString()
        {
            return string.Format(
                "[Id={1},Number={2},LanguageID={3},Title={4},Value={5}]",
                this.Id,
                this.Number,
                this.LanguageID,
                this.Title,
                this.Value
            );
        }
    }
}
