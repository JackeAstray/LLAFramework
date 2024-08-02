using System.IO;
using UnityEditor;
using UnityEngine;

namespace GameLogic.UI.ImageExtensions.Editor
{
    [InitializeOnLoad]
    internal static class EditorContents
    {
        private static string imagesDirectory = string.Empty;

        private static GUIContent flipHorizontalNormal, flipHorizontalActive;
        private static GUIContent flipVerticalNormal, flipVerticalActive;

        private static GUIContent rotateLeftNormal, rotateLeftActive;
        private static GUIContent rotateRightNormal, rotateRightActive;

        private static Texture2D logo, background, title;

        public static GUIContent FlipHorizontalNormal
        {
            get
            {
                if (flipHorizontalNormal != null) return flipHorizontalNormal;
                flipHorizontalNormal = new GUIContent(LoadImage("flip_h", false));
                return flipHorizontalNormal;
            }
        }

        public static GUIContent FlipHorizontalActive
        {
            get
            {
                if (flipHorizontalActive != null) return flipHorizontalActive;
                flipHorizontalActive = new GUIContent(LoadImage("flip_h", true));
                return flipHorizontalActive;
            }
        }

        public static GUIContent FlipVerticalNormal
        {
            get
            {
                if (flipVerticalNormal != null) return flipVerticalNormal;
                flipVerticalNormal = new GUIContent(LoadImage("flip_v", false));
                return flipVerticalNormal;
            }
        }

        public static GUIContent FlipVerticalActive
        {
            get
            {
                if (flipVerticalActive != null) return flipVerticalActive;
                flipVerticalActive = new GUIContent(LoadImage("flip_v", true));
                return flipVerticalActive;
            }
        }

        public static GUIContent RotateLeftNormal
        {
            get
            {
                if (rotateLeftNormal != null) return rotateLeftNormal;
                rotateLeftNormal = new GUIContent(LoadImage("rotate_left", false));
                return rotateLeftNormal;
            }
        }

        public static GUIContent RotateLeftActive
        {
            get
            {
                if (rotateLeftActive != null) return rotateLeftActive;
                rotateLeftActive = new GUIContent(LoadImage("rotate_left", true));
                return rotateLeftActive;
            }
        }

        public static GUIContent RotateRightNormal
        {
            get
            {
                if (rotateRightNormal != null) return rotateRightNormal;
                rotateRightNormal = new GUIContent(LoadImage("rotate_right", false));
                return rotateRightNormal;
            }
        }

        public static GUIContent RotateRightActive
        {
            get
            {
                if (rotateRightActive != null) return rotateRightActive;
                rotateRightActive = new GUIContent(LoadImage("rotate_right", true));
                return rotateRightActive;
            }
        }

        public static Texture Logo
        {
            get
            {
                if (logo != null) return logo;
                logo = LoadImage("logo", false, true);
                return logo;
            }
        }

        public static Texture Background
        {
            get
            {
                if (background != null) return background;
                background = LoadImage("background", false, true);
                return background;
            }
        }

        public static Texture Title
        {
            get
            {
                if (title != null) return title;
                title = LoadImage("title", false, true);
                return title;
            }
        }

        static EditorContents()
        {            
            FindIconsDirectory();
        }

        /// <summary>
        /// 查找工具包图标目录
        /// </summary>
        private static void FindIconsDirectory()
        {
            string rootDir = EditorUtility.FindImageExtensionsRootDirectory();
            imagesDirectory = string.IsNullOrEmpty(rootDir) ? string.Empty : Path.Combine(rootDir, "Editor", "Images");
        }

        private static Texture2D LoadImage(string name, bool activeState, bool ignoreState = false)
        {
            int colorLevel = 0;
            if (!ignoreState)
            {
                if (activeState) colorLevel = 3;
                else colorLevel = EditorGUIUtility.isProSkin ? 2 : 1;
            }

            if (imagesDirectory == string.Empty) FindIconsDirectory();

            string assetPath = $"{imagesDirectory}{Path.DirectorySeparatorChar}{name}{(ignoreState ? string.Empty : $"_{colorLevel}")}.png";
            return AssetDatabase.LoadAssetAtPath(assetPath, typeof(Texture2D)) as Texture2D;
        }
    }
}