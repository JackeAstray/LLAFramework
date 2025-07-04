using UnityEditor;
using UnityEngine;

namespace LLAFramework.UI.ImageExtensions.Editor
{
    [CustomPropertyDrawer(typeof(TriangleImg))]
    public class TrianglePropertyDrawer : PropertyDrawer
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

                float floatVal = cornerRadius.vector3Value.x;
                Vector3 vectorValue = cornerRadius.vector3Value;

                EditorGUI.BeginChangeCheck();
                {
                    if (uniformCornerRadius.boolValue)
                    {
                        floatVal = EditorGUI.FloatField(radiusVectorRect, "均匀半径", floatVal);
                    }
                    else
                    {
                        vectorValue = EditorGUI.Vector3Field(radiusVectorRect, string.Empty, vectorValue);
                    }
                }
                if (EditorGUI.EndChangeCheck())
                {
                    cornerRadius.vector3Value = uniformCornerRadius.boolValue ? new Vector3(floatVal, floatVal, floatVal) : vectorValue;
                }
            }
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing * 2;
        }
    }
}