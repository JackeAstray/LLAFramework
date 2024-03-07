using UnityEditor;
using UnityEngine;
using ColorPaletterV2;

namespace ColorPaletterV2
{
#if UNITY_EDITOR
    /// <summary>
    /// Input prompt window for color paletter
    /// </summary>
    public class ColorPaletterInputWindow : EditorWindow
    {
        private string userInput = string.Empty;
        private static readonly Vector2 windowSize = new Vector2(200, 250);

        public static void ShowWindow()
        {
            ColorPaletterInputWindow window = GetWindow<ColorPaletterInputWindow>();
            window.titleContent = new GUIContent("Color Paletter Prompt");
            window.minSize = windowSize;
            window.maxSize = window.minSize;
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Import palette string:");
            userInput = EditorGUILayout.TextField(userInput);

            GUILayout.Space(10);

            if (GUILayout.Button("Submit"))
            {
                ColorPalette newPalette = ColorPaletter.AddColorPaletteFromString(userInput);

                // message color paletter
                if (newPalette != null)
                {
                    Debug.Log($"Successfully imported new palette! ({newPalette.paletteName})");
                }
                else
                {
                    Debug.LogError("Could not import invalid palette string");
                }

                // close the window
                Close();
            }
        }
    }
#endif
}