//此脚本为工具生成，请勿手动创建 2025-04-07 16:35:33.802 <ExcelTo>
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace GameLogic
{
    [Serializable]
    public class SoundConfig
    {
        
        public int Id;    //索引
        public string Path;    //路径
        public string Name;    //名称
        public int Type;    //类型
        public string Detailed;    //介绍

        public override string ToString()
        {
            return string.Format(
                "[Id={1},Path={2},Name={3},Type={4},Detailed={5}]",
                this.Id,
                this.Path,
                this.Name,
                this.Type,
                this.Detailed
            );
        }
    }
}
