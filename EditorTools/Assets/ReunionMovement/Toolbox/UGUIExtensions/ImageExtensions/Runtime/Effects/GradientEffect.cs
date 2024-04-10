using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic.UI.ImageExtensions
{
    /// <summary>
    /// 渐变效果
    /// </summary>
    public struct GradientEffect : UIImgComponent
    {
        [SerializeField] private bool enabled;
        [SerializeField] private GradientType gradientType;
        [SerializeField] private Gradient gradient;
        [SerializeField] private Color[] cornerGradientColors;
        [SerializeField] private float rotation;

        public Material sharedMat { get; set; }
        public bool shouldModifySharedMat { get; set; }
        public RectTransform rectTransform { get; set; }

        public event EventHandler onComponentSettingsChanged;

        private static readonly int gradientType_Sp = Shader.PropertyToID("_GradientType");
        private static readonly int gradientColors_Sp = Shader.PropertyToID("colors");
        private static readonly int gradientAlphas_Sp = Shader.PropertyToID("alphas");
        private static readonly int gradientColorsLength_Sp = Shader.PropertyToID("_GradientColorLength");
        private static readonly int gradientAlphasLength_Sp = Shader.PropertyToID("_GradientAlphaLength");
        private static readonly int gradientInterpolationType_Sp = Shader.PropertyToID("_GradientInterpolationType");
        private static readonly int enableGradient_Sp = Shader.PropertyToID("_EnableGradient");
        private static readonly int gradientRotation_Sp = Shader.PropertyToID("_GradientRotation");

        /// <summary>
        /// 启用/禁用渐变覆盖
        /// </summary>
        public bool Enabled
        {
            get => enabled;
            set
            {
                enabled = value;
                if (shouldModifySharedMat)
                {
                    sharedMat.SetInt(enableGradient_Sp, enabled ? 1 : 0);
                }
                onComponentSettingsChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// 渐变的类型。有三种类型：线性、角、径向
        /// </summary>
        public GradientType GradientType
        {
            get => gradientType;
            set
            {
                gradientType = value;
                if (shouldModifySharedMat)
                {
                    sharedMat.SetInt(gradientType_Sp, (int)gradientType);
                }
                onComponentSettingsChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// 渐变的旋转。仅适用于“线性渐变”。
        /// </summary>
        public float Rotation
        {
            get => rotation;
            set
            {
                rotation = value;
                if (shouldModifySharedMat)
                {
                    sharedMat.SetFloat(gradientRotation_Sp, rotation);
                }
                onComponentSettingsChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// 将覆盖到图像上的渐变
        /// </summary>
        public Gradient Gradient
        {
            get => gradient;
            set
            {
                gradient = value;
                if (shouldModifySharedMat)
                {
                    List<Color> Colors = new List<Color>(8);
                    List<Color> Alphas = new List<Color>(8);
                    for (int i = 0; i < 8; i++)
                    {
                        if (i < gradient.colorKeys.Length)
                        {
                            Color col = gradient.colorKeys[i].color;
                            Vector4 data = new Vector4(col.r, col.g, col.b,
                                gradient.colorKeys[i].time);
                            Colors.Add(data);
                            sharedMat.SetColor("_GradientColor" + i, data);
                        }
                        else
                        {
                            sharedMat.SetColor("_GradientColor" + i, Vector4.zero);
                        }
                        if (i < gradient.alphaKeys.Length)
                        {
                            Vector4 data = new Vector4(gradient.alphaKeys[i].alpha, gradient.alphaKeys[i].time);
                            Alphas.Add(data);
                            sharedMat.SetColor("_GradientAlpha" + i, data);
                        }
                        else
                        {
                            sharedMat.SetColor("_GradientAlpha" + i, Vector4.zero);
                        }
                    }

                    sharedMat.SetInt(gradientColorsLength_Sp, gradient.colorKeys.Length);
                    sharedMat.SetInt(gradientAlphasLength_Sp, gradient.alphaKeys.Length);

                    for (int i = Colors.Count; i < 8; i++)
                    {
                        Colors.Add(Vector4.zero);
                    }

                    for (int i = Alphas.Count; i < 8; i++)
                    {
                        Alphas.Add(Vector4.zero);
                    }
                    sharedMat.SetColorArray(gradientColors_Sp, Colors);
                    sharedMat.GetColorArray(gradientAlphas_Sp, Alphas);
                    sharedMat.SetInt(gradientInterpolationType_Sp, (int)gradient.mode);
                }
                onComponentSettingsChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// 4种颜色用于角渐变覆盖。
        /// [0]=>左上角，[1]=>右上角
        /// [2]=>左下，[3]=>右下
        /// </summary>
        public Color[] CornerGradientColors
        {
            get => cornerGradientColors;
            set
            {
                if (cornerGradientColors.Length != 4)
                {
                    cornerGradientColors = new Color[4];
                }

                for (int i = 0; i < value.Length && i < 4; i++)
                {
                    cornerGradientColors[i] = value[i];
                }

                if (shouldModifySharedMat)
                {
                    for (int i = 0; i < cornerGradientColors.Length; i++)
                    {
                        sharedMat.SetColor("_CornerGradientColor" + i, cornerGradientColors[i]);
                    }
                }
                onComponentSettingsChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="SharedMat"></param>
        /// <param name="renderMat"></param>
        /// <param name="rectTransform"></param>
        public void Init(Material SharedMat, Material renderMat, RectTransform rectTransform)
        {
            this.sharedMat = SharedMat;
            this.shouldModifySharedMat = SharedMat == renderMat;
            this.rectTransform = rectTransform;

            if (cornerGradientColors == null || cornerGradientColors.Length != 4)
            {
                cornerGradientColors = new Color[4];
            }
        }

        

        public void OnValidate()
        {
            Enabled = enabled;
            GradientType = gradientType;
            Gradient = gradient;
            CornerGradientColors = cornerGradientColors;
            Rotation = rotation;
        }

        /// <summary>
        /// 材质的初始化值
        /// </summary>
        /// <param name="material"></param>
        public void InitValuesFromMaterial(ref Material material)
        {
            enabled = material.GetInt(enableGradient_Sp) == 1;
            gradientType = (GradientType)material.GetInt(gradientType_Sp);
            rotation = material.GetFloat(gradientRotation_Sp);
            int colorLength = material.GetInt(gradientColorsLength_Sp);
            int alphaLength = material.GetInt(gradientAlphasLength_Sp);
            Gradient gradient = new Gradient();
            GradientColorKey[] colorKeys = new GradientColorKey[colorLength];
            GradientAlphaKey[] alphaKeys = new GradientAlphaKey[alphaLength];
            for (int i = 0; i < colorLength; i++)
            {
                Color colorValue = material.GetColor("_GradientColor" + i);
                colorKeys[i].color = new Color(colorValue.r, colorValue.g, colorValue.b);
                colorKeys[i].time = colorValue.a;
            }

            gradient.colorKeys = colorKeys;
            for (int i = 0; i < alphaLength; i++)
            {
                Color alphaValue = material.GetColor("_GradientAlpha" + i);
                alphaKeys[i].alpha = alphaValue.r;
                alphaKeys[i].time = alphaValue.g;
            }

            gradient.alphaKeys = alphaKeys;
            gradient.mode = (GradientMode)material.GetInt(gradientInterpolationType_Sp);
            this.gradient = gradient;

            cornerGradientColors = new Color[4];
            for (int i = 0; i < CornerGradientColors.Length; i++)
            {
                CornerGradientColors[i] = material.GetColor("_CornerGradientColor" + i);
            }
        }

        /// <summary>
        /// 修改材质
        /// </summary>
        /// <param name="material"></param>
        /// <param name="otherProperties"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void ModifyMaterial(ref Material material, params object[] otherProperties)
        {
            material.DisableKeyword("GRADIENT_LINEAR");
            material.DisableKeyword("GRADIENT_RADIAL");
            material.DisableKeyword("GRADIENT_CORNER");


            if (!enabled) return;
            material.SetInt(enableGradient_Sp, enabled ? 1 : 0);
            material.SetInt(gradientType_Sp, (int)gradientType);
            switch (gradientType)
            {
                case GradientType.Linear:
                    material.EnableKeyword("GRADIENT_LINEAR");
                    break;
                case GradientType.Radial:
                    material.EnableKeyword("GRADIENT_RADIAL");
                    break;
                case GradientType.Corner:
                    material.EnableKeyword("GRADIENT_CORNER");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }


            if (gradientType == GradientType.Corner)
            {
                for (int i = 0; i < cornerGradientColors.Length; i++)
                {
                    material.SetColor("_CornerGradientColor" + i, cornerGradientColors[i]);
                }
            }
            else
            {
                Color[] colors = new Color[8];
                Color[] alphas = new Color[8];
                for (int i = 0; i < gradient.colorKeys.Length; i++)
                {
                    Color col = gradient.colorKeys[i].color;
                    colors[i] = new Color(col.r, col.g, col.b, gradient.colorKeys[i].time);
                }
                for (int i = 0; i < gradient.alphaKeys.Length; i++)
                {
                    alphas[i] = new Color(gradient.alphaKeys[i].alpha, gradient.alphaKeys[i].time, 0, 0);
                }

                material.SetFloat(gradientColorsLength_Sp, gradient.colorKeys.Length);
                material.SetFloat(gradientAlphasLength_Sp, gradient.alphaKeys.Length);
                material.SetFloat(gradientInterpolationType_Sp, (int)gradient.mode);
                material.SetFloat(gradientRotation_Sp, rotation);

                for (int i = 0; i < colors.Length; i++)
                {
                    material.SetColor("_GradientColor" + i, colors[i]);
                }

                for (int i = 0; i < alphas.Length; i++)
                {
                    material.SetColor("_GradientAlpha" + i, alphas[i]);
                }
            }
        }
    }
}