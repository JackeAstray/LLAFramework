using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameLogic
{
    /// <summary>
    /// 摄影机视野跟随
    /// </summary>
    public class CameraViewFollowing : MonoBehaviour
    {
        //摄影机碰撞
        [SerializeField] private bool cameraCollisions;
        //选定Gizmo
        [SerializeField] private bool gizmosOnSelected;
        //加载时锁定光标
        [SerializeField] private bool lockCursorOnLoad;
        //跟随
        [SerializeField] private Transform follow;
        //运行轨道
        [SerializeField] private CameraOrbits cameraOrbit;
        //相机目标位置
        private Vector3 freelookCameraTargetPosition;
        //相机目标位置
        private Vector3 freelookCameraPosition;
        //检测到碰撞
        private bool collisionDetected;
        //x灵敏度
        [SerializeField, Range(0, 100)] private int xSensitivity = 80;
        //y灵敏度
        [SerializeField, Range(0, 100)] private int ySensitivity = 80;
        //旋转偏移
        [SerializeField, Range(-180, 180)] private float rotationOffset = 0f;
        //碰撞偏移
        [SerializeField, Range(0, 1)] private float collisionOffset = 0.2f;
        //x旋转
        private float xRotation;
        //y旋转
        private float yRotation;
        //X夹具上
        private float xClampTop;
        //X夹具下
        private float xClampBottom;

        private Vector2 lastTouchPos0;
        private Vector2 lastTouchPos1;

        public float zoomValue = 50f;

        void Start()
        {
            if (lockCursorOnLoad)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        void Update()
        {
            GetViewInput();
            UpdateFollowPosition();

            if (follow != null)
            {
                transform.position = follow.position + new Vector3(0, cameraOrbit.rigOffset, 0);
            }
        }

        #region Debug [Editor]
#if UNITY_EDITOR

        private void Reset()
        {
            StartCoroutine(PauseForSeconds(0.1f));
        }

        /// <summary>
        /// 暂停几秒钟
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private IEnumerator PauseForSeconds(float value)
        {
            yield return new WaitForSeconds(value);

            SetCameraOrbit(0, 3, -1, 2, 4, 3);
            CameraProperty newCameraRig = GetComponentInChildren<CameraProperty>();
            GameObject newFreelookRig;

            if (newCameraRig == null)
            {
                newFreelookRig = new GameObject("CameraRig (Freelook)");
                newFreelookRig.transform.parent = gameObject.transform;
                newFreelookRig.transform.localPosition = Vector3.zero;
                CameraProperty cameraRig = newFreelookRig.AddComponent<CameraProperty>();
                cameraRig.SetFreelookRig(this);
            }
            else
            {
                newCameraRig.SetFreelookRig(this);
            }
        }

        private void OnDrawGizmos()
        {
            UpdateFollowPosition();

            if (follow != null)
            {
                transform.position = follow.position + new Vector3(0, cameraOrbit.rigOffset, 0);
            }

            if (!gizmosOnSelected)
            {
                DrawCameraGizmos();
            }

            UpdateEditorValidation();
        }

        private void OnDrawGizmosSelected()
        {
            if (gizmosOnSelected)
            {
                DrawCameraGizmos();
            }
        }

        /// <summary>
        /// 更新编译器
        /// </summary>
        private void UpdateEditorValidation()
        {
            if (cameraOrbit.height.up < 0)
            {
                cameraOrbit.height.up = 0;
            }

            if (0 < cameraOrbit.height.down)
            {
                cameraOrbit.height.down = 0;
            }

            if (cameraOrbit.radius.top < 0.1)
            {
                cameraOrbit.radius.top = 0.1f;
            }

            if (cameraOrbit.radius.middle < 0.1)
            {
                cameraOrbit.radius.middle = 0.1f;
            }

            if (cameraOrbit.radius.bottom < 0.1)
            {
                cameraOrbit.radius.bottom = 0.1f;
            }
        }

        private void DrawCameraGizmos()
        {
            // set gizmo matrix to local transform
            Matrix4x4 resetMatrix = Gizmos.matrix;
            Gizmos.matrix = gameObject.transform.localToWorldMatrix;

            // draw camera orbits
            UnityEditor.Handles.color = Color.cyan;
            UnityEditor.Handles.DrawWireDisc(new Vector3(transform.position.x, transform.position.y + cameraOrbit.height.up, transform.position.z), transform.up, cameraOrbit.radius.top);
            UnityEditor.Handles.DrawWireDisc(new Vector3(transform.position.x, transform.position.y, transform.position.z), transform.up, cameraOrbit.radius.middle);
            UnityEditor.Handles.DrawWireDisc(new Vector3(transform.position.x, transform.position.y + cameraOrbit.height.down, transform.position.z), transform.up, cameraOrbit.radius.bottom);

            // draw freelook current orbit
            if (collisionDetected) { UnityEditor.Handles.color = Color.red; }
            else { UnityEditor.Handles.color = Color.yellow; }
            float currentRadius = Vector3.Magnitude(new Vector2(freelookCameraTargetPosition.x, freelookCameraTargetPosition.z));
            UnityEditor.Handles.DrawWireDisc(new Vector3(transform.position.x, transform.position.y + freelookCameraTargetPosition.y, transform.position.z), transform.up, currentRadius);

            // draw line between orbits
            Gizmos.color = Color.cyan;
            Vector3 top = Quaternion.AngleAxis(-rotationOffset, Vector3.up) * new Vector3(cameraOrbit.radius.top, cameraOrbit.height.up, 0);
            Vector3 middle = Quaternion.AngleAxis(-rotationOffset, Vector3.up) * new Vector3(cameraOrbit.radius.middle, 0, 0);
            Vector3 bottom = Quaternion.AngleAxis(-rotationOffset, Vector3.up) * new Vector3(cameraOrbit.radius.bottom, cameraOrbit.height.down, 0);
            Gizmos.DrawLine(top, middle);
            Gizmos.DrawLine(middle, bottom);

            // draw line for rotation directions
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(Vector3.zero, freelookCameraPosition);

            if (collisionDetected) { UnityEditor.Handles.color = Color.red; }
            else { UnityEditor.Handles.color = Color.yellow; }
            Vector3 lineOut = new Vector3(freelookCameraTargetPosition.x, 0, freelookCameraTargetPosition.z);
            Gizmos.DrawLine(Vector3.zero, lineOut);
            Gizmos.DrawLine(lineOut, freelookCameraTargetPosition);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(freelookCameraPosition, freelookCameraTargetPosition);

            // reset gizmo matrix to public position
            Gizmos.matrix = resetMatrix;
        }
#endif
        #endregion

        /// <summary>
        /// 获取视图输入
        /// </summary>
        private void GetViewInput()
        {
            float mouseX = 0;
            float mouseY = 0;

            if (Application.platform == RuntimePlatform.WindowsPlayer ||
                Application.platform == RuntimePlatform.WindowsEditor ||
                Application.platform == RuntimePlatform.WebGLPlayer)
            {
                if (IsPointerOverUI())
                {
                    return;
                }

                float rotationXInput = Input.GetAxis("Mouse X");
                float rotationYInput = Input.GetAxis("Mouse Y");

                mouseX = rotationXInput * Time.deltaTime * xSensitivity * 2;
                mouseY = rotationYInput * Time.deltaTime * ySensitivity * 2;

                yRotation += mouseX;
                xRotation -= mouseY;

                xRotation = Mathf.Clamp(xRotation, xClampBottom, xClampTop);

                SetZoom();
            }

            if (Application.platform == RuntimePlatform.Android ||
                Application.platform == RuntimePlatform.IPhonePlayer)
            {
                if (IsPointerOverUI())
                {
                    return;
                }

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

                    float rotationXInput = touch.deltaPosition.x;
                    float rotationYInput = touch.deltaPosition.y;

                    mouseX = rotationXInput * Time.deltaTime * xSensitivity * 2;
                    mouseY = rotationYInput * Time.deltaTime * ySensitivity * 2;

                    yRotation += mouseX;
                    xRotation -= mouseY;

                    xRotation = Mathf.Clamp(xRotation, xClampBottom, xClampTop);
                }
                else if (2 == Input.touchCount)
                {
                    HandleMultiTouch();
                }
            }
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

        /// <summary>
        /// 设置摄像头远近
        /// </summary>
        public void SetZoom()
        {
            float delta = Input.GetAxis("Mouse ScrollWheel") * -zoomValue;
            CameraOrbits cameraOrbits = GetCameraOrbit();
            cameraOrbits.height.up = Mathf.Clamp(cameraOrbits.height.up + delta, 3f, 8f);
            cameraOrbits.height.down = Mathf.Clamp(cameraOrbits.height.down + delta, -3f, -8f);
            cameraOrbits.radius.middle = Mathf.Clamp(cameraOrbits.radius.middle + delta, 3f, 8f);
        }

        /// <summary>
        /// 设置摄像头远近
        /// </summary>
        public void SetZoom(float delta)
        {
            CameraOrbits cameraOrbits = GetCameraOrbit();
            cameraOrbits.height.up = Mathf.Clamp(cameraOrbits.height.up + delta, 3f, 8f);
            cameraOrbits.height.down = Mathf.Clamp(cameraOrbits.height.down + delta, -3f, -8f);
            cameraOrbits.radius.middle = Mathf.Clamp(cameraOrbits.radius.middle + delta, 3f, 8f);
        }

        /// <summary>
        /// 更新跟随位置
        /// </summary>
        private void UpdateFollowPosition()
        {
            UpdateTargetPosition();
            UpdateCameraCollisions();
        }

        /// <summary>
        /// 更新目标位置
        /// </summary>
        private void UpdateTargetPosition()
        {
            float angleTop = GetAngleOnRightAngleTriangle(Mathf.Abs(cameraOrbit.radius.top), Mathf.Abs(cameraOrbit.height.up));
            float angleBottom = GetAngleOnRightAngleTriangle(Mathf.Abs(cameraOrbit.radius.bottom), Mathf.Abs(cameraOrbit.height.down));

            xClampTop = (90.0f - angleTop) * Mathf.Sign(cameraOrbit.height.up);
            xClampBottom = (90.0f - angleBottom) * Mathf.Sign(cameraOrbit.height.down);

            float magnitudeTop = Vector3.Magnitude(new Vector2(cameraOrbit.radius.top, cameraOrbit.height.up));
            float magnitudeBottom = Vector3.Magnitude(new Vector2(cameraOrbit.radius.bottom, cameraOrbit.height.down));

            float distance; ;
            if (0 < xRotation)
            {
                float ratio = xRotation / xClampTop;
                distance = (magnitudeTop * ratio) + (cameraOrbit.radius.middle * (1.0f - ratio));
            }
            else if (0 > xRotation)
            {
                float ratio = xRotation / xClampBottom;
                distance = (cameraOrbit.radius.middle * (1.0f - ratio)) + (magnitudeBottom * ratio);
            }
            else
            {
                distance = cameraOrbit.radius.middle;
            }

            float longitude = (rotationOffset - yRotation) * Mathf.Deg2Rad;
            float latitude = xRotation * Mathf.Deg2Rad;
            float equatorX = Mathf.Cos(longitude);
            float equatorZ = Mathf.Sin(longitude);
            float multiplier = Mathf.Cos(latitude);
            float x = multiplier * equatorX;
            float y = Mathf.Sin(latitude);
            float z = multiplier * equatorZ;

            freelookCameraTargetPosition = new Vector3(x, y, z) * distance;
        }

        /// <summary>
        /// 更新摄影机碰撞
        /// </summary>
        private void UpdateCameraCollisions()
        {
            if (cameraCollisions)
            {
                float maxDistance = freelookCameraTargetPosition.magnitude;
                Vector3 direction = freelookCameraTargetPosition.normalized;

                RaycastHit hit;
                Ray ray = new Ray(transform.position, direction);

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.distance < maxDistance)
                    {
                        freelookCameraPosition = direction * (hit.distance - collisionOffset);
                        collisionDetected = true;
                    }
                    else
                    {
                        freelookCameraPosition = freelookCameraTargetPosition;
                        collisionDetected = false;
                    }
                }
                else
                {
                    freelookCameraPosition = freelookCameraTargetPosition;
                    collisionDetected = false;
                }
            }
            else
            {
                freelookCameraPosition = freelookCameraTargetPosition;
                collisionDetected = false;
            }
        }

        /// <summary>
        /// 获取直角三角形上的角度
        /// </summary>
        /// <param name="Opposite"></param>
        /// <param name="Adjacent"></param>
        /// <returns></returns>
        private float GetAngleOnRightAngleTriangle(float Opposite, float Adjacent)
        {
            float angle = Mathf.Atan2(Opposite, Adjacent);
            angle = angle * Mathf.Rad2Deg;
            return angle;
        }

        /// <summary>
        /// 判断是否在UI上
        /// </summary>
        /// <returns></returns>
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
        /// 获取相机位置
        /// </summary>
        /// <returns></returns>
        public Vector3 GetCameraPosition() { return transform.position + freelookCameraPosition; }
        public Transform GetFreelookRigFollow() { return follow; }
        public void SetFreelookRigFollow(Transform newFollow) { follow = newFollow; }
        public CameraOrbits GetCameraOrbit() { return cameraOrbit; }
        public void SetCameraOrbit(float rigOffset, float heightUp, float heightDown, float radiusTop, float radiusMiddle, float radiusBottom)
        {
            cameraOrbit.rigOffset = rigOffset;
            cameraOrbit.height.up = heightUp;
            cameraOrbit.height.down = heightDown;
            cameraOrbit.radius.top = radiusTop;
            cameraOrbit.radius.middle = radiusMiddle;
            cameraOrbit.radius.bottom = radiusBottom;
        }
        public void SetSensitivity(Vector2 newSensitivity) { xSensitivity = (int)newSensitivity.x; ySensitivity = (int)newSensitivity.y; }
    }
}