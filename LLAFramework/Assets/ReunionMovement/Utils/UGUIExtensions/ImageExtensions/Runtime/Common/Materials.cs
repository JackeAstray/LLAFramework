using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LLAFramework.UI.ImageExtensions
{
    public class Materials
    {
        private const string basicProceduralShaderName = "ReunionMovement/UI/Basic Procedural Image";
        private static string[] shapeKeywords = { "CIRCLE", "TRIANGLE", "RECTANGLE", "NSTAR_POLYGON" };
        private const string strokeKeyword = "STROKE";
        private const string outlineKeyword = "OUTLINED";
        private const string outlinedStrokeKeyword = "OUTLINED_STROKE";
        private static Shader proceduralShader;

        internal static Shader BasicProceduralShader
        {
            get
            {
                if (proceduralShader == null)
                {
                    proceduralShader = Shader.Find(basicProceduralShaderName);
                }
                return proceduralShader;
            }
        }

        private static Material[] materialDB = new Material[16];

        /// <summary>
        /// 获取材质
        /// </summary>
        /// <param name="shapeIndex"></param>
        /// <param name="stroked"></param>
        /// <param name="outlined"></param>
        /// <returns></returns>
        internal static ref Material GetMaterial(int shapeIndex, bool stroked, bool outlined)
        {
            int index = shapeIndex * 4;
            if (stroked && outlined) index += 3;
            else if (outlined) index += 2;
            else if (stroked) index += 1;

            ref Material mat = ref materialDB[index];
            if (mat != null) return ref mat;

            mat = new Material(BasicProceduralShader);
            string shapeKeyword = shapeKeywords[shapeIndex];

            mat.name = $"Basic Procedural Sprite - {shapeKeyword} {(stroked ? strokeKeyword : string.Empty)} {(outlined ? outlineKeyword : string.Empty)}";
            mat.EnableKeyword(shapeKeyword);
            if (stroked && outlined) mat.EnableKeyword(outlinedStrokeKeyword);
            else if (stroked) mat.EnableKeyword(strokeKeyword);
            else if (outlined) mat.EnableKeyword(outlineKeyword);

            return ref mat;
        }
    }
}