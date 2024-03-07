namespace ColorPaletterV2
{
    using System.Linq;
    using System;

    public static class GUIDGenerator
    {
        /// <summary>
        /// Generate random palette ID
        /// </summary>
        public static string GeneratePaletteID()
        {
            string guid = Guid.NewGuid().ToString();

            // no duplications
            while (ColorPaletter.GetCustomPalettes().Find(palette => palette.paletteID.Equals(guid)) != null)
            {
                guid = Guid.NewGuid().ToString();
            }
            while (ColorPaletter.GetCustomPalettes().Where(palette => palette.paletteColors.Find(palette => palette.paletteColorID.Equals(guid)) != null).Count() > 0)
            {
                guid = Guid.NewGuid().ToString();
            }

            return guid;
        }

        /// <summary>
        /// Generate GUID for Colorizers
        /// </summary>
        public static string GenerateGUID()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
