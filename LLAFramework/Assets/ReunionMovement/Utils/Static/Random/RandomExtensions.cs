using UnityEngine;
using Random = System.Random;

namespace LLAFramework
{
    public static class RandomExtensions
    {
        #region Types

        private class RandomImpl : IRandom
        {
            #region Private Fields

            private readonly Random random;

            #endregion

            #region Constructors

            public RandomImpl()
            {
                random = new Random();
            }

            public RandomImpl(int seed)
            {
                random = new Random(seed);
            }

            #endregion

            #region Public Methods

            public double NextDouble()
            {
                return random.NextDouble();
            }

            public int Next()
            {
                return random.Next();
            }

            public int Next(int maxValue)
            {
                return random.Next(maxValue);
            }

            public int Next(int minValue, int maxValue)
            {
                return random.Next(minValue, maxValue);
            }

            public override string ToString()
            {
                return random.ToString();
            }

            public void NextBytes(byte[] bytes)
            {
                random.NextBytes(bytes);
            }

            /// <summary>
            /// Returns a point randomly selected the on a sphere.
            /// </summary>
            /// <param name="radius">The radius of the sphere.</param>
            /// <returns></returns>
            public Vector3 RandomOnSphere(float radius)
            {
                float u = (float)random.NextDouble();
                float v = (float)random.NextDouble();

                float theta = 2 * Mathf.PI * u;
                float phi = Mathf.Acos(2 * v - 1);

                float x = radius * Mathf.Cos(theta) * Mathf.Sin(phi);
                float y = radius * Mathf.Sin(theta) * Mathf.Sin(phi);
                float z = radius * Mathf.Cos(phi);

                return new Vector3(x, y, z);
            }

            #endregion
        }

        #endregion

        #region 常量

        /// <summary>
        /// 全局调用 用于随机调用
        /// </summary>
        public static readonly IRandom GlobalRandom = new RandomImpl();

        #endregion

        #region 静态方法

        /// <summary>
        /// 随机生成-1.0f或1.0f
        /// </summary>
        /// <returns></returns>
        public static float Sign()
        {
            return Bool(0.5f) ? -1.0f : 1.0f;
        }

        /// <summary>
        /// 生成一个随机布尔值，以提供的概率为真
        /// </summary>
        /// <param name="probability">几率</param>
        /// <returns></returns>
        public static bool Bool(float probability)
        {
            return GlobalRandom.NextDouble() < probability;
        }

        /// <summary>
        /// 生成随机整数。
        /// </summary>
        /// <param name="max">最大值(不包括)</param>
        /// <returns></returns>
        public static int Range(int max)
        {
            return GlobalRandom.Next(max);
        }

        /// <summary>
        /// 随机整数
        /// </summary>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns></returns>
        public static int Range(int min, int max)
        {
            return GlobalRandom.Next(min, max);
        }

        /// <summary>
        /// 生成介于0.0f（含）和给定最大值之间的随机浮点数
        /// </summary>
        /// <param name="max">最大值</param>
        /// <returns></returns>
        public static float Range(float max)
        {
            return (float)GlobalRandom.NextDouble() * max;
        }

        /// <summary>
        /// 生成随机浮点数
        /// </summary>
        /// <param name="min">最小值(包括)</param>
        /// <param name="max">最大值(不包括)</param>
        /// <returns></returns>
        public static float Range(float min, float max)
        {
            return Range(max - min) + min;
        }

        /// <summary>
        /// 在以给定值为中心的给定范围内给出随机值
        /// </summary>
        /// <param name="value">随机值的中心值</param>
        /// <param name="range">返回值的范围</param>
        /// <returns>随机值</returns>
        public static float RandomOffset(float value, float range)
        {
            var offset = GlobalRandom.NextDouble() * range - range / 2;
            return (float)(value + offset);
        }

        /// <summary>
        /// 获取新的随机生成器
        /// </summary>
        /// <returns>实例</returns>
        public static IRandom GetRandom()
        {
            return new RandomImpl();
        }

        /// <summary>
        /// 获取新的随机生成器
        /// </summary>
        /// <param name="seed">实例化生成器的种子</param>
        /// <returns>实例</returns>
        public static IRandom GetRandom(int seed)
        {
            return new RandomImpl(seed);
        }

        #endregion
    }

    /// <summary>
    /// 随机接口
    /// </summary>
    public interface IRandom
    {
        /// <summary>
        /// 获取下一个随机双精度值
        /// </summary>
        double NextDouble();

        /// <summary>
        /// 获取下一个随机整数值
        /// </summary>
        int Next();

        /// <summary>
        /// 获取下一个随机整数值
        /// </summary>
        /// <param name="maxValue">小于最大值</param>
        /// <returns></returns>
        int Next(int maxValue);

        /// <summary>
        /// 获取下一个随机整数值
        /// </summary>
        /// <param name="minValue">大于或等于最小值</param>
        /// <param name="maxValue">小于最大值</param>
        /// <returns></returns>
        int Next(int minValue, int maxValue);

        /// <summary>
        /// 用随机字节填充给定的数组
        /// </summary>
        /// <param name="bytes"></param>
        void NextBytes(byte[] bytes);

        /// <summary>
        /// 返回具有给定半径的球体表面上的随机值
        /// </summary>
        /// <param name="radius">球体的半径</param>
        /// <returns></returns>
        Vector3 RandomOnSphere(float radius);
    }
}