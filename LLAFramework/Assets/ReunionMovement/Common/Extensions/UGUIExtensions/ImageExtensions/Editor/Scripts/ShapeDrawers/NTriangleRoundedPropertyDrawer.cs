using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace GameLogic.UI.ImageExtensions
{
    [CustomPropertyDrawer(typeof(NTriangleRoundedImg))]
    public class NTriangleRoundedPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            {
                // 定义每个字段的高度
                float fieldHeight = EditorGUIUtility.singleLineHeight;
                float spacing = 2f; // 字段之间的间距

                // 定义第一个字段的 Rect
                Rect timeRect = new Rect(position.x, position.y, position.width, fieldHeight);

                // 定义第二个字段的 Rect，y 坐标向下偏移
                Rect numberRect = new Rect(position.x, position.y + fieldHeight + spacing, position.width, fieldHeight);

                // 绘制字段
                EditorGUI.PropertyField(timeRect, property.FindPropertyRelative("nTriangleRoundedTime"), new GUIContent("NTriangleRoundedTime"));
                EditorGUI.PropertyField(numberRect, property.FindPropertyRelative("nTriangleRoundedNumber"), new GUIContent("NTriangleRoundedNumber"));
            }
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing * 2;
        }
    }
}
