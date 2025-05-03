using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LLAFramework.UI.ImageExtensions
{
    /// <summary>
    /// 三角形
    /// </summary>
    [Serializable]
    public class TriangleImg : UIImgComponent
    {
        [SerializeField] private Vector3 cornerRadius;
#if UNITY_EDITOR
        [SerializeField] private bool uniformCornerRadius;
#endif

        private static readonly int triangleCornerRadius_Sp = Shader.PropertyToID("_TriangleCornerRadius");

        public Material sharedMat { get; set; }
        public bool shouldModifySharedMat { get; set; }
        public RectTransform rectTransform { get; set; }
        
        public event EventHandler onComponentSettingsChanged;

        /// <summary>
        /// 三个角的半径。从左下角逆时针
        /// x=>左下，y=>右下
        /// z=>顶部
        /// </summary>
        public Vector3 CornerRadius
        {
            get => cornerRadius;
            set
            {
                cornerRadius = Vector3.Max(value, Vector3.zero);
                if (shouldModifySharedMat)
                {
                    sharedMat.SetVector(triangleCornerRadius_Sp, cornerRadius);
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
            this.shouldModifySharedMat = sharedMat == renderMat;
            this.rectTransform = rectTransform;
        }

        public void OnValidate()
        {
            CornerRadius = cornerRadius;
        }

        /// <summary>
        /// 材质的初始化值
        /// </summary>
        /// <param name="material"></param>
        public void InitValuesFromMaterial(ref Material material)
        {
            cornerRadius = material.GetVector(triangleCornerRadius_Sp);
        }

        /// <summary>
        /// 修改材质
        /// </summary>
        /// <param name="material"></param>
        /// <param name="otherProperties"></param>
        public void ModifyMaterial(ref Material material, params object[] otherProperties)
        {
            material.SetVector(triangleCornerRadius_Sp, cornerRadius);
        }
    }
}