using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LLAFramework
{
    /// <summary>
    /// 事件类型
    /// (根据需要取名称，不得重复)
    /// </summary>
    public enum EventModuleType
    {
        StartGame,      //开始游戏
        ButtonClick,    //点击按钮
        ClickBlock,     //点击方块
        Quit,           //退出游戏
        SendMessage,    //发送消息
        GoToNextScene,  //进入下一个场景
    }
}