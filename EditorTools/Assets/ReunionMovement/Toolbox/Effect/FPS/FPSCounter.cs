using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// FPS计数器工具
    /// </summary>
    public class FPSCounter : MonoBehaviour
    {
        // 仅在编辑器中显示
        public bool editorOnly;

        // 更新间隔
        [SerializeField] private float updateInterval = 1f;
        // 目标帧率
        [SerializeField] private int targetFrameRate = 30;

        [SerializeField] private Anchor anchor;
        [SerializeField] private int xOffset;
        [SerializeField] private int yOffset;

        /// <summary>
        /// 锚
        /// </summary>
        private enum Anchor
        {
            LeftTop, 
            LeftBottom, 
            RightTop, 
            RightBottom
        }

        private float idleTime = 2f;

        private float elapsed;
        private int frames;
        private float fps;
        private float averageFps;

        private Color goodColor;
        private Color okColor;
        private Color badColor;

        private float okFps;
        private float badFps;

        private Rect rect1;
        private Rect rect2;

        private void Awake()
        {
            if (editorOnly && !Application.isEditor) return;

            goodColor = new Color(0.5f, 1f, 0f);
            okColor = new Color(1f, 0.8f, 0f);
            badColor = new Color(1f, 0f, 0.25f);

            var percent = targetFrameRate / 100f;
            okFps = targetFrameRate - percent * 10;
            badFps = targetFrameRate - percent * 40;

            var xPos = 0;
            var yPos = 0;
            var linesHeight = 40;
            var linesWidth = 90;
            if (anchor == Anchor.LeftBottom || anchor == Anchor.RightBottom) yPos = Screen.height - linesHeight;
            if (anchor == Anchor.RightTop || anchor == Anchor.RightBottom) xPos = Screen.width - linesWidth;
            xPos += xOffset;
            yPos += yOffset;
            rect1 = new Rect(xPos, yPos, linesWidth, linesHeight);
            rect2 = new Rect(xPos, yPos + 18, linesWidth, linesHeight);

            elapsed = updateInterval;
        }

        private void Update()
        {
            if (editorOnly && !Application.isEditor) return;

            if (idleTime > 0)
            {
                idleTime -= Time.deltaTime;
                return;
            }

            elapsed += Time.deltaTime;
            ++frames;

            if (elapsed >= updateInterval)
            {
                fps = frames / elapsed;
                elapsed = 0;
                frames = 0;
                averageFps = (averageFps * (frames - 1) + fps) / frames;
            }
        }

        private void OnGUI()
        {
            if (editorOnly && !Application.isEditor) return;

            var defaultColor = GUI.color;
            var color = fps <= badFps || averageFps <= badFps ? badColor : fps <= okFps || averageFps <= okFps ? okColor : goodColor;
            GUI.color = color;
            GUI.Label(rect1, "FPS: " + (int)fps);
            GUI.Label(rect2, "平均值FPS: " + (int)averageFps);
            GUI.color = defaultColor;
        }
    }
}