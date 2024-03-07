//此脚本为自动生成 2024-03-07 10:43:23.262 <ExcelTo>
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
        
        public int Id { get; set; }    //索引
        public string Path { get; set; }    //路径
        public string Name { get; set; }    //名称
        public int Type { get; set; }    //类型
        public string Detailed { get; set; }    //介绍


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
