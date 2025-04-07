//此脚本为工具生成，请勿手动创建 2025-04-07 16:35:33.797 <ExcelTo>
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace GameLogic
{
    [Serializable]
    public class QuestionConfig
    {
        
        public int Id;    //索引
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
                "[Id={1},Type={2},Title={3},CorrectAnswer={4},Score={5},A={6},B={7},C={8},D={9},E={10},F={11},G={12},H={13},TitlePicture={14},APicture={15},BPicture={16},CPicture={17},DPicture={18},EPicture={19},FPicture={20},GPicture={21},HPicture={22}]",
                this.Id,
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
    }
}
