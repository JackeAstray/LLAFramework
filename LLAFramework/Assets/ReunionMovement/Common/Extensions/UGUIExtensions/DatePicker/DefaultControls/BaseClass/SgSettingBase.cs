using System;
using UnityEngine;

namespace GameLogin.DatePicker
{
    [Serializable]
    public class SgSettingBase
    {
        // Axis
        [Header("Axis Setting")]
        public bool AxisEnable = false;
        public bool AxisArrowEnable = true;
        [Range(1.0f, 10.0f)]
        public float AxisWidth = 2.0f;
        public Color AxisColor = Color.black;

        [Space(3)]
        // Background
        [Header("Mesh Setting")]
        public bool MeshEnable = true;
        public bool HorizontalMeshEnable = true;
        public bool VerticalMeshEnable = true;
        public bool ImaginaryLine = true;
        [Range(5.0f, 1000f)]
        public float HorizontalSpacing = 50.0f;
        [Range(5.0f, 1000f)]
        public float VerticalSpacing = 50.0f;
        [Range(1.0f, 10.0f)]
        public float MeshWidth = 2.0f;
        public Color MeshColor = Color.black;

        [Space(3)]
        // Unit
        [Header("Unit Setting")]
        public bool UnitEnable = false;
        public string YUnit = "KWh";
        public float UnitYScale = 1000;
        public string XUnit = "";
        public float UnitXScale = 200;
    }
}