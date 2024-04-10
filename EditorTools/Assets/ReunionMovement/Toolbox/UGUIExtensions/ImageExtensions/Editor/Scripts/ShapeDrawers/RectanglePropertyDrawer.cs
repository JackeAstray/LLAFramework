using UnityEditor;
using UnityEngine;

namespace GameLogic.UI.ImageExtensions.Editor
{
    [CustomPropertyDrawer(typeof(RectangleImg))]
    public class RectanglePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            {
                Rect LabelRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
                Rect RadiusVectorRect = new Rect(position.x,
                    position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing,
                    position.width, EditorGUIUtility.singleLineHeight);

                SerializedProperty uniform = property.FindPropertyRelative("uniformCornerRadius");
                SerializedProperty radius = property.FindPropertyRelative("cornerRadius");

                EditorUtility.CornerRadiusModeGUI(LabelRect, ref uniform, new[] { "自由", "一致" });

                float floatVal = radius.vector4Value.x;
                Vector4 vectorValue = radius.vector4Value;
                float[] zw = new[] { vectorValue.w, vectorValue.z };
                float[] xy = new[] { vectorValue.x, vectorValue.y };

                EditorGUI.BeginChangeCheck();
                {
                    EditorGUI.showMixedValue = radius.hasMultipleDifferentValues;
                    if (uniform.boolValue)
                    {
                        floatVal = EditorGUI.FloatField(RadiusVectorRect, "均匀半径", floatVal);
                    }
                    else
                    {
                        EditorGUI.MultiFloatField(RadiusVectorRect, new[] {
                            new GUIContent("W"), new GUIContent("Z")}, zw);
                        RadiusVectorRect.y +=
                            EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                        EditorGUI.MultiFloatField(RadiusVectorRect, new[] {
                            new GUIContent("X "), new GUIContent("Y")}, xy);

                    }
                    EditorGUI.showMixedValue = false;
                }
                if (EditorGUI.EndChangeCheck())
                {
                    radius.vector4Value = uniform.boolValue
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