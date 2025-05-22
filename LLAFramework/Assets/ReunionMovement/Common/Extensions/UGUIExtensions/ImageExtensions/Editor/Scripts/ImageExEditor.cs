using Codice.CM.Client.Differences.Graphic;
using System;
using System.Security.Policy;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace LLAFramework.UI.ImageExtensions.Editor
{
    [CustomEditor(typeof(ImageEx), true)]
    [CanEditMultipleObjects]
    public class ImageExEditor : ImageEditor
    {
        private SerializedProperty spSprite;
        private SerializedProperty spCircle, spTriangle, spRectangle, spPentagon, spHexagon, spChamferBox, spNStarPolygon, spHeart, spBlobbyCross, spSquircle, spNTriangleRounded;
        private SerializedProperty spPreserveAspect;
        private SerializedProperty spFillMethod, spFillOrigin, spFillAmount, spFillClockwise;
        private SerializedProperty spAlphaThreshold;
        private SerializedProperty spShape;
        private SerializedProperty spStrokeWidth, spOutlineWidth, spOutlineColor, spFalloffDistance, spEnableDashedOutline, spCustomTime;
        private SerializedProperty spConstrainRotation, spShapeRotation, spFlipHorizontal, spFlipVertical;
        private SerializedProperty spMaterialSettings, spMaterial, spImageType;

        private SerializedProperty spGradient;

        private bool gsInitialized, shaderChannelsNeedUpdate;

        protected override void OnEnable()
        {
            foreach (Object obj in serializedObject.targetObjects)
            {
                ((ImageEx)obj).UpdateSerializedValuesFromSharedMaterial();
            }

            base.OnEnable();

            spSprite = serializedObject.FindProperty("m_Sprite");

            spShape = serializedObject.FindProperty("drawShape");

            spStrokeWidth = serializedObject.FindProperty("strokeWidth");
            spOutlineWidth = serializedObject.FindProperty("outlineWidth");
            spOutlineColor = serializedObject.FindProperty("outlineColor");
            spFalloffDistance = serializedObject.FindProperty("falloffDistance");
            spEnableDashedOutline = serializedObject.FindProperty("enableDashedOutline");
            spCustomTime = serializedObject.FindProperty("customTime");

            spMaterialSettings = serializedObject.FindProperty("materialMode");
            spMaterial = serializedObject.FindProperty("m_Material");
            spImageType = serializedObject.FindProperty("imageType");

            spFillMethod = serializedObject.FindProperty("m_FillMethod");
            spFillOrigin = serializedObject.FindProperty("m_FillOrigin");
            spFillAmount = serializedObject.FindProperty("m_FillAmount");
            spFillClockwise = serializedObject.FindProperty("m_FillClockwise");

            spConstrainRotation = serializedObject.FindProperty("constrainRotation");
            spShapeRotation = serializedObject.FindProperty("shapeRotation");
            spFlipHorizontal = serializedObject.FindProperty("flipHorizontal");
            spFlipVertical = serializedObject.FindProperty("flipVertical");

            spAlphaThreshold = serializedObject.FindProperty("alphaThreshold");

            spCircle = serializedObject.FindProperty("circle");
            spRectangle = serializedObject.FindProperty("rectangle");
            spTriangle = serializedObject.FindProperty("triangle");
            spPentagon = serializedObject.FindProperty("pentagon");
            spHexagon = serializedObject.FindProperty("hexagon");
            spChamferBox = serializedObject.FindProperty("chamferBox");
            spNStarPolygon = serializedObject.FindProperty("nStarPolygon");
            spHeart = serializedObject.FindProperty("heart");
            spBlobbyCross = serializedObject.FindProperty("blobbyCross");
            spSquircle = serializedObject.FindProperty("squircle");
            spNTriangleRounded = serializedObject.FindProperty("nTriangleRounded");

            spPreserveAspect = serializedObject.FindProperty("m_PreserveAspect");

            spGradient = serializedObject.FindProperty("gradientEffect");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            FixShaderChannelGUI();

            RaycastControlsGUI();
            EditorGUILayout.PropertyField(m_Color);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(spShape, new GUIContent("绘制形状"));

            if (spShape.enumValueIndex != (int)DrawShape.None)
            {
                EditorGUILayout.BeginVertical("Box");
                if (!spShape.hasMultipleDifferentValues)
                {
                    switch ((DrawShape)spShape.enumValueIndex)
                    {
                        case DrawShape.Circle:
                            EditorGUILayout.PropertyField(spCircle);
                            break;
                        case DrawShape.Rectangle:
                            EditorGUILayout.PropertyField(spRectangle);
                            break;
                        case DrawShape.Pentagon:
                            EditorGUILayout.PropertyField(spPentagon);
                            break;
                        case DrawShape.Triangle:
                            EditorGUILayout.PropertyField(spTriangle);
                            break;
                        case DrawShape.Hexagon:
                            EditorGUILayout.PropertyField(spHexagon);
                            break;
                        case DrawShape.ChamferBox:
                            EditorGUILayout.PropertyField(spChamferBox);
                            break;
                        case DrawShape.NStarPolygon:
                            EditorGUILayout.PropertyField(spNStarPolygon);
                            break;
                        case DrawShape.Heart:
                            EditorGUILayout.PropertyField(spHeart);
                            break;
                        case DrawShape.BlobbyCross:
                            EditorGUILayout.PropertyField(spBlobbyCross);
                            break;
                        case DrawShape.Squircle:
                            EditorGUILayout.PropertyField(spSquircle);
                            break;
                        case DrawShape.NTriangleRounded:
                            EditorGUILayout.PropertyField(spNTriangleRounded);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                EditorGUILayout.Space();

                if (spShape.enumValueIndex != (int)DrawShape.None)
                {
                    AdditionalShapeDataGUI();
                }

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.Space();
            ImageTypeGUI();

            SpriteGUI();

            if (!spSprite.hasMultipleDifferentValues && spSprite.objectReferenceValue != null)
            {
                EditorGUILayout.PropertyField(spPreserveAspect);
            }

            SetShowNativeSize(spSprite.objectReferenceValue != null, true);
            NativeSizeButtonGUI();

            EditorGUILayout.Space();
            SharedMaterialGUI();

            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical("Box");
            {
                EditorGUILayout.PropertyField(spGradient);
            }
            EditorGUILayout.EndVertical();

            //刷新
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
            Repaint();
        }

        private void AdditionalShapeDataGUI()
        {
            EditorGUILayout.Space();

            float strokeWidth = spStrokeWidth.floatValue;
            float outlineWidth = spOutlineWidth.floatValue;
            float falloff = spFalloffDistance.floatValue;
            Color outlineColor = spOutlineColor.colorValue;

            float customTime = spCustomTime.floatValue;

            float h = 2;

            if (spShape.enumValueIndex == (int)DrawShape.Circle || spShape.enumValueIndex == (int)DrawShape.Rectangle)
            {
                h = 3;
            }

            Rect r = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight * h + EditorGUIUtility.standardVerticalSpacing);

            Rect line = r;
            line.height = EditorGUIUtility.singleLineHeight;
            float x = (line.width - 10f) / 2;

            float fieldWidth = x / 2 - 10f;
            float labelWidth = x - fieldWidth;

            line.width = labelWidth;
            EditorGUI.LabelField(line, "线条");
            Rect dragZone = line;
            line.x += labelWidth;
            line.width = fieldWidth;
            EditorGUI.BeginChangeCheck();
            {
                EditorGUI.showMixedValue = spStrokeWidth.hasMultipleDifferentValues;
                strokeWidth =
                    EditorGUILayoutExtended.FloatFieldExtended(line, spStrokeWidth.floatValue, dragZone);
                EditorGUI.showMixedValue = false;
            }
            if (EditorGUI.EndChangeCheck())
            {
                spStrokeWidth.floatValue = strokeWidth;
            }
            line.x += fieldWidth + 10;
            line.width = labelWidth;
            EditorGUI.LabelField(line, "衰减");
            dragZone = line;
            line.x += labelWidth;
            line.width = fieldWidth;

            EditorGUI.BeginChangeCheck();
            {
                EditorGUI.showMixedValue = spFalloffDistance.hasMultipleDifferentValues;
                falloff =
                    EditorGUILayoutExtended.FloatFieldExtended(line, spFalloffDistance.floatValue, dragZone);
                EditorGUI.showMixedValue = false;
            }
            if (EditorGUI.EndChangeCheck())
            {
                spFalloffDistance.floatValue = falloff;
            }
            line.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            line.x = r.x;
            line.width = labelWidth;
            EditorGUI.LabelField(line, "轮廓宽度");
            dragZone = line;
            line.x += labelWidth;
            line.width = fieldWidth;
            EditorGUI.BeginChangeCheck();
            {
                EditorGUI.showMixedValue = spOutlineWidth.hasMultipleDifferentValues;
                outlineWidth =
                    EditorGUILayoutExtended.FloatFieldExtended(line, spOutlineWidth.floatValue, dragZone);
                EditorGUI.showMixedValue = false;
            }
            if (EditorGUI.EndChangeCheck())
            {
                spOutlineWidth.floatValue = outlineWidth;
            }
            line.x += fieldWidth + 10;
            line.width = labelWidth;
            EditorGUI.LabelField(line, "轮廓颜色");
            dragZone = line;
            line.width = fieldWidth;
            line.x += labelWidth;
            EditorGUI.BeginChangeCheck();
            {
                EditorGUI.showMixedValue = spOutlineColor.hasMultipleDifferentValues;
                outlineColor = EditorGUI.ColorField(line, spOutlineColor.colorValue);
                EditorGUI.showMixedValue = false;
            }
            if (EditorGUI.EndChangeCheck())
            {
                spOutlineColor.colorValue = outlineColor;
            }

            // ---------------------
            if (spShape.enumValueIndex == (int)DrawShape.Circle || spShape.enumValueIndex == (int)DrawShape.Rectangle)
            {
                line.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                line.x = r.x;
                line.width = labelWidth;

                // 绘制标签
                EditorGUI.LabelField(line, "是否开启虚线");

                // 绘制复选框
                line.x += labelWidth; // 添加间距以对齐复选框
                line.width = fieldWidth - 5; // 调整宽度以适配复选框
                EditorGUI.BeginChangeCheck();
                {
                    EditorGUI.showMixedValue = spEnableDashedOutline.hasMultipleDifferentValues;
                    bool enableDashedOutline = EditorGUI.Toggle(line, spEnableDashedOutline.intValue != 0);
                    EditorGUI.showMixedValue = false;

                    if (EditorGUI.EndChangeCheck())
                    {
                        spEnableDashedOutline.intValue = enableDashedOutline ? 1 : 0;
                    }
                }
                line.x += fieldWidth + 10;
                line.width = labelWidth;
                EditorGUI.LabelField(line, "自定义时间");
                dragZone = line;
                line.width = fieldWidth;
                line.x += labelWidth;
                EditorGUI.BeginChangeCheck();
                {
                    EditorGUI.showMixedValue = spCustomTime.hasMultipleDifferentValues;
                    customTime = EditorGUILayoutExtended.FloatFieldExtended(line, spCustomTime.floatValue, dragZone);
                    EditorGUI.showMixedValue = false;
                }
                if (EditorGUI.EndChangeCheck())
                {
                    spCustomTime.floatValue = customTime;
                }
            }
            EditorGUILayout.Space();

            RotationGUI();
        }

        private void RotationGUI()
        {
            Rect r = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight + 24 + EditorGUIUtility.standardVerticalSpacing);
            Rect line = r;
            line.height = EditorGUIUtility.singleLineHeight;
            float x = (line.width - 10f) / 2;

            float fieldWidth = x / 2 - 10f;
            float labelWidth = x - fieldWidth;

            line.width = labelWidth;
            EditorGUI.LabelField(line, "旋转");
            line.x += labelWidth;
            line.width = r.width - labelWidth - 78;

            string[] options = spConstrainRotation.hasMultipleDifferentValues ? new[] { "---", "---" } : new[] { "自由", "限制" };
            bool boolVal = spConstrainRotation.boolValue;
            EditorGUI.BeginChangeCheck();
            {
                boolVal = GUI.Toolbar(line, boolVal ? 1 : 0, options) == 1;
            }
            if (EditorGUI.EndChangeCheck())
            {
                spConstrainRotation.boolValue = boolVal;
                GUI.FocusControl(null);
            }

            line.x += line.width + 14;
            line.width = 64;
            EditorGUI.LabelField(line, "图像翻转");

            line.y += EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight;
            line.x = r.x + 10;
            line.height = EditorGUIUtility.singleLineHeight;
            line.width = labelWidth - 10;
            EditorGUI.BeginDisabledGroup(spConstrainRotation.boolValue);
            {
                Rect dragZone = line;
                EditorGUI.LabelField(line, "角度");
                line.x = r.x + labelWidth;
                line.width = r.width - labelWidth - 148;

                float rotationValue = spShapeRotation.floatValue;
                EditorGUI.BeginChangeCheck();
                {
                    EditorGUI.showMixedValue = spShapeRotation.hasMultipleDifferentValues;
                    rotationValue = EditorGUILayoutExtended.FloatFieldExtended(line, spShapeRotation.floatValue, dragZone);
                    EditorGUI.showMixedValue = false;
                }
                if (EditorGUI.EndChangeCheck())
                {
                    spShapeRotation.floatValue = rotationValue;
                }
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(!spConstrainRotation.boolValue);
            {
                line.x += line.width + 4;
                line.width = 30;
                line.height = 24;
                if (GUI.Button(line, EditorContents.RotateLeftNormal))
                {
                    float rotation = spShapeRotation.floatValue;
                    float remainder = rotation % 90;
                    if (Mathf.Abs(remainder) <= 0)
                    {
                        rotation += 90;
                    }
                    else
                    {
                        rotation = rotation - remainder + 90;
                    }
                    if (Math.Abs(rotation) >= 360) rotation = 0;
                    spShapeRotation.floatValue = rotation;
                }

                line.x += 34;
                if (GUI.Button(line, EditorContents.RotateRightNormal))
                {
                    float rotation = spShapeRotation.floatValue;
                    float remainder = rotation % 90;
                    if (Mathf.Abs(remainder) <= 0)
                    {
                        rotation -= 90;
                    }
                    else
                    {
                        rotation -= remainder;
                    }

                    if (Math.Abs(rotation) >= 360) rotation = 0;
                    spShapeRotation.floatValue = rotation;
                }
            }
            EditorGUI.EndDisabledGroup();

            line.x += 46;
            bool flipH = spFlipHorizontal.boolValue;
            bool flipV = spFlipVertical.boolValue;
            EditorGUI.BeginChangeCheck();
            {
                EditorGUI.BeginDisabledGroup(spFlipHorizontal.hasMultipleDifferentValues || spFlipVertical.hasMultipleDifferentValues);
                flipH = GUI.Toggle(line, spFlipHorizontal.boolValue, spFlipHorizontal.boolValue ? EditorContents.FlipHorizontalActive : EditorContents.FlipHorizontalNormal, "button");
                line.x += 34;
                flipV = GUI.Toggle(line, spFlipVertical.boolValue, spFlipVertical.boolValue ? EditorContents.FlipVerticalActive : EditorContents.FlipVerticalNormal, "button");
                EditorGUI.EndDisabledGroup();
            }
            if (EditorGUI.EndChangeCheck())
            {
                spFlipHorizontal.boolValue = flipH;
                spFlipVertical.boolValue = flipV;
            }

        }

        private void FixShaderChannelGUI()
        {
            if (!shaderChannelsNeedUpdate) return;
            EditorGUILayout.HelpBox("父画布需要具有以下附加着色器通道：Texcord1、Texcord2", MessageType.Error);
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Fix", GUILayout.Width(100)))
                {
                    Canvas canvas = (target as ImageEx)?.GetComponentInParent<Canvas>();
                    if (canvas != null)
                    {
                        EditorUtility.AddAdditionalShaderChannelsToCanvas(canvas);
                        shaderChannelsNeedUpdate = false;
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private new void SpriteGUI()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(spSprite, new GUIContent("精灵"));
            if (EditorGUI.EndChangeCheck())
            {
                Sprite newSprite = spSprite.objectReferenceValue as Sprite;
                if (newSprite)
                {
                    Image.Type oldType = (Image.Type)spImageType.enumValueIndex;
                    if (newSprite.border.SqrMagnitude() > 0)
                    {
                        spImageType.enumValueIndex = (int)Image.Type.Sliced;
                    }
                    else if (oldType == Image.Type.Sliced)
                    {
                        spImageType.enumValueIndex = (int)Image.Type.Simple;
                    }
                }

                (serializedObject.targetObject as Image)?.DisableSpriteOptimizations();
            }

            Rect rect = EditorGUILayout.GetControlRect();
            EditorGUI.Slider(rect, spAlphaThreshold, 0f, 1f, "Alpha阈值");
        }

        private void ImageTypeGUI()
        {
            int selectedIndex = spImageType.enumValueIndex == (int)Image.Type.Simple ? 0 : 1;
            Rect imageTypeRect = EditorGUILayout.GetControlRect();
            EditorGUI.BeginChangeCheck();
            {
                EditorGUI.LabelField(
                    new Rect(imageTypeRect.x, imageTypeRect.y, EditorGUIUtility.labelWidth, imageTypeRect.height),
                    "类型");
                imageTypeRect.x += EditorGUIUtility.labelWidth + 2;
                imageTypeRect.width -= EditorGUIUtility.labelWidth + 2;
                selectedIndex = EditorGUI.Popup(imageTypeRect, selectedIndex, new[] { "简单", "填充" });
            }
            if (EditorGUI.EndChangeCheck())
            {
                spImageType.enumValueIndex = (int)(selectedIndex == 0 ? Image.Type.Simple : Image.Type.Filled);
            }

            if (!spImageType.hasMultipleDifferentValues && spImageType.enumValueIndex == (int)Image.Type.Filled)
            {
                ++EditorGUI.indentLevel;
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(spFillMethod);
                if (EditorGUI.EndChangeCheck())
                {
                    spFillOrigin.intValue = 0;
                }

                switch ((Image.FillMethod)spFillMethod.enumValueIndex)
                {
                    case Image.FillMethod.Horizontal:
                        spFillOrigin.intValue = (int)(Image.OriginHorizontal)EditorGUILayout.EnumPopup("填充原点",
                            (Image.OriginHorizontal)spFillOrigin.intValue);
                        break;
                    case Image.FillMethod.Vertical:
                        spFillOrigin.intValue = (int)(Image.OriginVertical)EditorGUILayout.EnumPopup("填充原点",
                            (Image.OriginVertical)spFillOrigin.intValue);
                        break;
                    case Image.FillMethod.Radial90:
                        spFillOrigin.intValue =
                            (int)(Image.Origin90)EditorGUILayout.EnumPopup("填充原点",
                                (Image.Origin90)spFillOrigin.intValue);
                        break;
                    case Image.FillMethod.Radial180:
                        spFillOrigin.intValue =
                            (int)(Image.Origin180)EditorGUILayout.EnumPopup("填充原点",
                                (Image.Origin180)spFillOrigin.intValue);
                        break;
                    case Image.FillMethod.Radial360:
                        spFillOrigin.intValue =
                            (int)(Image.Origin360)EditorGUILayout.EnumPopup("填充原点",
                                (Image.Origin360)spFillOrigin.intValue);
                        break;
                }

                EditorGUILayout.PropertyField(spFillAmount);
                if ((Image.FillMethod)spFillMethod.enumValueIndex > Image.FillMethod.Vertical)
                {
                    EditorGUILayout.PropertyField(spFillClockwise, new GUIContent("顺时针"));
                }

                --EditorGUI.indentLevel;
            }
        }

        private void SharedMaterialGUI()
        {
            Rect rect = EditorGUILayout.GetControlRect(true,
                EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
            int matSett = spMaterialSettings.enumValueIndex;
            Rect labelRect = rect;
            labelRect.width = EditorGUIUtility.labelWidth;
            EditorGUI.LabelField(labelRect, "材质模式");
            rect.x += labelRect.width;
            rect.width -= labelRect.width;

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = spMaterialSettings.hasMultipleDifferentValues;
            string[] options = new[] { "动态", "共享" };
            if (EditorGUI.showMixedValue) options = new[] { "---", "---" };
            matSett = GUI.Toolbar(rect, matSett, options);

            if (EditorGUI.EndChangeCheck())
            {
                spMaterialSettings.enumValueIndex = matSett;
                foreach (Object obj in targets)
                {
                    ((ImageEx)obj).MaterialMode = (MaterialMode)matSett;
                    UnityEditor.EditorUtility.SetDirty(obj);
                }
            }

            EditorGUI.showMixedValue = false;


            EditorGUI.BeginDisabledGroup(spMaterialSettings.enumValueIndex == (int)MaterialMode.Dynamic);
            {
                rect = EditorGUILayout.GetControlRect(true,
                    EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);

                Object matObj = spMaterial.objectReferenceValue;

                EditorGUI.BeginChangeCheck();
                {
                    EditorGUI.showMixedValue = spMaterialSettings.hasMultipleDifferentValues;
                    matObj = (Material)EditorGUI.ObjectField(
                        new Rect(rect.x, rect.y, rect.width - 60, EditorGUIUtility.singleLineHeight),
                        matObj, typeof(Material), false);
                    EditorGUI.showMixedValue = false;
                }
                if (EditorGUI.EndChangeCheck())
                {
                    spMaterial.objectReferenceValue = matObj;
                    foreach (Object obj in targets)
                    {
                        ((ImageEx)obj).material = (Material)matObj;
                        UnityEditor.EditorUtility.SetDirty(obj);
                    }
                }

                EditorGUI.BeginDisabledGroup(spMaterial.objectReferenceValue != null);
                {
                    if (GUI.Button(new Rect(rect.x + rect.width - 55, rect.y, 55, EditorGUIUtility.singleLineHeight), "创建"))
                    {
                        Material mat = ((ImageEx)target).CreateMaterialAssetFromComponentSettings();
                        spMaterial.objectReferenceValue = mat;
                        foreach (Object obj in targets)
                        {
                            ((ImageEx)obj).material = mat;
                            UnityEditor.EditorUtility.SetDirty(obj);
                        }
                    }
                }
                EditorGUI.EndDisabledGroup();
            }
            EditorGUI.EndDisabledGroup();
        }
    }
}
