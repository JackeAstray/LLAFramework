using UnityEditor;
using UnityEngine;

namespace LLAFramework.UI.ImageExtensions.Editor
{
    [CustomPropertyDrawer(typeof(HexagonImg))]
    public class HexagonPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            {
                SerializedProperty cornerRadius = property.FindPropertyRelative("cornerRadius");
                SerializedProperty uniformCornerRadius = property.FindPropertyRelative("uniformCornerRadius");
                SerializedProperty triSizes = property.FindPropertyRelative("tipSize");
                SerializedProperty uniformTipSize = property.FindPropertyRelative("uniformTipSize");
                SerializedProperty tipRadius = property.FindPropertyRelative("tipRadius");
                SerializedProperty uniformTipRadius = property.FindPropertyRelative("uniformTipRadius");

                Vector4 radiusVectorValue = cornerRadius.vector4Value;
                float radiusFloatValue = radiusVectorValue.x;
                bool rectBoolVal = uniformCornerRadius.boolValue;
                float[] zw = new[] { radiusVectorValue.w, radiusVectorValue.z };
                float[] xy = new[] { radiusVectorValue.x, radiusVectorValue.y };
                Vector2 triSizesVectorValue = triSizes.vector2Value;
                float triSizesFloatValue = triSizesVectorValue.x;
                Vector2 triRadiusVectorValue = tipRadius.vector2Value;
                float triRadiusFloatValue = triRadiusVectorValue.x;

                Rect line = position;
                line.height = EditorGUIUtility.singleLineHeight;
                string[] toolbarLabel = new[] { "自由", "统一" };
                EditorUtility.CornerRadiusModeGUI(line, ref uniformCornerRadius, toolbarLabel);
                line.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.BeginChangeCheck();
                {
                    EditorGUI.showMixedValue = cornerRadius.hasMultipleDifferentValues;
                    if (rectBoolVal)
                    {
                        radiusFloatValue = EditorGUI.FloatField(line, "均匀半径", radiusFloatValue);
                    }
                    else
                    {
                        EditorGUI.MultiFloatField(line, new[] { new GUIContent("W"), new GUIContent("Z") }, zw);
                        line.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                        EditorGUI.MultiFloatField(line, new[] { new GUIContent("X"), new GUIContent("Y") }, xy);
                    }
                    EditorGUI.showMixedValue = false;
                }
                if (EditorGUI.EndChangeCheck())
                {
                    cornerRadius.vector4Value = rectBoolVal
                        ? new Vector4(radiusFloatValue, radiusFloatValue, radiusFloatValue, radiusFloatValue)
                        : new Vector4(xy[0], xy[1], zw[1], zw[0]);
                }


                line.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorUtility.CornerRadiusModeGUI(line, ref uniformTipSize, toolbarLabel, "尖端大小");
                line.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.BeginChangeCheck();
                {
                    EditorGUI.showMixedValue = triSizes.hasMultipleDifferentValues;
                    if (uniformTipSize.boolValue)
                    {
                        triSizesFloatValue = EditorGUI.FloatField(line, "统一大小", triSizesFloatValue);
                    }
                    else
                    {
                        triSizesVectorValue = EditorGUI.Vector2Field(line, string.Empty, triSizesVectorValue);
                    }

                    EditorGUI.showMixedValue = false;
                }
                if (EditorGUI.EndChangeCheck())
                {
                    triSizes.vector2Value = uniformTipSize.boolValue
                        ? new Vector2(triSizesFloatValue, triSizesFloatValue)
                        : triSizesVectorValue;
                }


                line.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorUtility.CornerRadiusModeGUI(line, ref uniformTipRadius, toolbarLabel, "尖端半径");
                line.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.BeginChangeCheck();
                {
                    EditorGUI.showMixedValue = tipRadius.hasMultipleDifferentValues;
                    if (uniformTipRadius.boolValue)
                    {
                        triRadiusFloatValue = EditorGUI.FloatField(line, "均匀半径", triRadiusFloatValue);
                    }
                    else
                    {
                        triRadiusVectorValue = EditorGUI.Vector2Field(line, string.Empty, triRadiusVectorValue);
                    }

                    EditorGUI.showMixedValue = false;
                }
                if (EditorGUI.EndChangeCheck())
                {
                    tipRadius.vector2Value = uniformTipRadius.boolValue
                        ? new Vector2(triRadiusFloatValue, triRadiusFloatValue)
                        : triRadiusVectorValue;
                }

            }
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.FindPropertyRelative("uniformCornerRadius").boolValue)
            {
                return EditorGUIUtility.singleLineHeight * 6 + EditorGUIUtility.standardVerticalSpacing * 5;
            }
            return EditorGUIUtility.singleLineHeight * 7 + EditorGUIUtility.standardVerticalSpacing * 6;
        }
    }
}