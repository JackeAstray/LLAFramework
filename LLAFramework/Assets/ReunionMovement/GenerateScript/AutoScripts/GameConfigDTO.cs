//此脚本为工具生成，请勿手动创建 2025-04-08 17:12:01.307 <ExcelTo>
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using SQLite.Attributes;

namespace LLAFramework
{
    [Serializable]
    public class GameConfigDTO
    {
        
        public int Id;    //索引
        public int Number;    //编号
        public int LanguageID;    //语言ID
        public string Title;    //文本介绍
        public string Value;    //值

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

        /// <summary>
        /// 将 DTO 转换为无 DTO 实例
        /// </summary>
        public GameConfig ToEntity()
        {
            return new GameConfig
            {
                Id         = this.Id,
                Number     = this.Number,
                LanguageID = this.LanguageID,
                Title      = this.Title,
                Value      = this.Value,

            };
        }

        /// <summary>
        /// 从无 DTO 实例转换为 DTO
        /// </summary>
        public static GameConfigDTO FromEntity(GameConfig entity)
        {
            return new GameConfigDTO
            {
                Id         = entity.Id,
                Number     = entity.Number,
                LanguageID = entity.LanguageID,
                Title      = entity.Title,
                Value      = entity.Value,

            };
        }
    }
}
