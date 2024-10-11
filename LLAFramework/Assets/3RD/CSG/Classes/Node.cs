using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GameLogic
{
    /// <summary>
    /// 节点
    /// </summary>
    sealed class Node
    {
        public List<Polygon> polygons;

        public Node front;
        public Node back;

        public Plane plane;

        public Node()
        {
            front = null;
            back = null;
        }

        public Node(List<Polygon> list)
        {
            Build(list);
        }

        public Node(List<Polygon> list, Plane plane, Node front, Node back)
        {
            this.polygons = list;
            this.plane = plane;
            this.front = front;
            this.back = back;
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public Node Clone()
        {
            Node clone = new Node(this.polygons, this.plane, this.front, this.back);

            return clone;
        }

        /// <summary>
        /// 删除此BSP树中位于另一个BSP树“BSP”内的所有多边形。
        /// </summary>
        /// <param name="other"></param>
        public void ClipTo(Node other)
        {
            this.polygons = other.ClipPolygons(this.polygons);

            if (this.front != null)
            {
                this.front.ClipTo(other);
            }

            if (this.back != null)
            {
                this.back.ClipTo(other);
            }
        }

        /// <summary>
        /// 将实体空间转换为空白空间，将空白空间转换为实体空间。
        /// </summary>
        public void Invert()
        {
            for (int i = 0; i < this.polygons.Count; i++)
                this.polygons[i].Flip();

            this.plane.Flip();

            if (this.front != null)
            {
                this.front.Invert();
            }

            if (this.back != null)
            {
                this.back.Invert();
            }

            Node tmp = this.front;
            this.front = this.back;
            this.back = tmp;
        }

        /// <summary>
        /// 用“多边形”构建BSP树。
        /// 在现有树上调用时，新多边形会向下过滤到树的底部，并在那里成为新节点。
        /// 每组多边形都使用第一个多边形进行分区（不使用启发式方法来选择一个好的分割）。
        /// </summary>
        /// <param name="list"></param>
        public void Build(List<Polygon> list)
        {
            if (list.Count < 1)
                return;

            bool newNode = plane == null || !plane.Valid();

            if (newNode)
            {
                plane = new Plane();
                plane.normal = list[0].plane.normal;
                plane.w = list[0].plane.w;
            }

            if (polygons == null)
                polygons = new List<Polygon>();

            var listFront = new List<Polygon>();
            var listBack = new List<Polygon>();

            for (int i = 0; i < list.Count; i++)
                plane.SplitPolygon(list[i], polygons, polygons, listFront, listBack);


            if (listFront.Count > 0)
            {
                // 当epsilon值过低时，SplitPolygon可能无法正确识别共面平面。什么时候？
                // 发生这种情况时，前列表或后列表将被递归填充并构建到新节点中。
                // 此检查会捕获该情况，并将前 / 后列表排序到共面多边形集合中。
                if (newNode && list.SequenceEqual(listFront))
                    polygons.AddRange(listFront);
                else
                    (front ?? (front = new Node())).Build(listFront);
            }

            if (listBack.Count > 0)
            {
                if (newNode && list.SequenceEqual(listBack))
                    polygons.AddRange(listBack);
                else
                    (back ?? (back = new Node())).Build(listBack);
            }
        }

        /// <summary>
        /// 递归删除此BSP树内“多边形”中的所有多边形
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public List<Polygon> ClipPolygons(List<Polygon> list)
        {
            if (!this.plane.Valid())
            {
                return list;
            }

            List<Polygon> list_front = new List<Polygon>();
            List<Polygon> list_back = new List<Polygon>();

            for (int i = 0; i < list.Count; i++)
            {
                this.plane.SplitPolygon(list[i], list_front, list_back, list_front, list_back);
            }

            if (this.front != null)
            {
                list_front = this.front.ClipPolygons(list_front);
            }

            if (this.back != null)
            {
                list_back = this.back.ClipPolygons(list_back);
            }
            else
            {
                list_back.Clear();
            }

            // Position [First, Last]
            // list_front.insert(list_front.end(), list_back.begin(), list_back.end());
            list_front.AddRange(list_back);

            return list_front;
        }

        /// <summary>
        /// 返回此BSP树中所有多边形的列表
        /// </summary>
        /// <returns></returns>
        public List<Polygon> AllPolygons()
        {
            List<Polygon> list = this.polygons;
            List<Polygon> list_front = new List<Polygon>(), list_back = new List<Polygon>();

            if (this.front != null)
            {
                list_front = this.front.AllPolygons();
            }

            if (this.back != null)
            {
                list_back = this.back.AllPolygons();
            }

            list.AddRange(list_front);
            list.AddRange(list_back);

            return list;
        }

        #region 静态操作

        /// <summary>
        /// 返回一个新的CSG实体，表示该实体或CSG`中的空间。
        /// 这个固体和固体“csg”都没有被修改。
        /// </summary>
        /// <param name="a1"></param>
        /// <param name="b1"></param>
        /// <returns></returns>
        public static Node Union(Node a1, Node b1)
        {
            Node a = a1.Clone();
            Node b = b1.Clone();

            a.ClipTo(b);
            b.ClipTo(a);
            b.Invert();
            b.ClipTo(a);
            b.Invert();

            a.Build(b.AllPolygons());

            Node ret = new Node(a.AllPolygons());

            return ret;
        }

        /// <summary>
        /// 返回一个新的CSG实体，表示该实体中的空间，但不表示实体“CSG”中的空间。
        /// 这个固体和固体“csg”都没有被修改。
        /// </summary>
        /// <param name="a1"></param>
        /// <param name="b1"></param>
        /// <returns></returns>
        public static Node Subtract(Node a1, Node b1)
        {
            Node a = a1.Clone();
            Node b = b1.Clone();

            a.Invert();
            a.ClipTo(b);
            b.ClipTo(a);
            b.Invert();
            b.ClipTo(a);
            b.Invert();
            a.Build(b.AllPolygons());
            a.Invert();

            Node ret = new Node(a.AllPolygons());

            return ret;
        }

        /// <summary>
        /// 返回一个新的CSG实体，表示该实体和实体“CSG”中的空间。
        /// 这个固体和固体“csg”都没有被修改。
        /// </summary>
        /// <param name="a1"></param>
        /// <param name="b1"></param>
        /// <returns></returns>
        public static Node Intersect(Node a1, Node b1)
        {
            Node a = a1.Clone();
            Node b = b1.Clone();

            a.Invert();
            b.ClipTo(a);
            b.Invert();
            a.ClipTo(b);
            b.ClipTo(a);

            a.Build(b.AllPolygons());
            a.Invert();

            Node ret = new Node(a.AllPolygons());

            return ret;
        }

        #endregion
    }
}