using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace GameLogic.UI.ImageExtensions.Editor
{
    public static class EditorUtility
    {
        [MenuItem("GameObject/UI/ImageEx")]
        public static void AddImageExObject()
        {
            GameObject g = new GameObject { name = "ImageEx" };

            Transform parent = GetParentTransform();
            g.transform.SetParent(parent, false);
            g.AddComponent<ImageEx>();
            Selection.activeGameObject = g;

            Undo.RegisterCreatedObjectUndo(g, "ImageEx Created");
            UnityEditor.EditorUtility.SetDirty(g);
        }

        [MenuItem("GameObject/UI/ImageEx Basic")]
        public static void AddImageExBasicObject()
        {
            GameObject g = new GameObject { name = "ImageExBasic" };
            Transform parent = GetParentTransform();
            g.transform.SetParent(parent, false);
            g.AddComponent<ImageExBasic>();
            Selection.activeGameObject = g;

            Undo.RegisterCreatedObjectUndo(g, "ImageEx Basic Created");
            UnityEditor.EditorUtility.SetDirty(g);
        }

        private static Transform GetParentTransform()
        {
            Transform parent;
            if (Selection.activeGameObject != null &&
                Selection.activeGameObject.GetComponentInParent<Canvas>() != null)
            {
                parent = Selection.activeGameObject.transform;
            }
            else
            {
                Canvas c = GetCanvas();
                AddAdditionalShaderChannelsToCanvas(c);
                parent = c.transform;
            }

            return parent;
        }

        private static Canvas GetCanvas()
        {
            StageHandle handle = StageUtility.GetCurrentStageHandle();
            if (!handle.FindComponentOfType<Canvas>())
            {
                EditorApplication.ExecuteMenuItem("GameObject/UI/Canvas");
            }

            Canvas c = handle.FindComponentOfType<Canvas>();
            return c;
        }

        [MenuItem("CONTEXT/Image/Replace with ImageEx")]
        public static void ReplaceWithImageEx(MenuCommand command)
        {
            if (command.context is ImageEx) return;
            if (command.context is ImageExBasic)
            {
                // Convert ImageExBasic to ImageEx
                ImageExBasic img = (ImageExBasic)command.context;
                GameObject obj = img.gameObject;
                Object.DestroyImmediate(img);
                obj.AddComponent<ImageEx>();
                UnityEditor.EditorUtility.SetDirty(obj);
            }
            else
            {
                Image img = (Image)command.context;
                GameObject obj = img.gameObject;
                Object.DestroyImmediate(img);
                obj.AddComponent<ImageEx>();
                UnityEditor.EditorUtility.SetDirty(obj);
            }

        }

        [MenuItem("CONTEXT/Image/Replace with ImageEx Basic")]
        public static void ReplaceWithImageExBasic(MenuCommand command)
        {
            if (command.context is ImageExBasic) return;
            if (command.context is ImageEx)
            {
                // Convert ImageEx to ImageExBasic
                ImageEx img = (ImageEx)command.context;
                GameObject obj = img.gameObject;
                Object.DestroyImmediate(img);
                obj.AddComponent<ImageExBasic>();
                UnityEditor.EditorUtility.SetDirty(obj);
            }
            else
            {
                Image img = (Image)command.context;
                GameObject obj = img.gameObject;
                Object.DestroyImmediate(img);
                obj.AddComponent<ImageExBasic>();
                UnityEditor.EditorUtility.SetDirty(obj);
            }
        }

        internal static void AddAdditionalShaderChannelsToCanvas(Canvas c)
        {
            AdditionalCanvasShaderChannels additionalShaderChannels = c.additionalShaderChannels;
            additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord1;
            additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord2;
            c.additionalShaderChannels = additionalShaderChannels;
        }

        internal static bool HasAdditionalShaderChannels(Canvas c)
        {
            AdditionalCanvasShaderChannels asc = c.additionalShaderChannels;
            return (asc & AdditionalCanvasShaderChannels.TexCoord1) != 0 &&
                   (asc & AdditionalCanvasShaderChannels.TexCoord2) != 0;
        }

        public static void CornerRadiusModeGUI(Rect rect, ref SerializedProperty property, string[] toolBarHeading,
            string label = "Corner Radius")
        {
            bool boolVal = property.boolValue;
            Rect labelRect = new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, rect.height);
            Rect toolBarRect = new Rect(rect.x + EditorGUIUtility.labelWidth, rect.y,
                rect.width - EditorGUIUtility.labelWidth, rect.height);

            EditorGUI.BeginChangeCheck();
            {
                EditorGUI.showMixedValue = property.hasMultipleDifferentValues;
                EditorGUI.LabelField(labelRect, label);

                boolVal = GUI.Toolbar(toolBarRect, boolVal ? 1 : 0, toolBarHeading) == 1;
                EditorGUI.showMixedValue = false;
            }
            if (EditorGUI.EndChangeCheck())
            {
                property.boolValue = boolVal;
            }
        }

        private static Sprite emptySprite;

        internal static Sprite EmptySprite
        {
            get
            {
                if (emptySprite == null)
                {
                    emptySprite = Resources.Load<Sprite>("UI/Sprites/default_empty_sprite");
                }

                return emptySprite;
            }
        }

        internal static string FindImageExtensionsRootDirectory()
        {
            string guid = AssetDatabase.FindAssets("ImageEx t:Shader").FirstOrDefault();
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrEmpty(path)) return String.Empty;
            string[] directories = path.Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar });
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < directories.Length; i++)
            {
                sb.Append(directories[i]);
                sb.Append(Path.DirectorySeparatorChar);
                if (directories[i].Equals("ImageExtensions", StringComparison.OrdinalIgnoreCase))
                    break;
            }
            return sb.ToString();
        }
    }
}