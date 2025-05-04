//此脚本为工具生成，请勿手动创建 2025-05-04 13:52:11.249 <ExcelTo>
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using SQLite.Attributes;

namespace LLAFramework
{
    [Serializable]
    public class ItemConfigDTO
    {
        
        public int Id;    //索引
        public int Number;    //编号
        public int Type;    //类型
        public bool Gacha;    //抽卡
        public int Star;    //星级
        public string Name;    //名称
        public string Describe;    //说明
        public double Gold;    //金币
        public double Crystal;    //水晶
        public int SkillPoint;    //技能点

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

        /// <summary>
        /// 将 DTO 转换为无 DTO 实例
        /// </summary>
        public ItemConfig ToEntity()
        {
            return new ItemConfig
            {
                Id         = this.Id,
                Number     = this.Number,
                Type       = this.Type,
                Gacha      = this.Gacha,
                Star       = this.Star,
                Name       = this.Name,
                Describe   = this.Describe,
                Gold       = this.Gold,
                Crystal    = this.Crystal,
                SkillPoint = this.SkillPoint,

            };
        }

        /// <summary>
        /// 从无 DTO 实例转换为 DTO
        /// </summary>
        public static ItemConfigDTO FromEntity(ItemConfig entity)
        {
            return new ItemConfigDTO
            {
                Id         = entity.Id,
                Number     = entity.Number,
                Type       = entity.Type,
                Gacha      = entity.Gacha,
                Star       = entity.Star,
                Name       = entity.Name,
                Describe   = entity.Describe,
                Gold       = entity.Gold,
                Crystal    = entity.Crystal,
                SkillPoint = entity.SkillPoint,

            };
        }
    }
}
