using UnityEngine;
using System.Collections.Generic;

namespace GameLogic
{
    /// <summary>
    /// 平面 （不包含位置）
    /// </summary>
    sealed class Plane
    {
        public Vector3 normal;
        public float w;

        [System.Flags]
        enum EPolygonType
        {
            Coplanar = 0,
            Front = 1,
            Back = 2,
            Spanning = 3         /// 3 is Front | Back - not a separate entry
        };

        public Plane()
        {
            normal = Vector3.zero;
            w = 0f;
        }

        public Plane(Vector3 a, Vector3 b, Vector3 c)
        {
            normal = Vector3.Cross(b - a, c - a);//.normalized;
            w = Vector3.Dot(normal, a);
        }

        public override string ToString() => $"{normal} {w}";

        public bool Valid()
        {
            return normal.magnitude > 0f;
        }

        public void Flip()
        {
            normal *= -1f;
            w *= -1f;
        }

        /// <summary>
        /// 如果需要，用该平面分割“polygon”，然后将多边形或多边形片段放入适当的列表中。
        /// 共面多边形根据其相对于该平面的方向进入“coplanarFront”或“coplanarBack”。
        /// 位于该平面前面或后面的多边形进入“front”或“back”。
        /// </summary>
        /// <param name="polygon"></param>
        /// <param name="coplanarFront"></param>
        /// <param name="coplanarBack"></param>
        /// <param name="front"></param>
        /// <param name="back"></param>
        public void SplitPolygon(Polygon polygon, List<Polygon> coplanarFront, List<Polygon> coplanarBack, List<Polygon> front, List<Polygon> back)
        {
            // 将每个点以及整个多边形分类为上述四类之一
            EPolygonType polygonType = 0;
            List<EPolygonType> types = new List<EPolygonType>();

            for (int i = 0; i < polygon.vertices.Count; i++)
            {
                float t = Vector3.Dot(this.normal, polygon.vertices[i].Position) - this.w;
                EPolygonType type = (t < -CSG.Epsilon) ? EPolygonType.Back : ((t > CSG.Epsilon) ? EPolygonType.Front : EPolygonType.Coplanar);
                polygonType |= type;
                types.Add(type);
            }

            // 将多边形放入正确的列表中，必要时将其拆分。
            switch (polygonType)
            {
                case EPolygonType.Coplanar:
                    {
                        if (Vector3.Dot(this.normal, polygon.plane.normal) > 0)
                            coplanarFront.Add(polygon);
                        else
                            coplanarBack.Add(polygon);
                    }
                    break;

                case EPolygonType.Front:
                    {
                        front.Add(polygon);
                    }
                    break;

                case EPolygonType.Back:
                    {
                        back.Add(polygon);
                    }
                    break;

                case EPolygonType.Spanning:
                    {
                        List<Vertex> f = new List<Vertex>();
                        List<Vertex> b = new List<Vertex>();

                        for (int i = 0; i < polygon.vertices.Count; i++)
                        {
                            int j = (i + 1) % polygon.vertices.Count;

                            EPolygonType ti = types[i], tj = types[j];

                            Vertex vi = polygon.vertices[i], vj = polygon.vertices[j];

                            if (ti != EPolygonType.Back)
                            {
                                f.Add(vi);
                            }

                            if (ti != EPolygonType.Front)
                            {
                                b.Add(vi);
                            }

                            if ((ti | tj) == EPolygonType.Spanning)
                            {
                                float t = (this.w - Vector3.Dot(this.normal, vi.Position)) / Vector3.Dot(this.normal, vj.Position - vi.Position);

                                Vertex v = VertexUtility.Mix(vi, vj, t);

                                f.Add(v);
                                b.Add(v);
                            }
                        }

                        if (f.Count >= 3)
                        {
                            front.Add(new Polygon(f, polygon.material));
                        }

                        if (b.Count >= 3)
                        {
                            back.Add(new Polygon(b, polygon.material));
                        }
                    }
                    break;
            }   // End switch(polygonType)
        }
    }
}