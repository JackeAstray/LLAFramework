using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic.UI.ImageExtensions
{
    [Serializable]
    public class BlobbyCrossImg : UIImgComponent
    {
        [SerializeField] private float blobbyCrossTime;

        private static readonly int blobbyCrossTime_Sp = Shader.PropertyToID("_BlobbyCrossTime");

        public Material sharedMat { get; set; }
        public bool shouldModifySharedMat { get; set; }
        public RectTransform rectTransform { get; set; }
        public event EventHandler onComponentSettingsChanged;

        public float BlobbyCrossTime
        {
            get => blobbyCrossTime;
            set
            {
                blobbyCrossTime = Mathf.Max(value, 0);
                if (shouldModifySharedMat)
                {
                    sharedMat.SetFloat(blobbyCrossTime_Sp, blobbyCrossTime);
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
            BlobbyCrossTime = blobbyCrossTime;
        }

        /// <summary>
        /// 材质的初始化值
        /// </summary>
        /// <param name="material"></param>
        public void InitValuesFromMaterial(ref Material material)
        {
            BlobbyCrossTime = material.GetFloat(blobbyCrossTime_Sp);
        }

        /// <summary>
        /// 修改材质
        /// </summary>
        /// <param name="material"></param>
        /// <param name="otherProperties"></param>
        public void ModifyMaterial(ref Material material, params object[] otherProperties)
        {
            material.SetFloat(blobbyCrossTime_Sp, blobbyCrossTime);
        }
    }
}