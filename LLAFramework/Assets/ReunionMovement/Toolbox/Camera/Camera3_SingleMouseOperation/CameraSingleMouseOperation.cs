using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace GameLogic
{
    /// <summary>
    /// 相机单鼠标操作
    /// </summary>
    public class CameraSingleMouseOperation : MonoBehaviour
    {
        // 目标对象
        public Transform targetPos;

        #region 摄像机移动
        // 摄像机
        public Camera csmoCamera { get; private set; }
        // 移动速度
        public float csmoCameraSpeed = 50;
        // 如果不为空，摄像机将限制在该盒子碰撞器内
        public BoxCollider restrictedZone;
        // 鼠标
        private Mouse mouse;
        // 射线管理器
        private RaycastBase raycastBase;
        // 是否检查鼠标是否在UI上
        public bool checkPointerOverUI = true;

        public LayerMask layerMask;
        #endregion

        [Space(10)]

        #region 摄像机旋转/远近
        // 初始角度
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
        #endregion

        [Space(10)]

        #region 旋转控制
        // 旋转控制变量
        public bool isRotating = false;
        public RotationDirection rotationDirection = RotationDirection.None;
        public float autoRotateSpeed = 15f;                 //自动转速
        public enum RotationDirection
        {
            None,
            Left,
            Right
        }
        #endregion

        void Start()
        {
            mouse = Mouse.current;

            if (csmoCamera == null)
            {
                csmoCamera = transform.Find("Camera").GetComponent<Camera>();
            }

            raycastBase = new RaycastBase(layerMask, csmoCamera);
        }

        void Update()
        {
            if (mouse != null && checkPointerOverUI && IsPointerOverUI())
            {
                return;
            }

            HandleCameraMovement();
            HandleCameraRotation();
            HandleCameraZoom();
            HandleAutoRotation();
            HandleMouseClick();
            UpdatePosition();
        }

        /// <summary>
        /// 处理摄像机移动
        /// </summary>
        private void HandleCameraMovement()
        {
            if (mouse.rightButton.isPressed)
            {
                float horz = mouse.delta.x.ReadValue();
                float vert = mouse.delta.y.ReadValue();
                Vector3 moveDirection = (csmoCamera.transform.right * -horz) + (csmoCamera.transform.up * -vert);
                moveDirection *= (csmoCameraSpeed * 0.001f);
                targetPos.position += moveDirection;

                if (restrictedZone != null)
                {
                    Vector3 newPosition = targetPos.position;
                    if (restrictedZone.bounds.Contains(newPosition))
                    {
                        targetPos.position = newPosition;
                    }
                    else
                    {
                        targetPos.position = restrictedZone.bounds.ClosestPoint(newPosition);
                    }
                }
            }
        }

        /// <summary>
        /// 处理摄像机旋转
        /// </summary>
        private void HandleCameraRotation()
        {
            if (mouse.leftButton.isPressed)
            {
                float horz = mouse.delta.x.ReadValue();
                float vert = mouse.delta.y.ReadValue();
                OrbitCamera(horz, -vert);
            }
        }

        /// <summary>
        /// 处理自动旋转
        /// </summary>
        private void HandleAutoRotation()
        {
            if (isRotating && !mouse.leftButton.isPressed && !mouse.rightButton.isPressed)
            {
                float rotationStep = autoRotateSpeed * Time.deltaTime;
                if (rotationDirection == RotationDirection.Left)
                {
                    OrbitCamera(rotationStep, 0);
                }
                else if (rotationDirection == RotationDirection.Right)
                {
                    OrbitCamera(-rotationStep, 0);
                }
            }
        }

        /// <summary>
        /// 处理摄像机缩放
        /// </summary>
        private void HandleCameraZoom()
        {
            float value = mouse.scroll.ReadValue().y;
            float delta = value > 0 ? 1 : (value < 0 ? -1 : 0);
            SetZoom(delta * -zoomValue);
        }

        /// <summary>
        /// 处理鼠标点击
        /// </summary>
        private void HandleMouseClick()
        {
            if (mouse.leftButton.wasPressedThisFrame)
            {
                Vector2 mousePosition = mouse.position.ReadValue();
                if (raycastBase.CastRayFromScreenPoint(mousePosition, out RaycastHit hitInfo))
                {
                    Debug.Log("Hit: " + hitInfo.collider.name);
                }
            }
        }

        /// <summary>
        /// 设置目标点
        /// </summary>
        /// <param name="target"></param>
        public void SetTargetPos(Transform target)
        {
            targetPos = target;
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
            csmoCamera.transform.rotation = destRot;
            UpdatePosition();
        }

        /// <summary>
        /// 更新摄像机位置
        /// </summary>
        public void UpdatePosition()
        {
            offsetDistance = Mathf.MoveTowards(offsetDistance, distance, Time.deltaTime * zoomSpeed);

            Vector3 target = targetPos != null ? targetPos.position : Vector3.zero;
            relativePosition = (target + (Vector3.up * offsetHeight)) +
                               (csmoCamera.transform.rotation * (Vector3.forward * -offsetDistance)) +
                               (csmoCamera.transform.right * lateralOffset);

            csmoCamera.transform.position = relativePosition;
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

        /// <summary>
        /// 设置摄像头远近
        /// </summary>
        public void SetZoom(float delta)
        {
            distance += delta;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);
            UpdatePosition();
        }
    }
}