using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace GameLogic.UI.ImageExtensions.Editor
{
    public class EditorGUILayoutExtended : UnityEditor.Editor
    {
        private static readonly Type editorGUIType = typeof(EditorGUI);

        private static readonly Type RecycledTextEditorType =
            Assembly.GetAssembly(editorGUIType).GetType("UnityEditor.EditorGUI+RecycledTextEditor");

        private static readonly Type[] argumentTypes =
        {
            RecycledTextEditorType, typeof(Rect), typeof(Rect), typeof(int), typeof(float), typeof(string),
            typeof(GUIStyle), typeof(bool)
        };

        private static readonly MethodInfo doFloatFieldMethod = editorGUIType.GetMethod("DoFloatField",
            BindingFlags.NonPublic | BindingFlags.Static, null, argumentTypes, null);

        private static readonly FieldInfo fieldInfo =
            editorGUIType.GetField("s_RecycledEditor", BindingFlags.NonPublic | BindingFlags.Static);

        private static readonly object recycledEditor = fieldInfo.GetValue(null);
        private static readonly GUIStyle style = EditorStyles.numberField;

        public static float FloatFieldExtended(Rect _position, float _value, Rect _dragHotZone)
        {
            int controlId = GUIUtility.GetControlID("EditorTextField".GetHashCode(), FocusType.Keyboard, _position);
            object[] parameters = { recycledEditor, _position, _dragHotZone, controlId, _value, "g7", style, true };
            return (float)doFloatFieldMethod.Invoke(null, parameters);
        }

        public static float FloatField(GUIContent _content, float _value, float _labelwidth,
            params GUILayoutOption[] _options)
        {
            Rect totalRect = EditorGUILayout.GetControlRect(_options);
            
            Rect labelRect = new Rect(totalRect.x, totalRect.y, _labelwidth, totalRect.height);
            Rect inputRect = new Rect(totalRect.x + _labelwidth,
                                      totalRect.y,
                                      totalRect.width - _labelwidth,
                                      totalRect.height);

            EditorGUI.LabelField(labelRect, _content);
            return FloatFieldExtended(inputRect, _value, labelRect);
        }

    }
}