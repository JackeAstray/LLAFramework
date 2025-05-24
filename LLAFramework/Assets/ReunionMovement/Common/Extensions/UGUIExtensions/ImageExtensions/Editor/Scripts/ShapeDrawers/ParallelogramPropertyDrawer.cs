using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace LLAFramework.UI.ImageExtensions.Editor
{
    [CustomPropertyDrawer(typeof(ParallelogramImg))]
    public class ParallelogramPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            {
                SerializedProperty parallelogramValue = property.FindPropertyRelative("parallelogramValue");

                float chamferBoxRadiusValue = parallelogramValue.floatValue;

                Rect line = position;
                line.height = EditorGUIUtility.singleLineHeight;

                EditorGUI.BeginChangeCheck();
                {
                    EditorGUI.showMixedValue = parallelogramValue.hasMultipleDifferentValues;
                    chamferBoxRadiusValue = EditorGUI.FloatField(line, "平行四边形值", chamferBoxRadiusValue);
                    EditorGUI.showMixedValue = false;
                }
                if (EditorGUI.EndChangeCheck())
                {
                    parallelogramValue.floatValue = chamferBoxRadiusValue;
                }
            }
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 1;
        }
    }
}
