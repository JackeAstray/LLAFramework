//此脚本为工具生成，请勿手动创建 2025-04-08 17:12:01.322 <ExcelTo>
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace LLAFramework
{
    [CreateAssetMenu(fileName = "QuestionConfigContainer", menuName = "ScriptableObjects/QuestionConfigContainer", order = 0)]
    public class QuestionConfigContainer : ScriptableObject
    {
        
        public List<QuestionConfigDTO> configs;
    }
}
