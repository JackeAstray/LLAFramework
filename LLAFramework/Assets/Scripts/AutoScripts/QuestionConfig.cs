//此脚本为工具生成，请勿手动创建 2024-10-10 07:57:34.587 <ExcelTo>
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
        
        public int Id { get; set; }    //索引
        public int Type { get; set; }    //类型
        public string Title { get; set; }    //题目
        public string CorrectAnswer { get; set; }    //正确答案
        public int Score { get; set; }    //分数
        public string A { get; set; }    //选项A
        public string B { get; set; }    //选项B
        public string C { get; set; }    //选项C
        public string D { get; set; }    //选项D
        public string E { get; set; }    //选项E
        public string F { get; set; }    //选项F
        public string G { get; set; }    //选项G
        public string H { get; set; }    //选项H
        public string TitlePicture { get; set; }    //标题图片
        public string APicture { get; set; }    //A答案图片
        public string BPicture { get; set; }    //B答案图片
        public string CPicture { get; set; }    //C答案图片
        public string DPicture { get; set; }    //D答案图片
        public string EPicture { get; set; }    //E答案图片
        public string FPicture { get; set; }    //F答案图片
        public string GPicture { get; set; }    //G答案图片
        public string HPicture { get; set; }    //H答案图片


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
