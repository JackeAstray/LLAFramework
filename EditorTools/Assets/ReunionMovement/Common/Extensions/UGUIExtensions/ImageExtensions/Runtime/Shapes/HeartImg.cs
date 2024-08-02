using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GameLogic.UI.ImageExtensions
{
    /// <summary>
    /// 心形
    /// </summary>
    [Serializable]
    public class HeartImg : UIImgComponent
    {
        public Material sharedMat { get; set; }
        public bool shouldModifySharedMat { get; set; }
        public RectTransform rectTransform { get; set; }
        public event EventHandler onComponentSettingsChanged;

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
        }

        /// <summary>
        /// 材质的初始化值
        /// </summary>
        /// <param name="material"></param>
        public void InitValuesFromMaterial(ref Material material)
        {
        }

        /// <summary>
        /// 修改材质
        /// </summary>
        /// <param name="material"></param>
        /// <param name="otherProperties"></param>
        public void ModifyMaterial(ref Material material, params object[] otherProperties)
        {
        }

        /// <summary>
        /// 更新圆半径
        /// </summary>
        /// <param name="rectT"></param>
        internal void UpdateCircleRadius(RectTransform rectT)
        {
        }
    }
}
