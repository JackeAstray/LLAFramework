using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace GameLogic
{
    public class CameraSingleMouseOperation : MonoBehaviour
    {
        //目标对象
        public Transform targetPos;

        #region 摄像机移动
        //摄像机
        public Camera csmoCamera { get; private set; }
        //移动速度
        public float csmoCameraSpeed = 50;

        public BoxCollider restrictedZone;
        #endregion

        #region 摄像机旋转/远近
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
        #endregion

        Mouse mouse;

        void Start()
        {
            mouse = Mouse.current;

            if (csmoCamera == null)
            {
                csmoCamera = transform.Find("Camera").GetComponent<Camera>();
            }
        }

        void Update()
        {
            if (mouse != null)
            {
                if (IsPointerOverUI())
                {
                    return;
                }

                //控制摄像机父对象移动
                if (mouse.rightButton.isPressed)
                {
                    float horz = mouse.delta.x.ReadValue();
                    float vert = mouse.delta.y.ReadValue();
                    Vector3 right = csmoCamera.transform.right;
                    Vector3 up = csmoCamera.transform.up;
                    Vector3 moveDirection = (right * -horz) + (up * -vert);
                    moveDirection *= (csmoCameraSpeed * 0.001f);
                    targetPos.position += moveDirection;

                    if (restrictedZone != null)
                    {
                        Vector3 newPosition = targetPos.position;
                        // 检查新的位置是否在盒子碰撞器的边界内
                        if (restrictedZone.bounds.Contains(newPosition))
                        {
                            // 如果在边界内，更新位置
                            targetPos.position = newPosition;
                        }
                        else
                        {
                            // 如果在边界外，将位置调整到边界上
                            targetPos.position = restrictedZone.bounds.ClosestPoint(newPosition);
                        }
                    }
                }

                //鼠标左键拖拽
                if (mouse.leftButton.isPressed)
                {
                    float horz = mouse.delta.x.ReadValue();
                    float vert = mouse.delta.y.ReadValue();
                    OrbitCamera(horz, -vert);
                }

                //滚轮
                SetZoom();

                UpdatePosition();
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

        public void SetZoom()
        {
            float value = mouse.scroll.ReadValue().y;
            if (value > 0)
            {
                value = 1;
            }
            else if (value < 0)
            {
                value = -1;
            }
            else
            {
                value = 0;
            }

            float delta = value * -zoomValue;

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
    }
}