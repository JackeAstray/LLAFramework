using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LLAFramework.UI.ImageExtensions
{
    internal struct VertexDataStream
    {
        internal RectTransform RectTransform;
        internal Vector2 Uv1, Uv2, Uv3;
        internal Vector3 Normal;
        internal Vector4 Tangent;
    }
}