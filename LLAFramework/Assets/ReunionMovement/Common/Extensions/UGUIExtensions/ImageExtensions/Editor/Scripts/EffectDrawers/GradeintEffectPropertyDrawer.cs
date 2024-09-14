using UnityEditor;
using UnityEngine;

namespace GameLogic.UI.ImageExtensions.Editor
{
    [CustomPropertyDrawer(typeof(GradientEffect))]
    public class GradeintEffectPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            {
                SerializedProperty Enabled = property.FindPropertyRelative("enabled");
                bool enabled = Enabled.boolValue;
                SerializedProperty gradientType = property.FindPropertyRelative("gradientType");
                GradientType gradType = (GradientType)gradientType.enumValueIndex;
                SerializedProperty gradient = property.FindPropertyRelative("gradient");
                SerializedProperty rotation = property.FindPropertyRelative("rotation");
                SerializedProperty cornerColors = property.FindPropertyRelative("cornerGradientColors");

                Rect line = position;
                line.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.BeginChangeCheck();
                {
                    EditorGUI.showMixedValue = Enabled.hasMultipleDifferentValues;
                    enabled = EditorGUI.Toggle(line, "渐变", enabled);
                    EditorGUI.showMixedValue = false;

                    if (enabled)
                    {
                        line.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                        EditorGUI.showMixedValue = gradientType.hasMultipleDifferentValues;
                        gradType = (GradientType)EditorGUI.EnumPopup(line, "类型", gradType);
                        EditorGUI.showMixedValue = false;
                    }
                }
                if (EditorGUI.EndChangeCheck())
                {
                    Enabled.boolValue = enabled;
                    gradientType.enumValueIndex = (int)gradType;
                }

                if (enabled)
                {
                    if (gradType == GradientType.Corner)
                    {

                        if (cornerColors.arraySize != 4)
                            cornerColors.arraySize = 4;

                        line.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                        float colFieldWidth = line.width / 2f - 5f;
                        line.width = colFieldWidth;
                        EditorGUI.PropertyField(line, cornerColors.GetArrayElementAtIndex(0), GUIContent.none);
                        line.x += colFieldWidth + 10;
                        EditorGUI.PropertyField(line, cornerColors.GetArrayElementAtIndex(1), GUIContent.none);
                        line.x -= colFieldWidth + 10;
                        line.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                        EditorGUI.PropertyField(line, cornerColors.GetArrayElementAtIndex(2), GUIContent.none);
                        line.x += colFieldWidth + 10;
                        EditorGUI.PropertyField(line, cornerColors.GetArrayElementAtIndex(3), GUIContent.none);
                        line.x -= colFieldWidth + 10;
                        line.width = colFieldWidth * 2 + 10;
                    }
                    else
                    {
                        line.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                        EditorGUI.showMixedValue = gradient.hasMultipleDifferentValues;
                        EditorGUI.PropertyField(line, gradient, false);

                        if (gradType == GradientType.Linear)
                        {
                            line.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                            EditorGUI.showMixedValue = rotation.hasMultipleDifferentValues;
                            EditorGUI.PropertyField(line, rotation, new GUIContent("旋转"));
                        }

                        EditorGUI.showMixedValue = false;
                    }
                }
            }
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty enabled = property.FindPropertyRelative("enabled");
            if (enabled.boolValue)
            {
                SerializedProperty gradientMode = property.FindPropertyRelative("gradientType");
                if (gradientMode.enumValueIndex == (int)GradientType.Radial)
                {
                    return EditorGUIUtility.singleLineHeight * 3 + EditorGUIUtility.standardVerticalSpacing * 2;
                }
                return EditorGUIUtility.singleLineHeight * 4 + EditorGUIUtility.standardVerticalSpacing * 3;
            }
            return EditorGUIUtility.singleLineHeight;
        }
    }
}