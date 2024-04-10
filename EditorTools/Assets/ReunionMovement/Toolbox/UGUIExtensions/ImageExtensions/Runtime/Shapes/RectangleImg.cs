using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic.UI.ImageExtensions
{
    /// <summary>
    /// 矩形
    /// </summary>
    [Serializable]
    public class RectangleImg : UIImgComponent
    {
        [SerializeField] private Vector4 cornerRadius;
#if UNITY_EDITOR
        [SerializeField] private bool uniformCornerRadius;
#endif
        private static readonly int rectangleCornerRadius_Sp = Shader.PropertyToID("_RectangleCornerRadius");

        public Material sharedMat { get; set; }
        public bool shouldModifySharedMat { get; set; }
        public RectTransform rectTransform { get; set; }

        public event EventHandler onComponentSettingsChanged;

        /// <summary>
        /// 四个角的半径。从左下角逆时针
        /// x=>左下，y=>右下
        /// z=>右上角，w=>左上角
        /// </summary>
        public Vector4 CornerRadius
        {
            get => cornerRadius;
            set
            {
                cornerRadius = Vector4.Max(value, Vector4.zero);
                if (shouldModifySharedMat)
                {
                    sharedMat.SetVector(rectangleCornerRadius_Sp, cornerRadius);
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
            cornerRadius = material.GetVector(rectangleCornerRadius_Sp);
        }

        /// <summary>
        /// 修改材质
        /// </summary>
        /// <param name="material"></param>
        /// <param name="otherProperties"></param>
        public void ModifyMaterial(ref Material material, params object[] otherProperties)
        {
            Vector4 tempCornerRadius = FixRadius(cornerRadius);
            material.SetVector(rectangleCornerRadius_Sp, tempCornerRadius);
        }

        /// <summary>
        /// 修正半径
        /// </summary>
        /// <param name="radius"></param>
        /// <returns></returns>
        private Vector4 FixRadius(Vector4 radius)
        {
            Rect rect = rectTransform.rect;

            radius = Vector4.Max(radius, Vector4.zero);
            radius = Vector4.Min(radius, Vector4.one * Mathf.Min(rect.width, rect.height));
            float scaleFactor =
                Mathf.Min(
                    Mathf.Min(
                        Mathf.Min(
                            Mathf.Min(
                                rect.width / (radius.x + radius.y),
                                rect.width / (radius.z + radius.w)),
                            rect.height / (radius.x + radius.w)),
                        rect.height / (radius.z + radius.y)),
                    1f);
            return radius * scaleFactor;
        }
    }
}