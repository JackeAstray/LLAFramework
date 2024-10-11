using UnityEngine;
using System.Collections.Generic;

namespace GameLogic
{
    /// <summary>
    /// CSG操作的基类。包含用于减法、交集和并集操作的游戏对象级方法。传递给这些函数的游戏对象将不会被修改。
    /// </summary>
    public static class CSG
    {
        public enum BooleanOp
        {
            Intersection,
            Union,
            Subtraction
        }

        const float k_DefaultEpsilon = 0.00001f;
        static float s_Epsilon = k_DefaultEpsilon;

        /// <summary>
        /// 使用的公差确定平面是否重合
        /// <see cref="Plane.SplitPolygon"/> 
        /// </summary>
        public static float epsilon
        {
            get => s_Epsilon;
            set => s_Epsilon = value;
        }

        /// <summary>
        /// 对两个游戏对象执行布尔运算
        /// </summary>
        /// <returns>A new mesh.</returns>
        public static Model Perform(BooleanOp op, GameObject lhs, GameObject rhs)
        {
            switch (op)
            {
                case BooleanOp.Intersection:
                    return Intersect(lhs, rhs);
                case BooleanOp.Union:
                    return Union(lhs, rhs);
                case BooleanOp.Subtraction:
                    return Subtract(lhs, rhs);
                default:
                    return null;
            }
        }

        /// <summary>
        /// 通过合并@lhs和@rhs返回一个新网格
        /// </summary>
        /// <param name="lhs">布尔运算的基本网格</param>
        /// <param name="rhs">布尔运算的基本网格</param>
        /// <returns>如果操作成功，则生成一个新网格，如果发生错误，则返回null</returns>
        public static Model Union(GameObject lhs, GameObject rhs)
        {
            Model csg_model_a = new Model(lhs);
            Model csg_model_b = new Model(rhs);

            Node a = new Node(csg_model_a.ToPolygons());
            Node b = new Node(csg_model_b.ToPolygons());

            List<Polygon> polygons = Node.Union(a, b).AllPolygons();

            return new Model(polygons);
        }

        /// <summary>
        /// 通过用@rhs减去@lhs来返回一个新网格
        /// </summary>
        /// <param name="lhs">布尔运算的基本网格</param>
        /// <param name="rhs">布尔运算的基本网格</param>
        /// <returns>如果操作成功，则生成一个新网格，如果发生错误，则为null</returns>
        public static Model Subtract(GameObject lhs, GameObject rhs)
        {
            Model csg_model_a = new Model(lhs);
            Model csg_model_b = new Model(rhs);

            Node a = new Node(csg_model_a.ToPolygons());
            Node b = new Node(csg_model_b.ToPolygons());

            List<Polygon> polygons = Node.Subtract(a, b).AllPolygons();

            return new Model(polygons);
        }

        /// <summary>
        /// 通过将@lhs与@rhs相交来返回新网格
        /// </summary>
        /// <param name="lhs">布尔运算的基本网格</param>
        /// <param name="rhs">布尔运算的基本网格</param>
        /// <returns>如果操作成功，则生成一个新网格，如果发生错误，则为null</returns>
        public static Model Intersect(GameObject lhs, GameObject rhs)
        {
            Model csg_model_a = new Model(lhs);
            Model csg_model_b = new Model(rhs);

            Node a = new Node(csg_model_a.ToPolygons());
            Node b = new Node(csg_model_b.ToPolygons());

            List<Polygon> polygons = Node.Intersect(a, b).AllPolygons();

            return new Model(polygons);
        }
    }
}