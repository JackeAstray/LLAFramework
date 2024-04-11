
using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEditor.UI;


namespace GameLogic.UI.ImageExtensions.Editor
{
    [CustomEditor(typeof(ImageExBasic))]
    public class ImageExBasicEditor : ImageEditor
    {
        private SerializedProperty spSprite;

        private SerializedProperty spCircleRadius, spCircleFitToRect;
        private SerializedProperty spTriangleCornerRadius, spTriangleUniformCornerRadius;
        private SerializedProperty spRectangleCornerRadius, spRectangleUniformCornerRadius;
        private SerializedProperty spNStarPolygonSideCount, spNStarPolygonInset, spNStarPolygonCornerRadius;

        private SerializedProperty spPreserveAspect;
        private SerializedProperty spFillMethod, spFillOrigin, spFillAmount, spFillClockwise;
        private SerializedProperty spShape;
        private SerializedProperty spStrokeWidth, spOutlineWidth, spOutlineColor, spFalloffDistance;
        private SerializedProperty spConstrainRotation, spShapeRotation, spFlipHorizontal, spFlipVertical;
        private SerializedProperty spImageType;

        protected override void OnEnable()
        {
            base.OnEnable();

            spSprite = serializedObject.FindProperty("m_Sprite");

            spShape = serializedObject.FindProperty("drawShape");

            spStrokeWidth = serializedObject.FindProperty("strokeWidth");
            spOutlineWidth = serializedObject.FindProperty("outlineWidth");
            spOutlineColor = serializedObject.FindProperty("outlineColor");
            spFalloffDistance = serializedObject.FindProperty("falloffDistance");

            spImageType = serializedObject.FindProperty("imageType");

            spFillMethod = serializedObject.FindProperty("m_FillMethod");
            spFillOrigin = serializedObject.FindProperty("m_FillOrigin");
            spFillAmount = serializedObject.FindProperty("m_FillAmount");
            spFillClockwise = serializedObject.FindProperty("m_FillClockwise");

            spConstrainRotation = serializedObject.FindProperty("constrainRotation");
            spShapeRotation = serializedObject.FindProperty("shapeRotation");
            spFlipHorizontal = serializedObject.FindProperty("flipHorizontal");
            spFlipVertical = serializedObject.FindProperty("flipVertical");


            spCircleRadius = serializedObject.FindProperty("circleRadius");
            spCircleFitToRect = serializedObject.FindProperty("circleFitToRect");
            spTriangleCornerRadius = serializedObject.FindProperty("triangleCornerRadius");
            spTriangleUniformCornerRadius = serializedObject.FindProperty("triangleUniformCornerRadius");
            spRectangleCornerRadius = serializedObject.FindProperty("rectangleCornerRadius");
            spRectangleUniformCornerRadius = serializedObject.FindProperty("triangleUniformCornerRadius");
            spNStarPolygonSideCount = serializedObject.FindProperty("nStarPolygonSideCount");
            spNStarPolygonInset = serializedObject.FindProperty("nStarPolygonInset");
            spNStarPolygonCornerRadius = serializedObject.FindProperty("nStarPolygonCornerRadius");

            spPreserveAspect = serializedObject.FindProperty("m_PreserveAspect");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            RaycastControlsGUI();
            EditorGUILayout.PropertyField(m_Color);
            EditorGUILayout.Space();
            Rect shapePopupRect = EditorGUILayout.GetControlRect();
            DrawShapeBasic selectedIndex = (DrawShapeBasic)spShape.enumValueIndex;
            if (selectedIndex == (DrawShapeBasic)DrawShape.Pentagon || selectedIndex == (DrawShapeBasic)DrawShape.Hexagon)
            {
                selectedIndex = (DrawShapeBasic)DrawShape.None;
                spShape.enumValueIndex = (int)selectedIndex;
            }

            EditorGUI.BeginChangeCheck();
            {
                EditorGUI.showMixedValue = spShape.hasMultipleDifferentValues;
                selectedIndex = (DrawShapeBasic)EditorGUI.EnumPopup(shapePopupRect, "绘制形状", selectedIndex);
                EditorGUI.showMixedValue = false;
            }
            if (EditorGUI.EndChangeCheck())
            {
                spShape.enumValueIndex = (int)selectedIndex;
            }

            if (spShape.enumValueIndex != (int)DrawShape.None && !spShape.hasMultipleDifferentValues)
            {
                EditorGUILayout.BeginVertical("Box");
                if (!spShape.hasMultipleDifferentValues)
                {
                    switch ((DrawShape)spShape.enumValueIndex)
                    {
                        case DrawShape.Circle:
                            CircleGUI();
                            break;
                        case DrawShape.Rectangle:
                            RectangleGUI();
                            break;
                        case DrawShape.Triangle:
                            TriangleGUI();
                            break;
                        case DrawShape.NStarPolygon:
                            NStarPolygonGUI();
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

            serializedObject.ApplyModifiedProperties();
        }

        private void NStarPolygonGUI()
        {
            spNStarPolygonSideCount.intValue =
                EditorGUILayout.IntSlider("边数", spNStarPolygonSideCount.intValue, 3, 10);
            spNStarPolygonInset.floatValue =
                EditorGUILayout.Slider("插入", spNStarPolygonInset.floatValue, 2f,
                    spNStarPolygonSideCount.intValue - 0.1f);
            spNStarPolygonCornerRadius.floatValue =
                EditorGUILayout.FloatField("圆角半径", spNStarPolygonCornerRadius.floatValue);

        }

        private void CircleGUI()
        {
            EditorGUI.BeginDisabledGroup(spCircleFitToRect.boolValue);
            EditorGUILayout.PropertyField(spCircleRadius, new GUIContent("半径"));
            EditorGUI.EndDisabledGroup();
            Rect rect = EditorGUILayout.GetControlRect();
            EditorUtility.CornerRadiusModeGUI(rect, ref spCircleFitToRect, new[] { "自由", "拟合" }, String.Empty);
        }

        private void RectangleGUI()
        {
            Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
            EditorUtility.CornerRadiusModeGUI(rect, ref spRectangleUniformCornerRadius, new[] { "自由", "统一" });

            Vector4 vectorValue = spRectangleCornerRadius.vector4Value;
            float floatVal = vectorValue.x;

            float[] zw = new[] { vectorValue.w, vectorValue.z };
            float[] xy = new[] { vectorValue.x, vectorValue.y };

            EditorGUI.BeginChangeCheck();
            {
                EditorGUI.showMixedValue = spRectangleCornerRadius.hasMultipleDifferentValues;
                if (spRectangleUniformCornerRadius.boolValue)
                {
                    floatVal = EditorGUILayout.FloatField("均匀半径", floatVal);
                }
                else
                {
                    rect = EditorGUILayout.GetControlRect(true,
                        EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight * 2);
                    Rect RadiusVectorRect = new Rect(rect.x, rect.y,
                        rect.width, EditorGUIUtility.singleLineHeight);

                    EditorGUI.MultiFloatField(RadiusVectorRect, new[] {
                        new GUIContent("W"), new GUIContent("Z")
                    }, zw);
                    RadiusVectorRect.y +=
                        EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    EditorGUI.MultiFloatField(RadiusVectorRect, new[] {
                        new GUIContent("X "), new GUIContent("Y")
                    }, xy);
                }

                EditorGUI.showMixedValue = false;
            }
            if (EditorGUI.EndChangeCheck())
            {
                spRectangleCornerRadius.vector4Value = spRectangleUniformCornerRadius.boolValue
                    ? new Vector4(floatVal, floatVal, floatVal, floatVal)
                    : new Vector4(xy[0], xy[1], zw[1], zw[0]);
            }
        }

        private void TriangleGUI()
        {
            Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
            EditorUtility.CornerRadiusModeGUI(rect, ref spTriangleUniformCornerRadius, new[] { "自由", "统一" });

            Vector3 vectorValue = spTriangleCornerRadius.vector3Value;
            float floatVal = vectorValue.x;

            EditorGUI.BeginChangeCheck();
            {
                EditorGUI.showMixedValue = spTriangleCornerRadius.hasMultipleDifferentValues;
                if (spTriangleUniformCornerRadius.boolValue)
                {
                    floatVal = EditorGUILayout.FloatField("均匀半径", floatVal);
                }
                else
                {
                    vectorValue = EditorGUILayout.Vector3Field("", vectorValue);
                }

                EditorGUI.showMixedValue = false;
            }
            if (EditorGUI.EndChangeCheck())
            {
                spTriangleCornerRadius.vector3Value = spTriangleUniformCornerRadius.boolValue
                    ? new Vector3(floatVal, floatVal, floatVal)
                    : vectorValue;
            }
        }

        private void AdditionalShapeDataGUI()
        {
            EditorGUILayout.Space();

            float strokeWidth = spStrokeWidth.floatValue;
            float outlineWidth = spOutlineWidth.floatValue;
            float falloff = spFalloffDistance.floatValue;
            Color outlineColor = spOutlineColor.colorValue;

            Rect r = EditorGUILayout.GetControlRect(true,
                EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing);
            Rect line = r;
            line.height = EditorGUIUtility.singleLineHeight;
            float x = (line.width - 10f) / 2;

            float fieldWidth = x / 2 - 10f;
            float labelWidth = x - fieldWidth;

            line.width = labelWidth;
            EditorGUI.LabelField(line, "一条线");
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

            EditorGUILayout.Space();

            RotationGUI();
        }

        private void RotationGUI()
        {
            Rect r = EditorGUILayout.GetControlRect(true,
                EditorGUIUtility.singleLineHeight + 24 + EditorGUIUtility.standardVerticalSpacing);
            Rect line = r;
            line.height = EditorGUIUtility.singleLineHeight;
            float x = (line.width - 10f) / 2;

            float fieldWidth = x / 2 - 10f;
            float labelWidth = x - fieldWidth;

            line.width = labelWidth;
            EditorGUI.LabelField(line, "旋转");
            line.x += labelWidth;
            line.width = r.width - labelWidth - 78;

            string[] options = spConstrainRotation.hasMultipleDifferentValues
                ? new[] { "---", "---" }
                : new[] { "自由", "限制" };
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
                    rotationValue =
                        EditorGUILayoutExtended.FloatFieldExtended(line, spShapeRotation.floatValue, dragZone);
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
                EditorGUI.BeginDisabledGroup(spFlipHorizontal.hasMultipleDifferentValues ||
                                             spFlipVertical.hasMultipleDifferentValues);
                flipH = GUI.Toggle(line, spFlipHorizontal.boolValue,
                    spFlipHorizontal.boolValue
                        ? EditorContents.FlipHorizontalActive
                        : EditorContents.FlipHorizontalNormal, "button");
                line.x += 34;
                flipV = GUI.Toggle(line, spFlipVertical.boolValue,
                    spFlipVertical.boolValue
                        ? EditorContents.FlipVerticalActive
                        : EditorContents.FlipVerticalNormal, "button");
                EditorGUI.EndDisabledGroup();
            }
            if (EditorGUI.EndChangeCheck())
            {
                spFlipHorizontal.boolValue = flipH;
                spFlipVertical.boolValue = flipV;
            }
        }

        private new void SpriteGUI()
        {
            Sprite spriteRef = spSprite.objectReferenceValue as Sprite;
            Sprite sprite = EditorUtility.EmptySprite.Equals(spriteRef) ? null : spriteRef;

            EditorGUI.BeginChangeCheck();
            {
                EditorGUI.showMixedValue = spSprite.hasMultipleDifferentValues;
                sprite = EditorGUILayout.ObjectField("精灵", sprite, typeof(Sprite), false, GUILayout.Height(EditorGUIUtility.singleLineHeight)) as Sprite;

                EditorGUI.showMixedValue = false;
            }
            if (EditorGUI.EndChangeCheck())
            {
                Sprite newSprite = sprite == null ? EditorUtility.EmptySprite : sprite;
                if (newSprite != null)
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

                spSprite.objectReferenceValue = newSprite;
                (serializedObject.targetObject as Image)?.DisableSpriteOptimizations();
            }
            EditorGUI.EndDisabledGroup();
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
                selectedIndex = EditorGUI.Popup(imageTypeRect, selectedIndex, new[] { "简单", "填满" });
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
        private enum DrawShapeBasic
        {
            None = 0,
            Circle = 1,
            Triangle = 2,
            Rectangle = 3,
            NStarPolygon = 6
        }
    }

}