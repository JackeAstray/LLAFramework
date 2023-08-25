//此脚本为自动生成 2023-08-25 19:09:58.558 <ExcelTo>
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace GameLogic
{
    
    [Serializable]
    public class Languages
    {
        
        public int Id { get; set; }    //索引
        public string ZH { get; set; }    //中文
        public string EN { get; set; }    //英文


        public override string ToString()
        {
            return string.Format(
                "[Id={1},ZH={2},EN={3}]",
                this.Id,
                this.ZH,
                this.EN
            );
        }
    }
}
