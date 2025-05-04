//此脚本为工具生成，请勿手动创建 2025-05-04 13:52:11.289 <ExcelTo>
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using SQLite.Attributes;

namespace LLAFramework
{
    [Serializable]
    public class SoundConfigDTO
    {
        
        public int Id;    //索引
        public int Number;    //编号
        public string Path;    //路径
        public string Name;    //名称
        public int Type;    //类型
        public string Detailed;    //介绍

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

        /// <summary>
        /// 将 DTO 转换为无 DTO 实例
        /// </summary>
        public SoundConfig ToEntity()
        {
            return new SoundConfig
            {
                Id       = this.Id,
                Number   = this.Number,
                Path     = this.Path,
                Name     = this.Name,
                Type     = this.Type,
                Detailed = this.Detailed,

            };
        }

        /// <summary>
        /// 从无 DTO 实例转换为 DTO
        /// </summary>
        public static SoundConfigDTO FromEntity(SoundConfig entity)
        {
            return new SoundConfigDTO
            {
                Id       = entity.Id,
                Number   = entity.Number,
                Path     = entity.Path,
                Name     = entity.Name,
                Type     = entity.Type,
                Detailed = entity.Detailed,

            };
        }
    }
}
