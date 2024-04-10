using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic.UI.ImageExtensions
{
    internal class ImageUtility
    {
        /// <summary>
        /// 修复Canvas的附加Shader通道
        /// </summary>
        /// <param name="canvas"></param>
        internal static void FixAdditionalShaderChannelsInCanvas(Canvas canvas)
        {
            Canvas c = canvas;
            if (canvas == null) return;
            AdditionalCanvasShaderChannels additionalShaderChannels = c.additionalShaderChannels;
            additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord1;
            additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord2;
            additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord3;
            additionalShaderChannels |= AdditionalCanvasShaderChannels.Normal;
            additionalShaderChannels |= AdditionalCanvasShaderChannels.Tangent;
            c.additionalShaderChannels = additionalShaderChannels;
        }

        /// <summary>
        /// 将高维数据编码为低维数据，以便在shader中使用。
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        internal static Vector2 Encode_0_1_16(Vector4 input)
        {
            float e = 255f / 256f;
            float m = 65535f;
            float ms = m * m;
            float n = m - 1;
            Vector4 value = input * e * n;
            float x = Mathf.Floor(value.x) / m + Mathf.Floor(value.y) / ms;
            float y = Mathf.Floor(value.z) / m + Mathf.Floor(value.w) / ms;
            return new Vector2(x, y);
        }

        private static Sprite emptySprite;

        internal static Sprite EmptySprite
        {
            get
            {
                if (emptySprite == null)
                {
                    emptySprite = Resources.Load<Sprite>("UI/Sprites/mpui_default_empty_sprite");
                }

                return emptySprite;
            }
        }
    }
}