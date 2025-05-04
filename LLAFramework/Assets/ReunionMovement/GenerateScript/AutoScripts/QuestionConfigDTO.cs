//此脚本为工具生成，请勿手动创建 2025-05-04 13:52:11.278 <ExcelTo>
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using SQLite.Attributes;

namespace LLAFramework
{
    [Serializable]
    public class QuestionConfigDTO
    {
        
        public int Id;    //索引
        public int Number;    //编号
        public int Type;    //类型
        public string Title;    //题目
        public string CorrectAnswer;    //正确答案
        public int Score;    //分数
        public string A;    //选项A
        public string B;    //选项B
        public string C;    //选项C
        public string D;    //选项D
        public string E;    //选项E
        public string F;    //选项F
        public string G;    //选项G
        public string H;    //选项H
        public string TitlePicture;    //标题图片
        public string APicture;    //A答案图片
        public string BPicture;    //B答案图片
        public string CPicture;    //C答案图片
        public string DPicture;    //D答案图片
        public string EPicture;    //E答案图片
        public string FPicture;    //F答案图片
        public string GPicture;    //G答案图片
        public string HPicture;    //H答案图片

        public override string ToString()
        {
            return string.Format(
                "[Id={1},Number={2},Type={3},Title={4},CorrectAnswer={5},Score={6},A={7},B={8},C={9},D={10},E={11},F={12},G={13},H={14},TitlePicture={15},APicture={16},BPicture={17},CPicture={18},DPicture={19},EPicture={20},FPicture={21},GPicture={22},HPicture={23}]",
                this.Id,
                this.Number,
                this.Type,
                this.Title,
                this.CorrectAnswer,
                this.Score,
                this.A,
                this.B,
                this.C,
                this.D,
                this.E,
                this.F,
                this.G,
                this.H,
                this.TitlePicture,
                this.APicture,
                this.BPicture,
                this.CPicture,
                this.DPicture,
                this.EPicture,
                this.FPicture,
                this.GPicture,
                this.HPicture
            );
        }

        /// <summary>
        /// 将 DTO 转换为无 DTO 实例
        /// </summary>
        public QuestionConfig ToEntity()
        {
            return new QuestionConfig
            {
                Id            = this.Id,
                Number        = this.Number,
                Type          = this.Type,
                Title         = this.Title,
                CorrectAnswer = this.CorrectAnswer,
                Score         = this.Score,
                A             = this.A,
                B             = this.B,
                C             = this.C,
                D             = this.D,
                E             = this.E,
                F             = this.F,
                G             = this.G,
                H             = this.H,
                TitlePicture  = this.TitlePicture,
                APicture      = this.APicture,
                BPicture      = this.BPicture,
                CPicture      = this.CPicture,
                DPicture      = this.DPicture,
                EPicture      = this.EPicture,
                FPicture      = this.FPicture,
                GPicture      = this.GPicture,
                HPicture      = this.HPicture,

            };
        }

        /// <summary>
        /// 从无 DTO 实例转换为 DTO
        /// </summary>
        public static QuestionConfigDTO FromEntity(QuestionConfig entity)
        {
            return new QuestionConfigDTO
            {
                Id            = entity.Id,
                Number        = entity.Number,
                Type          = entity.Type,
                Title         = entity.Title,
                CorrectAnswer = entity.CorrectAnswer,
                Score         = entity.Score,
                A             = entity.A,
                B             = entity.B,
                C             = entity.C,
                D             = entity.D,
                E             = entity.E,
                F             = entity.F,
                G             = entity.G,
                H             = entity.H,
                TitlePicture  = entity.TitlePicture,
                APicture      = entity.APicture,
                BPicture      = entity.BPicture,
                CPicture      = entity.CPicture,
                DPicture      = entity.DPicture,
                EPicture      = entity.EPicture,
                FPicture      = entity.FPicture,
                GPicture      = entity.GPicture,
                HPicture      = entity.HPicture,

            };
        }
    }
}
