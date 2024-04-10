using System.Text;

namespace UnityEngine.UI.ImageExtensions
{
    public static class ImageHelper
    {
        private static readonly Vector3[] sxy = new Vector3[4];
        private static readonly Vector3[] suv = new Vector3[4];

        public static void GenerateSimpleSprite(VertexHelper vh, bool preserveAspect, Canvas canvas,
            RectTransform rectTransform, Sprite activeSprite, Color32 color, float falloffDistance)
        {
            vh.Clear();

            Vector4 v = GetDrawingDimensions(preserveAspect, activeSprite, canvas, rectTransform);
            Vector4 uv = (activeSprite != null)
                ? Sprites.DataUtility.GetOuterUV(activeSprite)
                : new Vector4(0, 0, 1, 1);

            Color32 color32 = color;
            vh.Clear();

            Vector3[] pos = {
                new Vector3(v.x, v.y),
                new Vector3(v.x, v.w),
                new Vector3(v.z, v.w),
                new Vector3(v.z, v.y),
            };

            Vector2[] uvs = {
                new Vector2(uv.x, uv.y),
                new Vector2(uv.x, uv.w),
                new Vector2(uv.z, uv.w),
                new Vector2(uv.z, uv.y),
            };

            Vector2[] uv1s =
            {
                new Vector2(0, 0),
                new Vector2(0, 1),
                new Vector2(1, 1),
                new Vector2(1, 0),
            };

            Vector2 size = new Vector2(v.z - v.x, v.w - v.y);

            vh.AddVert(pos[0], color32, uvs[0], uv1s[0], size, Vector2.zero, Vector3.zero, Vector4.zero);
            vh.AddVert(pos[1], color32, uvs[1], uv1s[1], size, Vector2.zero, Vector3.zero, Vector4.zero);
            vh.AddVert(pos[2], color32, uvs[2], uv1s[2], size, Vector2.zero, Vector3.zero, Vector4.zero);
            vh.AddVert(pos[3], color32, uvs[3], uv1s[3], size, Vector2.zero, Vector3.zero, Vector4.zero);

            vh.AddTriangle(0, 1, 2);
            vh.AddTriangle(2, 3, 0);
        }

        public static void GenerateFilledSprite(VertexHelper toFill,
            bool preserveAspect,
            Canvas canvas,
            RectTransform rectTransform,
            Sprite activeSprite,
            Color32 color,
            Image.FillMethod fillMethod,
            float fillAmount,
            int fillOrigin,
            bool fillClockwise,
            float falloffDistance)
        {
            toFill.Clear();

            if (fillAmount < 0.001f)
                return;

            Vector4 v = GetDrawingDimensions(preserveAspect, activeSprite, canvas, rectTransform);
            Vector2 size = new Vector2(v.z - v.x, v.w - v.y);

            Vector4 outer = activeSprite != null
                ? Sprites.DataUtility.GetOuterUV(activeSprite)
                : new Vector4(0, 0, 1, 1);
            UIVertex uiv = UIVertex.simpleVert;
            uiv.color = color;

            float tx0 = outer.x;
            float ty0 = outer.y;
            float tx1 = outer.z;
            float ty1 = outer.w;

            //水平和垂直填充的精灵很简单——只需提前结束图像即可
            if (fillMethod == Image.FillMethod.Horizontal || fillMethod == Image.FillMethod.Vertical)
            {
                if (fillMethod == Image.FillMethod.Horizontal)
                {
                    float fill = (tx1 - tx0) * fillAmount;

                    if (fillOrigin == 1)
                    {
                        v.x = v.z - (v.z - v.x) * fillAmount;
                        tx0 = tx1 - fill;
                    }
                    else
                    {
                        v.z = v.x + (v.z - v.x) * fillAmount;
                        tx1 = tx0 + fill;
                    }
                }
                else if (fillMethod == Image.FillMethod.Vertical)
                {
                    float fill = (ty1 - ty0) * fillAmount;

                    if (fillOrigin == 1)
                    {
                        v.y = v.w - (v.w - v.y) * fillAmount;
                        ty0 = ty1 - fill;
                    }
                    else
                    {
                        v.w = v.y + (v.w - v.y) * fillAmount;
                        ty1 = ty0 + fill;
                    }
                }
            }

            sxy[0] = new Vector2(v.x, v.y);
            sxy[1] = new Vector2(v.x, v.w);
            sxy[2] = new Vector2(v.z, v.w);
            sxy[3] = new Vector2(v.z, v.y);

            suv[0] = new Vector2(tx0, ty0);
            suv[1] = new Vector2(tx0, ty1);
            suv[2] = new Vector2(tx1, ty1);
            suv[3] = new Vector2(tx1, ty0);


            if (fillAmount < 1f && 
                fillMethod != Image.FillMethod.Horizontal &&
                fillMethod != Image.FillMethod.Vertical)
            {
                if (fillMethod == Image.FillMethod.Radial90)
                {
                    if (RadialCut(sxy, suv, fillAmount, fillClockwise, fillOrigin))
                        AddQuad(toFill, sxy, color, suv, size);
                }
                else if (fillMethod == Image.FillMethod.Radial180)
                {
                    for (int side = 0; side < 2; ++side)
                    {
                        float fx0, fx1, fy0, fy1;
                        int even = fillOrigin > 1 ? 1 : 0;

                        if (fillOrigin == 0 || fillOrigin == 2)
                        {
                            fy0 = 0f;
                            fy1 = 1f;
                            if (side == even)
                            {
                                fx0 = 0f;
                                fx1 = 0.5f;
                            }
                            else
                            {
                                fx0 = 0.5f;
                                fx1 = 1f;
                            }
                        }
                        else
                        {
                            fx0 = 0f;
                            fx1 = 1f;
                            if (side == even)
                            {
                                fy0 = 0.5f;
                                fy1 = 1f;
                            }
                            else
                            {
                                fy0 = 0f;
                                fy1 = 0.5f;
                            }
                        }

                        sxy[0].x = Mathf.Lerp(v.x, v.z, fx0);
                        sxy[1].x = sxy[0].x;
                        sxy[2].x = Mathf.Lerp(v.x, v.z, fx1);
                        sxy[3].x = sxy[2].x;

                        sxy[0].y = Mathf.Lerp(v.y, v.w, fy0);
                        sxy[1].y = Mathf.Lerp(v.y, v.w, fy1);
                        sxy[2].y = sxy[1].y;
                        sxy[3].y = sxy[0].y;

                        suv[0].x = Mathf.Lerp(tx0, tx1, fx0);
                        suv[1].x = suv[0].x;
                        suv[2].x = Mathf.Lerp(tx0, tx1, fx1);
                        suv[3].x = suv[2].x;

                        suv[0].y = Mathf.Lerp(ty0, ty1, fy0);
                        suv[1].y = Mathf.Lerp(ty0, ty1, fy1);
                        suv[2].y = suv[1].y;
                        suv[3].y = suv[0].y;

                        float val = fillClockwise ? fillAmount * 2f - side : fillAmount * 2f - (1 - side);

                        if (RadialCut(sxy, suv, Mathf.Clamp01(val), fillClockwise,
                            ((side + fillOrigin + 3) % 4)))
                        {
                            AddQuad(toFill, sxy, color, suv, size);
                        }
                    }
                }
                else if (fillMethod == Image.FillMethod.Radial360)
                {
                    for (int corner = 0; corner < 4; ++corner)
                    {
                        float fx0, fx1, fy0, fy1;

                        if (corner < 2)
                        {
                            fx0 = 0f;
                            fx1 = 0.5f;
                        }
                        else
                        {
                            fx0 = 0.5f;
                            fx1 = 1f;
                        }

                        if (corner == 0 || corner == 3)
                        {
                            fy0 = 0f;
                            fy1 = 0.5f;
                        }
                        else
                        {
                            fy0 = 0.5f;
                            fy1 = 1f;
                        }

                        sxy[0].x = Mathf.Lerp(v.x, v.z, fx0);
                        sxy[1].x = sxy[0].x;
                        sxy[2].x = Mathf.Lerp(v.x, v.z, fx1);
                        sxy[3].x = sxy[2].x;

                        sxy[0].y = Mathf.Lerp(v.y, v.w, fy0);
                        sxy[1].y = Mathf.Lerp(v.y, v.w, fy1);
                        sxy[2].y = sxy[1].y;
                        sxy[3].y = sxy[0].y;

                        suv[0].x = Mathf.Lerp(tx0, tx1, fx0);
                        suv[1].x = suv[0].x;
                        suv[2].x = Mathf.Lerp(tx0, tx1, fx1);
                        suv[3].x = suv[2].x;

                        suv[0].y = Mathf.Lerp(ty0, ty1, fy0);
                        suv[1].y = Mathf.Lerp(ty0, ty1, fy1);
                        suv[2].y = suv[1].y;
                        suv[3].y = suv[0].y;

                        float val = fillClockwise
                            ? fillAmount * 4f - ((corner + fillOrigin) % 4)
                            : fillAmount * 4f - (3 - ((corner + fillOrigin) % 4));

                        if (RadialCut(sxy, suv, Mathf.Clamp01(val), fillClockwise, ((corner + 2) % 4)))
                            AddQuad(toFill, sxy, color, suv, size);
                    }
                }
            }
            else
            {
                AddQuad(toFill, sxy, color, suv, size);
            }
        }

        /// <summary>
        /// 增加方形面片
        /// </summary>
        /// <param name="vertexHelper"></param>
        /// <param name="quadPositions"></param>
        /// <param name="color"></param>
        /// <param name="quadUVs"></param>
        /// <param name="size"></param>
        private static void AddQuad(VertexHelper vertexHelper,
            Vector3[] quadPositions,
            Color32 color,
            Vector3[] quadUVs,
            Vector2 size)
        {
            int startIndex = vertexHelper.currentVertCount;

            StringBuilder sr = new StringBuilder();
            for (int i = 0; i < 4; ++i)
            {
                vertexHelper.AddVert(quadPositions[i], color, quadUVs[i], quadUVs[i], size, Vector2.zero,
                    Vector3.zero, Vector4.zero);
                sr.AppendLine($"Pos: {quadPositions[i]}, uv: {quadUVs[i]}");
            }


            vertexHelper.AddTriangle(startIndex, startIndex + 1, startIndex + 2);
            vertexHelper.AddTriangle(startIndex + 2, startIndex + 3, startIndex);
        }

        /// <summary>
        /// 获取绘画尺寸
        /// </summary>
        /// <param name="shouldPreserveAspect"></param>
        /// <param name="activeSprite"></param>
        /// <param name="canvas"></param>
        /// <param name="rectTransform"></param>
        /// <returns></returns>
        private static Vector4 GetDrawingDimensions(bool shouldPreserveAspect, 
            Sprite activeSprite,
            Canvas canvas,
            RectTransform rectTransform)
        {
            var padding = activeSprite == null ? Vector4.zero : Sprites.DataUtility.GetPadding(activeSprite);
            var size = activeSprite == null
                ? new Vector2(rectTransform.rect.width, rectTransform.rect.height)
                : new Vector2(activeSprite.rect.width, activeSprite.rect.height);

            if (size.x <= 0) size.x = 1;
            if (size.y <= 0) size.y = 1;
            Rect r = GetPixelAdjustedRect(canvas, rectTransform);
            
            int spriteW = Mathf.RoundToInt(size.x);
            int spriteH = Mathf.RoundToInt(size.y);

            Vector4 v = new Vector4(
                padding.x / spriteW,
                padding.y / spriteH,
                (spriteW - padding.z) / spriteW,
                (spriteH - padding.w) / spriteH);

            if (shouldPreserveAspect && size.sqrMagnitude > 0.0f)
            {
                PreserveSpriteAspectRatio(ref r, rectTransform, size);
            }

            v = new Vector4(
                r.x + r.width * v.x,
                r.y + r.height * v.y,
                r.x + r.width * v.z,
                r.y + r.height * v.w
            );

            return v;
        }

        /// <summary>
        /// 保持精灵纵横比
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="rectTransform"></param>
        /// <param name="spriteSize"></param>
        public static void PreserveSpriteAspectRatio(ref Rect rect, RectTransform rectTransform, Vector2 spriteSize)
        {
            float spriteRatio = spriteSize.x / spriteSize.y;
            float rectRatio = rect.width / rect.height;

            if (spriteRatio > rectRatio)
            {
                float oldHeight = rect.height;
                rect.height = rect.width * (1.0f / spriteRatio);
                rect.y += (oldHeight - rect.height) * rectTransform.pivot.y;
            }
            else
            {
                float oldWidth = rect.width;
                rect.width = rect.height * spriteRatio;
                rect.x += (oldWidth - rect.width) * rectTransform.pivot.x;
            }
        }

        /// <summary>
        /// 获取像素调整后的矩形
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="rectTransform"></param>
        /// <returns></returns>
        private static Rect GetPixelAdjustedRect(Canvas canvas, RectTransform rectTransform)
        {
            if (!canvas || canvas.renderMode == RenderMode.WorldSpace || canvas.scaleFactor == 0.0f ||
                !canvas.pixelPerfect)
            {
                return rectTransform.rect;
            }

            return RectTransformUtility.PixelAdjustRect(rectTransform, canvas);
        }

        /// <summary>
        /// 径向切割
        /// </summary>
        /// <param name="xy"></param>
        /// <param name="uv"></param>
        /// <param name="fill"></param>
        /// <param name="invert"></param>
        /// <param name="corner"></param>
        /// <returns></returns>
        private static bool RadialCut(Vector3[] xy, Vector3[] uv, float fill, bool invert, int corner)
        {
            // 无需填充
            if (fill < 0.001f) return false;

            // 偶数角反转填充方向
            if ((corner & 1) == 1) invert = !invert;

            // 无需调整
            if (!invert && fill > 0.999f) return true;

            // 将0-1值转换为0到90度的弧度角度
            float angle = Mathf.Clamp01(fill);
            if (invert) angle = 1f - angle;
            angle *= 90f * Mathf.Deg2Rad;

            // 计算有效的X和Y因子
            float cos = Mathf.Cos(angle);
            float sin = Mathf.Sin(angle);

            RadialCut(xy, cos, sin, invert, corner);
            RadialCut(uv, cos, sin, invert, corner);
            return true;
        }

        /// <summary>
        /// 径向切割
        /// </summary>
        /// <param name="xy"></param>
        /// <param name="cos"></param>
        /// <param name="sin"></param>
        /// <param name="invert"></param>
        /// <param name="corner"></param>
        private static void RadialCut(Vector3[] xy, float cos, float sin, bool invert, int corner)
        {
            int i0 = corner;
            int i1 = ((corner + 1) % 4);
            int i2 = ((corner + 2) % 4);
            int i3 = ((corner + 3) % 4);

            if ((corner & 1) == 1)
            {
                if (sin > cos)
                {
                    cos /= sin;
                    sin = 1f;

                    if (invert)
                    {
                        xy[i1].x = Mathf.Lerp(xy[i0].x, xy[i2].x, cos);
                        xy[i2].x = xy[i1].x;
                    }
                }
                else if (cos > sin)
                {
                    sin /= cos;
                    cos = 1f;

                    if (!invert)
                    {
                        xy[i2].y = Mathf.Lerp(xy[i0].y, xy[i2].y, sin);
                        xy[i3].y = xy[i2].y;
                    }
                }
                else
                {
                    cos = 1f;
                    sin = 1f;
                }

                if (!invert) xy[i3].x = Mathf.Lerp(xy[i0].x, xy[i2].x, cos);
                else xy[i1].y = Mathf.Lerp(xy[i0].y, xy[i2].y, sin);
            }
            else
            {
                if (cos > sin)
                {
                    sin /= cos;
                    cos = 1f;

                    if (!invert)
                    {
                        xy[i1].y = Mathf.Lerp(xy[i0].y, xy[i2].y, sin);
                        xy[i2].y = xy[i1].y;
                    }
                }
                else if (sin > cos)
                {
                    cos /= sin;
                    sin = 1f;

                    if (invert)
                    {
                        xy[i2].x = Mathf.Lerp(xy[i0].x, xy[i2].x, cos);
                        xy[i3].x = xy[i2].x;
                    }
                }
                else
                {
                    cos = 1f;
                    sin = 1f;
                }

                if (invert) xy[i3].y = Mathf.Lerp(xy[i0].y, xy[i2].y, sin);
                else xy[i1].x = Mathf.Lerp(xy[i0].x, xy[i2].x, cos);
            }
        }
    }
}