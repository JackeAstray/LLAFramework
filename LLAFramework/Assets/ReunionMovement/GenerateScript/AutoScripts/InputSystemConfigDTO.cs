//此脚本为工具生成，请勿手动创建 2025-05-04 13:52:11.240 <ExcelTo>
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using SQLite.Attributes;

namespace LLAFramework
{
    [Serializable]
    public class InputSystemConfigDTO
    {
        
        public int Id;    //索引
        public int Number;    //编号
        public int Name;    //名称
        public string ActionName;    //动作名称
        public string BindingPath;    //按键绑定路径
        public string Interactions;    //输入交互
        public string Processors;    //输入处理器
        public string Groups;    //输入绑定的控制组

        public override string ToString()
        {
            return string.Format(
                "[Id={1},Number={2},Name={3},ActionName={4},BindingPath={5},Interactions={6},Processors={7},Groups={8}]",
                this.Id,
                this.Number,
                this.Name,
                this.ActionName,
                this.BindingPath,
                this.Interactions,
                this.Processors,
                this.Groups
            );
        }

        /// <summary>
        /// 将 DTO 转换为无 DTO 实例
        /// </summary>
        public InputSystemConfig ToEntity()
        {
            return new InputSystemConfig
            {
                Id           = this.Id,
                Number       = this.Number,
                Name         = this.Name,
                ActionName   = this.ActionName,
                BindingPath  = this.BindingPath,
                Interactions = this.Interactions,
                Processors   = this.Processors,
                Groups       = this.Groups,

            };
        }

        /// <summary>
        /// 从无 DTO 实例转换为 DTO
        /// </summary>
        public static InputSystemConfigDTO FromEntity(InputSystemConfig entity)
        {
            return new InputSystemConfigDTO
            {
                Id           = entity.Id,
                Number       = entity.Number,
                Name         = entity.Name,
                ActionName   = entity.ActionName,
                BindingPath  = entity.BindingPath,
                Interactions = entity.Interactions,
                Processors   = entity.Processors,
                Groups       = entity.Groups,

            };
        }
    }
}
