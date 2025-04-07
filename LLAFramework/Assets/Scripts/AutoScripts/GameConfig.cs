//此脚本为工具生成，请勿手动创建 2025-04-07 16:35:33.779 <ExcelTo>
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace GameLogic
{
    [Serializable]
    public class GameConfig
    {
        
        public int Id;    //索引
        public int LanguageID;    //语言ID
        public string Title;    //文本介绍
        public string Value;    //值

        public override string ToString()
        {
            return string.Format(
                "[Id={1},LanguageID={2},Title={3},Value={4}]",
                this.Id,
                this.LanguageID,
                this.Title,
                this.Value
            );
        }
    }
}
