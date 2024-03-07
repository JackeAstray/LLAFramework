using System;
using UnityEditor;

namespace ColorPaletterV2
{
    /// <summary>
    /// Class doesn't contain much of a use as of now...
    /// </summary>
#if UNITY_EDITOR
    [InitializeOnLoad]
    public static class ProjectOpenCloseDetector
    {
        static ProjectOpenCloseDetector()
        {
            EditorApplication.wantsToQuit += OnWantsToQuit;
            EditorApplication.quitting += OnQuit;
        }

        public static Action OnProjectClose;
        public static Action OnProjectWantsToClose;

        /// <summary>
        /// Unity project closed
        /// </summary>
        private static bool OnWantsToQuit()
        {
            OnProjectWantsToClose?.Invoke();

            ColorPaletter.SaveData(false);
            return true;
        }

        /// <summary>
        /// On quit game
        /// </summary>
        private static void OnQuit()
        {
            // detect when the Unity editor is closing
            OnProjectClose?.Invoke();
        }
    }
#endif
}
