using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 摄像机镜头
    /// </summary>
    [System.Serializable]
    public class CameraLens
    {
        //垂直视野
        public int verticalFOV = 50;
        //近裁剪面
        public float nearClipPlane = 0.1f;
        //远裁剪面
        public float farClipPlane = 1000f;
        //显示截锥体
        public bool showFrustrum = true;
        //倾斜
        [Range(-180, 180)] public float tilt = 0;
        //剔除遮罩
        public LayerMask cullingMask = ~0;
    }
}