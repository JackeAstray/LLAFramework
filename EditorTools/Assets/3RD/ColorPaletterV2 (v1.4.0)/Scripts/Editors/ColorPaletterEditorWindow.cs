using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ColorPaletterV2
{
#if UNITY_EDITOR
    [InitializeOnLoad]

    /// <summary>
    /// The editor window for the ColorPaletter 
    /// </summary>
    public class ColorPaletterEditorWindow : EditorWindow
    {
        private static bool showCustomPalettes = false;
        private static bool showPresetPalettes = false;

        private const string fileName = "colorPaletterSettings.json";

        private static Texture colorPaletterIcon64x;
        private static Texture colorPaletterIconGray;
        private static Texture upArrowTex;
        private static Texture downArrowTex;

        // GUIStyles
        private GUIStyle titleStyle;
        private GUIStyle foldoutStyleBig;
        private GUIStyle paletteNameStyle;
        private GUIStyle smallLabel;

        private const float h3LineHeight = 20f; // normal text

        private Vector2 windowScrollPos = Vector2.zero;

        // on load
        static ColorPaletterEditorWindow()
        {
            EditorApplication.delayCall += Startup;
        }

        static void Startup()
        {
            LoadEditorData();
            ColorPaletter.LoadData(false);
        }

        /// <summary>
        /// Show the editor window
        /// </summary>
        [MenuItem("Window/ColorPaletterV2 Pro/Color Paletter")]
        public static void OpenWindow()
        {
            ColorPaletterEditorWindow window = GetWindow<ColorPaletterEditorWindow>();
            window.titleContent = new GUIContent("Color Paletter", colorPaletterIconGray);
            window.Show();
        }

        private void OnEnable()
        {
            // sort all preset color palette colors
            foreach (ColorPalette presetPalette in ColorPaletter.presetPalettes.ToList())
            {
                presetPalette.SortColors();
            }

            LoadEditorData();
            ColorPaletter.LoadData(false);

            if (colorPaletterIcon64x == null)
                colorPaletterIcon64x = Resources.Load<Texture>("ScriptIcons/ColorPaletterIcon64x64");

            if (colorPaletterIconGray == null)
                colorPaletterIconGray = Resources.Load<Texture>("ScriptIcons/ColorPaletterIconGray");

            if (upArrowTex == null)
                upArrowTex = Resources.Load<Texture>("Other/UpArrow");
            if (downArrowTex == null)
                downArrowTex = Resources.Load<Texture>("Other/DownArrow");

            // asign ID's if needed 
            foreach (ColorPalette customPalette in ColorPaletter.GetCustomPalettes().ToList())
            {
                if (customPalette.paletteID == string.Empty)
                {
                    customPalette.paletteID = GUIDGenerator.GeneratePaletteID();
                }
            }
        }

        private void OnDisable()
        {
            SaveEditorData();
            ColorPaletter.SaveData(false);
        }

        private void OnGUI()
        {
            InitializeGUIStyles();

            windowScrollPos = EditorGUILayout.BeginScrollView(windowScrollPos);

            EditorGUILayout.LabelField($"ColorPaletter v{ColorPaletterVersionControl.colorPaletterVersion}{(ColorPaletterVersionControl.isProVersion ? $", Pro v{ColorPaletterVersionControl.colorPaletterProVersion}" : "")}", smallLabel);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.Space(10f);

            EditorGUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();

            // Display the image to the left of the label
            if (colorPaletterIcon64x != null)
            {
                Rect iconRect = GUILayoutUtility.GetRect(25f, 25f);
                GUI.DrawTexture(iconRect, colorPaletterIcon64x, ScaleMode.ScaleToFit);
            }

            // Color palette title
            EditorGUILayout.LabelField("Color Paletter", titleStyle);

            GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10f);

            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.BeginHorizontal();

            showCustomPalettes = EditorGUILayout.Foldout(showCustomPalettes, $"Custom Palettes ({ColorPaletter.GetCustomPalettes().Count})", true, foldoutStyleBig);

            GUILayout.FlexibleSpace();

            // Add a custom palette
            if (GUILayout.Button("Add Palette"))
            {
                ColorPaletter.AddNewCustomPalette("New Palette");
            }

            // Importing palettes
            if (GUILayout.Button("Import"))
            {
                ColorPaletterInputWindow.ShowWindow();
            }

            // Clearing all palettes
            if (GUILayout.Button("Clear"))
            {
                ColorPaletter.DeleteAllPalettes();
            }
            
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5f);

            // Draw custom color palettes
            if (showCustomPalettes)
            {
                if (ColorPaletter.GetCustomPalettes().Count == 0)
                {
                    EditorGUILayout.HelpBox("Click 'Add Palette' to create a new palette", MessageType.Info);
                }
                else
                {
                    EditorGUI.indentLevel++;

                    foreach (ColorPalette customPalette in ColorPaletter.GetCustomPalettes().ToList())
                    {
                        EditorGUILayout.BeginHorizontal();

                        customPalette.panelOpen = EditorGUILayout.Foldout(customPalette.panelOpen, customPalette.paletteName, true);

                        // get the rect of the label
                        Rect labelRect = GUILayoutUtility.GetLastRect();

                        GUILayout.FlexibleSpace();

                        float buttonWidth = 20f;
                        float spacing = 40f;

                        // calculate the width of the label
                        float labelWidth = EditorStyles.label.CalcSize(new GUIContent(customPalette.paletteName)).x;

                        Rect buttonRect = new Rect(labelRect.x + labelWidth + spacing, labelRect.y, buttonWidth, labelRect.height);

                        EditorGUILayout.BeginHorizontal();

                        // palette deletion
                        if (GUI.Button(buttonRect, "-"))
                        {
                            ColorPaletter.GetCustomPalettes().Remove(customPalette);
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.EndHorizontal();
                            continue;
                        }

                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.EndHorizontal();

                        if (customPalette.panelOpen)
                        {
                            EditorGUI.indentLevel++;
                            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                            EditorGUILayout.Space(1f);

                            EditorGUILayout.BeginHorizontal();

                            customPalette.paletteName = EditorGUILayout.TextField(customPalette.paletteName, paletteNameStyle);

                            if (GUILayout.Button("Add Color", GUILayout.Width(70f)))
                            {
                                customPalette.AddColor();
                            }

                            if (GUILayout.Button("Sort", GUILayout.Width(50f)))
                            {
                                customPalette.SortColors();
                            }

                            if (GUILayout.Button("Random", GUILayout.Width(70f)))
                            {
                                customPalette.GenerateRandomPalette(customPalette.paletteColors.Count);
                            }

                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.Space(1f);

                            foreach (PaletteColor color in customPalette.paletteColors.ToList())
                            {
                                EditorGUILayout.BeginHorizontal();

                                EditorGUILayout.Space(5f);

                                EditorGUILayout.BeginHorizontal(GUILayout.MinWidth(80f));
                                
                                // palette movement in list
                                if (GUILayout.Button(customPalette.paletteColors.IndexOf(color) > 0 ? upArrowTex : null, GUILayout.Width(20f), GUILayout.Height(18f)))
                                {
                                    if (customPalette.paletteColors.IndexOf(color) > 0)
                                    {
                                        MoveColorPalettePosition(customPalette.paletteColors, color, customPalette.paletteColors.IndexOf(color) - 1);
                                    }
                                }
                                
                                if (GUILayout.Button(customPalette.paletteColors.IndexOf(color) < customPalette.paletteColors.Count - 1 ? downArrowTex : null, GUILayout.Width(20f), GUILayout.Height(18f)))
                                {
                                    if (customPalette.paletteColors.IndexOf(color) < customPalette.paletteColors.Count - 1)
                                    {
                                        MoveColorPalettePosition(customPalette.paletteColors, color, customPalette.paletteColors.IndexOf(color) + 1);
                                    }
                                }

                                if (GUILayout.Button("-", GUILayout.Width(20f)))
                                {
                                    customPalette.paletteColors.Remove(color);
                                    EditorGUILayout.EndHorizontal();
                                    EditorGUILayout.EndHorizontal();
                                    continue;
                                }

                                EditorGUILayout.EndHorizontal();

                                color.colorName = EditorGUILayout.TextField(color.colorName, GUILayout.MinWidth(EditorGUIUtility.currentViewWidth / 3.5f));

                                if (color.autoName)
                                {
                                    color.colorName = PaletteColor.AutoFindColorName(color.color);
                                }

                                color.autoName = GUILayout.Toggle(color.autoName, new GUIContent("Auto"));

                                Color storeColor = color.color;
                                color.color = EditorGUILayout.ColorField(color.color, GUILayout.MinWidth(EditorGUIUtility.currentViewWidth / 3.6f));

                                if (GUILayout.Button("Copy"))
                                {
                                    color.CopyColorToClipboard();
                                }

                                EditorGUILayout.EndHorizontal();
                            }

                            EditorGUILayout.Space(5f);

                            EditorGUILayout.BeginHorizontal();

                            if (GUILayout.Button("Get Preset String"))
                            {
                                customPalette.CopyPaletteStringToClipboard();
                            }

                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.Space(1f);

                            EditorGUILayout.EndVertical();
                            EditorGUI.indentLevel--;
                        }
                    }

                    EditorGUI.indentLevel--;
                }
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            showPresetPalettes = EditorGUILayout.Foldout(showPresetPalettes, $"Preset Palettes ({ColorPaletter.presetPalettes.Count})", true, foldoutStyleBig);

            // Draw preset color palettes
            if (showPresetPalettes)
            {
                EditorGUI.indentLevel++;

                foreach (ColorPalette presetPalette in ColorPaletter.presetPalettes.ToList())
                {
                    presetPalette.panelOpen = EditorGUILayout.Foldout(presetPalette.panelOpen, presetPalette.paletteName, true);

                    if (presetPalette.panelOpen)
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                        EditorGUILayout.Space(1f);

                        EditorGUILayout.LabelField(presetPalette.paletteName, paletteNameStyle);

                        EditorGUILayout.Space(1f);

                        foreach (PaletteColor color in presetPalette.paletteColors)
                        {
                            EditorGUILayout.BeginHorizontal();

                            string colorName = color.autoName ? PaletteColor.AutoFindColorName(color.color) : color.colorName;

                            EditorGUILayout.TextField(colorName, GUILayout.MinWidth(EditorGUIUtility.currentViewWidth / 3.5f));

                            EditorGUILayout.ColorField(color.color, GUILayout.MinWidth(EditorGUIUtility.currentViewWidth / 3.6f));

                            if (GUILayout.Button("Copy"))
                            {
                                color.CopyColorToClipboard();
                            }

                            EditorGUILayout.EndHorizontal();
                        }

                        EditorGUILayout.BeginHorizontal();

                        if (GUILayout.Button("Get Preset String"))
                        {
                            presetPalette.CopyPaletteStringToClipboard();
                        }

                        EditorGUILayout.EndHorizontal();

                        EditorGUI.indentLevel--;

                        EditorGUILayout.Space(1f);

                        EditorGUILayout.EndVertical();

                        EditorGUI.indentLevel--;
                    }
                }

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space(5f);

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndScrollView();

            Repaint();
        }

        /// <summary>
        /// Move the colorpalette's position in the list
        /// </summary>
        private void MoveColorPalettePosition(List<PaletteColor> paletteColors, PaletteColor targetColor, int newIndex)
        {
            if (targetColor != null)
            {
                int oldIndex = paletteColors.IndexOf(targetColor);
                if (oldIndex > -1)
                {
                    paletteColors.RemoveAt(oldIndex);
                    // if (newIndex > oldIndex) newIndex--;

                    paletteColors.Insert(newIndex, targetColor);
                }
            }
        }
        
        /// <summary>
        /// Save the data
        /// </summary>
        private void SaveEditorData()
        {
            ColorPaletterSettingsSaveData data = new ColorPaletterSettingsSaveData(showCustomPalettes, showPresetPalettes);
            ColorPaletterDataManager.SaveData<ColorPaletterSettingsSaveData>(fileName, data, false);
        }

        /// <summary>
        /// Load editor data
        /// </summary>
        private static void LoadEditorData()
        {
            ColorPaletterSettingsSaveData data = ColorPaletterDataManager.LoadData<ColorPaletterSettingsSaveData>(fileName, false);

            if (data != null)
            {
                showCustomPalettes = data.showCustomPalettes;
                showPresetPalettes = data.showPresetPalettes;
            }
        }

        /// <summary>
        /// Get editor data
        /// </summary>
        private ColorPaletterSettingsSaveData GetEditorData()
        {
            ColorPaletterSettingsSaveData data = ColorPaletterDataManager.LoadData<ColorPaletterSettingsSaveData>(fileName, false);
            return data;
        }

        /// <summary>
        /// Initialize GUI styles
        /// </summary>
        private void InitializeGUIStyles()
        {
            titleStyle = new GUIStyle(EditorStyles.label)
            {
                fontStyle = FontStyle.Bold,
                fontSize = 20,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.white }
            };

            foldoutStyleBig = new GUIStyle(EditorStyles.foldout)
            {
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.white }
            };

            paletteNameStyle = new GUIStyle(EditorStyles.textField)
            {
                fixedHeight = h3LineHeight
            };

            smallLabel = new GUIStyle(GUI.skin.label)
            {
                fontSize = 9
            };
        }
    }
#endif
}