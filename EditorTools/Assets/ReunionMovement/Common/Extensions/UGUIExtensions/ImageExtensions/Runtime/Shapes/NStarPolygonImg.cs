using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic.UI.ImageExtensions
{
    /// <summary>
    /// N角星形
    /// </summary>
    [Serializable]
    public class NStarPolygonImg : UIImgComponent
    {
        [SerializeField] private float sideCount;
        [SerializeField] private float inset;
        [SerializeField] private float cornerRadius;
        [SerializeField] private Vector2 offset;

        private static readonly int nStarPolygonSideCount_Sp = Shader.PropertyToID("_NStarPolygonSideCount");
        private static readonly int nStarPolygonInset_Sp = Shader.PropertyToID("_NStarPolygonInset");
        private static readonly int nStarPolygonCornerRadius_Sp = Shader.PropertyToID("_NStarPolygonCornerRadius");
        private static readonly int nStarPolygonOffset_Sp = Shader.PropertyToID("_NStarPolygonOffset");

        public Material sharedMat { get; set; }
        public bool shouldModifySharedMat { get; set; }
        public RectTransform rectTransform { get; set; }

        public event EventHandler onComponentSettingsChanged;

        /// <summary>
        /// 这个形状应该有多少边。这些边大小相等。
        /// 3条边形成等边三角形，6条边形成六边形，以此类推
        /// SideCount的值应保持在3到10之间。
        /// </summary>
        public float SideCount
        {
            get => sideCount;
            set
            {
                sideCount = Mathf.Clamp(value, 3f, 10f);
                if (shouldModifySharedMat)
                {
                    sharedMat.SetFloat(nStarPolygonSideCount_Sp, sideCount);
                }
                onComponentSettingsChanged?.Invoke(this, EventArgs.Empty);
                Inset = inset;
            }
        }

        /// <summary>
        /// “插入”是一个值，用于确定边在形状内部的长度并形成凹形星形。
        /// 每一边将分成两半和中间点将指向形状的中心
        /// 插入的值应保持在2和（SideCount-0.01）之间。2为默认值并且意味着侧面不会断裂
        /// </summary>
        public float Inset
        {
            get => inset;
            set
            {
                inset = Mathf.Clamp(value, 2f, SideCount - 0.01f);
                if (shouldModifySharedMat)
                {
                    sharedMat.SetFloat(nStarPolygonInset_Sp, inset);
                }
                onComponentSettingsChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// 形状的所有角的角半径
        /// </summary>
        public float CornerRadius
        {
            get => cornerRadius;
            set
            {
                cornerRadius = Mathf.Max(value, 0);
                if (shouldModifySharedMat)
                {
                    sharedMat.SetFloat(nStarPolygonCornerRadius_Sp, cornerRadius);
                }
                onComponentSettingsChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// 形状相对于原点的位置偏移
        /// </summary>
        public Vector2 Offset
        {
            get => offset;
            set
            {
                offset = value;
                if (shouldModifySharedMat)
                {
                    sharedMat.SetVector(nStarPolygonOffset_Sp, offset);
                }
                onComponentSettingsChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="sharedMat"></param>
        /// <param name="renderMat"></param>
        /// <param name="rectTransform"></param>
        public void Init(Material sharedMat, Material renderMat, RectTransform rectTransform)
        {
            this.sharedMat = sharedMat;
            shouldModifySharedMat = sharedMat == renderMat;
            this.rectTransform = rectTransform;

            OnValidate();
        }

        public void OnValidate()
        {
            SideCount = sideCount;
            Inset = inset;
            CornerRadius = cornerRadius;
            Offset = offset;
        }

        /// <summary>
        /// 材质的初始化值
        /// </summary>
        /// <param name="material"></param>
        public void InitValuesFromMaterial(ref Material material)
        {
            sideCount = material.GetFloat(nStarPolygonSideCount_Sp);
            inset = material.GetFloat(nStarPolygonInset_Sp);
            cornerRadius = material.GetFloat(nStarPolygonCornerRadius_Sp);
            offset = material.GetVector(nStarPolygonOffset_Sp);
        }

        /// <summary>
        /// 修改材质
        /// </summary>
        /// <param name="material"></param>
        /// <param name="otherProperties"></param>
        public void ModifyMaterial(ref Material material, params object[] otherProperties)
        {
            material.SetFloat(nStarPolygonSideCount_Sp, sideCount);
            material.SetFloat(nStarPolygonInset_Sp, inset);
            material.SetFloat(nStarPolygonCornerRadius_Sp, cornerRadius);
            material.SetVector(nStarPolygonOffset_Sp, offset);
        }
    }
}