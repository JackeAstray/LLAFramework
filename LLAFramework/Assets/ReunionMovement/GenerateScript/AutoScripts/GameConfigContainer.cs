//此脚本为工具生成，请勿手动创建 2025-04-08 17:12:01.308 <ExcelTo>
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace GameLogic
{
    [CreateAssetMenu(fileName = "GameConfigContainer", menuName = "ScriptableObjects/GameConfigContainer", order = 0)]
    public class GameConfigContainer : ScriptableObject
    {
        
        public List<GameConfigDTO> configs;
    }
}
