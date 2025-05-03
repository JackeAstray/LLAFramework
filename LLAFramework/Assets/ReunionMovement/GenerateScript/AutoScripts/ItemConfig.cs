//此脚本为工具生成，请勿手动创建 2025-04-16 15:54:32.712 <ExcelTo>
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using SQLite.Attributes;

namespace LLAFramework
{
    [Serializable]
    [UnityEngine.Scripting.Preserve]
    public class ItemConfig
    {
        
        [PrimaryKey][AutoIncrement] public int Id{ get; set; }    //索引
        public int Number{ get; set; }    //编号
        public int Type{ get; set; }    //类型
        public bool Gacha{ get; set; }    //抽卡
        public int Star{ get; set; }    //星级
        public string Name{ get; set; }    //名称
        public string Describe{ get; set; }    //说明
        public double Gold{ get; set; }    //金币
        public double Crystal{ get; set; }    //水晶
        public int SkillPoint{ get; set; }    //技能点

        public override string ToString()
        {
            return string.Format(
                "[Id={1},Number={2},Type={3},Gacha={4},Star={5},Name={6},Describe={7},Gold={8},Crystal={9},SkillPoint={10}]",
                this.Id,
                this.Number,
                this.Type,
                this.Gacha,
                this.Star,
                this.Name,
                this.Describe,
                this.Gold,
                this.Crystal,
                this.SkillPoint
            );
        }
    }
}
