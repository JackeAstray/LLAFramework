using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ColorPaletterV2
{
    [System.Serializable]
    /// <summary>
    /// Color palette that holds a name and a list of colors
    /// Contains sorting methods
    /// </summary>
    public class ColorPalette
    {
        public string paletteName;
        public List<PaletteColor> paletteColors = new List<PaletteColor>();
        public bool panelOpen;
        /// <summary>
        /// ID for the color palette - Do not touch unless you want to mess things up :)
        /// Not using getters bc other scripts need it
        /// </summary>
        public string paletteID;

        /// <summary>
        /// Default constructor
        /// </summary>
        public ColorPalette(string paletteName)
        {
            this.paletteName = paletteName;
            this.paletteColors = new List<PaletteColor>();
            paletteID = GUIDGenerator.GeneratePaletteID();
        }

        /// <summary>
        /// Constructor with initial colors
        /// </summary>
        public ColorPalette(string paletteName, List<PaletteColor> paletteColors, bool sortColors = true)
        {
            this.paletteName = paletteName;
            this.paletteColors = paletteColors;
            paletteID = GUIDGenerator.GeneratePaletteID();

            if (sortColors)
                SortColors();
        }

        /// <summary>
        /// Sort the colors by their RGB values
        /// </summary>
        public void SortColors()
        {
            paletteColors.Sort(ColorSorter);
        }

        /// <summary>
        /// Color sorter used for sorting the colors
        /// </summary>
        private int ColorSorter(PaletteColor a, PaletteColor b)
        {
            float hueDifferenceA = GetHue(a.color);
            float hueDifferenceB = GetHue(b.color);

            if (hueDifferenceA < hueDifferenceB)
                return -1;
            else if (hueDifferenceA > hueDifferenceB)
                return 1;

            float weightedSumDifferenceA = GetWeightedSum(a.color);
            float weightedSumDifferenceB = GetWeightedSum(b.color);

            if (weightedSumDifferenceA < weightedSumDifferenceB)
                return -1;
            else if (weightedSumDifferenceA > weightedSumDifferenceB)
                return 1;

            return 0;
        }

        /// <summary>
        /// Get the hue of a color
        /// </summary>
        private float GetHue(Color color)
        {
            Color.RGBToHSV(color, out float hue, out _, out _);
            return hue;
        }

        /// <summary>
        /// Get the weighted sum of a color
        /// </summary>
        private float GetWeightedSum(Color color)
        {
            return color.r * 3 + color.g * 2 + color.b * 1;
        }

        /// <summary>
        /// Copy the palette string to the clipboard
        /// </summary>
        public void CopyPaletteStringToClipboard()
        {
            string paletteString = GetPaletteString();
            GUIUtility.systemCopyBuffer = paletteString;
        }

        /// <summary>
        /// Add a new color to the palette
        /// </summary>
        public void AddColor()
        {
            paletteColors.Add(new PaletteColor());
        }

        /// <summary>
        /// Add a new color to the palette
        /// </summary>
        public void AddColor(string colorName, Color color)
        {
            paletteColors.Add(new PaletteColor(colorName, color));
        }

        /// <summary>
        /// Generate a random color palette with specific color categories using color theory
        /// </summary>
        public void GenerateRandomPalette(int numColors)
        {
            paletteColors.Clear();

            if (numColors <= 0)
                return;

            // generate primary color
            Color primaryColor = GeneratePrimaryColor();
            paletteColors.Add(new PaletteColor("Primary", primaryColor));

            // generate secondary color
            Color secondaryColor = GenerateSecondaryColor(primaryColor);
            paletteColors.Add(new PaletteColor("Secondary", secondaryColor));

            // generate background color
            Color backgroundColor = GenerateBackgroundColor();
            paletteColors.Add(new PaletteColor("Background", backgroundColor));

            // generate text color
            Color textColor = GenerateTextColor(backgroundColor);
            paletteColors.Add(new PaletteColor("Text", textColor));

            // generate accent color
            Color accentColor = GenerateAccentColor(primaryColor, secondaryColor);
            paletteColors.Add(new PaletteColor("Accent", accentColor));

            // generate additional colors if needed
            int additionalColors = numColors - 5;
            for (int i = 0; i < additionalColors; i++)
            {
                Color additionalColor = GenerateAdditionalColor();
                paletteColors.Add(new PaletteColor($"Additional {i + 1}", additionalColor));
            }

            SortColors();
        }

        /// <summary>
        /// Generate random primary color
        /// </summary>
        /// <returns></returns>
        private Color GeneratePrimaryColor()
        {
            // generate a random primary color
            return RandomColor();
        }

        /// <summary>
        /// Generate random secondary color based on the primary color
        /// </summary>
        private Color GenerateSecondaryColor(Color primaryColor)
        {
            // generate a random secondary color that complements the primary color
            return RandomComplementaryColor(primaryColor);
        }

        /// <summary>
        /// Generate random background color
        /// </summary>
        private Color GenerateBackgroundColor()
        {
            // generate a random background color
            return RandomColor();
        }

        /// <summary>
        /// Generate a random text color
        /// </summary>
        private Color GenerateTextColor(Color backgroundColor)
        {
            // generate a random text color that provides sufficient contrast with the background color
            return RandomContrastingColor(backgroundColor);
        }

        /// <summary>
        /// Generate an accent to the primary and secondary colors
        /// </summary>
        private Color GenerateAccentColor(Color primaryColor, Color secondaryColor)
        {
            // generate a random accent color that is different from the primary and secondary colors
            Color accentColor = RandomColor();
            while (accentColor == primaryColor || accentColor == secondaryColor)
            {
                accentColor = RandomColor();
            }
            return accentColor;
        }

        /// <summary>
        /// Generate additional colors
        /// </summary>
        /// <returns></returns>
        private Color GenerateAdditionalColor()
        {
            // generate a random additional color
            return RandomColor();
        }

        /// <summary>
        /// Get a random color value
        /// </summary>
        private Color RandomColor()
        {
            return new Color(Random.value, Random.value, Random.value);
        }

        /// <summary>
        /// Get a random complementary color
        /// </summary>
        private Color RandomComplementaryColor(Color baseColor)
        {
            // generate a random complementary color for the given base color
            float hue = GetHue(baseColor);
            hue += 0.5f;
            if (hue > 1f)
                hue -= 1f;
            return Color.HSVToRGB(hue, Random.value, Random.value);
        }

        /// <summary>
        /// Get a random contrast color
        /// </summary>
        private Color RandomContrastingColor(Color baseColor)
        {
            // generate a random contrasting color with sufficient contrast to the given base color
            float hue = GetHue(baseColor);
            hue += 0.5f;
            if (hue > 1f)
                hue -= 1f;
            return Color.HSVToRGB(hue, Random.Range(0.4f, 0.6f), Random.Range(0.4f, 0.6f));
        }

        /// <summary>
        /// Get a formatted palette string that can be used to import palettes ("{paletteName}|{colorName}-{RRGGBBAA}")
        /// </summary>
        public string GetPaletteString()
        {
            string finalString = $"{paletteName}";
            foreach (PaletteColor paletteColor in paletteColors)
            {
                finalString += $"|{paletteColor.colorName}-{ColorUtility.ToHtmlStringRGBA(paletteColor.color)}-{paletteColor.autoName}";
            }

            return finalString;
        }
    }
}

