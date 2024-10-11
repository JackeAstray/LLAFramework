using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    static class VertexUtility
    {
        /// <summary>
        /// 分配并填充所有属性数组。此方法将填充所有数组，无论实际数据是否填充值（使用HasAttribute()检查顶点包含哪些属性）。
        /// </summary>
        /// <remarks>
        /// 如果使用此函数重建网格，请改用SetMesh。SetMesh处理在适当的地方设置空数组。
        /// </remarks>
        /// <seealso cref="SetMesh"/>
        /// <param name="vertices">The source vertices.</param>
        /// <param name="position">A new array of the vertex position values.</param>
        /// <param name="color">A new array of the vertex color values.</param>
        /// <param name="uv0">A new array of the vertex uv0 values.</param>
        /// <param name="normal">A new array of the vertex normal values.</param>
        /// <param name="tangent">A new array of the vertex tangent values.</param>
        /// <param name="uv2">A new array of the vertex uv2 values.</param>
        /// <param name="uv3">A new array of the vertex uv3 values.</param>
        /// <param name="uv4">A new array of the vertex uv4 values.</param>
        public static void GetArrays(
            IList<Vertex> vertices,
            out Vector3[] position,
            out Color[] color,
            out Vector2[] uv0,
            out Vector3[] normal,
            out Vector4[] tangent,
            out Vector2[] uv2,
            out List<Vector4> uv3,
            out List<Vector4> uv4)
        {
            GetArrays(vertices, out position, out color, out uv0, out normal, out tangent, out uv2, out uv3, out uv4, VertexAttributes.All);
        }

        /// <summary>
        /// 分配并填充所请求的属性数组
        /// </summary>
        /// <remarks>
        /// 如果使用此函数重建网格，请改用SetMesh。SetMesh处理在适当的地方设置空数组。
        /// </remarks>
        /// <seealso cref="SetMesh"/>
        /// <param name="vertices">The source vertices.</param>
        /// <param name="position">A new array of the vertex position values if requested by the attributes parameter, or null.</param>
        /// <param name="color">A new array of the vertex color values if requested by the attributes parameter, or null.</param>
        /// <param name="uv0">A new array of the vertex uv0 values if requested by the attributes parameter, or null.</param>
        /// <param name="normal">A new array of the vertex normal values if requested by the attributes parameter, or null.</param>
        /// <param name="tangent">A new array of the vertex tangent values if requested by the attributes parameter, or null.</param>
        /// <param name="uv2">A new array of the vertex uv2 values if requested by the attributes parameter, or null.</param>
        /// <param name="uv3">A new array of the vertex uv3 values if requested by the attributes parameter, or null.</param>
        /// <param name="uv4">A new array of the vertex uv4 values if requested by the attributes parameter, or null.</param>
        /// <param name="attributes">A flag with the MeshAttributes requested.</param>
        /// <seealso cref="HasArrays"/>
        public static void GetArrays(
            IList<Vertex> vertices,
            out Vector3[] position,
            out Color[] color,
            out Vector2[] uv0,
            out Vector3[] normal,
            out Vector4[] tangent,
            out Vector2[] uv2,
            out List<Vector4> uv3,
            out List<Vector4> uv4,
            VertexAttributes attributes)
        {
            if (vertices == null)
                throw new ArgumentNullException("vertices");

            int vc = vertices.Count;
            var first = vc < 1 ? new Vertex() : vertices[0];

            bool hasPosition = ((attributes & VertexAttributes.Position) == VertexAttributes.Position) && first.HasPosition;
            bool hasColor = ((attributes & VertexAttributes.Color) == VertexAttributes.Color) && first.HasColor;
            bool hasUv0 = ((attributes & VertexAttributes.Texture0) == VertexAttributes.Texture0) && first.HasUV0;
            bool hasNormal = ((attributes & VertexAttributes.Normal) == VertexAttributes.Normal) && first.HasNormal;
            bool hasTangent = ((attributes & VertexAttributes.Tangent) == VertexAttributes.Tangent) && first.HasTangent;
            bool hasUv2 = ((attributes & VertexAttributes.Texture1) == VertexAttributes.Texture1) && first.HasUV2;
            bool hasUv3 = ((attributes & VertexAttributes.Texture2) == VertexAttributes.Texture2) && first.HasUV3;
            bool hasUv4 = ((attributes & VertexAttributes.Texture3) == VertexAttributes.Texture3) && first.HasUV4;

            position = hasPosition ? new Vector3[vc] : null;
            color = hasColor ? new Color[vc] : null;
            uv0 = hasUv0 ? new Vector2[vc] : null;
            normal = hasNormal ? new Vector3[vc] : null;
            tangent = hasTangent ? new Vector4[vc] : null;
            uv2 = hasUv2 ? new Vector2[vc] : null;
            uv3 = hasUv3 ? new List<Vector4>(vc) : null;
            uv4 = hasUv4 ? new List<Vector4>(vc) : null;

            for (int i = 0; i < vc; i++)
            {
                if (hasPosition)
                    position[i] = vertices[i].Position;
                if (hasColor)
                    color[i] = vertices[i].Color;
                if (hasUv0)
                    uv0[i] = vertices[i].UV0;
                if (hasNormal)
                    normal[i] = vertices[i].Normal;
                if (hasTangent)
                    tangent[i] = vertices[i].Tangent;
                if (hasUv2)
                    uv2[i] = vertices[i].UV2;
                if (hasUv3)
                    uv3.Add(vertices[i].UV3);
                if (hasUv4)
                    uv4.Add(vertices[i].UV4);
            }
        }

        /// <summary>
        /// 获取网格的顶点数组
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static Vertex[] GetVertices(this Mesh mesh)
        {
            if (mesh == null)
                return null;

            int vertexCount = mesh.vertexCount;
            Vertex[] v = new Vertex[vertexCount];

            Vector3[] positions = mesh.vertices;
            Color[] colors = mesh.colors;
            Vector3[] normals = mesh.normals;
            Vector4[] tangents = mesh.tangents;
            Vector2[] uv0s = mesh.uv;
            Vector2[] uv2s = mesh.uv2;
            List<Vector4> uv3s = new List<Vector4>();
            List<Vector4> uv4s = new List<Vector4>();
            mesh.GetUVs(2, uv3s);
            mesh.GetUVs(3, uv4s);

            bool hasPositions = positions != null && positions.Length == vertexCount;
            bool hasColors = colors != null && colors.Length == vertexCount;
            bool hasNormals = normals != null && normals.Length == vertexCount;
            bool hasTangents = tangents != null && tangents.Length == vertexCount;
            bool hasUv0 = uv0s != null && uv0s.Length == vertexCount;
            bool hasUv2 = uv2s != null && uv2s.Length == vertexCount;
            bool hasUv3 = uv3s.Count == vertexCount;
            bool hasUv4 = uv4s.Count == vertexCount;

            for (int i = 0; i < vertexCount; i++)
            {
                v[i] = new Vertex();

                if (hasPositions)
                    v[i].Position = positions[i];

                if (hasColors)
                    v[i].Color = colors[i];

                if (hasNormals)
                    v[i].Normal = normals[i];

                if (hasTangents)
                    v[i].Tangent = tangents[i];

                if (hasUv0)
                    v[i].UV0 = uv0s[i];

                if (hasUv2)
                    v[i].UV2 = uv2s[i];

                if (hasUv3)
                    v[i].UV3 = uv3s[i];

                if (hasUv4)
                    v[i].UV4 = uv4s[i];
            }

            return v;
        }

        /// <summary>
        /// 将网格值替换为顶点数组。
        /// 此函数期间会清除网格，因此请务必在调用后设置三角形。
        /// </summary>
        /// <param name="mesh">The target mesh.</param>
        /// <param name="vertices">The vertices to replace the mesh attributes with.</param>
        public static void SetMesh(Mesh mesh, IList<Vertex> vertices)
        {
            if (mesh == null)
                throw new ArgumentNullException("mesh");

            if (vertices == null)
                throw new ArgumentNullException("vertices");

            Vector3[] positions = null;
            Color[] colors = null;
            Vector2[] uv0s = null;
            Vector3[] normals = null;
            Vector4[] tangents = null;
            Vector2[] uv2s = null;
            List<Vector4> uv3s = null;
            List<Vector4> uv4s = null;

            GetArrays(vertices, out positions,
                out colors,
                out uv0s,
                out normals,
                out tangents,
                out uv2s,
                out uv3s,
                out uv4s);

            mesh.Clear();

            Vertex first = vertices[0];

            if (first.HasPosition) mesh.vertices = positions;
            if (first.HasColor) mesh.colors = colors;
            if (first.HasUV0) mesh.uv = uv0s;
            if (first.HasNormal) mesh.normals = normals;
            if (first.HasTangent) mesh.tangents = tangents;
            if (first.HasUV2) mesh.uv2 = uv2s;
            if (first.HasUV3)
                if (uv3s != null)
                    mesh.SetUVs(2, uv3s);
            if (first.HasUV4)
                if (uv4s != null)
                    mesh.SetUVs(3, uv4s);
        }

        /// <summary>
        /// 在两个顶点之间进行线性插值
        /// </summary>
        /// <param name="x">Left parameter.</param>
        /// <param name="y">Right parameter.</param>
        /// <param name="weight">The weight of the interpolation. 0 is fully x, 1 is fully y.</param>
        /// <returns>A new vertex interpolated by weight between x and y.</returns>
        public static Vertex Mix(this Vertex x, Vertex y, float weight)
        {
            float i = 1f - weight;

            Vertex v = new Vertex();

            v.Position = x.Position * i + y.Position * weight;

            if (x.HasColor && y.HasColor)
                v.Color = x.Color * i + y.Color * weight;
            else if (x.HasColor)
                v.Color = x.Color;
            else if (y.HasColor)
                v.Color = y.Color;

            if (x.HasNormal && y.HasNormal)
                v.Normal = x.Normal * i + y.Normal * weight;
            else if (x.HasNormal)
                v.Normal = x.Normal;
            else if (y.HasNormal)
                v.Normal = y.Normal;

            if (x.HasTangent && y.HasTangent)
                v.Tangent = x.Tangent * i + y.Tangent * weight;
            else if (x.HasTangent)
                v.Tangent = x.Tangent;
            else if (y.HasTangent)
                v.Tangent = y.Tangent;

            if (x.HasUV0 && y.HasUV0)
                v.UV0 = x.UV0 * i + y.UV0 * weight;
            else if (x.HasUV0)
                v.UV0 = x.UV0;
            else if (y.HasUV0)
                v.UV0 = y.UV0;

            if (x.HasUV2 && y.HasUV2)
                v.UV2 = x.UV2 * i + y.UV2 * weight;
            else if (x.HasUV2)
                v.UV2 = x.UV2;
            else if (y.HasUV2)
                v.UV2 = y.UV2;

            if (x.HasUV3 && y.HasUV3)
                v.UV3 = x.UV3 * i + y.UV3 * weight;
            else if (x.HasUV3)
                v.UV3 = x.UV3;
            else if (y.HasUV3)
                v.UV3 = y.UV3;

            if (x.HasUV4 && y.HasUV4)
                v.UV4 = x.UV4 * i + y.UV4 * weight;
            else if (x.HasUV4)
                v.UV4 = x.UV4;
            else if (y.HasUV4)
                v.UV4 = y.UV4;

            return v;
        }

        /// <summary>
        /// 将顶点转换为世界空间
        /// </summary>
        /// <param name="transform">The transform to apply.</param>
        /// <param name="vertex">A model space vertex.</param>
        /// <returns>A new vertex in world coordinate space.</returns>
        public static Vertex TransformVertex(this Transform transform, Vertex vertex)
        {
            var v = new Vertex();

            if (vertex.HasArrays(VertexAttributes.Position))
                v.Position = transform.TransformPoint(vertex.Position);

            if (vertex.HasArrays(VertexAttributes.Color))
                v.Color = vertex.Color;

            if (vertex.HasArrays(VertexAttributes.Normal))
                v.Normal = transform.TransformDirection(vertex.Normal);

            if (vertex.HasArrays(VertexAttributes.Tangent))
                v.Tangent = transform.rotation * vertex.Tangent;

            if (vertex.HasArrays(VertexAttributes.Texture0))
                v.UV0 = vertex.UV0;

            if (vertex.HasArrays(VertexAttributes.Texture1))
                v.UV2 = vertex.UV2;

            if (vertex.HasArrays(VertexAttributes.Texture2))
                v.UV3 = vertex.UV3;

            if (vertex.HasArrays(VertexAttributes.Texture3))
                v.UV4 = vertex.UV4;

            return v;
        }
    }
}