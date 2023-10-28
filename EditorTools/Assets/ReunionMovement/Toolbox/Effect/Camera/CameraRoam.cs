using Unity.VisualScripting;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 摄像机漫游
    /// </summary>
    public class CameraRoam : MonoBehaviour
    {
        //隐藏光标
        public bool hideCursor = true;

        public float normalSpeed = 25.0f;
        public float shiftSpeed = 54.0f;
        public float speedCap = 54.0f;
        public float cameraSensitivity = 0.6f;
        private float totalSpeed = 1.0f;
        //旋转速度
        public float rotationSpeed = 5f;
        //上下旋转角度限制
        public float maxRot = 90f;                          //最大旋转角度
        public float minRot = -90f;                         //最小旋转角度
        //旋转缓冲速度
        public float lerpSpeed = 10f;
        private float targetRotationX = 0f;
        private float targetRotationY = 0f;

        public void Start()
        {
            if (hideCursor)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        private void Update()
        {
            UpdateCameraRotation();

            Vector3 cameraVelocity = GetBaseInput();

            if (Input.GetKey(KeyCode.LeftShift))
            {
                HandleShiftMovement(ref cameraVelocity);
            }
            else
            {
                totalSpeed = Mathf.Clamp(totalSpeed * 0.5f, 1f, 1000f);
                cameraVelocity *= normalSpeed;
            }

            cameraVelocity *= Time.deltaTime;
            UpdateCameraPosition(cameraVelocity);
        }

        private void UpdateCameraRotation()
        {
            // 获取鼠标输入的旋转增量
            float rotationXInput = -Input.GetAxis("Mouse Y");
            float rotationYInput = Input.GetAxis("Mouse X");
            // 根据旋转速度进行摄像机的旋转
            targetRotationX += rotationXInput * rotationSpeed;
            targetRotationY += rotationYInput * rotationSpeed;
            // 对上下旋转角度进行限制
            targetRotationX = Mathf.Clamp(targetRotationX, minRot, maxRot);
            // 根据旋转角度更新摄像机的欧拉角，Quaternion.Lerp可以使摄像机旋转更加平滑
            Quaternion targetRotation = Quaternion.Euler(targetRotationX, targetRotationY, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, lerpSpeed * Time.deltaTime);
        }

        private void HandleShiftMovement(ref Vector3 cameraVelocity)
        {
            totalSpeed += Time.deltaTime;
            cameraVelocity = cameraVelocity * totalSpeed * shiftSpeed;
            cameraVelocity.x = Mathf.Clamp(cameraVelocity.x, -speedCap, speedCap);
            cameraVelocity.y = Mathf.Clamp(cameraVelocity.y, -speedCap, speedCap);
            cameraVelocity.z = Mathf.Clamp(cameraVelocity.z, -speedCap, speedCap);
        }

        private Vector3 GetBaseInput()
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            return new Vector3(horizontalInput, 0, verticalInput);
        }

        private void UpdateCameraPosition(Vector3 cameraVelocity)
        {
            transform.Translate(cameraVelocity);
        }
    }
}