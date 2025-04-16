//此脚本为工具生成，请勿手动创建 2025-04-16 15:54:32.762 <ExcelTo>
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
    public class SoundConfig
    {
        
        [PrimaryKey][AutoIncrement] public int Id{ get; set; }    //索引
        public int Number{ get; set; }    //编号
        public string Path{ get; set; }    //路径
        public string Name{ get; set; }    //名称
        public int Type{ get; set; }    //类型
        public string Detailed{ get; set; }    //介绍

        public override string ToString()
        {
            return string.Format(
                "[Id={1},Number={2},Path={3},Name={4},Type={5},Detailed={6}]",
                this.Id,
                this.Number,
                this.Path,
                this.Name,
                this.Type,
                this.Detailed
            );
        }
    }
}
