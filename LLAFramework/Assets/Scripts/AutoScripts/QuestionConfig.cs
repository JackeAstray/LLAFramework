//此脚本为工具生成，请勿手动创建 2025-04-08 21:31:08.620 <ExcelTo>
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using SQLite.Attributes;

namespace GameLogic
{
    [Serializable]
    [UnityEngine.Scripting.Preserve]
    public class QuestionConfig
    {
        
        [PrimaryKey][AutoIncrement] public int Id{ get; set; }    //索引
        public int Number{ get; set; }    //编号
        public int Type{ get; set; }    //类型
        public string Title{ get; set; }    //题目
        public string CorrectAnswer{ get; set; }    //正确答案
        public int Score{ get; set; }    //分数
        public string A{ get; set; }    //选项A
        public string B{ get; set; }    //选项B
        public string C{ get; set; }    //选项C
        public string D{ get; set; }    //选项D
        public string E{ get; set; }    //选项E
        public string F{ get; set; }    //选项F
        public string G{ get; set; }    //选项G
        public string H{ get; set; }    //选项H
        public string TitlePicture{ get; set; }    //标题图片
        public string APicture{ get; set; }    //A答案图片
        public string BPicture{ get; set; }    //B答案图片
        public string CPicture{ get; set; }    //C答案图片
        public string DPicture{ get; set; }    //D答案图片
        public string EPicture{ get; set; }    //E答案图片
        public string FPicture{ get; set; }    //F答案图片
        public string GPicture{ get; set; }    //G答案图片
        public string HPicture{ get; set; }    //H答案图片

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
    }
}
