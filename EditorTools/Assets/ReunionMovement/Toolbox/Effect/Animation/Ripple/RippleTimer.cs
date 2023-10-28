using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GameLogic.AnimationUI
{
    /// <summary>
    /// UI波纹动画 - 速率
    /// </summary>

    [RequireComponent(typeof(UIRipple))]
    public class RippleTimer : MonoBehaviour
    {
        /// <summary> 
        /// 偏移量
        /// </summary>
        public Vector2 Offset;

        /// <summary> 
        /// 波纹出现的速率
        /// </summary>
        public float Rate;

        //时间
        float T;

        /// <summary> 
        /// 颜色列表
        /// <summary> 
        public List<Color> Colors = new List<Color>();

        //颜色索引
        int ColorIndex = 0;

        void Update()
        {
            //当前时间 - 最后一个波纹的时间 >= 波纹
            if (Time.time - T >= Rate)
            {
                //创建波纹
                GetComponent<UIRipple>().CreateRipple(Offset);
                //设置新的时间
                T = Time.time;

                //改变颜色
                if (Colors.Count > 0)
                {
                    GetComponent<UIRipple>().StartColor = Colors[ColorIndex];
                    GetComponent<UIRipple>().EndColor = Colors[ColorIndex];

                    ColorIndex += 1;

                    //如果位于颜色列表末尾则循环返回
                    ColorIndex = ColorIndex % Colors.Count;
                }
            }
        }
    }
}