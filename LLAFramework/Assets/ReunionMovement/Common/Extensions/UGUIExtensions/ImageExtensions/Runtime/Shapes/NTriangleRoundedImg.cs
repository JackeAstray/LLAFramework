using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LLAFramework.UI.ImageExtensions
{
    [Serializable]
    public class NTriangleRoundedImg : UIImgComponent
    {
        [SerializeField] private float nTriangleRoundedTime;
        [SerializeField] private float nTriangleRoundedNumber;

        private static readonly int nTriangleRoundedTime_Sp = Shader.PropertyToID("_NTriangleRoundedTime");
        private static readonly int nTriangleRoundedNumber_Sp = Shader.PropertyToID("_NTriangleRoundedNumber");

        public Material sharedMat { get; set; }
        public bool shouldModifySharedMat { get; set; }
        public RectTransform rectTransform { get; set; }

        public event EventHandler onComponentSettingsChanged;

        public float NTriangleRoundedTime
        {
            get => nTriangleRoundedTime;
            set
            {
                nTriangleRoundedTime = Mathf.Max(value, 0);
                if (shouldModifySharedMat)
                {
                    sharedMat.SetFloat(nTriangleRoundedTime_Sp, nTriangleRoundedTime);
                }
                onComponentSettingsChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public float NTriangleRoundedNumber
        {
            get => nTriangleRoundedNumber;
            set
            {
                nTriangleRoundedNumber = Mathf.Max(value, 0);
                if (shouldModifySharedMat)
                {
                    sharedMat.SetFloat(nTriangleRoundedNumber_Sp, nTriangleRoundedNumber);
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
            NTriangleRoundedTime = nTriangleRoundedTime;
            NTriangleRoundedNumber = nTriangleRoundedNumber;
        }

        /// <summary>
        /// 材质的初始化值
        /// </summary>
        /// <param name="material"></param>
        public void InitValuesFromMaterial(ref Material material)
        {
            NTriangleRoundedTime = material.GetFloat(nTriangleRoundedTime_Sp);
            NTriangleRoundedNumber = material.GetFloat(nTriangleRoundedNumber_Sp);
        }

        /// <summary>
        /// 修改材质
        /// </summary>
        /// <param name="material"></param>
        /// <param name="otherProperties"></param>
        public void ModifyMaterial(ref Material material, params object[] otherProperties)
        {
            material.SetFloat(nTriangleRoundedTime_Sp, nTriangleRoundedTime);
            material.SetFloat(nTriangleRoundedNumber_Sp, nTriangleRoundedNumber);
        }
    }
}
