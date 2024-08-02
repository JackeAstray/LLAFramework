using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;

namespace GameLogic.UI.ImageExtensions
{
    /// <summary>
    /// 镜像图像
    /// </summary>
    public class MirrorImage : Image
    {
        /// <summary>
        /// 镜像类型
        /// </summary>
        public enum MirrorTypes
        {
            /// <summary>
            /// 水平
            /// 提供左侧一半素材
            /// </summary>
            Horizontal,

            /// <summary>
            /// 垂直
            /// 提供下侧一半素材
            /// </summary>
            Vertical,

            /// <summary>
            /// 四分之一
            /// 相当于水平，然后再垂直
            /// 提供左下侧素材
            /// </summary>
            Quarter
        }

        /// <summary>
        /// 镜像类型
        /// </summary>
        [SerializeField]
        private MirrorTypes mirrorType = MirrorTypes.Quarter;

        public MirrorTypes MirrorType
        {
            get { return mirrorType; }
            set
            {
                if (mirrorType != value)
                {
                    mirrorType = value;
                    SetVerticesDirty();
                }
            }
        }

        /// <summary>
        /// 是否填充中心
        /// </summary>
        /// <param name="toFill"></param>
        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            Sprite sp = overrideSprite != null ? overrideSprite : sprite;
            if (sp == null)
            {
                base.OnPopulateMesh(toFill);
                return;
            }

            switch (type)
            {
                case Type.Simple:
                    GenerateSimpleSprite(toFill, preserveAspect);
                    break;
                case Type.Sliced:
                    GenerateSlicedSprite(toFill);
                    break;
                case Type.Tiled:
                    GenerateTiledSprite(toFill);
                    break;
                case Type.Filled:
                    break;
            }
        }

        public override void SetNativeSize()
        {
            if (sprite != null)
            {
                float w = sprite.rect.width / pixelsPerUnit;
                float h = sprite.rect.height / pixelsPerUnit;
                if (MirrorType == MirrorTypes.Horizontal)
                {
                    w *= 2;
                }
                else if (MirrorType == MirrorTypes.Vertical)
                {
                    h *= 2;
                }
                else
                {
                    w *= 2;
                    h *= 2;
                }

                rectTransform.anchorMax = rectTransform.anchorMin;
                rectTransform.sizeDelta = new Vector2(w, h);
                SetAllDirty();
            }
        }

        /// <summary>
        /// 为简单图像生成顶点
        /// </summary>
        void GenerateSimpleSprite(VertexHelper vh, bool lPreserveAspect)
        {
            Vector4 v = GetDrawingDimensions(lPreserveAspect);
            var uv = (sprite != null) ? DataUtility.GetOuterUV(sprite) : Vector4.zero;
            Vector4 v1 = v;

            switch (MirrorType)
            {
                case MirrorTypes.Horizontal:
                    v.z = (v.z + v.x) / 2;
                    break;
                case MirrorTypes.Vertical:
                    v.w = (v.w + v.y) / 2;
                    break;
                case MirrorTypes.Quarter:
                    v.z = (v.z + v.x) / 2;
                    v.w = (v.w + v.y) / 2;
                    break;
                default:
                    break;
            }


            //v.w = (v.w + v.y) / 2;
            var color32 = color;
            vh.Clear();
            vh.AddVert(new Vector3(v.x, v.y), color32, new Vector2(uv.x, uv.y));
            vh.AddVert(new Vector3(v.x, v.w), color32, new Vector2(uv.x, uv.w));
            vh.AddVert(new Vector3(v.z, v.w), color32, new Vector2(uv.z, uv.w));
            vh.AddVert(new Vector3(v.z, v.y), color32, new Vector2(uv.z, uv.y));
            vh.AddTriangle(0, 1, 2);
            vh.AddTriangle(2, 3, 0);

            switch (MirrorType)
            {
                /// 1,2,5
                /// 0,3,4
                case MirrorTypes.Horizontal:
                    vh.AddVert(new Vector3(v1.z, v1.y), color32, new Vector2(uv.x, uv.y));
                    vh.AddVert(new Vector3(v1.z, v1.w), color32, new Vector2(uv.x, uv.w));
                    vh.AddTriangle(3, 2, 5);
                    vh.AddTriangle(5, 4, 3);
                    break;
                /// 4,5
                /// 1,2
                /// 0,3
                case MirrorTypes.Vertical:
                    vh.AddVert(new Vector3(v1.x, v1.w), color32, new Vector2(uv.x, uv.y));
                    vh.AddVert(new Vector3(v1.z, v1.w), color32, new Vector2(uv.z, uv.y));
                    vh.AddTriangle(1, 4, 5);
                    vh.AddTriangle(5, 2, 1);
                    break;
                /// 8,7,6
                /// 1,2,5
                /// 0,3,4
                case MirrorTypes.Quarter:
                    vh.AddVert(new Vector3(v1.z, v1.y), color32, new Vector2(uv.x, uv.y));
                    vh.AddVert(new Vector3(v1.z, v.w), color32, new Vector2(uv.x, uv.w));
                    vh.AddTriangle(3, 2, 5);
                    vh.AddTriangle(5, 4, 3);
                    vh.AddVert(new Vector3(v1.z, v1.w), color32, new Vector2(uv.x, uv.y));
                    vh.AddVert(new Vector3(v.z, v1.w), color32, new Vector2(uv.z, uv.y));
                    vh.AddVert(new Vector3(v1.x, v1.w), color32, new Vector2(uv.x, uv.y));
                    vh.AddTriangle(6, 5, 2);
                    vh.AddTriangle(2, 7, 6);
                    vh.AddTriangle(7, 2, 1);
                    vh.AddTriangle(1, 8, 7);
                    break;
                default:
                    break;
            }

        }

        static readonly Vector2[] s_VertScratch = new Vector2[4];
        static readonly Vector2[] s_UVScratch = new Vector2[4];

        /// <summary>
        /// 生成切片精灵
        /// </summary>
        /// <param name="toFill"></param>
        private void GenerateSlicedSprite(VertexHelper toFill)
        {
            if (!hasBorder)
            {
                GenerateSimpleSprite(toFill, false);
                return;
            }

            Vector4 outer, inner, padding, border;

            if (sprite != null)
            {
                outer = DataUtility.GetOuterUV(sprite);
                inner = DataUtility.GetInnerUV(sprite);
                padding = DataUtility.GetPadding(sprite);
                border = sprite.border;
            }
            else
            {
                outer = Vector4.zero;
                inner = Vector4.zero;
                padding = Vector4.zero;
                border = Vector4.zero;
            }

            Rect rect = GetPixelAdjustedRect();

            Vector4 adjustedBorders = GetAdjustedBorders(border / pixelsPerUnit, rect);

            padding = padding / pixelsPerUnit;

            s_VertScratch[0] = new Vector2(padding.x, padding.y);
            s_VertScratch[3] = new Vector2(rect.width - padding.z, rect.height - padding.w);

            s_VertScratch[1].x = adjustedBorders.x;
            s_VertScratch[1].y = adjustedBorders.y;

            s_VertScratch[2].x = rect.width - adjustedBorders.z;
            s_VertScratch[2].y = rect.height - adjustedBorders.w;

            s_UVScratch[0] = new Vector2(outer.x, outer.y);
            s_UVScratch[1] = new Vector2(inner.x, inner.y);
            s_UVScratch[2] = new Vector2(inner.z, inner.w);
            s_UVScratch[3] = new Vector2(outer.z, outer.w);


            switch (MirrorType)
            {
                case MirrorTypes.Horizontal:
                    s_VertScratch[2].x = rect.width - (s_VertScratch[1].x - s_VertScratch[0].x);
                    s_VertScratch[2].y = rect.height - (s_VertScratch[1].y - s_VertScratch[0].y);
                    s_UVScratch[2] = new Vector2(inner.x, inner.w);
                    s_UVScratch[3] = new Vector2(outer.x, outer.w);
                    break;
                case MirrorTypes.Vertical:
                    s_VertScratch[2].x = rect.width - (s_VertScratch[1].x - s_VertScratch[0].x);
                    s_VertScratch[2].y = rect.height - (s_VertScratch[1].y - s_VertScratch[0].y);
                    s_UVScratch[2] = new Vector2(inner.z, inner.y);
                    s_UVScratch[3] = new Vector2(outer.z, outer.y);
                    break;
                case MirrorTypes.Quarter:
                    s_VertScratch[2].x = rect.width - (s_VertScratch[1].x - s_VertScratch[0].x);
                    s_VertScratch[2].y = rect.height - (s_VertScratch[1].y - s_VertScratch[0].y);
                    s_UVScratch[2] = new Vector2(inner.x, inner.y);
                    s_UVScratch[3] = new Vector2(outer.x, outer.y);
                    break;
                default:
                    break;
            }

            for (int i = 0; i < 4; ++i)
            {
                s_VertScratch[i].x += rect.x;
                s_VertScratch[i].y += rect.y;
            }
            toFill.Clear();

            for (int x = 0; x < 3; ++x)
            {
                int x2 = x + 1;

                for (int y = 0; y < 3; ++y)
                {
                    if (!fillCenter && x == 1 && y == 1)
                        continue;

                    int y2 = y + 1;


                    AddQuad(toFill,
                        new Vector2(s_VertScratch[x].x, s_VertScratch[y].y),
                        new Vector2(s_VertScratch[x2].x, s_VertScratch[y2].y),
                        color,
                        new Vector2(s_UVScratch[x].x, s_UVScratch[y].y),
                        new Vector2(s_UVScratch[x2].x, s_UVScratch[y2].y));
                }
            }
        }

        /// <summary>
        /// 用于绘图的图像尺寸。X=左侧，Y=底部，Z=右侧，W=顶部。
        /// </summary>
        /// <param name="shouldPreserveAspect"></param>
        /// <returns></returns>
        private Vector4 GetDrawingDimensions(bool shouldPreserveAspect)
        {
            var padding = sprite == null ? Vector4.zero : DataUtility.GetPadding(sprite);
            var size = sprite == null ? Vector2.zero : new Vector2(sprite.rect.width, sprite.rect.height);

            Rect r = GetPixelAdjustedRect();


            int spriteW = Mathf.RoundToInt(size.x);
            int spriteH = Mathf.RoundToInt(size.y);

            var v = new Vector4(
                    padding.x / spriteW,
                    padding.y / spriteH,
                    (spriteW - padding.z) / spriteW,
                    (spriteH - padding.w) / spriteH);

            if (shouldPreserveAspect && size.sqrMagnitude > 0.0f)
            {
                var spriteRatio = size.x / size.y;
                var rectRatio = r.width / r.height;

                if (spriteRatio > rectRatio)
                {
                    var oldHeight = r.height;
                    r.height = r.width * (1.0f / spriteRatio);
                    r.y += (oldHeight - r.height) * rectTransform.pivot.y;
                }
                else
                {
                    var oldWidth = r.width;
                    r.width = r.height * spriteRatio;
                    r.x += (oldWidth - r.width) * rectTransform.pivot.x;
                }
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
        /// 获取调整后的边框
        /// </summary>
        /// <param name="border"></param>
        /// <param name="adjustedRect"></param>
        /// <returns></returns>
        private Vector4 GetAdjustedBorders(Vector4 border, Rect adjustedRect)
        {
            Rect originalRect = rectTransform.rect;

            for (int axis = 0; axis <= 1; axis++)
            {
                float borderScaleRatio;

                // 调整后的矩形（为了像素的正确性进行调整）
                // 可能会比原始的矩形稍微大一些。
                // 调整边框以匹配调整后的矩形，以避免边框之间的小缝隙（案例833201）。
                if (originalRect.size[axis] != 0)
                {
                    borderScaleRatio = adjustedRect.size[axis] / originalRect.size[axis];
                    border[axis] *= borderScaleRatio;
                    border[axis + 2] *= borderScaleRatio;
                }

                // 如果矩形小于边框的总和，那么就没有足够的空间放置正常大小的边框。
                // 为了避免边框重叠产生的视觉效果，我们将边框缩小以适应。
                float combinedBorders = border[axis] + border[axis + 2];
                switch (MirrorType)
                {
                    case MirrorTypes.Horizontal:
                        if (axis == 0)
                        {
                            combinedBorders = border[axis] + border[axis];
                        }
                        break;
                    case MirrorTypes.Vertical:
                        if (axis == 1)
                        {
                            combinedBorders = border[axis] + border[axis];
                        }
                        break;
                    case MirrorTypes.Quarter:
                        combinedBorders = border[axis] + border[axis];
                        break;
                    default:
                        break;
                }
                if (adjustedRect.size[axis] < combinedBorders && combinedBorders != 0)
                {
                    borderScaleRatio = adjustedRect.size[axis] / combinedBorders;
                    border[axis] *= borderScaleRatio;
                    border[axis + 2] *= borderScaleRatio;
                }
            }
            return border;
        }

        /// <summary>
        /// 增加方形面片
        /// </summary>
        /// <param name="vertexHelper"></param>
        /// <param name="posMin"></param>
        /// <param name="posMax"></param>
        /// <param name="color"></param>
        /// <param name="uvMin"></param>
        /// <param name="uvMax"></param>
        static void AddQuad(VertexHelper vertexHelper, Vector2 posMin, Vector2 posMax, Color32 color, Vector2 uvMin, Vector2 uvMax)
        {
            //Debug.Log($"posMin:{posMin},posMax:{posMax},uvMin:{uvMin},uvMax:{uvMax}");
            int startIndex = vertexHelper.currentVertCount;

            vertexHelper.AddVert(new Vector3(posMin.x, posMin.y, 0), color, new Vector2(uvMin.x, uvMin.y));
            vertexHelper.AddVert(new Vector3(posMin.x, posMax.y, 0), color, new Vector2(uvMin.x, uvMax.y));
            vertexHelper.AddVert(new Vector3(posMax.x, posMax.y, 0), color, new Vector2(uvMax.x, uvMax.y));
            vertexHelper.AddVert(new Vector3(posMax.x, posMin.y, 0), color, new Vector2(uvMax.x, uvMin.y));

            vertexHelper.AddTriangle(startIndex, startIndex + 1, startIndex + 2);
            vertexHelper.AddTriangle(startIndex + 2, startIndex + 3, startIndex);
        }

        /// <summary>
        /// 为平铺的图像生成顶点
        /// </summary>
        /// <param name="toFill"></param>

        void GenerateTiledSprite(VertexHelper toFill)
        {
            Vector4 outer, inner, border;
            Vector2 spriteSize;

            if (sprite != null)
            {
                outer = DataUtility.GetOuterUV(sprite);
                inner = DataUtility.GetInnerUV(sprite);
                border = sprite.border;
                spriteSize = sprite.rect.size;
            }
            else
            {
                outer = Vector4.zero;
                inner = Vector4.zero;
                border = Vector4.zero;
                spriteSize = Vector2.one * 100;
            }

            Rect rect = GetPixelAdjustedRect();
            float tileWidth = (spriteSize.x - border.x - border.z) / pixelsPerUnit;
            float tileHeight = (spriteSize.y - border.y - border.w) / pixelsPerUnit;
            border = GetAdjustedBorders(border / pixelsPerUnit, rect);

            var uvMin = new Vector2(inner.x, inner.y);
            var uvMax = new Vector2(inner.z, inner.w);

            // 在相对于左下角坐标的平铺区域中，最小值到最大值的范围。
            float xMin = border.x;
            float xMax = rect.width - border.z;
            float yMin = border.y;
            float yMax = rect.height - border.w;

            toFill.Clear();
            var clipped = uvMax;

            // 如果宽度为零，我们就不能进行平铺，所以就假设它是全宽。
            if (tileWidth <= 0)
                tileWidth = xMax - xMin;

            if (tileHeight <= 0)
                tileHeight = yMax - yMin;

            if (sprite != null && (hasBorder || sprite.packed || sprite.texture.wrapMode != TextureWrapMode.Repeat))
            {
                // 精灵有边框，或者不在重复模式，或者因为打包不能被重复。
                // 我们不能使用纹理平铺，所以我们将生成一个四边形的网格来平铺纹理。

                // 评估我们将生成多少个顶点。将这个数字限制在一个合理的范围内，
                // 尤其是因为网格不能有超过65000个顶点。

                long nTilesW = 0;
                long nTilesH = 0;
                if (fillCenter)
                {
                    nTilesW = (long)Mathf.Ceil((xMax - xMin) / tileWidth);
                    nTilesH = (long)Mathf.Ceil((yMax - yMin) / tileHeight);

                    double nVertices = 0;
                    if (hasBorder)
                    {
                        nVertices = (nTilesW + 2.0) * (nTilesH + 2.0) * 4.0; // 4 vertices per tile
                    }
                    else
                    {
                        nVertices = nTilesW * nTilesH * 4.0; // 4 vertices per tile
                    }

                    if (nVertices > 65000.0)
                    {
                        //Debug.LogError("Too many sprite tiles on Image \"" + name + "\". The tile size will be increased. To remove the limit on the number of tiles, convert the Sprite to an Advanced texture, remove the borders, clear the Packing tag and set the Wrap mode to Repeat.", this);

                        double maxTiles = 65000.0 / 4.0; // Max number of vertices is 65000; 4 vertices per tile.
                        double imageRatio;
                        if (hasBorder)
                        {
                            imageRatio = (nTilesW + 2.0) / (nTilesH + 2.0);
                        }
                        else
                        {
                            imageRatio = (double)nTilesW / nTilesH;
                        }

                        float targetTilesW = Mathf.Sqrt((float)(maxTiles / imageRatio));
                        float targetTilesH = (float)(targetTilesW * imageRatio);
                        if (hasBorder)
                        {
                            targetTilesW -= 2;
                            targetTilesH -= 2;
                        }

                        nTilesW = (long)Mathf.Floor(targetTilesW);
                        nTilesH = (long)Mathf.Floor(targetTilesH);
                        tileWidth = (xMax - xMin) / nTilesW;
                        tileHeight = (yMax - yMin) / nTilesH;
                    }
                }
                else
                {
                    if (hasBorder)
                    {
                        // 边框上的纹理只在一个方向上重复
                        nTilesW = (long)Mathf.Ceil((xMax - xMin) / tileWidth);
                        nTilesH = (long)Mathf.Ceil((yMax - yMin) / tileHeight);
                        double nVertices = (nTilesH + nTilesW + 2.0 /*corners*/) * 2.0 /*sides*/ * 4.0 /*vertices per tile*/;
                        if (nVertices > 65000.0)
                        {
                            //Debug.LogError("Too many sprite tiles on Image \"" + name + "\". The tile size will be increased. To remove the limit on the number of tiles, convert the Sprite to an Advanced texture, remove the borders, clear the Packing tag and set the Wrap mode to Repeat.", this);

                            double maxTiles = 65000.0 / 4.0; // 最大顶点数为65000；每个瓦片有4个顶点。
                            double imageRatio = (double)nTilesW / nTilesH;
                            float targetTilesW = (float)((maxTiles - 4 /*corners*/) / (2 * (1.0 + imageRatio)));
                            float targetTilesH = (float)(targetTilesW * imageRatio);

                            nTilesW = (long)Mathf.Floor(targetTilesW);
                            nTilesH = (long)Mathf.Floor(targetTilesH);
                            tileWidth = (xMax - xMin) / nTilesW;
                            tileHeight = (yMax - yMin) / nTilesH;
                        }
                    }
                    else
                    {
                        nTilesH = nTilesW = 0;
                    }
                }

                if (fillCenter)
                {
                    // TODO: 我们可以在四边形之间共享顶点。如果实现了顶点共享，相应地更新顶点数量的计算。
                    for (long j = 0; j < nTilesH; j++)
                    {
                        float y1 = yMin + j * tileHeight;
                        float y2 = yMin + (j + 1) * tileHeight;
                        float y2e = y2;
                        if (y2 > yMax)
                        {
                            clipped.y = uvMin.y + (uvMax.y - uvMin.y) * (yMax - y1) / (y2 - y1);
                            y2 = yMax;
                        }
                        clipped.x = uvMax.x;
                        for (long i = 0; i < nTilesW; i++)
                        {
                            float x1 = xMin + i * tileWidth;
                            float x2 = xMin + (i + 1) * tileWidth;
                            float x2e = x2;
                            if (x2 > xMax)
                            {
                                clipped.x = uvMin.x + (uvMax.x - uvMin.x) * (xMax - x1) / (x2 - x1);
                                x2 = xMax;
                            }

                            var uvMin1 = uvMin;
                            var clipped1 = clipped;
                            //Debug.Log("i::" + i + "  j:::" + j);
                            switch (MirrorType)
                            {
                                case MirrorTypes.Horizontal:
                                    if (i % 2 == 1)
                                    {
                                        float offsetX = 0;
                                        if (x2e > xMax)
                                        {
                                            offsetX = uvMax.x - (uvMax.x - uvMin.x) * (xMax - x1) / (x2e - x1);
                                        }
                                        uvMin1 = new Vector2(uvMax.x, uvMin.y);
                                        //clipped1 = new Vector2(uvMin.x, clipped.y);
                                        clipped1 = new Vector2(offsetX, clipped.y);
                                    }
                                    break;
                                case MirrorTypes.Vertical:
                                    if (j % 2 == 1)
                                    {
                                        float offsetY = 0;
                                        if (y2e > yMax)
                                        {
                                            offsetY = uvMax.y - (uvMax.y - uvMin.y) * (yMax - y1) / (y2e - y1);
                                        }
                                        //uvMin1 = new Vector2(uvMin.x, clipped.y);
                                        uvMin1 = new Vector2(uvMin.x, uvMax.y);
                                        //clipped1 = new Vector2(clipped.x, uvMin.y);
                                        clipped1 = new Vector2(clipped.x, offsetY);

                                    }
                                    break;
                                case MirrorTypes.Quarter:
                                    if (j % 2 == 1 && i % 2 == 1)
                                    {

                                        float offsetX = uvMin.x;
                                        if (x2e > xMax)
                                        {
                                            offsetX = uvMax.x - (uvMax.x - uvMin.x) * (xMax - x1) / (x2e - x1);
                                        }

                                        float offsetY = uvMin.y;
                                        if (y2e > yMax)
                                        {
                                            offsetY = uvMax.y - (uvMax.y - uvMin.y) * (yMax - y1) / (y2e - y1);
                                        }

                                        clipped1 = new Vector2(offsetX, offsetY);
                                        uvMin1 = uvMax;
                                    }
                                    else if (j % 2 == 1)
                                    {
                                        float offsetY = 0;
                                        if (y2e > yMax)
                                        {
                                            offsetY = uvMax.y - (uvMax.y - uvMin.y) * (yMax - y1) / (y2e - y1);
                                        }
                                        //uvMin1 = new Vector2(uvMin.x, clipped.y);
                                        uvMin1 = new Vector2(uvMin.x, uvMax.y);
                                        //clipped1 = new Vector2(clipped.x, uvMin.y);
                                        clipped1 = new Vector2(clipped.x, offsetY);
                                    }
                                    else if (i % 2 == 1)
                                    {
                                        float offsetX = 0;
                                        if (x2e > xMax)
                                        {
                                            offsetX = uvMax.x - (uvMax.x - uvMin.x) * (xMax - x1) / (x2e - x1);
                                        }
                                        uvMin1 = new Vector2(uvMax.x, uvMin.y);
                                        //clipped1 = new Vector2(uvMin.x, clipped.y);
                                        clipped1 = new Vector2(offsetX, clipped.y);
                                    }
                                    break;
                                default:
                                    break;
                            }


                            AddQuad(toFill, new Vector2(x1, y1) + rect.position, new Vector2(x2, y2) + rect.position, color, uvMin1, clipped1);
                        }
                    }
                }
                if (hasBorder)
                {
                    clipped = uvMax;
                    for (long j = 0; j < nTilesH; j++)
                    {
                        float y1 = yMin + j * tileHeight;
                        float y2 = yMin + (j + 1) * tileHeight;
                        if (y2 > yMax)
                        {
                            clipped.y = uvMin.y + (uvMax.y - uvMin.y) * (yMax - y1) / (y2 - y1);
                            y2 = yMax;
                        }
                        AddQuad(toFill,
                            new Vector2(0, y1) + rect.position,
                            new Vector2(xMin, y2) + rect.position,
                            color,
                            new Vector2(outer.x, uvMin.y),
                            new Vector2(uvMin.x, clipped.y));
                        AddQuad(toFill,
                            new Vector2(xMax, y1) + rect.position,
                            new Vector2(rect.width, y2) + rect.position,
                            color,
                            new Vector2(uvMax.x, uvMin.y),
                            new Vector2(outer.z, clipped.y));
                    }

                    // 底部和顶部的平铺边框
                    clipped = uvMax;
                    for (long i = 0; i < nTilesW; i++)
                    {
                        float x1 = xMin + i * tileWidth;
                        float x2 = xMin + (i + 1) * tileWidth;
                        if (x2 > xMax)
                        {
                            clipped.x = uvMin.x + (uvMax.x - uvMin.x) * (xMax - x1) / (x2 - x1);
                            x2 = xMax;
                        }
                        AddQuad(toFill,
                            new Vector2(x1, 0) + rect.position,
                            new Vector2(x2, yMin) + rect.position,
                            color,
                            new Vector2(uvMin.x, outer.y),
                            new Vector2(clipped.x, uvMin.y));
                        AddQuad(toFill,
                            new Vector2(x1, yMax) + rect.position,
                            new Vector2(x2, rect.height) + rect.position,
                            color,
                            new Vector2(uvMin.x, uvMax.y),
                            new Vector2(clipped.x, outer.w));
                    }

                    // Corners
                    AddQuad(toFill,
                        new Vector2(0, 0) + rect.position,
                        new Vector2(xMin, yMin) + rect.position,
                        color,
                        new Vector2(outer.x, outer.y),
                        new Vector2(uvMin.x, uvMin.y));
                    AddQuad(toFill,
                        new Vector2(xMax, 0) + rect.position,
                        new Vector2(rect.width, yMin) + rect.position,
                        color,
                        new Vector2(uvMax.x, outer.y),
                        new Vector2(outer.z, uvMin.y));
                    AddQuad(toFill,
                        new Vector2(0, yMax) + rect.position,
                        new Vector2(xMin, rect.height) + rect.position,
                        color,
                        new Vector2(outer.x, uvMax.y),
                        new Vector2(uvMin.x, outer.w));
                    AddQuad(toFill,
                        new Vector2(xMax, yMax) + rect.position,
                        new Vector2(rect.width, rect.height) + rect.position,
                        color,
                        new Vector2(uvMax.x, uvMax.y),
                        new Vector2(outer.z, outer.w));
                }
            }
            else
            {
                // 纹理没有边界，处于重复模式且未打包。使用纹理平铺
                Vector2 uvScale = new Vector2((xMax - xMin) / tileWidth, (yMax - yMin) / tileHeight);

                if (fillCenter)
                {
                    AddQuad(toFill, new Vector2(xMin, yMin) + rect.position, new Vector2(xMax, yMax) + rect.position, color, Vector2.Scale(uvMin, uvScale), Vector2.Scale(uvMax, uvScale));
                }
            }
        }
    }
}