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
        public bool EditorOnly;

        [SerializeField] private float updateInterval = 1f;
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

        /// <summary>
        /// 在开始时跳过一些时间，以跳过游戏开始时的性能下降，并产生更准确的平均FPS
        /// </summary>
        private float idleTime = 2f;

        private float elapsed;
        private int frames;
        private int quantity;
        private float fps;
        private float averageFps;

        //FPS文字颜色
        private Color goodColor;
        private Color okColor;
        private Color badColor;

        private float okFps;
        private float badFps;

        private Rect rect1;
        private Rect rect2;

        private void Awake()
        {
            if (EditorOnly && !Application.isEditor) return;

            goodColor = new Color(0.5f, 1f, 0f);
            okColor = new Color(1f, 0.8f, 0f);
            badColor = new Color(1f, 0f, 0.25f);

            var percent = targetFrameRate / 100;
            var percent10 = percent * 10;
            var percent40 = percent * 40;
            okFps = targetFrameRate - percent10;
            badFps = targetFrameRate - percent40;

            var xPos = 0;
            var yPos = 0;
            var linesHeight = 40;
            var linesWidth = 90;
            if (anchor == Anchor.LeftBottom || anchor == Anchor.RightBottom) yPos = Screen.height - linesHeight;
            if (anchor == Anchor.RightTop || anchor == Anchor.RightBottom) xPos = Screen.width - linesWidth;
            xPos += xOffset;
            yPos += yOffset;
            var yPos2 = yPos + 18;
            rect1 = new Rect(xPos, yPos, linesWidth, linesHeight);
            rect2 = new Rect(xPos, yPos2, linesWidth, linesHeight);

            elapsed = updateInterval;
        }

        private void Update()
        {
            if (EditorOnly && !Application.isEditor) return;

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
            }

            quantity++;
            averageFps += (fps - averageFps) / quantity;
        }

        private void OnGUI()
        {
            if (EditorOnly && !Application.isEditor) return;

            var defaultColor = GUI.color;
            var color = goodColor;
            if (fps <= okFps || averageFps <= okFps) color = okColor;
            if (fps <= badFps || averageFps <= badFps) color = badColor;
            GUI.color = color;
            GUI.Label(rect1, "FPS: " + (int)fps);
            GUI.Label(rect2, "平均值FPS: " + (int)averageFps);
            GUI.color = defaultColor;
        }
    }
}