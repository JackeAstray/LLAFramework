using UnityEditor;
using UnityEngine;

namespace LLAFramework.UI.ImageExtensions.Editor
{
    [CustomPropertyDrawer(typeof(CircleImg))]
    public class CirclePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            {
                Rect radiusRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
                Rect toolBarRect = new Rect(position.x + EditorGUIUtility.labelWidth,
                    position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing,
                    position.width - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);

                SerializedProperty fitRadius = property.FindPropertyRelative("fitRadius");
                bool fitCirlce = fitRadius.boolValue;
                EditorGUI.BeginDisabledGroup(fitCirlce);
                {
                    EditorGUI.PropertyField(radiusRect, property.FindPropertyRelative("radius"), new GUIContent("半径"));
                }
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginChangeCheck();
                EditorGUI.showMixedValue = fitRadius.hasMultipleDifferentValues;
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(EditorGUIUtility.labelWidth);

                        fitCirlce = GUI.Toolbar(toolBarRect, fitCirlce ? 1 : 0, new[] { "自由", "拟合" }) == 1;
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUI.showMixedValue = false;

                if (EditorGUI.EndChangeCheck())
                {
                    fitRadius.boolValue = fitCirlce;
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