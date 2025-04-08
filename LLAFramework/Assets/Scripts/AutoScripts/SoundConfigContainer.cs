//此脚本为工具生成，请勿手动创建 2025-04-08 11:29:56.118 <ExcelTo>
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace GameLogic
{
    [CreateAssetMenu(fileName = "SoundConfigContainer", menuName = "ScriptableObjects/SoundConfigContainer", order = 0)]
    public class SoundConfigContainer : ScriptableObject
    {
        
        public List<SoundConfig> configs;
    }
}
