using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LLAFramework
{
    /// <summary>
    /// 宽高比工具
    /// </summary>
    public class AspectUtility : MonoBehaviour
    {
        static Camera backgroundCam;
        static Camera staticCam;
        Camera cam;

        void Awake()
        {
            cam = GetComponent<Camera>();
            if (!cam)
            {
                cam = Camera.main;
            }
            if (!cam)
            {
                Debug.LogError("无摄像头可用!");
                return;
            }

            staticCam = cam;
            UpdateCamera();
        }

        private void UpdateCamera()
        {
            if (!ResolutionMgr.FixedAspectRatio || !cam) return;

            float currentAspectRatio = (float)Screen.width / Screen.height;

            if (Mathf.Approximately(currentAspectRatio, ResolutionMgr.TargetAspectRatio))
            {
                cam.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
                if (backgroundCam)
                {
                    Destroy(backgroundCam.gameObject);
                }
                return;
            }

            if (currentAspectRatio > ResolutionMgr.TargetAspectRatio)
            {
                float inset = 1.0f - ResolutionMgr.TargetAspectRatio / currentAspectRatio;
                cam.rect = new Rect(inset / 2, 0.0f, 1.0f - inset, 1.0f);
            }
            else
            {
                float inset = 1.0f - currentAspectRatio / ResolutionMgr.TargetAspectRatio;
                cam.rect = new Rect(0.0f, inset / 2, 1.0f, 1.0f - inset);
            }

            if (!backgroundCam)
            {
                backgroundCam = new GameObject("BackgroundCam", typeof(Camera)).GetComponent<Camera>();
                backgroundCam.depth = int.MinValue;
                backgroundCam.clearFlags = CameraClearFlags.SolidColor;
                backgroundCam.backgroundColor = Color.black;
                backgroundCam.cullingMask = 0;
            }
        }

        public static int screenHeight => (int)(Screen.height * staticCam.rect.height);

        public static int screenWidth => (int)(Screen.width * staticCam.rect.width);

        public static int xOffset => (int)(Screen.width * staticCam.rect.x);

        public static int yOffset => (int)(Screen.height * staticCam.rect.y);

        public static Rect screenRect => new Rect(staticCam.rect.x * Screen.width, staticCam.rect.y * Screen.height, staticCam.rect.width * Screen.width, staticCam.rect.height * Screen.height);

        public static Vector3 mousePosition
        {
            get
            {
                Vector3 mousePos = Input.mousePosition;
                mousePos.y -= (int)(staticCam.rect.y * Screen.height);
                mousePos.x -= (int)(staticCam.rect.x * Screen.width);
                return mousePos;
            }
        }

        public static Vector2 guiMousePosition
        {
            get
            {
                Vector2 mousePos = Event.current.mousePosition;
                mousePos.y = Mathf.Clamp(mousePos.y, staticCam.rect.y * Screen.height, staticCam.rect.y * Screen.height + staticCam.rect.height * Screen.height);
                mousePos.x = Mathf.Clamp(mousePos.x, staticCam.rect.x * Screen.width, staticCam.rect.x * Screen.width + staticCam.rect.width * Screen.width);
                return mousePos;
            }
        }

        private int lastWidth = -1, lastHeight = -1;
        public void Update()
        {
            if (Screen.width != lastWidth || Screen.height != lastHeight)
            {
                lastWidth = Screen.width;
                lastHeight = Screen.height;
                UpdateCamera();
            }
        }
    }
}