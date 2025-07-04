using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LLAFramework.UI.ImageExtensions.Editor
{
    [CustomPropertyDrawer(typeof(BlobbyCrossImg))]
    internal class BlobbyCrossPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            {
                SerializedProperty blobbyCrossTime = property.FindPropertyRelative("blobbyCrossTime");

                Rect line = position;
                line.height = EditorGUIUtility.singleLineHeight;

                EditorGUI.Slider(line, blobbyCrossTime, 0f, 10f, "BlobbyCrossTime");
                line.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }
            EditorGUI.EndProperty();
        }
    }
}