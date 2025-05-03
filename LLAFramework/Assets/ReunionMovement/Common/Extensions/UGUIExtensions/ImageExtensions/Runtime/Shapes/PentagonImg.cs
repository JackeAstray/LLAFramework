using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LLAFramework.UI.ImageExtensions
{
    /// <summary>
    /// 五角形
    /// </summary>
    [Serializable]
    public class PentagonImg : UIImgComponent
    {
        [SerializeField] private Vector4 cornerRadius;
        [SerializeField] private bool uniformCornerRadius;
        [SerializeField] private float tipRadius;
        [SerializeField] private float tipSize;

        private static readonly int pentagonRectCornerRadius_Sp = Shader.PropertyToID("_PentagonCornerRadius");
        private static readonly int pentagonTriangleCornerRadius_Sp = Shader.PropertyToID("_PentagonTipRadius");
        private static readonly int pentagonTriangleSize_Sp = Shader.PropertyToID("_PentagonTipSize");

        public Material sharedMat { get; set; }
        public bool shouldModifySharedMat { get; set; }
        public RectTransform rectTransform { get; set; }

        public event EventHandler onComponentSettingsChanged;

        /// <summary>
        /// 矩形零件四个角的半径。从左上角顺时针
        /// x=>左上角，y=>右上角
        /// z=>右下，w=>左下
        /// </summary>
        public Vector4 CornerRadius
        {
            get => cornerRadius;
            set
            {
                cornerRadius = Vector4.Max(value, Vector4.zero);
                if (shouldModifySharedMat)
                {
                    sharedMat.SetVector(pentagonRectCornerRadius_Sp, cornerRadius);
                }
                onComponentSettingsChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// 尖端的拐角半径。五边形的第五个角。
        /// </summary>
        public float TipRadius
        {
            get => tipRadius;
            set
            {
                tipRadius = Mathf.Max(value, 0.001f);
                if (shouldModifySharedMat)
                {
                    sharedMat.SetFloat(pentagonTriangleCornerRadius_Sp, tipRadius);
                }
                onComponentSettingsChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// 尖端的尺寸（从形状的矩形部分伸出的三角形部分）
        /// </summary>
        public float TipSize
        {
            get => tipSize;
            set
            {
                tipSize = Mathf.Max(value, 1);
                if (shouldModifySharedMat)
                {
                    sharedMat.SetFloat(pentagonTriangleSize_Sp, tipSize);
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

            TipSize = tipSize;
            TipRadius = tipRadius;
        }

        public void OnValidate()
        {
            CornerRadius = cornerRadius;
            TipSize = tipSize;
            TipRadius = tipRadius;
        }
        /// <summary>
        /// 材质的初始化值
        /// </summary>
        /// <param name="material"></param>
        public void InitValuesFromMaterial(ref Material material)
        {
            cornerRadius = material.GetVector(pentagonRectCornerRadius_Sp);
            tipSize = material.GetFloat(pentagonTriangleSize_Sp);
            tipRadius = material.GetFloat(pentagonTriangleCornerRadius_Sp);
        }
        /// <summary>
        /// 修改材质
        /// </summary>
        /// <param name="material"></param>
        /// <param name="otherProperties"></param>
        public void ModifyMaterial(ref Material material, params object[] otherProperties)
        {
            material.SetVector(pentagonRectCornerRadius_Sp, cornerRadius);
            material.SetFloat(pentagonTriangleCornerRadius_Sp, tipRadius);
            material.SetFloat(pentagonTriangleSize_Sp, tipSize);
        }
    }
}