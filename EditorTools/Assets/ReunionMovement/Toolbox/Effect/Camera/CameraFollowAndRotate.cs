using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameLogic
{
    /// <summary>
    /// 摄像机围绕物体旋转
    /// </summary>
    public class CameraFollowAndRotate : MonoBehaviour
    {
        //目标对象
        public Transform targetPos;
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
        public float defaualtDistance = 30f;                //默认距离

        Quaternion destRot = Quaternion.identity;
        private Vector3 relativePosition = Vector3.zero;    //相对位置

        private Vector2 lastTouchPos0;
        private Vector2 lastTouchPos1;

        public bool isEnable = true;

        public void Start()
        {
            OrbitCamera(0, -0);
            UpdatePosition();

            if (hideCursor)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        public void SetIsEnableTrue()
        {
            isEnable = true;
        }

        public void Update()
        {
            if (isEnable == false)
            {

            }
            else
            {
                if (Application.platform == RuntimePlatform.WindowsPlayer ||
                    Application.platform == RuntimePlatform.WindowsEditor ||
                    Application.platform == RuntimePlatform.WebGLPlayer)
                {
                    if (IsPointerOverUI())
                    {
                        return;
                    }

                    //鼠标左键拖拽
                    if (Input.GetMouseButton(0))
                    {
                        float horz = Input.GetAxis("Mouse X");
                        float vert = Input.GetAxis("Mouse Y");
                        OrbitCamera(horz, -vert);
                    }

                    //滚轮
                    SetZoom();
                }

                if (Application.platform == RuntimePlatform.Android ||
                   Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    //没有触摸
                    if (Input.touchCount <= 0)
                    {
                        return;
                    }

                    //触摸为1 开始触摸
                    if (1 == Input.touchCount && Input.GetTouch(0).phase == TouchPhase.Began)
                    {

                    }
                    //触摸为1 滑动
                    else if (1 == Input.touchCount)
                    {
                        if (IsPointerOverUI())
                        {
                            return;
                        }

                        Touch touch = Input.GetTouch(0);

                        float horz = touch.deltaPosition.x;
                        float vert = touch.deltaPosition.y;

                        OrbitCamera(horz, -vert);
                    }
                    else if (2 == Input.touchCount)
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
            offsetDistance = Mathf.MoveTowards(offsetDistance, defaualtDistance, Time.deltaTime * zoomSpeed);
            relativePosition = (targetPos.position + (Vector3.up * offsetHeight)) + (transform.rotation * (Vector3.forward * -offsetDistance)) + (transform.right * lateralOffset);
            transform.position = relativePosition;
        }

        /// <summary>
        /// 滚轮
        /// </summary>
        public void SetZoom()
        {
            float delta = Input.GetAxis("Mouse ScrollWheel") * -zoomValue;
            defaualtDistance += delta;
            defaualtDistance = Mathf.Clamp(defaualtDistance, minDistance, maxDistance);
            UpdatePosition();
        }

        private bool IsPointerOverUI()
        {
#if UNITY_EDITOR
            if (EventSystem.current.IsPointerOverGameObject())
#elif UNITY_ANDROID || UNITY_IPHONE
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
#else
            if (EventSystem.current.IsPointerOverGameObject())
#endif
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 多点触摸
        /// </summary>
        private void HandleMultiTouch()
        {
            var touch0 = Input.GetTouch(0);
            var touch1 = Input.GetTouch(1);

            if (touch1.phase == TouchPhase.Began)
            {
                lastTouchPos0 = touch0.position;
                lastTouchPos1 = touch1.position;
            }
            else if (touch0.phase == TouchPhase.Moved || touch1.phase == TouchPhase.Moved)
            {
                var tempPos0 = touch0.position;
                var tempPos1 = touch1.position;
                var currDist = Vector2.Distance(tempPos0, tempPos1);
                var lastDist = Vector2.Distance(lastTouchPos0, lastTouchPos1);
                var delta = currDist - lastDist;

                SetZoom(-delta);

                lastTouchPos0 = tempPos0;
                lastTouchPos1 = tempPos1;
            }
        }

        public void SetZoom(float delta)
        {
            defaualtDistance += delta;
            defaualtDistance = Mathf.Clamp(defaualtDistance, minDistance, maxDistance);
            UpdatePosition();
        }
    }
}