using System;
using UnityEngine;

namespace GameLogic.UI.ImageExtensions
{
    public interface UIImgComponent
    {
        //共享材质
        Material sharedMat { get; set; }
        //渲染材质
        bool shouldModifySharedMat { get; set; }
        //变换矩阵
        RectTransform rectTransform { get; set; }
        //组件设置改变事件
        event EventHandler onComponentSettingsChanged;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="sharedMat"></param>
        /// <param name="renderMat"></param>
        /// <param name="rectTransform"></param>
        void Init(Material sharedMat, Material renderMat, RectTransform rectTransform);

        /// <summary>
        /// 验证
        /// </summary>
        void OnValidate();
        /// <summary>
        /// 初始化材质
        /// </summary>
        /// <param name="material"></param>
        void InitValuesFromMaterial(ref Material material);
        /// <summary>
        /// 修改材质
        /// </summary>
        /// <param name="material"></param>
        /// <param name="otherProperties"></param>
        void ModifyMaterial(ref Material material, params object[] otherProperties);
    }
}
