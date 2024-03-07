using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "调色板", menuName = "创建调色板", order = 0)]
public class ColorPalette : ScriptableObject
{
    public List<Color> colors;

    public Color GetColor(int index)
    {
        if (index >= 0 && index < colors.Count)
        {
            return colors[index];
        }
        else
        {
            Debug.LogError("Index out of range");
            return Color.white;
        }
    }
}