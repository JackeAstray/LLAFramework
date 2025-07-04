using UnityEditor;
using UnityEngine;

namespace LLAFramework.UI.ImageExtensions.Editor
{
    [CustomPropertyDrawer(typeof(NStarPolygonImg))]
    public class NStarPolygonPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            {
                SerializedProperty sideCount = property.FindPropertyRelative("sideCount");
                SerializedProperty inset = property.FindPropertyRelative("inset");
                SerializedProperty cornerRadius = property.FindPropertyRelative("cornerRadius");
                SerializedProperty offset = property.FindPropertyRelative("offset");

                Rect line = position;
                line.height = EditorGUIUtility.singleLineHeight;

                EditorGUI.Slider(line, sideCount, 3f, 10f, "边数");
                line.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.Slider(line, inset, 2f, sideCount.floatValue - 0.01f, "插入");
                line.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(line, cornerRadius, new GUIContent("圆角半径"));
                line.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(line, offset, new GUIContent("偏移量"));
            }
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 4.5f + EditorGUIUtility.standardVerticalSpacing * 4;
        }
    }
}