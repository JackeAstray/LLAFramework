//此脚本为工具生成，请勿手动创建 2025-05-04 13:52:11.289 <ExcelTo>
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace LLAFramework
{
    [CreateAssetMenu(fileName = "SoundConfigContainer", menuName = "ScriptableObjects/SoundConfigContainer", order = 0)]
    public class SoundConfigContainer : ScriptableObject
    {
        
        public List<SoundConfigDTO> configs;
    }
}
