using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LLAFramework
{
    /// <summary>
    /// UI默认值
    /// </summary>
    public static class UIDefs
    {
        public const int Camera_Size = 1;
        public const int Camera_Near = 1;
        public const int Camera_Far = 1000;
        public const int Camera_Depth = 1;

        //不同类型的ui的sortOrder范围
        public const int UIOrder_MainUI_Start = 1;
        public const int UIOrder_MainUI_Max = 10000;

        public const int UIOrder_Normal_Start = 20000;
        public const int UIOrder_Normal_Max = 100000;

        public const int UIOrder_Tips_Start = 200000;
        public const int UIOrder_Tips_Max = 300000;
    }
}
