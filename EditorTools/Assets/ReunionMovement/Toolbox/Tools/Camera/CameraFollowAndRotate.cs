using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace GameLogic
{
    /// <summary>
    /// 摄像机围绕物体旋转
    /// </summary>
    public class CameraFollowAndRotate : MonoBehaviour
    {
        //目标对象
        public Transform targetPos;
        //摄像机
        public Camera cfrCamera { get; private set; }

        //隐藏光标
        public bool hideCursor = true;

        //初始角度
        public float rotX = 0;
        public float rotY = 0;

        public float offsetHeight = 0f;
        public float lateralOffset = 0f;
        public float offsetDistance = 30f;
        public float maxDistance = 30f;                     //最大距离
        public float minDistance = 10f;                     //最小距离
        public float zoomSpeed = 50f;                       //缩放速度
        public float zoomValue = 50f;                       //缩放值
        public float rotateSpeed = 15f;                     //转速
        public float maxRot = 90f;                          //最大旋转角度
        public float minRot = -90f;                         //最小旋转角度
        public float distance = 30f;                        //默认距离

        Quaternion destRot = Quaternion.identity;
        private Vector3 relativePosition = Vector3.zero;    //相对位置

        private Vector2 lastTouchPos0;
        private Vector2 lastTouchPos1;

        public bool isEnable = true;
        private bool isMobilePlatform;

        Mouse mouse;
        Touchscreen touchscreen;
        Keyboard keyboard;

        public void Start()
        {
            OrbitCamera(0, -0);
            UpdatePosition();
            
            if (hideCursor)
            {
                HideCursor();
            }

            cfrCamera = GetComponent<Camera>();

            mouse = Mouse.current;
            touchscreen = Touchscreen.current;
            keyboard = Keyboard.current;

            isMobilePlatform = Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer;
        }

        public void Update()
        {
            bool isPointerOverUI = IsPointerOverUI();

            // 检查是否按下了Alt键
            if (keyboard[Key.LeftAlt].wasPressedThisFrame)
            {
                DisplayCursor();
            }
            else if (keyboard[Key.LeftAlt].wasReleasedThisFrame)
            {
                if (hideCursor)
                {
                    HideCursor();
                }
            }

            if (!isPointerOverUI)
            {
                GetViewInput();
            }
        }

        private void GetViewInput()
        {
            if (isEnable)
            {
                if (!isMobilePlatform)
                {
                    //鼠标左键拖拽
                    if (mouse.leftButton.isPressed)
                    {
                        float horz = mouse.delta.x.ReadValue();
                        float vert = mouse.delta.y.ReadValue();
                        OrbitCamera(horz, -vert);
                    }

                    //滚轮
                    SetZoom();
                }
                else
                {
                    //没有触摸
                    if (touchscreen.touches.Count <= 0)
                    {
                        return;
                    }

                    //触摸为1 开始触摸
                    if (touchscreen.touches.Count == 1 && touchscreen.touches[0].phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Began)
                    {

                    }
                    //触摸为1 滑动
                    else if (touchscreen.touches.Count == 1)
                    {
                        TouchControl touch = touchscreen.touches[0];

                        float horz = touch.delta.x.ReadValue();
                        float vert = touch.delta.y.ReadValue();

                        OrbitCamera(horz, -vert);
                    }
                    else if (touchscreen.touches.Count == 2)
                    {
                        HandleMultiTouch();
                    }
                }
            }
        }

        /// <summary>
        /// 旋转摄像机
        /// </summary>
        /// <param name="horz">水平调整</param>
        /// <param name="vert">垂直调整</param>
        private void OrbitCamera(float horz, float vert)
        {
            float step = Time.deltaTime * rotateSpeed;
            rotX += horz * step;
            rotY += vert * step;

            rotY = Mathf.Clamp(rotY, minRot, maxRot);
            Quaternion addRot = Quaternion.Euler(0f, rotX, 0f);
            destRot = addRot * Quaternion.Euler(rotY, 0f, 0f);
            transform.rotation = destRot;
            UpdatePosition();
        }

        /// <summary>
        /// 更新摄像机位置
        /// </summary>
        public void UpdatePosition()
        {
            offsetDistance = Mathf.MoveTowards(offsetDistance, distance, Time.deltaTime * zoomSpeed);
            relativePosition = (targetPos.position + (Vector3.up * offsetHeight)) + (transform.rotation * (Vector3.forward * -offsetDistance)) + (transform.right * lateralOffset);
            transform.position = relativePosition;
        }

        /// <summary>
        /// 判断是否在UI上
        /// </summary>
        /// <returns></returns>
        private bool IsPointerOverUI()
        {
#if UNITY_EDITOR
            return EventSystem.current.IsPointerOverGameObject();
#elif UNITY_ANDROID || UNITY_IPHONE
            return EventSystem.current.IsPointerOverGameObject(touchscreen.touches[0].touchId.ReadValue());
#else
            return EventSystem.current.IsPointerOverGameObject();
#endif
        }

        public void DisplayCursor()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        public void HideCursor()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        /// <summary>
        /// 多点触摸
        /// </summary>
        private void HandleMultiTouch()
        {
            TouchControl touch0 = touchscreen.touches[0];
            TouchControl touch1 = touchscreen.touches[1];

            if (touch1.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Began)
            {
                lastTouchPos0 = touch0.position.ReadValue();
                lastTouchPos1 = touch1.position.ReadValue();
            }
            else if (touch0.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Moved ||
                     touch1.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Moved)
            {
                var tempPos0 = touch0.position.ReadValue();
                var tempPos1 = touch1.position.ReadValue();
                var currDist = Vector2.Distance(tempPos0, tempPos1);
                var lastDist = Vector2.Distance(lastTouchPos0, lastTouchPos1);
                var delta = currDist - lastDist;

                SetZoom(-delta);

                lastTouchPos0 = tempPos0;
                lastTouchPos1 = tempPos1;
            }
        }

        #region 设置摄像头远近
        /// <summary>
        /// 设置摄像头远近
        /// </summary>
        public void SetZoom()
        {
            float delta = mouse.scroll.ReadValue().y * -zoomValue;
            SetZoom(delta);
        }

        /// <summary>
        /// 设置摄像头远近
        /// </summary>
        public void SetZoom(float delta)
        {
            distance += delta;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);
            UpdatePosition();
        }
        #endregion

        #region 距离
        public float GetDistance()
        {
            return distance;
        }

        public void SetDistance(float distance)
        {
            this.distance = distance;
        }
        #endregion

        #region Set Enable
        /// <summary>
        /// 设置Enable为True
        /// </summary>
        public void SetIsEnableTrue()
        {
            isEnable = true;
        }
        /// <summary>
        /// 设置Enable为False
        /// </summary>
        public void SetIsEnableFalse()
        {
            isEnable = false;
        }
        #endregion
    }
}