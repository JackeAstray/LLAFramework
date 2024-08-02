using UnityEditor;
using UnityEngine;

namespace GameLogic.UI.ImageExtensions.Editor
{
    [CustomPropertyDrawer(typeof(PentagonImg))]
    public class PentagonPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            {
                SerializedProperty RectRadius = property.FindPropertyRelative("cornerRadius");
                SerializedProperty uniform = property.FindPropertyRelative("uniformCornerRadius");
                SerializedProperty triSize = property.FindPropertyRelative("tipSize");
                SerializedProperty triRadius = property.FindPropertyRelative("tipRadius");

                Vector4 radiusVectorValue = RectRadius.vector4Value;
                float radiusFloatValue = radiusVectorValue.x;
                bool boolVal = uniform.boolValue;

                float[] zw = new[] { radiusVectorValue.w, radiusVectorValue.z };
                float[] xy = new[] { radiusVectorValue.x, radiusVectorValue.y };

                Rect line = position;
                line.height = EditorGUIUtility.singleLineHeight;
                EditorUtility.CornerRadiusModeGUI(line, ref uniform, new[] { "自由", "统一" });
                line.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                EditorGUI.BeginChangeCheck();
                {
                    EditorGUI.showMixedValue = RectRadius.hasMultipleDifferentValues;
                    if (boolVal)
                    {
                        radiusFloatValue = EditorGUI.FloatField(line, "均匀半径", radiusFloatValue);
                    }
                    else
                    {
                        line.x += 10;
                        line.width -= 10;
                        EditorGUI.MultiFloatField(line, new[] { new GUIContent("W"), new GUIContent("Z"), }, zw);
                        line.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                        EditorGUI.MultiFloatField(line, new[] { new GUIContent("X "), new GUIContent("Y"), }, xy);
                        line.x -= 10;
                        line.width += 10;
                    }
                    EditorGUI.showMixedValue = false;
                }
                if (EditorGUI.EndChangeCheck())
                {
                    RectRadius.vector4Value = boolVal ? new Vector4(radiusFloatValue, radiusFloatValue, radiusFloatValue, radiusFloatValue) : new Vector4(xy[0], xy[1], zw[1], zw[0]);
                }

                line.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(line, triSize, new GUIContent("尖端大小"));//Tip size
                line.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(line, triRadius, new GUIContent("尖端半径"));//Tip Radius
            }
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.FindPropertyRelative("uniformCornerRadius").boolValue)
            {
                return EditorGUIUtility.singleLineHeight * 4 + EditorGUIUtility.standardVerticalSpacing * 3;
            }
            return EditorGUIUtility.singleLineHeight * 5 + EditorGUIUtility.standardVerticalSpacing * 4;
        }
    }
}