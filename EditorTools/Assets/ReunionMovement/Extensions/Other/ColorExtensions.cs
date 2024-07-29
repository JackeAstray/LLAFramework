using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

/// <summary>
/// 颜色扩展
/// </summary>
public static class ColorExtensions
{
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
}
