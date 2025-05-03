using LLAFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LLAFramework
{
    public class ColorPaletteModule : CustommModuleInitialize
    {
        #region 实例与初始化
        public static ColorPaletteModule Instance = new ColorPaletteModule();

        public bool IsInited { get; private set; }
        private double initProgress = 0;
        public double InitProgress { get { return initProgress; } }
        #endregion

        ColorPalette colorPalette;

        public IEnumerator Init()
        {
            initProgress = 0;
            InitConfig();
            yield return null;
            initProgress = 100;
            IsInited = true;
            Log.Debug("ColorPaletteModule 初始化完成");
        }

        public void ClearData()
        {
            Log.Debug("ColorPaletteModule 清除数据");
        }

        void InitConfig()
        {
            colorPalette = ResourcesModule.Instance.Load<ColorPalette>("ScriptableObjects/ColorPalette");
            //Color color = colorPalette.GetColor("西瓜", ColorPaletteScheme.CommonlyUsed);
            //Log.Debug(color);
        }

        /// <summary>
        /// 获取颜色
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Color GetColor(string name, ColorPaletteScheme colorPaletteScheme)
        {
            return colorPalette.GetColor(name, colorPaletteScheme);
        }

        /// <summary>
        /// 添加颜色
        /// </summary>
        /// <param name="name"></param>
        /// <param name="color"></param>
        public void AddColor(string name, Color color, ColorPaletteScheme colorPaletteScheme)
        {
            colorPalette.AddColor(name, color, colorPaletteScheme);
        }

        /// <summary>
        /// 更新颜色
        /// </summary>
        /// <param name="name"></param>
        /// <param name="color"></param>
        public void UpdateColor(string name, Color color, ColorPaletteScheme colorPaletteScheme)
        {
            colorPalette.UpdateColor(name, color, colorPaletteScheme);
        }

        /// <summary>
        /// 移除颜色
        /// </summary>
        /// <param name="name"></param>
        public void RemoveColor(string name, ColorPaletteScheme colorPaletteScheme)
        {
            colorPalette.RemoveColor(name, colorPaletteScheme);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        public void UpdateTime(float elapseSeconds, float realElapseSeconds)
        {

        }
    }
}