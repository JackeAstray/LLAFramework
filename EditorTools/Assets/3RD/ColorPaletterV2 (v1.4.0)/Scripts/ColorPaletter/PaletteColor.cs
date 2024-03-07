using System.Collections.Generic;
using UnityEngine;

namespace ColorPaletterV2
{
    [System.Serializable]
    public class PaletteColor
    {
        [Tooltip("Name of the color")]
        public string colorName;
        [Tooltip("Color of the color")]
        public Color color;
        public string paletteColorID;

        private static readonly Dictionary<string, string> colorNames = new Dictionary<string, string>()
        {
            { "AliceBlue", "#F0F8FF" },
            { "AntiqueWhite", "#FAEBD7" },
            { "Aqua", "#00FFFF" },
            { "Aquamarine", "#7FFFD4" },
            { "Azure", "#F0FFFF" },
            { "Beige", "#F5F5DC" },
            { "Bisque", "#FFE4C4" },
            { "Black", "#000000" },
            { "BlanchedAlmond", "#FFEBCD" },
            { "Blue", "#0000FF" },
            { "BlueViolet", "#8A2BE2" },
            { "Brown", "#A52A2A" },
            { "BurlyWood", "#DEB887" },
            { "CadetBlue", "#5F9EA0" },
            { "Chartreuse", "#7FFF00" },
            { "Chocolate", "#D2691E" },
            { "Coral", "#FF7F50" },
            { "CornflowerBlue", "#6495ED" },
            { "Cornsilk", "#FFF8DC" },
            { "Cyan", "#00FFFF" },
            { "DarkBlue", "#00008B" },
            { "DarkCyan", "#008B8B" },
            { "DarkGoldenrod", "#B8860B" },
            { "DarkGray", "#A9A9A9" },
            { "DarkGreen", "#006400" },
            { "DarkKhaki", "#BDB76B" },
            { "DarkMagenta", "#8B008B" },
            { "DarkOliveGreen", "#556B2F" },
            { "DarkOrange", "#FF8C00" },
            { "DarkOrchid", "#9932CC" },
            { "DarkRed", "#8B0000" },
            { "DarkSalmon", "#E9967A" },
            { "DarkSeaGreen", "#8FBC8F" },
            { "DarkSlateBlue", "#483D8B" },
            { "DarkSlateGray", "#2F4F4F" },
            { "DarkTurquoise", "#00CED1" },
            { "DarkViolet", "#9400D3" },
            { "DeepPink", "#FF1493" },
            { "DeepSkyBlue", "#00BFFF" },
            { "DimGray", "#696969" },
            { "DodgerBlue", "#1E90FF" },
            { "Firebrick", "#B22222" },
            { "FloralWhite", "#FFFAF0" },
            { "ForestGreen", "#228B22" },
            { "Fuchsia", "#FF00FF" },
            { "Gainsboro", "#DCDCDC" },
            { "GhostWhite", "#F8F8FF" },
            { "Gold", "#FFD700" },
            { "Goldenrod", "#DAA520" },
            { "Gray", "#808080" },
            { "Green", "#008000" },
            { "GreenYellow", "#ADFF2F" },
            { "Honeydew", "#F0FFF0" },
            { "HotPink", "#FF69B4" },
            { "IndianRed", "#CD5C5C" },
            { "Indigo", "#4B0082" },
            { "Ivory", "#FFFFF0" },
            { "Khaki", "#F0E68C" },
            { "Lavender", "#E6E6FA" },
            { "LavenderBlush", "#FFF0F5" },
            { "LawnGreen", "#7CFC00" },
            { "LemonChiffon", "#FFFACD" },
            { "LightBlue", "#ADD8E6" },
            { "LightCoral", "#F08080" },
            { "LightCyan", "#E0FFFF" },
            { "LightGoldenrodYellow", "#FAFAD2" },
            { "LightGray", "#D3D3D3" },
            { "LightGreen", "#90EE90" },
            { "LightPink", "#FFB6C1" },
            { "LightSalmon", "#FFA07A" },
            { "LightSeaGreen", "#20B2AA" },
            { "LightSkyBlue", "#87CEFA" },
            { "LightSlateGray", "#778899" },
            { "LightSteelBlue", "#B0C4DE" },
            { "LightYellow", "#FFFFE0" },
            { "Lime", "#00FF00" },
            { "LimeGreen", "#32CD32" },
            { "Linen", "#FAF0E6" },
            { "Magenta", "#FF00FF" },
            { "Maroon", "#800000" },
            { "MediumAquamarine", "#66CDAA" },
            { "MediumBlue", "#0000CD" },
            { "MediumOrchid", "#BA55D3" },
            { "MediumPurple", "#9370DB" },
            { "MediumSeaGreen", "#3CB371" },
            { "MediumSlateBlue", "#7B68EE" },
            { "MediumSpringGreen", "#00FA9A" },
            { "MediumTurquoise", "#48D1CC" },
            { "MediumVioletRed", "#C71585" },
            { "MidnightBlue", "#191970" },
            { "MintCream", "#F5FFFA" },
            { "MistyRose", "#FFE4E1" },
            { "Moccasin", "#FFE4B5" },
            { "NavajoWhite", "#FFDEAD" },
            { "Navy", "#000080" },
            { "OldLace", "#FDF5E6" },
            { "Olive", "#808000" },
            { "OliveDrab", "#6B8E23" },
            { "Orange", "#FFA500" },
            { "OrangeRed", "#FF4500" },
            { "Orchid", "#DA70D6" },
            { "PaleGoldenrod", "#EEE8AA" },
            { "PaleGreen", "#98FB98" },
            { "PaleTurquoise", "#AFEEEE" },
            { "PaleVioletRed", "#DB7093" },
            { "PapayaWhip", "#FFEFD5" },
            { "PeachPuff", "#FFDAB9" },
            { "Peru", "#CD853F" },
            { "Pink", "#FFC0CB" },
            { "Plum", "#DDA0DD" },
            { "PowderBlue", "#B0E0E6" },
            { "Purple", "#800080" },
            { "RebeccaPurple", "#663399" },
            { "Red", "#FF0000" },
            { "RosyBrown", "#BC8F8F" },
            { "RoyalBlue", "#4169E1" },
            { "SaddleBrown", "#8B4513" },
            { "Salmon", "#FA8072" },
            { "SandyBrown", "#F4A460" },
            { "SeaGreen", "#2E8B57" },
            { "SeaShell", "#FFF5EE" },
            { "Sienna", "#A0522D" },
            { "Silver", "#C0C0C0" },
            { "SkyBlue", "#87CEEB" },
            { "SlateBlue", "#6A5ACD" },
            { "SlateGray", "#708090" },
            { "Snow", "#FFFAFA" },
            { "SpringGreen", "#00FF7F" },
            { "SteelBlue", "#4682B4" },
            { "Tan", "#D2B48C" },
            { "Teal", "#008080" },
            { "Thistle", "#D8BFD8" },
            { "Tomato", "#FF6347" },
            { "Turquoise", "#40E0D0" },
            { "Violet", "#EE82EE" },
            { "Wheat", "#F5DEB3" },
            { "White", "#FFFFFF" },
            { "WhiteSmoke", "#F5F5F5" },
            { "Yellow", "#FFFF00" },
            { "YellowGreen", "#9ACD32" },
            { "AeroBlue", "#C0E8D5" },
            { "Amber", "#FFBF00" },
            { "Apricot", "#FFC580" },
            { "Auburn", "#A52A2A" },
            { "BattleshipGray", "#848482" },
            { "Blush", "#DE5D83" },
            { "Bole", "#79443B" },
            { "Burgundy", "#800020" },
            { "Cerulean", "#007BA7" },
            { "Champagne", "#F7E7CE" },
            { "Citrine", "#E4D00A" },
            { "Cobalt", "#0047AB" },
            { "Copper", "#B87333" },
            { "Crimson", "#DC143C" },
            { "DesertSand", "#EDC9AF" },
            { "Emerald", "#50C878" },
            { "Fallow", "#C19A6B" },
            { "FernGreen", "#4F7942" },
            { "FlamingoPink", "#FC8EAC" },
            { "FuchsiaPink", "#FF77FF" },
            { "Grullo", "#A99A86" },
            { "Heliotrope", "#DF73FF" },
            { "Iceberg", "#71A6D2" },
            { "IlluminatingEmerald", "#319177" },
            { "ImperialRed", "#ED2939" },
            { "Jade", "#00A86B" },
            { "Lava", "#CF1020" },
            { "Lilac", "#C8A2C8" },
            { "Limeade", "#6F9D02" },
            { "Malachite", "#0BDA51" },
            { "MangoTango", "#FF8243" },
            { "Marigold", "#EAA221" },
            { "Mauve", "#E0B0FF" },
            { "MidnightGreen", "#004953" },
            { "MikadoYellow", "#FFC40C" },
            { "MintGreen", "#98FF98" },
            { "Mustard", "#FFDB58" },
            { "NeonBlue", "#4D4DFF" },
            { "Olivine", "#9AB973" },
            { "PansyPurple", "#78184A" },
            { "Peach", "#FFDAB9" },
            { "PineGreen", "#01796F" },
            { "Pistachio", "#93C572" },
            { "PoppyRed", "#B5002A" },
            { "Raspberry", "#E30B5C" },
            { "Rust", "#B7410E" },
            { "SageGreen", "#87AE73" },
            { "Scarlet", "#FF2400" },
            { "SeaFoamGreen", "#9FE2BF" },
            { "Slate", "#5C677D" },
            { "Sunset", "#FAD6A5" },
            { "TeaGreen", "#D0F0C0" },
            { "TerraCotta", "#E2725B" },
            { "TiffanyBlue", "#0ABAB5" },
            { "Tuscany", "#C09999" },
            { "Vanilla", "#F3E5AB" },
            { "Verdigris", "#43B3AE" },
            { "Vermilion", "#D9381E" },
            { "Viridian", "#40826D" },
            { "Watermelon", "#F05C85" },
            { "Wisteria", "#C9A0DC" },
            { "Zaffre", "#0014A8" }
        };

        public PaletteColor()
        {
            colorName = "New Color";
            color = Color.white;
            autoName = true;

            paletteColorID = GUIDGenerator.GeneratePaletteID();
        }

        public PaletteColor(Color color)
        {
            this.color = color;
            autoName = true;

            paletteColorID = GUIDGenerator.GeneratePaletteID();
        }

        public PaletteColor(string colorName, Color color)
        {
            this.colorName = colorName;
            this.color = color;
            autoName = true;

            paletteColorID = GUIDGenerator.GeneratePaletteID();
        }

        public PaletteColor(string colorName, Color color, bool autoName)
        {
            this.colorName = colorName;
            this.color = color;
            this.autoName = autoName;

            paletteColorID = GUIDGenerator.GeneratePaletteID();
        }

        public bool autoName = true;

        /// <summary>
        /// Get html string (ex: #ffffff)
        /// </summary>
        public string GetHTMLString()
        {
            return ColorUtility.ToHtmlStringRGB(color);
        }

        /// <summary>
        /// Copy color to clipboard
        /// </summary>
        public void CopyColorToClipboard()
        {
            string colorHTMLString = GetHTMLString();
            GUIUtility.systemCopyBuffer = colorHTMLString;

            Debug.Log($"Copied '<color=#{colorHTMLString}>{colorHTMLString}</color>' to clipboard!");
        }

        /// <summary>
        /// Find the closest name to a hex code
        /// </summary>
        public static string AutoFindColorName(Color color)
        {
            float minDistance = float.MaxValue;
            string closestColorName = string.Empty;

            foreach (KeyValuePair<string, string> colorEntry in colorNames)
            {
                ColorUtility.TryParseHtmlString(colorEntry.Value, out Color paletteColor);
                float distance = CalculateColorDistance(color, paletteColor);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestColorName = colorEntry.Key;
                }
            }

            return closestColorName;
        }

        /// <summary>
        /// Calculate the distance of 2 colors
        /// </summary>
        private static float CalculateColorDistance(Color color1, Color color2)
        {
            float rDiff = color1.r - color2.r;
            float gDiff = color1.g - color2.g;
            float bDiff = color1.b - color2.b;
            return Mathf.Sqrt(rDiff * rDiff + gDiff * gDiff + bDiff * bDiff);
        }
    }

}
