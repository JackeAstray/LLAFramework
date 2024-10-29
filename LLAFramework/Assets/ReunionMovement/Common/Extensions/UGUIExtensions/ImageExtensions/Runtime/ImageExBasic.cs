using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameLogic.UI.ImageExtensions
{
    [AddComponentMenu("UI/ReunionMovement/ImageExBasic")]
    public class ImageExBasic : Image
    {
        #region 序列化字段

        [SerializeField] private DrawShape drawShape = DrawShape.None;
        [SerializeField] private Type imageType = Type.Simple;                                                                           // Mapping in Vertex Stream
        [SerializeField] private float strokeWidth;                               // MapTo -> UV2.x
        [SerializeField] private float falloffDistance = 0.5f;                    // MapTo -> UV2.y
        [SerializeField] private float outlineWidth;                              // MapTo -> Normal.x
        [SerializeField] private Color outlineColor = Color.black;                // MapTo -> Tangent.x, Tangent.y, Tangent.z, Tangent.w
        [SerializeField] private float shapeRotation;                             // MapTo -> UV3.x Compressed
        [SerializeField] private bool constrainRotation = true;                   // MapTo -> UV3.x Compressed
        [SerializeField] private bool flipHorizontal;                             // MapTo -> UV3.x Compressed
        [SerializeField] private bool flipVertical;                               // MapTo -> UV3.x Compressed
        [SerializeField] private CornerStyleType cornerStyle;                     // MapTo -> UV3.y
        [SerializeField] private float alphaThreshold = 0f;

        [SerializeField] private Vector4 rectangleCornerRadius;                   // MapTo -> Normal.y, Normal.z compressed
        [SerializeField] private Vector3 triangleCornerRadius;                    // MapTo -> Normal.y, Normal.z compressed

#pragma warning disable
        [SerializeField] private bool triangleUniformCornerRadius = true;
        [SerializeField] private bool rectangleUniformCornerRadius = true;
#pragma warning restore

        [SerializeField] private float circleRadius;                              // MapTo -> Normal.y
        [SerializeField] private bool circleFitToRect = true;                     // MapTo -> Normal.z

        [SerializeField] private int nStarPolygonSideCount = 3;                   // MapTo -> Normal.y compressed
        [SerializeField] private float nStarPolygonInset = 2f;                    // MapTo -> Normal.y compressed
        [SerializeField] private float nStarPolygonCornerRadius;                  // MapTo -> Normal.z
        #endregion

        #region 公共访问器

        public DrawShape Shape
        {
            get => drawShape;
            set
            {
                drawShape = value;
                m_Material = null;
                base.SetMaterialDirty();
                base.SetVerticesDirty();
            }
        }
        public float StrokeWidth
        {
            get => strokeWidth;
            set
            {
                Vector2 size = GetPixelAdjustedRect().size;
                strokeWidth = Mathf.Clamp(value, 0, Mathf.Min(size.x, size.y) * 0.5f);
                base.SetVerticesDirty();
            }
        }
        public float FallOffDistance
        {
            get => falloffDistance;
            set
            {
                falloffDistance = Mathf.Max(0, value);
                base.SetVerticesDirty();
            }
        }
        public float OutlineWidth
        {
            get => outlineWidth;
            set
            {
                outlineWidth = Mathf.Max(0, value);
                base.SetVerticesDirty();
            }
        }
        public Color OutlineColor
        {
            get => outlineColor;
            set
            {
                outlineColor = value;
                base.SetVerticesDirty();
            }
        }

        public float ShapeRotation
        {
            get => shapeRotation;
            set
            {
                shapeRotation = value % 360;
                ConstrainRotationValue();
                base.SetVerticesDirty();
            }
        }
        public bool ConstrainRotation
        {
            get => constrainRotation;
            set
            {
                constrainRotation = value;
                ConstrainRotationValue();
                base.SetVerticesDirty();
            }
        }
        public bool FlipHorizontal
        {
            get => flipHorizontal;
            set
            {
                flipHorizontal = value;
                base.SetVerticesDirty();
            }
        }

        /// <summary>
        /// 翻转垂直
        /// </summary>
        public bool FlipVertical
        {
            get => flipVertical;
            set
            {
                flipVertical = value;
                base.SetVerticesDirty();
            }
        }

        /// <summary>
        /// Alpha阈值
        /// </summary>
        public float AlphaThreshold
        {
            get { return alphaThreshold; }
            set
            {
                alphaThreshold = value;
                alphaHitTestMinimumThreshold = alphaThreshold;
                base.SetMaterialDirty();
            }
        }

        /// <summary>
        /// 图像的类型。仅支持两种类型。简单而充实。
        /// 默认值和回退值为“简单”。
        /// </summary>
        public new Type type
        {
            get => imageType;
            set
            {
                if (imageType != value)
                {
                    switch (value)
                    {
                        case Type.Simple:
                        case Type.Filled:
                            if (sprite) imageType = value;
                            break;
                        case Type.Tiled:
                        case Type.Sliced:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(value.ToString(), value, null);
                    }
                }
                if (base.type != imageType) base.type = imageType;
                base.SetAllDirty();
            }
        }

        public CornerStyleType CornerStyle
        {
            get => cornerStyle;
            set
            {
                cornerStyle = value;
                base.SetVerticesDirty();
            }
        }
        public Vector3 TriangleCornerRadius
        {
            get => triangleCornerRadius;
            set
            {
                Vector2 size = GetPixelAdjustedRect().size;

                float zMax = size.x * 0.5f;
                triangleCornerRadius.z = Mathf.Clamp(value.z, 0, zMax);
                float hMax = Mathf.Min(size.x, size.y) * 0.3f;

                triangleCornerRadius.x = Mathf.Clamp(value.x, 0, hMax);
                triangleCornerRadius.y = Mathf.Clamp(value.y, 0, hMax);

                base.SetVerticesDirty();
            }
        }
        public Vector4 RectangleCornerRadius
        {
            get => rectangleCornerRadius;
            set
            {
                rectangleCornerRadius = value;
                base.SetVerticesDirty();
            }
        }
        public float CircleRadius
        {
            get => circleRadius;
            set
            {
                circleRadius = Mathf.Clamp(value, 0, GetMinSize());
                base.SetVerticesDirty();
            }
        }
        public bool CircleFitToRect
        {
            get => circleFitToRect;
            set
            {
                circleFitToRect = value;
                base.SetVerticesDirty();
            }
        }
        public float NStarPolygonCornerRadius
        {
            get => nStarPolygonCornerRadius;
            set
            {
                float halfHeight = GetPixelAdjustedRect().height * 0.5f;
                nStarPolygonCornerRadius = Mathf.Clamp(value, nStarPolygonSideCount == 2 ? 0.1f : 0f, halfHeight);
                base.SetVerticesDirty();
            }
        }
        public float NStarPolygonInset
        {
            get => nStarPolygonInset;
            set
            {
                nStarPolygonInset = Mathf.Clamp(value, 2f, nStarPolygonSideCount);
                base.SetVerticesDirty();
            }
        }
        public int NStarPolygonSideCount
        {
            get => nStarPolygonSideCount;
            set
            {
                nStarPolygonSideCount = Mathf.Clamp(value, 2, 10);
                base.SetVerticesDirty();
            }
        }

        #endregion

        public override Material material
        {
            get
            {
                switch (drawShape)
                {
                    case DrawShape.None:
                        return Canvas.GetDefaultCanvasMaterial();
                    case DrawShape.Circle:
                    case DrawShape.Triangle:
                    case DrawShape.Rectangle:
                        return Materials.GetMaterial((int)drawShape - 1, strokeWidth > 0f, outlineWidth > 0f);
                    case DrawShape.Pentagon:
                    case DrawShape.Hexagon:
                    case DrawShape.NStarPolygon:
                    case DrawShape.Heart:
                    case DrawShape.BlobbyCross:
                    case DrawShape.Squircle:
                        return Materials.GetMaterial(3, strokeWidth > 0f, outlineWidth > 0f);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            set => Debug.LogWarning("设置ImageExBasic的材质无效");
        }

        public override float preferredWidth => sprite == ImageUtility.EmptySprite ? 0 : base.preferredWidth;
        public override float preferredHeight => sprite == ImageUtility.EmptySprite ? 0 : base.preferredHeight;


        protected override void OnEnable()
        {
            base.OnEnable();
            ImageUtility.FixAdditionalShaderChannelsInCanvas(canvas);
            if (sprite == null) sprite = ImageUtility.EmptySprite;
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            Shape = drawShape;
            if (sprite == null) sprite = ImageUtility.EmptySprite;
            type = imageType;
            StrokeWidth = strokeWidth;
            FallOffDistance = falloffDistance;
            OutlineWidth = outlineWidth;
            OutlineColor = outlineColor;
            ShapeRotation = shapeRotation;
            ConstrainRotation = constrainRotation;
            FlipHorizontal = flipHorizontal;
            FlipVertical = flipVertical;
            AlphaThreshold = alphaThreshold;
            CornerStyle = cornerStyle;
        }
#endif

        /// <summary>
        /// 获取最小尺寸的一半
        /// </summary>
        /// <returns></returns>
        private float GetMinSizeHalf()
        {
            return GetMinSize() * 0.5f;
        }

        /// <summary>
        /// 获取最小尺寸
        /// </summary>
        /// <returns></returns>
        private float GetMinSize()
        {
            Vector2 size = GetPixelAdjustedRect().size;
            return Mathf.Min(size.x, size.y);
        }

        /// <summary>
        /// 限制旋转值
        /// </summary>
        private void ConstrainRotationValue()
        {
            if (!constrainRotation) return;
            float finalRotation = shapeRotation - (shapeRotation % 90);
            if (Mathf.Abs(finalRotation) >= 360) finalRotation = 0;
            shapeRotation = finalRotation;
        }

        protected override void OnTransformParentChanged()
        {
            base.OnTransformParentChanged();
            ImageUtility.FixAdditionalShaderChannelsInCanvas(canvas);
            base.SetVerticesDirty();
        }

        /// <summary>
        /// 创建顶点流
        /// </summary>
        /// <returns></returns>
        private VertexDataStream CreateVertexStream()
        {
            VertexDataStream stream = new VertexDataStream();
            RectTransform rectT = rectTransform;
            stream.RectTransform = rectT;
            Rect r = GetPixelAdjustedRect();
            stream.Uv1 = new Vector2(r.width + falloffDistance, r.height + falloffDistance);

            float packedRotData = PackRotationData(shapeRotation,
                                                   constrainRotation,
                                                   flipHorizontal,
                                                   flipVertical);

            stream.Uv3 = new Vector2(packedRotData, (float)cornerStyle);

            stream.Tangent = QualitySettings.activeColorSpace == ColorSpace.Linear ? outlineColor.linear : outlineColor;
            Vector3 normal = new Vector3();
            normal.x = outlineWidth;
            normal.y = strokeWidth;
            normal.z = falloffDistance;

            Vector4 data;
            Vector2 shapeProps;
            switch (drawShape)
            {
                case DrawShape.Circle:
                    shapeProps = new Vector2(circleRadius, circleFitToRect ? 1 : 0);
                    break;
                case DrawShape.Triangle:
                    data = triangleCornerRadius;
                    data = data / Mathf.Min(r.width, r.height);
                    shapeProps = ImageUtility.Encode_0_1_16(data);
                    break;
                case DrawShape.Rectangle:
                    data = FixRadius(rectangleCornerRadius);
                    data = data / Mathf.Min(r.width, r.height);
                    shapeProps = ImageUtility.Encode_0_1_16(data);
                    break;
                case DrawShape.NStarPolygon:
                    data = new Vector4(nStarPolygonSideCount, nStarPolygonCornerRadius, nStarPolygonInset);
                    data = data / Mathf.Min(r.width, r.height);
                    shapeProps = ImageUtility.Encode_0_1_16(data);
                    break;
                default:
                    shapeProps = Vector2.zero;
                    break;
            }

            stream.Uv2 = shapeProps;

            stream.Normal = normal;
            return stream;
        }

        /// <summary>
        /// 打包旋转数据
        /// </summary>
        /// <param name="rotation"></param>
        /// <param name="constrainRotation"></param>
        /// <param name="flipH"></param>
        /// <param name="flipV"></param>
        /// <returns></returns>
        private float PackRotationData(float rotation, bool constrainRotation, bool flipH, bool flipV)
        {
            int c = constrainRotation ? 1 : 0;
            c += flipH ? 10 : 0;
            c += flipV ? 100 : 0;
            float cr = rotation % 360f;
            float sign = cr >= 0 ? 1 : -1;
            cr = Mathf.Abs(cr) / 360f;
            cr = (cr + c) * sign;
            return cr;
        }

        /// <summary>
        /// 解包旋转
        /// </summary>
        /// <param name="f"></param>
        void UnPackRotation(float f)
        {
            float r = 0, x = 0, y = 0, z = 0;

            float sign = f >= 0.0f ? 1 : -1;
            f = Mathf.Abs(f);
            r = fract(f) * 360f * sign;

            f = Mathf.Floor(f);
            float p = f / 100f;
            z = Mathf.Floor(p);
            p = fract(p) * 10f;
            y = Mathf.Floor(p);
            p = fract(p) * 10f;
            x = Mathf.Round(p);

            // Debug.Log($"Rotation: {r}, X: {x}, Y: {y}, Z: {z}");
            float fract(float val)
            {
                val = Mathf.Abs(val);
                float ret = val - Mathf.Floor(val);
                return ret;
            }
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            base.SetVerticesDirty();
        }

        /// <summary>
        /// 修复半径
        /// </summary>
        /// <param name="radius"></param>
        /// <returns></returns>
        private Vector4 FixRadius(Vector4 radius)
        {
            Rect rect = rectTransform.rect;

            radius = Vector4.Max(radius, Vector4.zero);
            radius = Vector4.Min(radius, Vector4.one * Mathf.Min(rect.width, rect.height));
            float scaleFactor =
                Mathf.Min(
                    Mathf.Min(
                        Mathf.Min(
                            Mathf.Min(
                                rect.width / (radius.x + radius.y),
                                rect.width / (radius.z + radius.w)),
                            rect.height / (radius.x + radius.w)),
                        rect.height / (radius.z + radius.y)),
                    1f);
            return radius * scaleFactor;
        }

        /// <summary>
        /// 重写以填充网格
        /// </summary>
        /// <param name="toFill"></param>
        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            base.OnPopulateMesh(toFill);

            VertexDataStream stream = CreateVertexStream();

            UIVertex uiVert = new UIVertex();

            for (int i = 0; i < toFill.currentVertCount; i++)
            {
                toFill.PopulateUIVertex(ref uiVert, i);

                //uiVert.position += ((Vector3)uiVert.uv0 - new Vector3(0.5f, 0.5f)) * falloffDistance;
                uiVert.uv1 = stream.Uv1;
                uiVert.uv2 = stream.Uv2;
                uiVert.uv3 = stream.Uv3;
                uiVert.normal = stream.Normal;
                uiVert.tangent = stream.Tangent;

                toFill.SetUIVertex(uiVert, i);
            }
        }

#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();
            if (sprite == null) sprite = ImageUtility.EmptySprite;

        }
#else
        void Reset() {
            if (sprite == null) sprite = ImageUtility.EmptySprite;
        }
#endif
    }
}