using UnityEditor;
using UnityEngine;

namespace GameLogic.UI.ImageExtensions.Editor
{
    public class ImageShaderGUI : ShaderGUI
    {
        private bool showValues;

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            EditorGUILayout.HelpBox("此处无需修改。在层次结构中选择一个ImageEx组件，然后修改检查器中的值。",MessageType.Info);
            
            if (GUILayout.Button(showValues ? "隐藏调试值" : "显示调试值", EditorStyles.miniLabel)) showValues = !showValues;

            if (showValues)
            {
                EditorGUI.BeginDisabledGroup(true);
                base.OnGUI(materialEditor, properties);
                EditorGUI.EndDisabledGroup();
            }
        }
    }
}