using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

/// <summary>
/// 颜色扩展
/// </summary>
public static class ColorExtensions
{
    private const float LightOffset = 0.0625f;

    /// <summary>
    /// color 转换hex
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public static string ColorToHex(Color target)
    {
        int r = Mathf.RoundToInt(target.r * 255.0f);
        int g = Mathf.RoundToInt(target.g * 255.0f);
        int b = Mathf.RoundToInt(target.b * 255.0f);
        int a = Mathf.RoundToInt(target.a * 255.0f);
        return $"{r:X2}{g:X2}{b:X2}{a:X2}";
    }

    /// <summary>
    /// hex转换到color
    /// </summary>
    /// <param name="hex"></param>
    /// <returns></returns>
    public static Color HexToColor(string hex)
    {
        byte r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
        byte a = byte.Parse(hex.Substring(6, 2), NumberStyles.HexNumber);
        return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
    }

    /// <summary>
    /// 颜色变亮
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public static Color Lighter(this Color color)
    {
        return new Color(
            Mathf.Clamp(color.r + LightOffset, 0, 1),
            Mathf.Clamp(color.g + LightOffset, 0, 1),
            Mathf.Clamp(color.b + LightOffset, 0, 1),
            color.a);
    }

    /// <summary>
    /// 颜色变暗
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public static Color Darker(this Color color)
    {
        return new Color(
            Mathf.Clamp(color.r - LightOffset, 0, 1),
            Mathf.Clamp(color.g - LightOffset, 0, 1),
            Mathf.Clamp(color.b - LightOffset, 0, 1),
            color.a);
    }

    /// <summary>
    /// 获取颜色的亮度
    /// 定义为三个颜色通道的平均值
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public static float Brightness(this Color color)
    {
        return (color.r + color.g + color.b) / 3;
    }

    /// <summary>
    /// 获取颜色的饱和度
    /// </summary>
    /// <param name="color"></param>
    /// <param name="brightness"></param>
    /// <returns></returns>
    public static Color WithBrightness(this Color color, float brightness)
    {
        if (color.IsApproximatelyBlack())
        {
            return new Color(brightness, brightness, brightness, color.a);
        }

        float factor = brightness / color.Brightness();

        float r = color.r * factor;
        float g = color.g * factor;
        float b = color.b * factor;

        float a = color.a;

        return new Color(r, g, b, a);
    }

    /// <summary>
    /// 返回颜色是黑色还是接近黑色
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public static bool IsApproximatelyBlack(this Color color)
    {
        return color.r + color.g + color.b <= Mathf.Epsilon;
    }

    /// <summary>
    /// 返回颜色是白色还是接近白色
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public static bool IsApproximatelyWhite(this Color color)
    {
        return color.r + color.g + color.b >= 1 - Mathf.Epsilon;
    }
    
    /// <summary>
    /// 获取颜色反向
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public static Color Invert(this Color color)
    {
        return new Color(1 - color.r, 1 - color.g, 1 - color.b, color.a);
    }

    /// <summary>
    /// 获取不透明的颜色
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public static Color Opaque(this Color color)
    {
        return new Color(color.r, color.g, color.b);
    }

    /// <summary>
    /// 获取指定透明度的颜色
    /// </summary>
    /// <param name="color"></param>
    /// <param name="alpha"></param>
    /// <returns></returns>
    public static Color WithAlpha(this Color color, float alpha)
    {
        return new Color(color.r, color.g, color.b, alpha);
    }
}
