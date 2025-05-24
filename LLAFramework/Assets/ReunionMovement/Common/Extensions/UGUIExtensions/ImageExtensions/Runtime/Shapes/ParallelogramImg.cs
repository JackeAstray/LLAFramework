using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LLAFramework.UI.ImageExtensions
{
    /// <summary>
    /// 平行四边形
    /// </summary>
    [Serializable]
    public class ParallelogramImg : UIImgComponent
    {
        [SerializeField] private float parallelogramValue;

        private static readonly int parallelogramValue_Sp = Shader.PropertyToID("_ParallelogramValue");

        public Material sharedMat { get; set; }
        public bool shouldModifySharedMat { get; set; }
        public RectTransform rectTransform { get; set; }

        public event EventHandler onComponentSettingsChanged;

        public float ParallelogramValue
        {
            get => parallelogramValue;
            set
            {
                parallelogramValue = Mathf.Clamp(value, -1f, 1f);
                if (shouldModifySharedMat)
                {
                    sharedMat.SetFloat(parallelogramValue_Sp, parallelogramValue);
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

            ParallelogramValue = parallelogramValue;
        }

        public void OnValidate()
        {
            ParallelogramValue = parallelogramValue;
        }

        /// <summary>
        /// 材质的初始化值
        /// </summary>
        /// <param name="material"></param>
        public void InitValuesFromMaterial(ref Material material)
        {
            parallelogramValue = material.GetFloat(parallelogramValue_Sp);
        }

        /// <summary>
        /// 修改材质
        /// </summary>
        /// <param name="material"></param>
        /// <param name="otherProperties"></param>
        public void ModifyMaterial(ref Material material, params object[] otherProperties)
        {
            material.SetFloat(parallelogramValue_Sp, parallelogramValue);
        }
    }
}