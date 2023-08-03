using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GameLogic.EditorTools
{
    public class SmallFunctions : EditorWindow
    {
        [MenuItem("工具箱/小功能", false, 5)]
        public static void SmallFunctionsWindow()
        {
            //弹出编辑器
            GetWindow(typeof(SmallFunctions), true, "小功能", true);
        }

        void OnGUI()
        {

        }
    }
}