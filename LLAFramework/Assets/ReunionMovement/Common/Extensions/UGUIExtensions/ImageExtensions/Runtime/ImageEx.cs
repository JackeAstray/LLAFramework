using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.ImageExtensions;
using Unity.VisualScripting;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LLAFramework.UI.ImageExtensions
{
    [AddComponentMenu("UI/ReunionMovement/ImageEx")]
    public class ImageEx : Image
    {
        #region 常量
        public const string shaderName = "ReunionMovement/UI/Procedural Image";
        #endregion

        #region 序列化字段

        [SerializeField] private DrawShape drawShape = DrawShape.None;
        [SerializeField] private Type imageType = Type.Simple;
        [SerializeField] private MaterialMode materialMode;

        [SerializeField] private float strokeWidth;

        [SerializeField] private float outlineWidth;
        [SerializeField] private Color outlineColor = Color.black;
        [SerializeField] private float customTime;
        [SerializeField] private int enableDashedOutline;

        [SerializeField] private float falloffDistance = 0.5f;
        [SerializeField] private bool constrainRotation = true;
        [SerializeField] private float shapeRotation;
        [SerializeField] private bool flipHorizontal;
        [SerializeField] private bool flipVertical;
        [SerializeField] private float alphaThreshold = 0f;

        [SerializeField] private TriangleImg triangle = new TriangleImg();
        [SerializeField] private RectangleImg rectangle = new RectangleImg();
        [SerializeField] private CircleImg circle = new CircleImg();
        [SerializeField] private PentagonImg pentagon = new PentagonImg();
        [SerializeField] private HexagonImg hexagon = new HexagonImg();
        [SerializeField] private NStarPolygonImg nStarPolygon = new NStarPolygonImg();
        [SerializeField] private HeartImg heart = new HeartImg();
        [SerializeField] private BlobbyCrossImg blobbyCross = new BlobbyCrossImg();
        [SerializeField] private SquircleImg squircle = new SquircleImg();
        [SerializeField] private NTriangleRoundedImg nTriangleRounded = new NTriangleRoundedImg();

        [SerializeField] private GradientEffect gradientEffect = new GradientEffect();
        #endregion

        #region Material PropertyIds

        private static readonly int pixelWorldScale_Sp = Shader.PropertyToID("_PixelWorldScale");
        private static readonly int drawShape_Sp = Shader.PropertyToID("_DrawShape");
        private static readonly int strokeWidth_Sp = Shader.PropertyToID("_StrokeWidth");

        private static readonly int outlineWidth_Sp = Shader.PropertyToID("_OutlineWidth");
        private static readonly int outlineColor_Sp = Shader.PropertyToID("_OutlineColor");
        private static readonly int enableDashedOutline_Sp = Shader.PropertyToID("_EnableDashedOutline");
        private static readonly int customTime_Sp = Shader.PropertyToID("_CustomTime");

        private static readonly int falloffDistance_Sp = Shader.PropertyToID("_FalloffDistance");
        private static readonly int shapeRotation_Sp = Shader.PropertyToID("_ShapeRotation");
        private static readonly int constrainedRotation_Sp = Shader.PropertyToID("_ConstrainRotation");
        private static readonly int flipHorizontal_Sp = Shader.PropertyToID("_FlipHorizontal");
        private static readonly int flipVertical_Sp = Shader.PropertyToID("_FlipVertical");
        #endregion

        #region 公共属性

        #region 绘图设置

        /// <summary>
        /// 要绘制形状的类型
        /// </summary>
        public DrawShape DrawShape
        {
            get => drawShape;
            set
            {
                drawShape = value;
                if (material == m_Material)
                {
                    m_Material.SetInt(drawShape_Sp, (int)drawShape);
                }

                base.SetMaterialDirty();
                base.SetVerticesDirty();
            }
        }

        /// <summary>
        /// 绘制形状的线条宽度。0不是线条
        /// </summary>
        public float StrokeWidth
        {
            get => strokeWidth;
            set
            {
                strokeWidth = value;
                strokeWidth = strokeWidth < 0 ? 0 : strokeWidth;
                if (material == m_Material)
                {
                    m_Material.SetFloat(strokeWidth_Sp, strokeWidth);
                }

                base.SetMaterialDirty();
            }
        }

        /// <summary>
        /// 绘制形状的轮廓宽度。0不是轮廓。
        /// </summary>
        public float OutlineWidth
        {
            get => outlineWidth;
            set
            {
                outlineWidth = value;
                outlineWidth = outlineWidth < 0 ? 0 : outlineWidth;
                if (m_Material == material)
                {
                    m_Material.SetFloat(outlineWidth_Sp, outlineWidth);
                }

                base.SetMaterialDirty();
            }
        }

        /// <summary>
        /// 轮廓的颜色。如果“轮廓宽度”的值为0，则没有效果
        /// </summary>
        public Color OutlineColor
        {
            get => outlineColor;
            set
            {
                outlineColor = value;
                if (m_Material == material)
                {
                    m_Material.SetColor(outlineColor_Sp, outlineColor);
                }

                base.SetMaterialDirty();
            }
        }

        public int EnableDashedOutline
        {
            get => enableDashedOutline;
            set
            {
                enableDashedOutline = value;
                if (m_Material == material)
                {
                    m_Material.SetInt(enableDashedOutline_Sp, enableDashedOutline);
                }
                base.SetMaterialDirty();
            }
        }

        public float CustomTime
        {
            get => customTime;
            set
            {
                customTime = value;
                if (m_Material == material)
                {
                    m_Material.SetFloat(customTime_Sp, customTime);
                }
                base.SetMaterialDirty();
            }
        }

        /// <summary>
        /// 形状的边缘衰减距离
        /// </summary>
        public float FalloffDistance
        {
            get { return falloffDistance; }
            set
            {
                falloffDistance = Mathf.Max(value, 0f);
                if (material == m_Material)
                {
                    m_Material.SetFloat(falloffDistance_Sp, falloffDistance);
                }

                base.SetMaterialDirty();
            }
        }

        /// <summary>
        /// 如果设置为true，则将旋转约束为0、90、270度角。
        /// 但是形状的宽度和高度根据需要进行更换以避免剪裁。
        /// 如果设置为false，则任何形状都可以以任意角度旋转，但通常会导致形状的剪裁。
        /// </summary>
        public bool ConstrainRotation
        {
            get { return constrainRotation; }
            set
            {
                constrainRotation = value;

                if (m_Material == material)
                {
                    m_Material.SetInt(constrainedRotation_Sp, value ? 1 : 0);
                }
                if (value)
                {
                    shapeRotation = ConstrainRotationValue(shapeRotation);
                }

                base.SetVerticesDirty();
                base.SetMaterialDirty();
            }
        }

        private float ConstrainRotationValue(float val)
        {
            float finalRotation = val - val % 90;
            if (Mathf.Abs(finalRotation) >= 360) finalRotation = 0;
            return finalRotation;
        }

        /// <summary>
        /// 形状的旋转
        /// </summary>
        public float ShapeRotation
        {
            get { return shapeRotation; }
            set
            {
                shapeRotation = constrainRotation ? ConstrainRotationValue(value) : value;
                if (m_Material == material)
                {
                    m_Material.SetFloat(shapeRotation_Sp, shapeRotation);
                }

                base.SetMaterialDirty();
            }
        }

        /// <summary>
        /// 水平翻转形状
        /// </summary>
        public bool FlipHorizontal
        {
            get { return flipHorizontal; }
            set
            {
                flipHorizontal = value;
                if (m_Material == material)
                {
                    m_Material.SetInt(flipHorizontal_Sp, flipHorizontal ? 1 : 0);
                }

                base.SetMaterialDirty();
            }
        }

        /// <summary>
        /// 垂直翻转形状
        /// </summary>
        public bool FlipVertical
        {
            get { return flipVertical; }
            set
            {
                flipVertical = value;
                if (m_Material == material)
                {
                    m_Material.SetInt(flipVertical_Sp, flipVertical ? 1 : 0);
                }

                base.SetMaterialDirty();
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
            }
        }

        /// <summary>
        /// Defines what material type of use to render the shape. Dynamic or Shared.
        /// Default is Dynamic and will issue one draw call per image object. If set to shared, assigned
        /// material in the material slot will be used to render the image. It will fallback to dynamic
        /// if no material in the material slot is assigned
        /// </summary>
        public MaterialMode MaterialMode
        {
            get { return materialMode; }
            set
            {
                if (materialMode == value) return;
                materialMode = value;
                InitializeComponents();
                if (material == m_Material)
                {
                    InitValuesFromSharedMaterial();
#if UNITY_EDITOR
                    parseAgainOnValidate = true;
#endif
                }

                base.SetMaterialDirty();
            }
        }

        /// <summary>
        /// 用于渲染形状的共享材质。材质必须使用“ReunionMovement/UI/Procedural Image”着色器
        /// </summary>
        public override Material material
        {
            get
            {
                if (m_Material && materialMode == MaterialMode.Shared)
                {
                    return m_Material;
                }

                return DynamicMaterial;
            }
            set
            {
                m_Material = value;

                if (m_Material && materialMode == MaterialMode.Shared && m_Material.shader.name == shaderName)
                {
                    InitValuesFromSharedMaterial();
#if UNITY_EDITOR
                    parseAgainOnValidate = true;
#endif
                }

                InitializeComponents();
                base.SetMaterialDirty();
            }
        }

        /// <summary>
        /// 图像的类型。仅支持两种类型。简单和填充。
        /// 默认值和回退值为“简单”。
        /// </summary>
        public new Type type
        {
            get => imageType;
            set
            {
                if (imageType == value) return;
                switch (value)
                {
                    case Type.Simple:
                    case Type.Filled:
                        imageType = value;
                        break;
                    case Type.Tiled:
                    case Type.Sliced:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(value.ToString(), value, null);
                }

                base.type = imageType;
            }
        }

        #endregion

        public TriangleImg Triangle
        {
            get => triangle;
            set
            {
                triangle = value;
                SetMaterialDirty();
            }
        }

        public RectangleImg Rectangle
        {
            get => rectangle;
            set
            {
                rectangle = value;
                SetMaterialDirty();
            }
        }

        public CircleImg Circle
        {
            get => circle;
            set
            {
                circle = value;
                SetMaterialDirty();
            }
        }

        public PentagonImg Pentagon
        {
            get => pentagon;
            set
            {
                pentagon = value;
                SetMaterialDirty();
            }
        }

        public HexagonImg Hexagon
        {
            get => hexagon;
            set
            {
                hexagon = value;
                SetMaterialDirty();
            }
        }

        public NStarPolygonImg NStarPolygon
        {
            get => nStarPolygon;
            set
            {
                nStarPolygon = value;
                SetMaterialDirty();
            }
        }

        public HeartImg Heart
        {
            get => heart;
            set
            {
                heart = value;
                SetMaterialDirty();
            }
        }

        public BlobbyCrossImg BlobbyCross
        {
            get => blobbyCross;
            set
            {
                blobbyCross = value;
                SetMaterialDirty();
            }
        }

        public SquircleImg Squircle
        {
            get => squircle;
            set
            {
                squircle = value;
                SetMaterialDirty();
            }
        }

        public NTriangleRoundedImg NTriangleRounded
        {
            get => nTriangleRounded;
            set
            {
                nTriangleRounded = value;
                SetMaterialDirty();
            }
        }

        public GradientEffect GradientEffect
        {
            get => gradientEffect;
            set
            {
                gradientEffect = value;
                SetMaterialDirty();
            }
        }

        #endregion

        #region 私有变量

        private Material dynamicMaterial;

        private Material DynamicMaterial
        {
            get
            {
                if (dynamicMaterial == null)
                {
                    dynamicMaterial = new Material(Shader.Find(shaderName));
                    dynamicMaterial.name += " [Dynamic]";
                }

                return dynamicMaterial;
            }
        }

#if UNITY_EDITOR
        private bool parseAgainOnValidate;
#endif

        private Sprite ActiveSprite
        {
            get
            {
                Sprite overrideSprite1 = overrideSprite;
                return overrideSprite1 != null ? overrideSprite1 : sprite;
            }
        }

        #endregion

#if UNITY_EDITOR
        public void UpdateSerializedValuesFromSharedMaterial()
        {
            if (m_Material && MaterialMode == MaterialMode.Shared)
            {
                InitValuesFromSharedMaterial();
                base.SetMaterialDirty();
            }
        }

        protected override void OnValidate()
        {
            InitializeComponents();
            if (parseAgainOnValidate)
            {
                InitValuesFromSharedMaterial();
                parseAgainOnValidate = false;
            }

            DrawShape = drawShape;

            StrokeWidth = strokeWidth;
            OutlineWidth = outlineWidth;
            OutlineColor = outlineColor;
            FalloffDistance = falloffDistance;
            ConstrainRotation = constrainRotation;
            ShapeRotation = shapeRotation;
            FlipHorizontal = flipHorizontal;
            FlipVertical = flipVertical;
            AlphaThreshold = alphaThreshold;

            triangle.OnValidate();
            circle.OnValidate();
            rectangle.OnValidate();
            pentagon.OnValidate();
            hexagon.OnValidate();
            nStarPolygon.OnValidate();
            heart.OnValidate();
            blobbyCross.OnValidate();
            squircle.OnValidate();
            nTriangleRounded.OnValidate();

            gradientEffect.OnValidate();

            base.OnValidate();
            base.SetMaterialDirty();
        }
#endif
        /// <summary>
        /// 初始化组件
        /// </summary>
        private void InitializeComponents()
        {
            circle.Init(m_Material, material, rectTransform);
            triangle.Init(m_Material, material, rectTransform);
            rectangle.Init(m_Material, material, rectTransform);
            pentagon.Init(m_Material, material, rectTransform);
            hexagon.Init(m_Material, material, rectTransform);
            nStarPolygon.Init(m_Material, material, rectTransform);
            heart.Init(m_Material, material, rectTransform);
            blobbyCross.Init(m_Material, material, rectTransform);
            squircle.Init(m_Material, material, rectTransform);
            nTriangleRounded.Init(m_Material, material, rectTransform);
            gradientEffect.Init(m_Material, material, rectTransform);
        }

        /// <summary>
        /// 修复画布中的附加着色通道
        /// </summary>
        void FixAdditionalShaderChannelsInCanvas()
        {
            Canvas c = canvas;
            if (canvas == null) return;
            AdditionalCanvasShaderChannels additionalShaderChannels = c.additionalShaderChannels;
            additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord1;
            additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord2;
            c.additionalShaderChannels = additionalShaderChannels;
        }

#if UNITY_EDITOR
        protected override void Reset()
        {
            InitializeComponents();
            base.Reset();
        }
#else
        void Reset() {
            InitializeComponents();
        }
#endif

        protected override void Awake()
        {
            base.Awake();
            Init();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            InitializeComponents();
            FixAdditionalShaderChannelsInCanvas();
            if (m_Material && MaterialMode == MaterialMode.Shared)
            {
                InitValuesFromSharedMaterial();
            }
            ListenToComponentChanges(true);
            base.SetAllDirty();
        }

        protected override void OnDestroy()
        {
            ListenToComponentChanges(false);
            base.OnDestroy();
        }

        /// <summary>
        /// 监听组件更改
        /// </summary>
        /// <param name="toggle"></param>
        protected void ListenToComponentChanges(bool toggle)
        {
            if (toggle)
            {
                circle.onComponentSettingsChanged += OnComponentSettingsChanged;
                triangle.onComponentSettingsChanged += OnComponentSettingsChanged;
                rectangle.onComponentSettingsChanged += OnComponentSettingsChanged;
                pentagon.onComponentSettingsChanged += OnComponentSettingsChanged;
                hexagon.onComponentSettingsChanged += OnComponentSettingsChanged;
                nStarPolygon.onComponentSettingsChanged += OnComponentSettingsChanged;
                heart.onComponentSettingsChanged += OnComponentSettingsChanged;
                blobbyCross.onComponentSettingsChanged += OnComponentSettingsChanged;
                squircle.onComponentSettingsChanged += OnComponentSettingsChanged;
                nTriangleRounded.onComponentSettingsChanged += OnComponentSettingsChanged;
                gradientEffect.onComponentSettingsChanged += OnComponentSettingsChanged;
            }
            else
            {
                circle.onComponentSettingsChanged -= OnComponentSettingsChanged;
                triangle.onComponentSettingsChanged -= OnComponentSettingsChanged;
                rectangle.onComponentSettingsChanged -= OnComponentSettingsChanged;
                pentagon.onComponentSettingsChanged -= OnComponentSettingsChanged;
                hexagon.onComponentSettingsChanged -= OnComponentSettingsChanged;
                nStarPolygon.onComponentSettingsChanged -= OnComponentSettingsChanged;
                heart.onComponentSettingsChanged -= OnComponentSettingsChanged;
                blobbyCross.onComponentSettingsChanged -= OnComponentSettingsChanged;
                squircle.onComponentSettingsChanged -= OnComponentSettingsChanged;
                nTriangleRounded.onComponentSettingsChanged -= OnComponentSettingsChanged;
                gradientEffect.onComponentSettingsChanged -= OnComponentSettingsChanged;
            }
        }

        protected override void OnTransformParentChanged()
        {
            base.OnTransformParentChanged();
            FixAdditionalShaderChannelsInCanvas();
        }

        /// <summary>
        /// 当组件设置更改时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnComponentSettingsChanged(object sender, EventArgs e)
        {
            base.SetMaterialDirty();
        }


        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            circle.UpdateCircleRadius(rectTransform);
            heart.UpdateCircleRadius(rectTransform);
            base.SetMaterialDirty();
        }

        /// <summary>
        /// 生成网格
        /// </summary>
        /// <param name="vh"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            switch (type)
            {
                case Type.Simple:
                case Type.Sliced:
                case Type.Tiled:
                    ImageHelper.GenerateSimpleSprite(vh, preserveAspect, canvas, rectTransform, ActiveSprite,
                        color, falloffDistance);
                    break;
                case Type.Filled:
                    ImageHelper.GenerateFilledSprite(vh, preserveAspect, canvas, rectTransform, ActiveSprite,
                        color, fillMethod, fillAmount, fillOrigin, fillClockwise, falloffDistance);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// 获取修改后的材质
        /// </summary>
        /// <param name="baseMaterial"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public override Material GetModifiedMaterial(Material baseMaterial)
        {

            Material mat = base.GetModifiedMaterial(baseMaterial);


            if (m_Material && MaterialMode == MaterialMode.Shared)
            {
                InitValuesFromSharedMaterial();
            }

            DisableAllMaterialKeywords(mat);


            RectTransform rt = rectTransform;
            if (DrawShape != DrawShape.None)
            {
                mat.SetFloat(outlineWidth_Sp, outlineWidth);
                mat.SetInt(enableDashedOutline_Sp, enableDashedOutline);
                mat.SetFloat(customTime_Sp, customTime);

                mat.SetFloat(strokeWidth_Sp, strokeWidth);

                mat.SetColor(outlineColor_Sp, OutlineColor);
                mat.SetFloat(falloffDistance_Sp, FalloffDistance);

                float pixelSize = 1 / Mathf.Max(0, FalloffDistance);
                mat.SetFloat(pixelWorldScale_Sp, Mathf.Clamp(pixelSize, 0f, 999999f));


                if (strokeWidth > 0 && outlineWidth > 0)
                {
                    mat.EnableKeyword("OUTLINED_STROKE");
                }
                else
                {
                    if (strokeWidth > 0)
                    {
                        mat.EnableKeyword("STROKE");
                    }
                    else if (outlineWidth > 0)
                    {
                        mat.EnableKeyword("OUTLINED");
                    }
                    else
                    {
                        mat.DisableKeyword("OUTLINED_STROKE");
                        mat.DisableKeyword("STROKE");
                        mat.DisableKeyword("OUTLINED");
                    }
                }
            }


            triangle.ModifyMaterial(ref mat);
            circle.ModifyMaterial(ref mat, falloffDistance);
            rectangle.ModifyMaterial(ref mat);
            pentagon.ModifyMaterial(ref mat);
            hexagon.ModifyMaterial(ref mat);
            nStarPolygon.ModifyMaterial(ref mat);
            heart.ModifyMaterial(ref mat);
            blobbyCross.ModifyMaterial(ref mat);
            squircle.ModifyMaterial(ref mat);
            nTriangleRounded.ModifyMaterial(ref mat);

            gradientEffect.ModifyMaterial(ref mat);


            switch (DrawShape)
            {
                case DrawShape.None:
                    mat.DisableKeyword("CIRCLE");
                    mat.DisableKeyword("TRIANGLE");
                    mat.DisableKeyword("RECTANGLE");
                    mat.DisableKeyword("PENTAGON");
                    mat.DisableKeyword("HEXAGON");
                    mat.DisableKeyword("NSTAR_POLYGON");
                    mat.DisableKeyword("HEART");
                    mat.DisableKeyword("BLOBBYCROSS");
                    mat.DisableKeyword("SQUIRCLE");
                    mat.DisableKeyword("NTRIANGLE_ROUNDED");
                    break;
                case DrawShape.Circle:
                    mat.EnableKeyword("CIRCLE");
                    break;
                case DrawShape.Triangle:
                    mat.EnableKeyword("TRIANGLE");
                    break;
                case DrawShape.Rectangle:
                    mat.EnableKeyword("RECTANGLE");
                    break;
                case DrawShape.Pentagon:
                    mat.EnableKeyword("PENTAGON");
                    break;
                case DrawShape.NStarPolygon:
                    mat.EnableKeyword("NSTAR_POLYGON");
                    break;
                case DrawShape.Hexagon:
                    mat.EnableKeyword("HEXAGON");
                    break;
                case DrawShape.Heart:
                    mat.EnableKeyword("HEART");
                    break;
                case DrawShape.BlobbyCross:
                    mat.EnableKeyword("BLOBBYCROSS");
                    break;
                case DrawShape.Squircle:
                    mat.EnableKeyword("SQUIRCLE");
                    break;
                case DrawShape.NTriangleRounded:
                    mat.EnableKeyword("NTRIANGLE_ROUNDED");
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            mat.SetInt(drawShape_Sp, (int)DrawShape);
            mat.SetInt(flipHorizontal_Sp, flipHorizontal ? 1 : 0);
            mat.SetInt(flipVertical_Sp, flipVertical ? 1 : 0);

            mat.SetFloat(shapeRotation_Sp, shapeRotation);
            mat.SetInt(constrainedRotation_Sp, constrainRotation ? 1 : 0);

            return mat;
        }

        /// <summary>
        /// 禁用所有材质关键字
        /// </summary>
        /// <param name="mat"></param>
        private void DisableAllMaterialKeywords(Material mat)
        {
            mat.DisableKeyword("PROCEDURAL");
            mat.DisableKeyword("HYBRID");

            mat.DisableKeyword("CIRCLE");
            mat.DisableKeyword("TRIANGLE");
            mat.DisableKeyword("RECTANGLE");
            mat.DisableKeyword("PENTAGON");
            mat.DisableKeyword("HEXAGON");
            mat.DisableKeyword("NSTAR_POLYGON");
            mat.DisableKeyword("HEART");
            mat.DisableKeyword("BLOBBYCROSS");
            mat.DisableKeyword("SQUIRCLE");
            mat.DisableKeyword("NTRIANGLE_ROUNDED");

            mat.DisableKeyword("STROKE");
            mat.DisableKeyword("OUTLINED");
            mat.DisableKeyword("OUTLINED_STROKE");

            mat.DisableKeyword("ROUNDED_CORNERS");

            mat.DisableKeyword("GRADIENT_LINEAR");
            mat.DisableKeyword("GRADIENT_CORNER");
            mat.DisableKeyword("GRADIENT_RADIAL");
        }

        /// <summary>
        /// 从共享材质初始化值
        /// </summary>
        public void InitValuesFromSharedMaterial()
        {
            if (m_Material == null) return;
            Material mat = m_Material;

            //Basic Settings
            drawShape = (DrawShape)mat.GetInt(drawShape_Sp);

            strokeWidth = mat.GetFloat(strokeWidth_Sp);
            falloffDistance = mat.GetFloat(falloffDistance_Sp);

            outlineWidth = mat.GetFloat(outlineWidth_Sp);
            outlineColor = mat.GetColor(outlineColor_Sp);
            enableDashedOutline = mat.GetInt(enableDashedOutline_Sp);
            customTime = mat.GetFloat(customTime_Sp);

            flipHorizontal = mat.GetInt(flipHorizontal_Sp) == 1;
            flipVertical = mat.GetInt(flipVertical_Sp) == 1;
            constrainRotation = mat.GetInt(constrainedRotation_Sp) == 1;
            shapeRotation = mat.GetFloat(shapeRotation_Sp);

            triangle.InitValuesFromMaterial(ref mat);
            circle.InitValuesFromMaterial(ref mat);
            rectangle.InitValuesFromMaterial(ref mat);
            pentagon.InitValuesFromMaterial(ref mat);
            hexagon.InitValuesFromMaterial(ref mat);
            nStarPolygon.InitValuesFromMaterial(ref mat);
            heart.InitValuesFromMaterial(ref mat);
            blobbyCross.InitValuesFromMaterial(ref mat);
            squircle.InitValuesFromMaterial(ref mat);
            nTriangleRounded.InitValuesFromMaterial(ref mat);

            //GradientEffect
            gradientEffect.InitValuesFromMaterial(ref mat);
        }

#if UNITY_EDITOR
        /// <summary>
        /// 创建材质资产
        /// </summary>
        /// <returns></returns>
        public Material CreateMaterialAssetFromComponentSettings()
        {
            Material matAsset = new Material(Shader.Find(shaderName));
            matAsset = GetModifiedMaterial(matAsset);
            string path = EditorUtility.SaveFilePanelInProject("通过ImageEx创建材质",
                "Material", "mat", "选择位置");
            AssetDatabase.CreateAsset(matAsset, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return matAsset;
        }
#endif
    }
}