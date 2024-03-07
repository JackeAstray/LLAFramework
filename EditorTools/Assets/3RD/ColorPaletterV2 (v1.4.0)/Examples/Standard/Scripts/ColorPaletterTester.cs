using UnityEngine;

namespace ColorPaletterV2
{
    /// <summary>
    /// Test script
    /// Here you can find example usage
    /// </summary>
    [ExecuteAlways()]
    public class ColorPaletterTester : MonoBehaviour
    {
        [Header("Open script to see examples")]
        [SerializeField] private string testColorName = "Put Name Here";

        [Header("Showcase")]
        [SerializeField] private Color foundColor;

        private void Update()
        {
            // attempt to find the color
            PaletteColor color = ColorPaletter.GetColor(testColorName);

            if (color != null)
            {
                foundColor = color.color;
            }
        }
    }
}

