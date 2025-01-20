using UnityEngine;
using System.Collections.Generic;
using GameLogic;

[CreateAssetMenu(fileName = "ColorPalette", menuName = "创建调色板", order = 0)]
public class ColorPalette : ScriptableObject
{
    [System.Serializable]
    public class ColorEntry
    {
        public string name;
        public Color color;
    }

    public List<ColorEntry> commonlyUsed;
    public List<ColorEntry> scheme1;
    public List<ColorEntry> scheme2;
    public List<ColorEntry> scheme3;
    public List<ColorEntry> scheme4;
    public List<ColorEntry> scheme5;

    /// <summary>
    /// 获取颜色
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public Color GetColor(string name, ColorPaletteScheme colorPaletteScheme)
    {
        ColorEntry entry = null;
        switch (colorPaletteScheme)
        {
            case ColorPaletteScheme.CommonlyUsed:
                entry = commonlyUsed.Find(e => e.name == name);
                break;
            case ColorPaletteScheme.Scheme1:
                entry = scheme1.Find(e => e.name == name);
                break;
            case ColorPaletteScheme.Scheme2:
                entry = scheme2.Find(e => e.name == name);
                break;
            case ColorPaletteScheme.Scheme3:
                entry = scheme3.Find(e => e.name == name);
                break;
            case ColorPaletteScheme.Scheme4:
                entry = scheme4.Find(e => e.name == name);
                break;
            case ColorPaletteScheme.Scheme5:
                entry = scheme5.Find(e => e.name == name);
                break;
        }

        if (entry != null)
        {
            return entry.color;
        }
        else
        {
            Debug.LogError("找不到颜色");
            return Color.white;
        }
    }

    /// <summary>
    /// 添加颜色
    /// </summary>
    /// <param name="name"></param>
    /// <param name="color"></param>
    public void AddColor(string name, Color color, ColorPaletteScheme colorPaletteScheme)
    {
        switch (colorPaletteScheme)
        {
            case ColorPaletteScheme.CommonlyUsed:
                if (commonlyUsed.Find(e => e.name == name) != null)
                {
                    Debug.LogError("颜色已存在");
                    return;
                }
                commonlyUsed.Add(new ColorEntry { name = name, color = color });
                break;
            case ColorPaletteScheme.Scheme1:
                if (scheme1.Find(e => e.name == name) != null)
                {
                    Debug.LogError("颜色已存在");
                    return;
                }
                scheme1.Add(new ColorEntry { name = name, color = color });
                break;
            case ColorPaletteScheme.Scheme2:
                if (scheme2.Find(e => e.name == name) != null)
                {
                    Debug.LogError("颜色已存在");
                    return;
                }
                scheme2.Add(new ColorEntry { name = name, color = color });
                break;
            case ColorPaletteScheme.Scheme3:
                if (scheme3.Find(e => e.name == name) != null)
                {
                    Debug.LogError("颜色已存在");
                    return;
                }
                scheme3.Add(new ColorEntry { name = name, color = color });
                break;
            case ColorPaletteScheme.Scheme4:
                if (scheme4.Find(e => e.name == name) != null)
                {
                    Debug.LogError("颜色已存在");
                    return;
                }
                scheme4.Add(new ColorEntry { name = name, color = color });
                break;
            case ColorPaletteScheme.Scheme5:
                if (scheme5.Find(e => e.name == name) != null)
                {
                    Debug.LogError("颜色已存在");
                    return;
                }
                scheme5.Add(new ColorEntry { name = name, color = color });
                break;
        }
    }

    /// <summary>
    /// 更新颜色
    /// </summary>
    /// <param name="name"></param>
    /// <param name="color"></param>
    public void UpdateColor(string name, Color color, ColorPaletteScheme colorPaletteScheme)
    {
        ColorEntry entry = null;
        switch (colorPaletteScheme)
        {
            case ColorPaletteScheme.CommonlyUsed:
                entry = commonlyUsed.Find(e => e.name == name);
                break;
            case ColorPaletteScheme.Scheme1:
                entry = scheme1.Find(e => e.name == name);
                break;
            case ColorPaletteScheme.Scheme2:
                entry = scheme2.Find(e => e.name == name);
                break;
            case ColorPaletteScheme.Scheme3:
                entry = scheme3.Find(e => e.name == name);
                break;
            case ColorPaletteScheme.Scheme4:
                entry = scheme4.Find(e => e.name == name);
                break;
            case ColorPaletteScheme.Scheme5:
                entry = scheme5.Find(e => e.name == name);
                break;
        }

        if (entry != null)
        {
            entry.color = color;
        }
        else
        {
            Debug.LogError("找不到颜色");
        }
    }

    /// <summary>
    /// 移除颜色
    /// </summary>
    /// <param name="name"></param>
    public void RemoveColor(string name, ColorPaletteScheme colorPaletteScheme)
    {
        ColorEntry entry = null;
        switch (colorPaletteScheme)
        {
            case ColorPaletteScheme.CommonlyUsed:
                entry = commonlyUsed.Find(e => e.name == name);
                if (entry != null)
                {
                    commonlyUsed.Remove(entry);
                }
                else
                {
                    Debug.LogError("找不到颜色");
                }
                break;
            case ColorPaletteScheme.Scheme1:
                entry = scheme1.Find(e => e.name == name);
                if (entry != null)
                {
                    scheme1.Remove(entry);
                }
                else
                {
                    Debug.LogError("找不到颜色");
                }
                break;
            case ColorPaletteScheme.Scheme2:
                entry = scheme2.Find(e => e.name == name);
                if (entry != null)
                {
                    scheme2.Remove(entry);
                }
                else
                {
                    Debug.LogError("找不到颜色");
                }
                break;
            case ColorPaletteScheme.Scheme3:
                entry = scheme3.Find(e => e.name == name);
                if (entry != null)
                {
                    scheme3.Remove(entry);
                }
                else
                {
                    Debug.LogError("找不到颜色");
                }
                break;
            case ColorPaletteScheme.Scheme4:
                entry = scheme4.Find(e => e.name == name);
                if (entry != null)
                {
                    scheme4.Remove(entry);
                }
                else
                {
                    Debug.LogError("找不到颜色");
                }
                break;
            case ColorPaletteScheme.Scheme5:
                entry = scheme5.Find(e => e.name == name);
                if (entry != null)
                {
                    scheme5.Remove(entry);
                }
                else
                {
                    Debug.LogError("找不到颜色");
                }
                break;
        }
    }
}