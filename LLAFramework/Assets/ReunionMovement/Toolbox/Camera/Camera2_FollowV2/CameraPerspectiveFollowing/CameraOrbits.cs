using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 摄像机轨道
    /// </summary>
    [System.Serializable]
    public class CameraOrbits
    {
        public float rigOffset;
        public CameraOrbitsHeight height;   //高度
        public CameraOrbitsRadius radius;   //半径
    }

    /// <summary>
    /// 摄像机轨道高度
    /// </summary>
    [System.Serializable]
    public class CameraOrbitsHeight
    {
        public float up;    //上
        public float down;  //下
    }

    /// <summary>
    /// 摄影机轨道半径
    /// </summary>
    [System.Serializable]
    public class CameraOrbitsRadius
    {
        public float top;       //上
        public float middle;    //中
        public float bottom;    //下
    }
}