using System;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 保存单个顶点的信息，并提供在多个顶点之间求平均值的方法
    /// （所有值都是可选的。如果没有默认值，必要时将替换为默认值）
    /// </summary>
    public struct Vertex
    {
        Vector3 m_Position;
        Color m_Color;
        Vector3 m_Normal;
        Vector4 m_Tangent;
        Vector2 m_UV0;
        Vector2 m_UV2;
        Vector4 m_UV3;
        Vector4 m_UV4;
        VertexAttributes m_Attributes;

        /// <value>
        /// 模型空间中的位置
        /// </value>
        public Vector3 position
        {
            get { return m_Position; }
            set
            {
                hasPosition = true;
                m_Position = value;
            }
        }

        /// <value>
        /// 顶点颜色
        /// </value>
        public Color color
        {
            get { return m_Color; }
            set
            {
                hasColor = true;
                m_Color = value;
            }
        }

        /// <value>
        /// 单位向量法线
        /// </value>
        public Vector3 normal
        {
            get { return m_Normal; }
            set
            {
                hasNormal = true;
                m_Normal = value;
            }
        }

        /// <value>
        /// 顶点切线（有时称为binormal）
        /// </value>
        public Vector4 tangent
        {
            get { return m_Tangent; }
            set
            {
                hasTangent = true;
                m_Tangent = value;
            }
        }

        /// <value>
        /// UV 0 通道. 也称为纹理
        /// </value>
        public Vector2 uv0
        {
            get { return m_UV0; }
            set
            {
                hasUV0 = true;
                m_UV0 = value;
            }
        }

        /// <value>
        /// UV 2 通道.
        /// </value>
        public Vector2 uv2
        {
            get { return m_UV2; }
            set
            {
                hasUV2 = true;
                m_UV2 = value;
            }
        }

        /// <value>
        /// UV 3 通道.
        /// </value>
        public Vector4 uv3
        {
            get { return m_UV3; }
            set
            {
                hasUV3 = true;
                m_UV3 = value;
            }
        }

        /// <value>
        /// UV 4 通道.
        /// </value>
        public Vector4 uv4
        {
            get { return m_UV4; }
            set
            {
                hasUV4 = true;
                m_UV4 = value;
            }
        }

        /// <summary>
        /// 查找是否已设置顶点属性
        /// </summary>
        /// <param name="attribute">要测试的一个或多个属性</param>
        /// <returns>如果此顶点设置了指定的属性，则为True，如果这些属性是默认值，则为false</returns>
        public bool HasArrays(VertexAttributes attribute)
        {
            return (m_Attributes & attribute) == attribute;
        }

        /// <summary>
        /// 是否有位置
        /// </summary>
        public bool hasPosition
        {
            get { return (m_Attributes & VertexAttributes.Position) == VertexAttributes.Position; }
            private set { m_Attributes = value ? (m_Attributes | VertexAttributes.Position) : (m_Attributes & ~(VertexAttributes.Position)); }
        }

        /// <summary>
        /// 是否有颜色
        /// </summary>
        public bool hasColor
        {
            get { return (m_Attributes & VertexAttributes.Color) == VertexAttributes.Color; }
            private set { m_Attributes = value ? (m_Attributes | VertexAttributes.Color) : (m_Attributes & ~(VertexAttributes.Color)); }
        }

        /// <summary>
        /// 是否有法线
        /// </summary>
        public bool hasNormal
        {
            get { return (m_Attributes & VertexAttributes.Normal) == VertexAttributes.Normal; }
            private set { m_Attributes = value ? (m_Attributes | VertexAttributes.Normal) : (m_Attributes & ~(VertexAttributes.Normal)); }
        }

        /// <summary>
        /// 是否有切线
        /// </summary>
        public bool hasTangent
        {
            get { return (m_Attributes & VertexAttributes.Tangent) == VertexAttributes.Tangent; }
            private set { m_Attributes = value ? (m_Attributes | VertexAttributes.Tangent) : (m_Attributes & ~(VertexAttributes.Tangent)); }
        }

        /// <summary>
        /// 是否有UV0
        /// </summary>
        public bool hasUV0
        {
            get { return (m_Attributes & VertexAttributes.Texture0) == VertexAttributes.Texture0; }
            private set { m_Attributes = value ? (m_Attributes | VertexAttributes.Texture0) : (m_Attributes & ~(VertexAttributes.Texture0)); }
        }

        /// <summary>
        /// 是否有UV1
        /// </summary>
        public bool hasUV2
        {
            get { return (m_Attributes & VertexAttributes.Texture1) == VertexAttributes.Texture1; }
            private set { m_Attributes = value ? (m_Attributes | VertexAttributes.Texture1) : (m_Attributes & ~(VertexAttributes.Texture1)); }
        }

        /// <summary>
        /// 是否有UV3
        /// </summary>
        public bool hasUV3
        {
            get { return (m_Attributes & VertexAttributes.Texture2) == VertexAttributes.Texture2; }
            private set { m_Attributes = value ? (m_Attributes | VertexAttributes.Texture2) : (m_Attributes & ~(VertexAttributes.Texture2)); }
        }
        /// <summary>
        /// 是否有UV4
        /// </summary>
        public bool hasUV4
        {
            get { return (m_Attributes & VertexAttributes.Texture3) == VertexAttributes.Texture3; }
            private set { m_Attributes = value ? (m_Attributes | VertexAttributes.Texture3) : (m_Attributes & ~(VertexAttributes.Texture3)); }
        }

        /// <summary>
        /// 翻转顶点的法线和切线方向
        /// </summary>
        public void Flip()
        {
            if (hasNormal)
                m_Normal *= -1f;

            if (hasTangent)
                m_Tangent *= -1f;
        }
    }
}