//此脚本为工具生成，请勿手动创建 2024-10-10 07:57:34.585 <ExcelTo>
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
        public string RU { get; set; }    //俄文
        public string FR { get; set; }    //法文
        public string DE { get; set; }    //德文
        public string ES { get; set; }    //西班牙文
        public string IT { get; set; }    //意大利文
        public string PT { get; set; }    //葡萄牙文
        public string JP { get; set; }    //日文
        public string KR { get; set; }    //韩文


        public override string ToString()
        {
            return string.Format(
                "[Id={1},ZH={2},EN={3},RU={4},FR={5},DE={6},ES={7},IT={8},PT={9},JP={10},KR={11}]",
                this.Id,
                this.ZH,
                this.EN,
                this.RU,
                this.FR,
                this.DE,
                this.ES,
                this.IT,
                this.PT,
                this.JP,
                this.KR
            );
        }
    }
}
