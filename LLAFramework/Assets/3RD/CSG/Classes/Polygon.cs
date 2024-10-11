using UnityEngine;
using System.Collections.Generic;

namespace GameLogic
{
    /// <summary>
    /// 表示具有任意数量顶点的多边形面
    /// </summary>
    sealed class Polygon
    {
        public List<Vertex> vertices;
        public Plane plane;
        public Material material;

        public Polygon(List<Vertex> list, Material mat)
        {
            vertices = list;
            plane = new Plane(list[0].Position, list[1].Position, list[2].Position);
            material = mat;
        }

        public void Flip()
        {
            vertices.Reverse();

            for (int i = 0; i < vertices.Count; i++)
                vertices[i].Flip();

            plane.Flip();
        }

        public override string ToString()
        {
            return $"[{vertices.Count}] {plane.normal}";
        }
    }
}