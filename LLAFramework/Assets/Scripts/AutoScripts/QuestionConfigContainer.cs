//此脚本为工具生成，请勿手动创建 2025-04-08 15:46:40.591 <ExcelTo>
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace GameLogic
{
    [CreateAssetMenu(fileName = "QuestionConfigContainer", menuName = "ScriptableObjects/QuestionConfigContainer", order = 0)]
    public class QuestionConfigContainer : ScriptableObject
    {
        
        public List<QuestionConfigDTO> configs;
    }
}
