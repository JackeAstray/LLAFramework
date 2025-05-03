using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LLAFramework.UI.ImageExtensions
{
    /// <summary>
    /// 六边形
    /// </summary>
    [Serializable]
    public class HexagonImg : UIImgComponent
    {
        [SerializeField] private Vector4 cornerRadius;
        [SerializeField] private bool uniformCornerRadius;
        [SerializeField] private Vector2 tipSize;
        [SerializeField] private bool uniformTipSize;
        [SerializeField] private Vector2 tipRadius;
        [SerializeField] private bool uniformTipRadius;

        private static readonly int hexagonTipSizes_Sp = Shader.PropertyToID("_HexagonTipSize");
        private static readonly int hexagonTipRadius_Sp = Shader.PropertyToID("_HexagonTipRadius");
        private static readonly int hexagonRectCornerRadius_Sp = Shader.PropertyToID("_HexagonCornerRadius");

        public Material sharedMat { get; set; }
        public bool shouldModifySharedMat { get; set; }
        public RectTransform rectTransform { get; set; }

        public event EventHandler onComponentSettingsChanged;
        /// <summary>
        /// 两个尖端的尺寸（从形状的矩形部分伸出的三角形部分）
        /// x=>左尖端，y=>右尖端
        /// </summary>
        public Vector2 TipSize
        {
            get => tipSize;
            set
            {
                tipSize = Vector2.Max(value, Vector2.one);
                if (shouldModifySharedMat)
                {
                    sharedMat.SetVector(hexagonTipSizes_Sp, tipSize);
                }
                onComponentSettingsChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// 尖端拐角的半径
        /// x=>左上角，y=>右上角
        /// </summary>
        public Vector2 TipRadius
        {
            get => tipRadius;
            set
            {
                tipRadius = Vector2.Max(value, Vector2.one);
                if (shouldModifySharedMat)
                {
                    sharedMat.SetVector(hexagonTipRadius_Sp, tipRadius);
                }
                onComponentSettingsChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// 形状矩形部分四个角的半径。
        /// 从左下角逆时针
        /// x=>左下，y=>右下
        /// z=>右上角，w=>左上角
        /// </summary>
        public Vector4 CornerRadius
        {
            get => cornerRadius;
            set
            {
                cornerRadius = Vector4.Max(value, Vector4.one);
                if (shouldModifySharedMat)
                {
                    sharedMat.SetVector(hexagonRectCornerRadius_Sp, cornerRadius);
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

            TipRadius = tipRadius;
            TipSize = tipSize;
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
            cornerRadius = material.GetVector(hexagonRectCornerRadius_Sp);
            tipRadius = material.GetVector(hexagonTipRadius_Sp);
            tipSize = material.GetVector(hexagonTipSizes_Sp);
        }

        /// <summary>
        /// 修改材质
        /// </summary>
        /// <param name="material"></param>
        /// <param name="otherProperties"></param>
        public void ModifyMaterial(ref Material material, params object[] otherProperties)
        {
            material.SetVector(hexagonTipSizes_Sp, tipSize);
            material.SetVector(hexagonTipRadius_Sp, tipRadius);
            material.SetVector(hexagonRectCornerRadius_Sp, cornerRadius);
        }
    }
}