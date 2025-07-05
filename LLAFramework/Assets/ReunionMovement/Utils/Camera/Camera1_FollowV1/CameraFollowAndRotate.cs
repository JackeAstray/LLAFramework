using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using InputSystemTouch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using InputSystemTouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace LLAFramework
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

        protected const string MOUSEX = "Mouse X";
        protected const string MOUSEY = "Mouse Y";


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


#if USE_INPUT_SYSTEM
            mouse = Mouse.current;
            touchscreen = Touchscreen.current;
            keyboard = Keyboard.current;

            EnhancedTouchSupport.Enable();
#endif
            isMobilePlatform = Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer;
        }

        public void Update()
        {
            bool isPointerOverUI = IsPointerOverUI();
#if USE_INPUT_SYSTEM
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
#else
            // 检查是否按下了Alt键
            if (Input.GetKeyDown(KeyCode.LeftAlt))
            {
                DisplayCursor();
            }
            else if (Input.GetKeyUp(KeyCode.LeftAlt))
            {
                if (hideCursor)
                {
                    HideCursor();
                }
            }
#endif
            if (!isPointerOverUI)
            {
                GetViewInput();
            }
        }

        public void OnDestroy()
        {
#if USE_INPUT_SYSTEM

#endif
        }

        private void GetViewInput()
        {
            if (isEnable)
            {
                if (!isMobilePlatform)
                {
#if USE_INPUT_SYSTEM
                    //鼠标左键拖拽
                    if (mouse.leftButton.isPressed)
                    {
                        float horz = mouse.delta.x.ReadValue();
                        float vert = mouse.delta.y.ReadValue();
                        OrbitCamera(horz, -vert);
                    }

                    //滚轮
                    SetZoom();
#else
                    //鼠标左键拖拽
                    if (Input.GetMouseButton(0))
                    {
                        float horz = Input.GetAxis(MOUSEX);
                        float vert = Input.GetAxis(MOUSEY);
                        OrbitCamera(horz, -vert);
                    }

                    //滚轮
                    SetZoom();
#endif
                }
                else
                {
                    TouchSystem();
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
#if USE_INPUT_SYSTEM
            if (InputSystemTouch.activeTouches.Count > 0)
            {
                InputSystemTouch touch = InputSystemTouch.activeTouches[0];
                return EventSystem.current.IsPointerOverGameObject(touch.touchId);
            }
            return false;
#else
            if (Input.touchCount > 0)
            {
                return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
            }
            return false;
#endif
#else
            return EventSystem.current.IsPointerOverGameObject();
#endif
        }

        int GetTouchCount()
        {
#if USE_INPUT_SYSTEM
            return InputSystemTouch.activeTouches.Count;
#else
            return Input.touchCount;
#endif
        }

        public void TouchSystem()
        {
            //没有触摸
            if (GetTouchCount() <= 0)
            {
                return;
            }

#if USE_INPUT_SYSTEM
            //触摸为1 开始触摸
            if (InputSystemTouch.activeTouches.Count == 1)
            {
                InputSystemTouch touch = InputSystemTouch.activeTouches[0];
                if (touch.phase == InputSystemTouchPhase.Began)
                {
                    //开始触摸   
                }
                else
                {
                    // 滑动
                    float horz = touch.delta.x;
                    float vert = touch.delta.y;

                    OrbitCamera(horz, -vert);
                }
            }
            else
            {
                HandleMultiTouch();
            }
#else
            //触摸为1 开始触摸
            if (1 == Input.touchCount && Input.GetTouch(0).phase == UnityEngine.TouchPhase.Began)
            {

            }

            //触摸为1 滑动
            else if (Input.touchCount == 1)
            {
                UnityEngine.Touch touch = Input.GetTouch(0);

                float horz = touch.deltaPosition.x;
                float vert = touch.deltaPosition.y;

                OrbitCamera(horz, -vert);
            }
            else if (Input.touchCount == 2)
            {
                HandleMultiTouch();
            }
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
#if USE_INPUT_SYSTEM
            var firstTouch = InputSystemTouch.activeTouches[0];
            var secondTouch = InputSystemTouch.activeTouches[1];

            if (firstTouch.phase == InputSystemTouchPhase.Began ||
                secondTouch.phase == InputSystemTouchPhase.Began)
            {
                lastTouchPos0 = firstTouch.screenPosition;
                lastTouchPos1 = secondTouch.screenPosition;
            }
            else if (firstTouch.phase == InputSystemTouchPhase.Moved ||
                     secondTouch.phase == InputSystemTouchPhase.Moved)
            {
                var tempPos0 = firstTouch.screenPosition;
                var tempPos1 = secondTouch.screenPosition;
                var currDist = Vector2.Distance(tempPos0, tempPos1);
                var lastDist = Vector2.Distance(lastTouchPos0, lastTouchPos1);
                var delta = currDist - lastDist;

                SetZoom(-delta);

                lastTouchPos0 = tempPos0;
                lastTouchPos1 = tempPos1;
            }
#else
            UnityEngine.Touch newTouch1 = Input.GetTouch(0);
            UnityEngine.Touch newTouch2 = Input.GetTouch(1);

            //第二点刚接触屏幕，只做记录，不做处理
            if (newTouch2.phase == UnityEngine.TouchPhase.Began)
            {
                lastTouchPos0 = newTouch1.position;
                lastTouchPos1 = newTouch2.position;
                return;
            }
            else if (newTouch1.phase == UnityEngine.TouchPhase.Moved ||
                     newTouch2.phase == UnityEngine.TouchPhase.Moved)
            {
                var tempPos0 = newTouch1.position;
                var tempPos1 = newTouch2.position;
                var currDist = Vector2.Distance(tempPos0, tempPos1);
                var lastDist = Vector2.Distance(lastTouchPos0, lastTouchPos1);
                var delta = currDist - lastDist;

                SetZoom(-delta);

                lastTouchPos0 = tempPos0;
                lastTouchPos1 = tempPos1;
            }
#endif
        }

        #region 设置摄像头远近
        /// <summary>
        /// 设置摄像头远近
        /// </summary>
        public void SetZoom()
        {
#if USE_INPUT_SYSTEM
            float delta = mouse.scroll.ReadValue().y * -zoomValue;
#else
            float delta = Input.mouseScrollDelta.y * -zoomValue;
#endif
            SetZoom(delta);
        }

        /// <summary>
        /// 设置摄像头远近
        /// </summary>
        public void SetZoom(float delta)
        {
#if USE_INPUT_SYSTEM
            distance += delta;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);
#else
            distance += delta;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);
#endif
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