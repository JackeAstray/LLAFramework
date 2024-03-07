namespace ColorPaletterV2
{
    /// <summary>
    /// Save data for the color paletter editor settings
    /// </summary>
    public class ColorPaletterSettingsSaveData
    {
        public bool showCustomPalettes;
        public bool showPresetPalettes;

        public ColorPaletterSettingsSaveData(bool showCustomPalettes, bool showPresetPalettes)
        {
            this.showCustomPalettes = showCustomPalettes;
            this.showPresetPalettes = showPresetPalettes;
        }
    }
}
