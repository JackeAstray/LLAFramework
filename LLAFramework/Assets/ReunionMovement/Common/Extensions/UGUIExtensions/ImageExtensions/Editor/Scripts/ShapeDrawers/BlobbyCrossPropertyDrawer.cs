using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GameLogic.UI.ImageExtensions.Editor
{
    [CustomPropertyDrawer(typeof(BlobbyCrossImg))]
    internal class BlobbyCrossPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            {
                SerializedProperty blobbyCrossTime = property.FindPropertyRelative("blobbyCrossTime");
                //SerializedProperty inset = property.FindPropertyRelative("inset");
                //SerializedProperty cornerRadius = property.FindPropertyRelative("cornerRadius");
                //SerializedProperty offset = property.FindPropertyRelative("offset");

                Rect line = position;
                line.height = EditorGUIUtility.singleLineHeight;

                EditorGUI.Slider(line, blobbyCrossTime, 0f, 10f, "BlobbyCrossTime");
                line.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                //EditorGUI.Slider(line, inset, 2f, sideCount.floatValue - 0.01f, "插入");
                //line.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                //EditorGUI.PropertyField(line, cornerRadius, new GUIContent("圆角半径"));
                //line.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                //EditorGUI.PropertyField(line, offset, new GUIContent("偏移量"));

                //Rect radiusRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
                //EditorGUI.LabelField(radiusRect, "没有可修改属性");
            }
            EditorGUI.EndProperty();
        }
    }
}