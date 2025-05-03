using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LLAFramework.Billboard;

namespace LLAFramework
{
    /// <summary>
    /// 广告牌
    /// </summary>
    [ExecuteAlways]
    public class Billboard : MonoBehaviour
    {
        public enum BillboardType
        {
            Mode1, //和摄像机保持一个方向和角度
            Mode2, //Z轴朝向摄像机，但角度一直为0
            Mode3, //Z轴朝向摄像机
        }

        public Transform targetTF;
        public BillboardType billboardType = BillboardType.Mode1;
        Quaternion originalRotation;

        void Start()
        {
            if (targetTF == null)
            {
                targetTF = Camera.main.transform;
            }
            originalRotation = transform.rotation;
        }

        void Update()
        {
            if(targetTF == null)
            {
                Log.Error("目标不存在，请查找原因！");
                return;
            }

            switch (billboardType)
            {
                case BillboardType.Mode1:
                    transform.rotation = targetTF.rotation * originalRotation;
                    break;
                case BillboardType.Mode2:
                    Vector3 v = targetTF.position - transform.position;
                    v.x = v.z = 0.0f;
                    transform.LookAt(targetTF.position - v);
                    break;
                case BillboardType.Mode3:
                    transform.LookAt(targetTF.position);
                    break;
            }
        }
    }
}