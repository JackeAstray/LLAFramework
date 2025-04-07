//此脚本为工具生成，请勿手动创建 2025-04-07 16:35:33.791 <ExcelTo>
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
        
        public int Id;    //索引
        public string ZH;    //中文
        public string EN;    //英文
        public string RU;    //俄文
        public string FR;    //法文
        public string DE;    //德文
        public string ES;    //西班牙文
        public string IT;    //意大利文
        public string PT;    //葡萄牙文
        public string JP;    //日文
        public string KR;    //韩文

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
