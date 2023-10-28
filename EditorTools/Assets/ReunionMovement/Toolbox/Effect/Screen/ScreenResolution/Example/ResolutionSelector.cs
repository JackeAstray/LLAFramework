using UnityEngine;
using System.Collections;

namespace GameLogic
{
    public class ResolutionSelector : MonoBehaviour
    {
        void OnGUI()
        {
            if (ResolutionMgr.Instance == null) return;

            ResolutionMgr resolutionManager = ResolutionMgr.Instance;

            GUILayout.BeginArea(new Rect(20, 10, 200, Screen.height - 10));

            GUILayout.Label("选择分辨率");

            if (GUILayout.Button(Screen.fullScreen ? "窗口" : "全屏"))
                resolutionManager.ToggleFullscreen();

            int i = 0;
            foreach (Vector2 r in Screen.fullScreen ? resolutionManager.FullscreenResolutions : resolutionManager.WindowedResolutions)
            {
                string label = r.x + "x" + r.y;
                if (r.x == Screen.width && r.y == Screen.height) label += "*";
                if (r.x == resolutionManager.DisplayResolution.width && r.y == resolutionManager.DisplayResolution.height) label += " (native)";

                if (GUILayout.Button(label))
                    resolutionManager.SetResolution(i, Screen.fullScreen);

                i++;
            }

            if (GUILayout.Button("获取当前分辨率"))
            {
                Resolution r = Screen.currentResolution;
                Debug.Log("显示分辨率为 " + r.width + " x " + r.height);
            }

            GUILayout.EndArea();
        }
    }
}