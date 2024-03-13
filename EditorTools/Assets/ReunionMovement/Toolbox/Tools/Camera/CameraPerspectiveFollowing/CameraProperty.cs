using GameLogic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 摄像机属性
    /// </summary>
    public class CameraProperty : MonoBehaviour
    {
        //装备
        [SerializeField] private CameraViewFollowing CameraViewFollowing;
        //跟随
        [SerializeField] private Transform follow;
        //锁视角
        [SerializeField] private Transform lookAt;
        //转弯速度
        [SerializeField, Range(0, 1)] private float turnSpeed = 1.0f;
        //跟随偏移
        [SerializeField] private Vector3 followOffset;
        //镜片
        [SerializeField] private CameraLens lens = new CameraLens();
        //lerp倍增器
        private float lerpSpeedMultiplier = 40f;
        //所需的摄像头倾斜
        private float desiredCameraTilt;
        //旋转目标
        private Quaternion rotationTarget;
        //距离
        private Vector3 direction;

        public void Update()
        {
            UpdateCamera();
        }

        #region Editor
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            UpdateCamera();
        }

        private void OnDrawGizmosSelected()
        {
            // draw camera frustrum
            if (lens.showFrustrum)
            {
                Matrix4x4 resetMatrix = Gizmos.matrix;
                Gizmos.matrix = gameObject.transform.localToWorldMatrix;
                Gizmos.color = Color.grey;

                float width = Screen.width * 1.000f;
                float height = Screen.height * 1.000f;
                float aspect = width / height;

                Gizmos.DrawFrustum(Vector3.zero, lens.verticalFOV, lens.farClipPlane, lens.nearClipPlane, aspect);
                Gizmos.matrix = resetMatrix;
            }
        }
#endif
        #endregion

        /// <summary>
        /// 更新摄像机
        /// </summary>
        private void UpdateCamera()
        {
            // update position
            if (CameraViewFollowing != null)
            {
                transform.position = CameraViewFollowing.GetCameraPosition();
            }
            else if (follow != null)
            {
                transform.position = follow.position + followOffset;
            }

            // update rotation
            bool updateRotation = false;
            if (CameraViewFollowing != null)
            {
                if (lookAt != null)
                {
                    direction = (lookAt.position - transform.position).normalized;
                }
                else
                {
                    direction = (CameraViewFollowing.transform.position - transform.position).normalized;
                }
                updateRotation = true;
            }
            else if (lookAt != null)
            {
                direction = (lookAt.position - transform.position).normalized;
                updateRotation = true;
            }
            if (updateRotation)
            {
                rotationTarget = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotationTarget, turnSpeed);
            }

            // update tilt
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, lens.tilt);
        }

        private IEnumerator SmoothlyLerpCameraTilt()
        {
            // smoothly lerp moveSpeed to moveState speed
            float time = 0;
            float difference = Mathf.Abs(desiredCameraTilt - lens.tilt);
            float startValue = lens.tilt;

            while (time < difference)
            {
                lens.tilt = Mathf.Lerp(startValue, desiredCameraTilt, time / difference);
                time += Time.deltaTime * lerpSpeedMultiplier;
                yield return null;
            }

            lens.tilt = desiredCameraTilt;
        }

        public void SetCameraTilt(float newCameraTilt)
        {
            StopAllCoroutines();
            desiredCameraTilt = newCameraTilt;
            StartCoroutine(SmoothlyLerpCameraTilt());
        }

        public CameraViewFollowing GetFreelookRig() { return CameraViewFollowing; }

        public void SetFreelookRig(CameraViewFollowing newFreelookRig) { CameraViewFollowing = newFreelookRig; }

        public Transform GetCameraFollow() { return follow; }

        public void SetCameraFollow(Transform newFollow) { follow = newFollow; }

        public Transform GetCameraLookAt() { return lookAt; }

        public void SetCameraLookAt(Transform newLookAt) { lookAt = newLookAt; }

        public CameraLens GetCameraLens() { return lens; }

        public void SetCameraLens(CameraLens newLens) { lens = newLens; }
    }
}