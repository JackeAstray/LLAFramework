//此脚本为工具生成，请勿手动创建 2025-05-04 14:06:55.589 <ExcelTo>
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
    public class InputSystemConfig
    {
        
        [PrimaryKey][AutoIncrement] public int Id{ get; set; }    //索引
        public int Number{ get; set; }    //编号
        public int Name{ get; set; }    //名称
        public string ActionName{ get; set; }    //动作名称
        public string BindingPath{ get; set; }    //按键绑定路径
        public string Interactions{ get; set; }    //输入交互
        public string Processors{ get; set; }    //输入处理器
        public string Groups{ get; set; }    //输入绑定的控制组

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
    }
}
