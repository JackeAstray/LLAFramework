//此脚本为工具生成，请勿手动创建 2025-04-08 17:12:01.315 <ExcelTo>
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using SQLite.Attributes;

namespace LLAFramework
{
    [Serializable]
    public class LanguagesDTO
    {
        
        public int Id;    //索引
        public int Number;    //编号
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
                "[Id={1},Number={2},ZH={3},EN={4},RU={5},FR={6},DE={7},ES={8},IT={9},PT={10},JP={11},KR={12}]",
                this.Id,
                this.Number,
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

        /// <summary>
        /// 将 DTO 转换为无 DTO 实例
        /// </summary>
        public Languages ToEntity()
        {
            return new Languages
            {
                Id     = this.Id,
                Number = this.Number,
                ZH     = this.ZH,
                EN     = this.EN,
                RU     = this.RU,
                FR     = this.FR,
                DE     = this.DE,
                ES     = this.ES,
                IT     = this.IT,
                PT     = this.PT,
                JP     = this.JP,
                KR     = this.KR,

            };
        }

        /// <summary>
        /// 从无 DTO 实例转换为 DTO
        /// </summary>
        public static LanguagesDTO FromEntity(Languages entity)
        {
            return new LanguagesDTO
            {
                Id     = entity.Id,
                Number = entity.Number,
                ZH     = entity.ZH,
                EN     = entity.EN,
                RU     = entity.RU,
                FR     = entity.FR,
                DE     = entity.DE,
                ES     = entity.ES,
                IT     = entity.IT,
                PT     = entity.PT,
                JP     = entity.JP,
                KR     = entity.KR,

            };
        }
    }
}
