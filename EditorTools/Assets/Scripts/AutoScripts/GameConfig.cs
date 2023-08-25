//此脚本为自动生成 2023-08-25 19:09:58.551 <ExcelTo>
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
        
        public int Id { get; set; }    //索引
        public int LanguageID { get; set; }    //语言ID
        public string Title { get; set; }    //文本介绍
        public string Value { get; set; }    //值


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
