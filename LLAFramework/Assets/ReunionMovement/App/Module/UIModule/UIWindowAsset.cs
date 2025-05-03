using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LLAFramework
{
    public class UIWindowAsset : MonoBehaviour
    {
        public string stringArgument;

        /// <summary>
        /// 界面类型
        /// </summary>
        public PanelType panelType = PanelType.NormalUI;
        /// <summary>
        /// 是否为全屏界面
        /// </summary>
        public PanelSize panelSize = PanelSize.SmallPanel;
        /// <summary>
        /// 切换场景时是否关闭当前界面
        /// </summary>
        public bool IsHidenWhenLeaveScene = true;
    }
}