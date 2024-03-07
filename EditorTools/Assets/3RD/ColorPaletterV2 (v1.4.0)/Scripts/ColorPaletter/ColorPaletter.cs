using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace ColorPaletterV2
{
    /// <summary>
    /// Color paletter V2
    /// 
    /// - Create palettes that syncronize over multiple components
    /// - Access your palettes through scripts or just copy the color from the inspector
    /// - Access multiple preset palettes
    /// - Randomly generate a 5-? sized color palette
    /// 
    /// Read documentation if you need!
    /// </summary>
    public class ColorPaletter
    {
        private static List<ColorPalette> customPalettes = new List<ColorPalette>();
        public static readonly List<ColorPalette> presetPalettes = new List<ColorPalette>()
        {
            // Palette taken from https://flatuicolors.com/palette/defo
            new ColorPalette(
                "Generic 1",
                new List<PaletteColor>()
                {
                    new PaletteColor("Turquoise", new Color(0.101961f, 0.737255f, 0.611765f), true),
                    new PaletteColor("GreenSea", new Color(0.086275f, 0.627451f, 0.521569f), true),
                    new PaletteColor("Emerald", new Color(0.180392f, 0.8f, 0.443137f), true),
                    new PaletteColor("Nephritis", new Color(0.152941f, 0.682353f, 0.376471f), true),
                    new PaletteColor("PeterRiver", new Color(0.203922f, 0.596078f, 0.858824f), true),
                    new PaletteColor("BelizeHole", new Color(0.160784f, 0.501961f, 0.72549f), true),
                    new PaletteColor("Amethyst", new Color(0.607843f, 0.34902f, 0.713725f), true),
                    new PaletteColor("Wisteria", new Color(0.556863f, 0.266667f, 0.678431f), true),
                    new PaletteColor("WetAsp", new Color(0.203922f, 0.286275f, 0.368627f), true),
                    new PaletteColor("Midnight", new Color(0.172549f, 0.243137f, 0.313725f), true),
                    new PaletteColor("Sunflower", new Color(0.945098f, 0.768627f, 0.058824f), true),
                    new PaletteColor("Orange", new Color(0.952941f, 0.611765f, 0.070588f), true),
                    new PaletteColor("Carrot", new Color(0.901961f, 0.494118f, 0.133333f), true),
                    new PaletteColor("Pumpkin", new Color(0.827451f, 0.329412f, 0f), true),
                    new PaletteColor("Alizarin", new Color(0.905882f, 0.298039f, 0.235294f), true),
                    new PaletteColor("Pomegranate", new Color(0.752941f, 0.223529f, 0.168627f), true),
                    new PaletteColor("Clouds", new Color(0.92549f, 0.941176f, 0.945098f), true),
                    new PaletteColor("Silver", new Color(0.741176f, 0.764706f, 0.780392f), true),
                    new PaletteColor("Cono", new Color(0.584314f, 0.647059f, 0.65098f), true),
                    new PaletteColor("Asbe", new Color(0.498039f, 0.54902f, 0.552941f), true),
                }
            ),
            new ColorPalette(
                "Generic 2",
                new List<PaletteColor>()
                {
                    new PaletteColor("Crimson", new Color(0.862745f, 0.078431f, 0.235294f), true),
                    new PaletteColor("Gold", new Color(1f, 0.843137f, 0f), true),
                    new PaletteColor("Lime", new Color(0f, 1f, 0f), true),
                    new PaletteColor("Indigo", new Color(0.294118f, 0f, 0.509804f), true),
                    new PaletteColor("SlateBlue", new Color(0.415686f, 0.352941f, 0.803922f), true),
                    new PaletteColor("Chocolate", new Color(0.823529f, 0.411765f, 0.117647f), true),
                    new PaletteColor("Magenta", new Color(1f, 0f, 1f), true),
                    new PaletteColor("Teal", new Color(0f, 0.501961f, 0.501961f), true),
                    new PaletteColor("Sienna", new Color(0.627451f, 0.321569f, 0.176471f), true),
                    new PaletteColor("YellowGreen", new Color(0.603922f, 0.803922f, 0.196078f), true),
                    new PaletteColor("DeepPink", new Color(1f, 0.078431f, 0.576471f), true),
                    new PaletteColor("Aqua", new Color(0f, 1f, 1f), true),
                    new PaletteColor("Purple", new Color(0.501961f, 0f, 0.501961f), true),
                    new PaletteColor("Olive", new Color(0.501961f, 0.501961f, 0f), true),
                    new PaletteColor("Coral", new Color(1f, 0.498039f, 0.313725f), true),
                    new PaletteColor("DodgerBlue", new Color(0.117647f, 0.564706f, 1f), true),
                    new PaletteColor("Peru", new Color(0.803922f, 0.521569f, 0.247059f), true),
                    new PaletteColor("Orchid", new Color(0.854902f, 0.439216f, 0.839216f), true),
                    new PaletteColor("MediumSeaGreen", new Color(0.235294f, 0.701961f, 0.443137f), true),
                    new PaletteColor("Cyan", new Color(0f, 1f, 1f), true),
                }
            ),
            new ColorPalette(
                "Generic 3",
                new List<PaletteColor>()
                {
                    new PaletteColor("Coral", new Color(1f, 0.498039f, 0.313725f), true),
                    new PaletteColor("DarkSlateBlue", new Color(0.282353f, 0.239216f, 0.545098f), true),
                    new PaletteColor("DarkOrange", new Color(1f, 0.54902f, 0f), true),
                    new PaletteColor("GoldenRod", new Color(0.854902f, 0.647059f, 0.12549f), true),
                    new PaletteColor("MediumPurple", new Color(0.576471f, 0.439216f, 0.858824f), true),
                    new PaletteColor("OliveDrab", new Color(0.419608f, 0.556863f, 0.137255f), true),
                    new PaletteColor("Orchid", new Color(0.854902f, 0.439216f, 0.839216f), true),
                    new PaletteColor("SlateGray", new Color(0.439216f, 0.501961f, 0.564706f), true),
                    new PaletteColor("SpringGreen", new Color(0f, 1f, 0.498039f), true),
                    new PaletteColor("Tomato", new Color(1f, 0.388235f, 0.278431f), true),
                    new PaletteColor("Violet", new Color(0.933333f, 0.509804f, 0.933333f), true),
                    new PaletteColor("Wheat", new Color(0.960784f, 0.870588f, 0.701961f), true),
                    new PaletteColor("BlueViolet", new Color(0.541176f, 0.168627f, 0.886275f), true),
                    new PaletteColor("CadetBlue", new Color(0.372549f, 0.619608f, 0.627451f), true),
                    new PaletteColor("Crimson", new Color(0.862745f, 0.078431f, 0.235294f), true),
                    new PaletteColor("DarkKhaki", new Color(0.741176f, 0.717647f, 0.419608f), true),
                    new PaletteColor("DarkOrchid", new Color(0.6f, 0.196078f, 0.8f), true),
                    new PaletteColor("LawnGreen", new Color(0.486275f, 0.988235f, 0f), true),
                    new PaletteColor("LightCoral", new Color(0.941176f, 0.501961f, 0.501961f), true),
                    new PaletteColor("MediumAquamarine", new Color(0.4f, 0.803922f, 0.666667f), true),
                    new PaletteColor("MediumSlateBlue", new Color(0.482353f, 0.407843f, 0.933333f), true),
                }
            ),
            new ColorPalette(
                "Garden",
                new List<PaletteColor>()
                {
                    new PaletteColor("Water Lily Pink", new Color(0.8352941f, 0.5882353f, 0.6745098f), true),
                    new PaletteColor("Meadow Green", new Color(0.4862745f, 0.6156863f, 0.3490196f), true),
                    new PaletteColor("Giverny Blue", new Color(0.3137255f, 0.4941176f, 0.6784314f), true),
                    new PaletteColor("Sunflower Yellow", new Color(0.9686275f, 0.8392157f, 0.2431373f), true),
                    new PaletteColor("Poppy Red", new Color(0.7607843f, 0.1490196f, 0.1254902f), true),
                    new PaletteColor("Bridge Brown", new Color(0.3803922f, 0.3137255f, 0.227451f), true),
                    new PaletteColor("Haystack Beige", new Color(0.8313726f, 0.6901961f, 0.4784314f), true),
                    new PaletteColor("Path Gray", new Color(0.4666667f, 0.4823529f, 0.4941176f), true),
                }
            ),
            new ColorPalette(
                "Van Gogh's Palette",
                new List<PaletteColor>()
                {
                    new PaletteColor("Starry Night Blue", new Color(0.1647059f, 0.2196078f, 0.3882353f), true),
                    new PaletteColor("Sunflower Yellow", new Color(0.9764706f, 0.8392157f, 0.3098039f), true),
                    new PaletteColor("Olive Green", new Color(0.4862745f, 0.5686275f, 0.3058824f), true),
                    new PaletteColor("Almond Blossom Pink", new Color(0.9568627f, 0.6627451f, 0.6745098f), true),
                    new PaletteColor("Wheatfield Gold", new Color(0.8156863f, 0.6156863f, 0.3333333f), true),
                    new PaletteColor("Cypress Green", new Color(0.2235294f, 0.2745098f, 0.172549f), true),
                }
            ),
            new ColorPalette(
                "Picasso's Palette",
                new List<PaletteColor>()
                {
                    new PaletteColor("Blue Period Blue", new Color(0.2470588f, 0.3294118f, 0.4588235f), true),
                    new PaletteColor("Cubist Yellow", new Color(0.9764706f, 0.8392157f, 0.3058824f), true),
                    new PaletteColor("Rose Period Pink", new Color(0.7843137f, 0.4705882f, 0.5529412f), true),
                    new PaletteColor("African Art Brown", new Color(0.3490196f, 0.3058824f, 0.2823529f), true),
                    new PaletteColor("Abstract Green", new Color(0.2705882f, 0.5254902f, 0.3647059f), true),
                    new PaletteColor("Picasso Pink", new Color(0.8901961f, 0.4745098f, 0.5372549f), true),
                    new PaletteColor("Cubism Blue", new Color(0.1490196f, 0.3098039f, 0.4117647f), true),
                }
            ),
            new ColorPalette(
                "Black and White",
                new List<PaletteColor>()
                {
                    new PaletteColor("Black", new Color(0, 0, 0), true),
                    new PaletteColor("DarkestGray", new Color(0.1f, 0.1f, 0.1f), true),
                    new PaletteColor("DarkerGray", new Color(0.2f, 0.2f, 0.2f), true),
                    new PaletteColor("DarkGray", new Color(0.3f, 0.3f, 0.3f), true),
                    new PaletteColor("DimGray", new Color(0.4f, 0.4f, 0.4f), true),
                    new PaletteColor("Gray", new Color(0.5f, 0.5f, 0.5f), true),
                    new PaletteColor("LightGray", new Color(0.5f, 0.5f, 0.5f), true),
                    new PaletteColor("LighterGray", new Color(0.6f, 0.6f, 0.6f), true),
                    new PaletteColor("LighterGray", new Color(0.7f, 0.7f, 0.7f), true),
                    new PaletteColor("BrightGray", new Color(0.8f, 0.8f, 0.8f), true),
                    new PaletteColor("BrighterGray", new Color(0.9f, 0.9f, 0.9f), true),
                    new PaletteColor("White", new Color(1, 1, 1), true),
                }
            )
        };

        private static readonly string fileName = "colorPaletter.json";

        [Tooltip("Produce debug messages")]
        public static bool debugLogging = true;

        /// <summary>
        /// Returns color by name (not case sensitive)
        /// </summary>
        /// <returns>Color from an palette (chooses first if there are multiple)</returns>
        /// <example>
        /// <code>
        /// PaletteColor myColor = GetColor("Red");
        /// 
        /// if (myColor != null)
        /// {
        ///     // ...
        /// }
        /// </code>
        /// </example>
        public static PaletteColor GetColor(string colorName)
        {
            // get list of color palettes
            List<ColorPalette> palletes = new List<ColorPalette>(customPalettes);

            // go through each one and attempt to find color
            foreach (ColorPalette palette in customPalettes.ToList())
            {
                PaletteColor color = Array.Find(palette.paletteColors.ToArray(), color => color.colorName.ToLower() == colorName.ToLower());

                if (color == null)
                    continue;

                return color;
            }

            // return null if found nothing
            LogMessage($"Couldn't find color '{colorName}' anywhere in custom palettes");
            return null;
        }

        /// <summary>
        /// Get color from specific palette (not case sensitive)
        /// </summary>
        /// <returns>Color from specified palette, null otherwise</returns>
        public static PaletteColor GetColorFromPalette(string paletteName, string colorName)
        {
            // get list of color palettes
            ColorPalette[] palettes = GetAllPalettes();

            // find pallete
            ColorPalette targetPalette = GetPaletteByName(paletteName);
            if (targetPalette == null)
            {
                LogMessage($"Couldn't find palette with the name, '{paletteName}'. Returning null");
                return null;
            }

            PaletteColor color = Array.Find(targetPalette.paletteColors.ToArray(), color => color.colorName.ToLower() == colorName.ToLower());
            if (color == null)
            {
                LogMessage($"Couldn't find color with the name, '{colorName}' in '{paletteName}'. Returning null");
                return null;
            }

            return color;
        }

        /// <summary>
        /// Get color from specific palette (not case sensitive)
        /// </summary>
        /// <returns>Color from specified palette, null otherwise</returns>
        public static PaletteColor GetColorFromPalette(ColorPalette colorPalette, string colorName)
        {
            PaletteColor color = Array.Find(colorPalette.paletteColors.ToArray(), color => color.colorName.ToLower() == colorName.ToLower());
            if (color == null)
            {
                LogMessage($"Couldn't find color with the name, '{colorName}' in '{colorPalette.paletteName}'. Returning null");
                return null;
            }

            return color;
        }

        /// <summary>
        /// Get all preset and custom palettes in one array
        /// </summary>
        private static ColorPalette[] GetAllPalettes()
        {
            // get list of color palettes
            List<ColorPalette> palettes = new List<ColorPalette>(customPalettes);
            palettes.AddRange(presetPalettes);

            return palettes.ToArray();
        }

        /// <summary>
        /// Log message
        /// </summary>
        private static void LogMessage(string content)
        {
            if (debugLogging)
                Debug.Log($"<ColorPaletter> {content}");
        }

        /// <summary>
        /// Clear list of palettes
        /// </summary>
        public static void DeleteAllPalettes()
        {
            customPalettes.Clear();
        }

        /// <summary>
        /// Add new custom color palette
        /// </summary>
        public static ColorPalette AddNewCustomPalette(string paletteName)
        {
            ColorPalette palette = new ColorPalette(paletteName);
            customPalettes.Add(palette);
            return palette;
        }

        /// <summary>
        /// Add custom color palette with a list of colors
        /// </summary>
        public static ColorPalette AddNewCustomPalette(string paletteName, List<PaletteColor> paletteColors)
        {
            ColorPalette palette = new ColorPalette(paletteName, paletteColors);
            customPalettes.Add(palette);
            return palette;
        }

        /// <summary>
        /// Set the color of a color within a color palette. Automatically updates in runtime for pro version
        /// </summary>
        /// <param name="palette">Desired palette</param>
        /// <param name="targetColorName">Name of color within palette</param>
        /// <param name="newColor">Desired color</param>
        public static void SetColor(ColorPalette palette, string targetColorName, Color newColor)
        {
            PaletteColor color = GetColorFromPalette(palette, targetColorName);
            
            if (color != null)
            {
                color.color = newColor;
            }
        }

        /// <summary>
        /// Set the color of a color within a color palette. Automatically updates in runtime for pro version
        /// </summary>
        /// <param name="paletteName">Desired palette name</param>
        /// <param name="targetColorName">Name of color within palette</param>
        /// <param name="newColor">Desired color</param>
        public static void SetColor(string paletteName, string targetColorName, Color newColor)
        {
            ColorPalette palette = GetPaletteByName(paletteName);

            if (palette == null)
            {
                LogMessage("Palette null, could not set color");
                return;
            }

            PaletteColor color = GetColorFromPalette(palette, targetColorName);

            if (color != null)
            {
                Debug.Log("Setting new color");
                color.color = newColor;
            }
        }

        /// <summary>
        /// Find a color palette within your created list
        /// </summary>
        /// <param name="paletteName">Desired name</param>
        public static ColorPalette GetPaletteByName(string paletteName)
        {
            ColorPalette found = customPalettes.Find(palette => palette.paletteName == paletteName);

            if (found == null)
            {
                LogMessage($"Couldn't find palette by the name of '{paletteName}'");
                return null;
            }

            return found;
        }

        /// <summary>
        /// Returns a list of custom palettes
        /// </summary>
        public static List<ColorPalette> GetCustomPalettes()
        {
            return customPalettes;
        }

        /// <summary>
        /// Set the custom palettes - used mainly for loading data, but you can use this method if you want to for some reason :)
        /// </summary>
        public static void SetCustomPalettes(List<ColorPalette> newPalettes)
        {
            customPalettes = newPalettes;
        }

        /// <summary>
        /// Add color palette from palette string - returns the color palette if successfull
        /// </summary>
        public static ColorPalette AddColorPaletteFromString(string paletteString)
        {
            ColorPalette newPalette = new ColorPalette("");

            // {name}|{colorName}-{RRGGBBAA}|{colorName}-{RRGGBBAA}
            string[] properties = paletteString.Split('|');

            // if properties.length == 0
            if (properties.Length == 0)
            {
                return null;
            }

            for (int i = 1; i < properties.Length; i++)
            {
                PaletteColor paletteColor = new PaletteColor();

                string[] colorProperties = properties[i].Split('-');

                // if it isn't equal to {colorName}-{RRGGBBAA}-{autoName}
                if (colorProperties.Length != 3)
                {
                    return null;
                }

                if (ColorUtility.TryParseHtmlString("#" + colorProperties[1], out Color color))
                {
                    paletteColor.colorName = colorProperties[0];
                    paletteColor.color = color;

                    if (bool.TryParse(colorProperties[2], out bool autoName))
                    {
                        paletteColor.autoName = autoName;
                    }
                    else
                    {
                        return null;
                    }

                    newPalette.paletteColors.Add(paletteColor);
                }
                // if you cannot parse html string
                else
                {
                    return null;
                }
            }

            // set name
            newPalette.paletteName = properties[0];

            customPalettes.Add(newPalette);

            return newPalette;
        }

        /// <summary>
        /// Find color palette by palette ID
        /// </summary>
        public static ColorPalette FindPaletteByID(string ID)
        {
            return customPalettes.Find(p => p.paletteID == ID);
        }

        /// <summary>
        /// Find a palette color by ID
        /// </summary>
        public static PaletteColor FindPaletteColorByID(string ID)
        {
            List<PaletteColor> paletteColors = new List<PaletteColor>();
            foreach (ColorPalette palette in customPalettes)
            {
                paletteColors.AddRange(palette.paletteColors);
            }

            return paletteColors.Find(pc => pc.paletteColorID == ID);
        }

        /// <summary>
        /// Save the data
        /// </summary>
        public static void SaveData(bool debugMessages)
        {
            ColorPaletterSaveData data = new ColorPaletterSaveData(customPalettes);
            ColorPaletterDataManager.SaveData<ColorPaletterSaveData>(fileName, data, debugMessages);
        }

        /// <summary>
        /// Load the data
        /// </summary>
        public static void LoadData(bool debugMessages)
        {
            if (ColorPaletterDataManager.LoadData<ColorPaletterSaveData>(fileName, debugMessages) != null)
            {
                SetCustomPalettes(ColorPaletterDataManager.LoadData<ColorPaletterSaveData>(fileName, debugMessages).colorPalettes);
            }
        }

        /// <summary>
        /// Get data from file
        /// </summary>
        public static List<ColorPalette> GetSavedData(bool debugMessages)
        {
            if (ColorPaletterDataManager.LoadData<ColorPaletterSaveData>(fileName, debugMessages) != null)
            {
                return ColorPaletterDataManager.LoadData<ColorPaletterSaveData>(fileName, debugMessages).colorPalettes;
            }
            else
            {
                return null;
            }
        }
    }
}

    



