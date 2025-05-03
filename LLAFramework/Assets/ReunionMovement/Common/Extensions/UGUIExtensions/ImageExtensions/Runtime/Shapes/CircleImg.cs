using System;
using UnityEngine;

namespace LLAFramework.UI.ImageExtensions
{
    /// <summary>
    /// 圆形
    /// </summary>
    [Serializable]
    public class CircleImg : UIImgComponent
    {
        [SerializeField] private float radius;
        [SerializeField] private bool fitRadius;

        private static readonly int radius_SP = Shader.PropertyToID("_CircleRadius");
        private static readonly int fitRadius_SP = Shader.PropertyToID("_CircleFitRadius");

        public Material sharedMat { get; set; }
        public bool shouldModifySharedMat { get; set; }
        public RectTransform rectTransform { get; set; }
        public event EventHandler onComponentSettingsChanged;

        /// <summary>
        /// 圆的半径。如果FitToRect设置为true，则此操作无效
        /// </summary>
        public float Radius
        {
            get => radius;
            set
            {
                radius = Mathf.Max(value, 0f);
                if (shouldModifySharedMat)
                {
                    sharedMat.SetFloat(radius_SP, radius);
                }
                onComponentSettingsChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        /// <summary>
        /// 将圆拟合到矩形变换
        /// </summary>
        public bool FitToRect
        {
            get => fitRadius;
            set
            {
                fitRadius = value;
                if (shouldModifySharedMat)
                {
                    sharedMat.SetInt(fitRadius_SP, fitRadius ? 1 : 0);
                }
                onComponentSettingsChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private float circleFitRadius => Mathf.Min(rectTransform.rect.width, rectTransform.rect.height) / 2;

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
            Radius = radius;
            FitToRect = fitRadius;
        }

        /// <summary>
        /// 材质的初始化值
        /// </summary>
        /// <param name="material"></param>
        public void InitValuesFromMaterial(ref Material material)
        {
            radius = material.GetFloat(radius_SP);
            fitRadius = material.GetInt(fitRadius_SP) == 1;
        }

        /// <summary>
        /// 修改材质
        /// </summary>
        /// <param name="material"></param>
        /// <param name="otherProperties"></param>
        public void ModifyMaterial(ref Material material, params object[] otherProperties)
        {
            material.SetFloat(radius_SP, radius);
            material.SetInt(fitRadius_SP, fitRadius ? 1 : 0);
        }

        /// <summary>
        /// 更新圆半径
        /// </summary>
        /// <param name="rectT"></param>
        internal void UpdateCircleRadius(RectTransform rectT)
        {
            this.rectTransform = rectT;
            if (fitRadius)
            {
                radius = circleFitRadius;
            }
        }
    }
}