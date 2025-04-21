using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic.UI.ImageExtensions
{
    //绘制形状
    public enum DrawShape
    {
        None = 0,           //无
        Circle,             //圆
        Triangle,           //三角形
        Rectangle,          //矩形
        Pentagon,           //五边形
        Hexagon,            //六边形
        NStarPolygon,       //N星多边形
        Heart,              //心形
        BlobbyCross,        //滴状十字
        Squircle,           //方圆形 菱形
        NTriangleRounded,   //N三角形圆角
    }

    //渐变类型
    public enum GradientType
    {
        Linear,         //线性
        Corner,         //角
        Radial          //径向
    }

    //材质模式
    public enum MaterialMode
    {
        Dynamic,        //动态
        Shared          //共享
    }

    //角样式类型
    public enum CornerStyleType
    {
        Sharp,          //尖角
        Rounded,        //圆角
        Cropped,        //截断
        BoxCut,         //盒子切割
        CircleCut       //圆形切割
    }
}