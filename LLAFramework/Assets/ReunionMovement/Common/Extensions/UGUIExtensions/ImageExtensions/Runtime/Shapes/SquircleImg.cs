using LLAFramework.UI.ImageExtensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LLAFramework.UI.ImageExtensions
{
    [Serializable]
    public class SquircleImg : UIImgComponent
    {
        [SerializeField] private float squircleTime;

        private static readonly int squircleTime_Sp = Shader.PropertyToID("_SquircleTime");

        public Material sharedMat { get; set; }
        public bool shouldModifySharedMat { get; set; }
        public RectTransform rectTransform { get; set; }
        public event EventHandler onComponentSettingsChanged;

        public float SquircleTime
        {
            get => squircleTime;
            set
            {
                squircleTime = Mathf.Max(value, 0);
                if (shouldModifySharedMat)
                {
                    sharedMat.SetFloat(squircleTime_Sp, squircleTime);
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
            SquircleTime = squircleTime;
        }

        /// <summary>
        /// 材质的初始化值
        /// </summary>
        /// <param name="material"></param>
        public void InitValuesFromMaterial(ref Material material)
        {
            SquircleTime = material.GetFloat(squircleTime_Sp);
        }

        /// <summary>
        /// 修改材质
        /// </summary>
        /// <param name="material"></param>
        /// <param name="otherProperties"></param>
        public void ModifyMaterial(ref Material material, params object[] otherProperties)
        {
            material.SetFloat(squircleTime_Sp, squircleTime);
        }
    }
}