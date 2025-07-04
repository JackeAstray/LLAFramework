using UnityEditor;
using UnityEngine;

namespace LLAFramework.UI.ImageExtensions.Editor
{
    [CustomPropertyDrawer(typeof(RectangleImg))]
    public class RectanglePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            {
                Rect labelRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
                Rect radiusVectorRect = new Rect(position.x,
                    position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing,
                    position.width, EditorGUIUtility.singleLineHeight);

                SerializedProperty uniformCornerRadius = property.FindPropertyRelative("uniformCornerRadius");
                SerializedProperty cornerRadius = property.FindPropertyRelative("cornerRadius");

                EditorUtility.CornerRadiusModeGUI(labelRect, ref uniformCornerRadius, new[] { "自由", "统一" });

                float floatVal = cornerRadius.vector4Value.x;
                Vector4 vectorValue = cornerRadius.vector4Value;
                float[] zw = new[] { vectorValue.w, vectorValue.z };
                float[] xy = new[] { vectorValue.x, vectorValue.y };

                EditorGUI.BeginChangeCheck();
                {
                    EditorGUI.showMixedValue = cornerRadius.hasMultipleDifferentValues;
                    if (uniformCornerRadius.boolValue)
                    {
                        floatVal = EditorGUI.FloatField(radiusVectorRect, "均匀半径", floatVal);
                    }
                    else
                    {
                        EditorGUI.MultiFloatField(radiusVectorRect, new[] {
                            new GUIContent("W"), new GUIContent("Z")}, zw);
                        radiusVectorRect.y +=
                            EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                        EditorGUI.MultiFloatField(radiusVectorRect, new[] {
                            new GUIContent("X "), new GUIContent("Y")}, xy);

                    }
                    EditorGUI.showMixedValue = false;
                }
                if (EditorGUI.EndChangeCheck())
                {
                    cornerRadius.vector4Value = uniformCornerRadius.boolValue
                        ? new Vector4(floatVal, floatVal, floatVal, floatVal)
                        : new Vector4(xy[0], xy[1], zw[1], zw[0]);
                }
            }
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.FindPropertyRelative("uniformCornerRadius").boolValue)
            {
                return EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing;
            }
            return EditorGUIUtility.singleLineHeight * 3 + EditorGUIUtility.standardVerticalSpacing * 2;
        }
    }
}