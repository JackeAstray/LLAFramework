using System.Collections.Generic;

namespace ColorPaletterV2
{
    /// <summary>
    /// Save data for the color palettes
    /// </summary>
    public class ColorPaletterSaveData
    {
        public List<ColorPalette> colorPalettes;

        public ColorPaletterSaveData(List<ColorPalette> colorPalettes)
        {
            this.colorPalettes = colorPalettes;
        }
    }
}
