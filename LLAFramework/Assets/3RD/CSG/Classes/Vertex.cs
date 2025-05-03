using System;
using UnityEngine;

namespace LLAFramework
{
    /// <summary>
    /// 保存单个顶点的信息，并提供在多个顶点之间求平均值的方法
    /// （所有值都是可选的。如果没有默认值，必要时将替换为默认值）
    /// </summary>
    public struct Vertex
    {
        Vector3 position;
        Color color;
        Vector3 normal;
        Vector4 tangent;
        Vector2 uv0;
        Vector2 uv2;
        Vector4 uv3;
        Vector4 uv4;
        VertexAttributes attributes;

        /// <value>
        /// 模型空间中的位置
        /// </value>
        public Vector3 Position
        {
            get { return position; }
            set
            {
                HasPosition = true;
                position = value;
            }
        }

        /// <value>
        /// 顶点颜色
        /// </value>
        public Color Color
        {
            get { return color; }
            set
            {
                HasColor = true;
                color = value;
            }
        }

        /// <value>
        /// 单位向量法线
        /// </value>
        public Vector3 Normal
        {
            get { return normal; }
            set
            {
                HasNormal = true;
                normal = value;
            }
        }

        /// <value>
        /// 顶点切线（有时称为binormal）
        /// </value>
        public Vector4 Tangent
        {
            get { return tangent; }
            set
            {
                HasTangent = true;
                tangent = value;
            }
        }

        /// <value>
        /// UV 0 通道. 也称为纹理
        /// </value>
        public Vector2 UV0
        {
            get { return uv0; }
            set
            {
                HasUV0 = true;
                uv0 = value;
            }
        }

        /// <value>
        /// UV 2 通道.
        /// </value>
        public Vector2 UV2
        {
            get { return uv2; }
            set
            {
                HasUV2 = true;
                uv2 = value;
            }
        }

        /// <value>
        /// UV 3 通道.
        /// </value>
        public Vector4 UV3
        {
            get { return uv3; }
            set
            {
                HasUV3 = true;
                uv3 = value;
            }
        }

        /// <value>
        /// UV 4 通道.
        /// </value>
        public Vector4 UV4
        {
            get { return uv4; }
            set
            {
                HasUV4 = true;
                uv4 = value;
            }
        }

        /// <summary>
        /// 查找是否已设置顶点属性
        /// </summary>
        /// <param name="attribute">要测试的一个或多个属性</param>
        /// <returns>如果此顶点设置了指定的属性，则为True，如果这些属性是默认值，则为false</returns>
        public bool HasArrays(VertexAttributes attribute)
        {
            return (attributes & attribute) == attribute;
        }

        /// <summary>
        /// 是否有位置
        /// </summary>
        public bool HasPosition
        {
            get { return (attributes & VertexAttributes.Position) == VertexAttributes.Position; }
            private set { attributes = value ? (attributes | VertexAttributes.Position) : (attributes & ~(VertexAttributes.Position)); }
        }

        /// <summary>
        /// 是否有颜色
        /// </summary>
        public bool HasColor
        {
            get { return (attributes & VertexAttributes.Color) == VertexAttributes.Color; }
            private set { attributes = value ? (attributes | VertexAttributes.Color) : (attributes & ~(VertexAttributes.Color)); }
        }

        /// <summary>
        /// 是否有法线
        /// </summary>
        public bool HasNormal
        {
            get { return (attributes & VertexAttributes.Normal) == VertexAttributes.Normal; }
            private set { attributes = value ? (attributes | VertexAttributes.Normal) : (attributes & ~(VertexAttributes.Normal)); }
        }

        /// <summary>
        /// 是否有切线
        /// </summary>
        public bool HasTangent
        {
            get { return (attributes & VertexAttributes.Tangent) == VertexAttributes.Tangent; }
            private set { attributes = value ? (attributes | VertexAttributes.Tangent) : (attributes & ~(VertexAttributes.Tangent)); }
        }

        /// <summary>
        /// 是否有UV0
        /// </summary>
        public bool HasUV0
        {
            get { return (attributes & VertexAttributes.Texture0) == VertexAttributes.Texture0; }
            private set { attributes = value ? (attributes | VertexAttributes.Texture0) : (attributes & ~(VertexAttributes.Texture0)); }
        }

        /// <summary>
        /// 是否有UV1
        /// </summary>
        public bool HasUV2
        {
            get { return (attributes & VertexAttributes.Texture1) == VertexAttributes.Texture1; }
            private set { attributes = value ? (attributes | VertexAttributes.Texture1) : (attributes & ~(VertexAttributes.Texture1)); }
        }

        /// <summary>
        /// 是否有UV3
        /// </summary>
        public bool HasUV3
        {
            get { return (attributes & VertexAttributes.Texture2) == VertexAttributes.Texture2; }
            private set { attributes = value ? (attributes | VertexAttributes.Texture2) : (attributes & ~(VertexAttributes.Texture2)); }
        }
        /// <summary>
        /// 是否有UV4
        /// </summary>
        public bool HasUV4
        {
            get { return (attributes & VertexAttributes.Texture3) == VertexAttributes.Texture3; }
            private set { attributes = value ? (attributes | VertexAttributes.Texture3) : (attributes & ~(VertexAttributes.Texture3)); }
        }

        /// <summary>
        /// 翻转顶点的法线和切线方向
        /// </summary>
        public void Flip()
        {
            if (HasNormal)
                normal *= -1f;

            if (HasTangent)
                tangent *= -1f;
        }
    }
}