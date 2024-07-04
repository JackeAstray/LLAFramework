using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Text;
using UnityEngine;


namespace GameLogic
{
    /// <summary>
    /// 算法工具类
    /// </summary>
    public static class Algorithm
    {
        static System.Random random = new System.Random(Guid.NewGuid().GetHashCode());

        #region 判断
        /// <summary>
        /// 是否是奇数
        /// </summary>
        /// <param name="value">检测的值</param>
        /// <returns>是否是奇数</returns>
        public static bool IsOdd(long value)
        {
            return !Convert.ToBoolean(value & 0x1);
        }

        /// <summary>
        /// 是否是偶数
        /// </summary>
        /// <param name="value">检测的值</param>
        /// <returns>是否是偶数</returns>
        public static bool IsEven(long value)
        {
            return Convert.ToBoolean(value & 0x1);
        }
        /// <summary>
        /// 数组值比较
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        /// <returns></returns>
        public static bool ArraysEqual<T>(T[] a1, T[] a2)
        {
            return Enumerable.SequenceEqual(a1, a2);
        }

        /// <summary>
        /// 是否约等于另一个浮点数
        /// </summary>
        public static bool Approximately(float sourceValue, float targetValue)
        {
            return Mathf.Approximately(sourceValue, targetValue);
        }

        /// <summary>
        /// 概率，百分比 FLOAT
        /// 注意，0的时候当是100%
        /// </summary>
        /// <param name="chancePercent"></param>
        /// <returns></returns>
        public static bool Probability(float chancePercent)
        {
            return UnityEngine.Random.Range(0f, 100f) <= chancePercent;
        }

        /// <summary>
        /// 概率，百分比 BYTE
        /// </summary>
        /// <param name="chancePercent"></param>
        /// <returns></returns>
        public static bool Probability(byte chancePercent)
        {
            return UnityEngine.Random.Range(1, 101) <= chancePercent;
        }

        /// <summary>
        /// 判断一个数是否2的次方
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static bool CheckPow2(int num)
        {
            int i = 1;
            while (true)
            {
                if (i > num)
                    return false;
                if (i == num)
                    return true;
                i = i * 2;
            }
        }
        #endregion

        #region 计算值
        /// <summary>
        /// 获取最近的2次方
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static int GetNearestPower2(int num)
        {
            return (int)(Mathf.Pow(2, Mathf.Ceil(Mathf.Log(num) / Mathf.Log(2))));
        }

        /// <summary>
        /// 计算最大公约数
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int CalculateMaximumCommonDivisor(int a, int b)
        {
            a = Math.Abs(a);
            b = Math.Abs(b);
            while (b != 0)
            {
                int t = b;
                b = a % b;
                a = t;
            }
            return a;
        }
        /// <summary>
        /// 计算两个向量之间的角度 3D
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static float Vector3DToAngel(Transform value1, Transform value2)
        {
            return Vector3.Angle(value1.forward, value2.forward);
        }

        /// <summary>
        /// 计算两个向量之间的角度 2D
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static float Vector2DToAngle(Vector2 value1, Vector2 value2)
        {
            return Vector2.Angle(value1, value2);
        }

        /// <summary>
        /// 计算角度转向量 2D
        /// </summary>
        public static Vector2 AngleToVector2D(float angle)
        {
            float radian = Mathf.Deg2Rad * angle;
            return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)).normalized;
        }

        /// <summary>
        /// 获取两点之间距离一定百分比的一个点
        /// </summary>
        /// <param name="start">起始点</param>
        /// <param name="end">结束点</param>
        /// <param name="distance">起始点到目标点距离百分比</param>
        /// <returns></returns>
        public static Vector3 GetBetweenPoint1(Vector3 start, Vector3 end, float percent)
        {
            // old
            // Vector3 normal = (end - start).normalized;
            // float distance = Vector3.Distance(start, end);
            // return normal * (distance * percent) + start;
            // new
            return Vector3.Lerp(start, end, percent);
        }

        /// <summary>
        /// 获取两点之间一定距离的点
        /// </summary>
        /// <param name="start">起始点</param>
        /// <param name="end">结束点</param>
        /// <param name="distance">距离</param>
        /// <returns></returns>
        public static Vector3 GetBetweenPoint2(Vector3 start, Vector3 end, float distance)
        {
            return start + (end - start).normalized * distance;
        }

        /// <summary>
        /// 获取椭圆上的某一点，相对坐标
        /// </summary>
        /// <param name="longHalfAxis">长半轴即目标距离</param>
        /// <param name="shortHalfAxis">短半轴</param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Vector2 GetRelativePositionOfEllipse(float longHalfAxis, float shortHalfAxis, float angle)
        {
            var rad = angle * Mathf.Deg2Rad; // 弧度
            var newPos = Vector2.right * longHalfAxis * Mathf.Cos(rad) + Vector2.up * shortHalfAxis * Mathf.Sin(rad);
            return newPos;
        }

        /// <summary>
        /// 返回一个0.0~1.0之间的随机数
        /// </summary>
        /// <returns>随机数</returns>
        public static double RandomDouble()
        {
            return random.NextDouble();
        }

        /// <summary>
        /// 产生均匀随机数
        /// </summary>
        public static double AverageRandom(double minValue, double maxValue)
        {
            int min = (int)(minValue * 10000);
            int max = (int)(maxValue * 10000);
            int result = random.Next(min, max);
            return result / 10000.0;
        }

        /// <summary>
        /// 正态分布概率密度函数
        /// </summary>
        public static double NormalDistributionProbability(double x, double miu, double sigma)
        {
            return 1.0 / (x * Math.Sqrt(2 * Math.PI) * sigma) *
                   Math.Exp(-1 * (Math.Log(x) - miu) * (Math.Log(x) - miu) / (2 * sigma * sigma));
        }

        /// <summary>
        /// 随机正态分布；
        /// </summary>
        public static double RandomNormalDistribution(double miu, double sigma, double min, double max) //产生正态分布随机数
        {
            double x;
            double dScope;
            double y;
            do
            {
                x = AverageRandom(min, max);
                y = NormalDistributionProbability(x, miu, sigma);
                dScope = AverageRandom(0, NormalDistributionProbability(miu, miu, sigma));
            } while (dScope > y);

            return x;
        }

        /// <summary>
        /// 1或-1的随机值
        /// </summary>
        /// <returns> 1或-1</returns>
        public static int OneOrMinusOne()
        {
            return random.Next(0, 2) * 2 - 1;
        }

        /// <summary>
        /// 泛型二分查找，需要传入升序数组
        /// </summary>
        /// <returns>返回对象在数组中的序号，若不存在，则返回-1</returns>
        public static int BinarySearch<T, K>(IList<T> array, K target, Func<T, K> handler)
            where K : IComparable<K>
        {
            int first = 0;
            int last = array.Count - 1;
            while (first <= last)
            {
                int mid = first + (last - first) / 2;
                if (handler(array[mid]).CompareTo(target) > 0)
                    last = mid - 1;
                else if (handler(array[mid]).CompareTo(target) < 0)
                    first = mid + 1;
                else
                    return mid;
            }

            return -1;
        }

        /// <summary>
        /// 将一个int数组转换为顺序的整数;
        /// 若数组中存在负值，则默认将负值取绝对值
        /// </summary>
        /// <param name="array">传入的数组</param>
        /// <returns>转换成整数后的int</returns>
        public static int ConvertIntArrayToInt(int[] array)
        {
            int result = 0;
            int length = array.Length;
            for (int i = 0; i < length; i++)
            {
                result += Convert.ToInt32((Math.Abs(array[i]) * Math.Pow(10, length - 1 - i)));
            }

            return result;
        }

        /// <summary>
        /// 生成指定长度的int整数
        /// </summary>
        /// <param name="length">数值长度</param>
        /// <param name="minValue">随机取值最小区间</param>
        /// <param name="maxValue">随机取值最大区间</param>
        /// <returns>生成的int整数</returns>
        public static int RandomRange(int length, int minValue, int maxValue)
        {
            if (minValue >= maxValue)
                throw new ArgumentNullException("RandomRange : minValue is greater than or equal to maxValue");
            string buffer = "0123456789"; // 随机字符中也可以为汉字（任何）
            StringBuilder strbuilder = new StringBuilder();
            int range = buffer.Length;
            int resultValue = 0;
            do
            {
                for (int i = 0; i < length; i++)
                {
                    strbuilder.Append(buffer.Substring(random.Next(range), 1));
                }

                resultValue = Int32.Parse(strbuilder.ToString());
            } while (resultValue > maxValue || resultValue < minValue);

            return resultValue;
        }

        /// <summary>
        /// 随机在范围内生成一个int
        /// </summary>
        /// <param name="minValue">随机取值最小区间</param>
        /// <param name="maxValue">随机取值最大区间</param>
        /// <returns>生成的int整数</returns>
        public static int RandomRange(int minValue, int maxValue)
        {
            if (minValue >= maxValue)
                throw new ArgumentNullException("RandomRange : minValue is greater than or equal to maxValue");
            int seed = Guid.NewGuid().GetHashCode();
            System.Random random = new System.Random(seed);
            int result = random.Next(minValue, maxValue);
            return result;
        }

        /// <summary>
        /// 随机在范围内生成一个long
        /// </summary>
        /// <param name="minValue">随机取值最小区间</param>
        /// <param name="maxValue">随机取值最大区间</param>
        /// <returns>生成的long</returns>
        public static long RandomRange(long minValue, long maxValue)
        {
            if (minValue >= maxValue)
                throw new ArgumentNullException("RandomRange : minValue is greater than or equal to maxValue");
            byte[] buf = new byte[8];
            random.NextBytes(buf);
            long longRand = BitConverter.ToInt64(buf, 0);
            // 计算随机值范围
            long range = maxValue - minValue + 1;
            // 将随机值映射到指定范围内
            long result = (long)Math.Floor(longRand / (double)long.MaxValue * range) + minValue;
            return result;
        }

        /// <summary>
        /// 限制一个向量在最大值与最小值之间
        /// </summary>
        public static Vector3 Clamp(Vector3 value, Vector3 min, Vector3 max)
        {
            value.x = Mathf.Clamp(value.x, min.x, max.x);
            value.y = Mathf.Clamp(value.y, min.y, max.y);
            value.z = Mathf.Clamp(value.z, min.z, max.z);
            return value;
        }

        public static Vector2 Clamp(Vector2 value, Vector2 min, Vector2 max)
        {
            value.x = Mathf.Clamp(value.x, min.x, max.x);
            value.y = Mathf.Clamp(value.y, min.y, max.y);
            return value;
        }

        /// <summary>
        /// 获得固定位数小数的向量
        /// </summary>
        public static Vector3 Round(Vector3 value, int decimals)
        {
            value.x = (float)Math.Round(value.x, decimals);
            value.y = (float)Math.Round(value.y, decimals);
            value.z = (float)Math.Round(value.z, decimals);
            return value;
        }

        /// <summary>
        /// 限制一个向量在最大值与最小值之间
        /// </summary>
        public static Vector3 Clamp(Vector3 value, float minX, float minY, float minZ, float maxX, float maxY, float maxZ)
        {
            value.x = Mathf.Clamp(value.x, minX, maxX);
            value.y = Mathf.Clamp(value.y, minY, maxY);
            value.z = Mathf.Clamp(value.z, minZ, maxZ);
            return value;
        }

        public static Vector2 Clamp(Vector2 value, float minX, float minY, float maxX, float maxY)
        {
            value.x = Mathf.Clamp(value.x, minX, maxX);
            value.y = Mathf.Clamp(value.y, minY, maxY);
            return value;
        }

        /// <summary>
        /// 获得固定位数小数的向量
        /// </summary>
        public static Vector2 Round(Vector2 value, int decimals)
        {
            value.x = (float)Math.Round(value.x, decimals);
            value.y = (float)Math.Round(value.y, decimals);
            return value;
        }

        /// <summary>
        /// 计算中心点
        /// </summary>
        /// <param name="Points"></param>
        /// <returns></returns>
        public static Vector3 CalculateCenterPoint(List<Transform> Points)
        {
            return Points.Aggregate(Vector3.zero, (acc, p) => acc + p.position) / Points.Count;
        }

        /// <summary>
        /// 在BoxCollider中获取随机位置
        /// </summary>
        /// <param name="collider"></param>
        /// <returns></returns>
        public static Vector3 GetRandomPositionInBoxCollider(BoxCollider collider, int method = 1)
        {
            Vector3 result = Vector3.zero;
            switch (method)
            {
                case 1://根据BoxCollider中心点和大小来获取随机位置的
                    Vector3 center = collider.bounds.center;
                    Vector3 size = collider.bounds.size;
                    float x = UnityEngine.Random.Range(center.x - size.x / 2, center.x + size.x / 2);
                    float y = UnityEngine.Random.Range(center.y - size.y / 2, center.y + size.y / 2);
                    float z = UnityEngine.Random.Range(center.z - size.z / 2, center.z + size.z / 2);
                    result = new Vector3(x, y, z);
                    break;
                case 2://根据BoxCollider边界获取随机位置的
                    result = new Vector3(UnityEngine.Random.Range(collider.bounds.min.x, collider.bounds.max.x),
                                         UnityEngine.Random.Range(collider.bounds.min.y, collider.bounds.max.y),
                                         UnityEngine.Random.Range(collider.bounds.min.z, collider.bounds.max.z));
                    break;
            }
            return result;
        }

        /// <summary>
        /// 获取一个球内随机点
        /// </summary>
        /// <param name="center">中心点</param>
        /// <param name="radius">半径</param>
        /// <returns>球内随机点</returns>
        public static Vector3 GetRandomPointInSphere(Vector3 center, float radius)
        {
            if (radius < 0)
                radius = 0;
            var rndPtr = UnityEngine.Random.insideUnitSphere * radius;
            var rndPos = rndPtr + center;
            return rndPos;
        }

        /// <summary>
        /// 获取一个球内随机点
        /// </summary>
        /// <param name="center">中心点</param>
        /// <param name="miniRadius">最小半径</param>
        /// <param name="maxRadius">最大半径</param>
        /// <returns>球内随机点</returns>
        public static Vector3 GetRandomPointInSphere(Vector3 center, float miniRadius, float maxRadius)
        {
            if (miniRadius < 0)
                miniRadius = 0;
            if (maxRadius < miniRadius)
                maxRadius = miniRadius;
            var randomRadius = UnityEngine.Random.Range(miniRadius, maxRadius);
            var rndPtr = UnityEngine.Random.insideUnitSphere * randomRadius;
            var rndPos = rndPtr + center;
            return rndPos;
        }

        /// <summary>
        /// 获取一个圆内随机点
        /// </summary>
        /// <param name="startPos"></param>
        /// <param name="startDirection"></param>
        /// <param name="nNum"></param>
        /// <param name="meterInterval"></param>
        /// <returns></returns>
        public static Vector3[] GetParallelPoints(Vector3 startPos, Vector3 startDirection, int nNum, float meterInterval)
        {
            Vector3[] targetPos = new Vector3[nNum];
            Vector3 perpendicularDirection = Quaternion.AngleAxis(90, Vector3.forward) * startDirection.normalized; // 计算垂直方向
            int halfNum = nNum / 2;
            bool isEven = nNum % 2 == 0;

            for (int i = 0; i < nNum; i++)
            {
                int indexOffset = i - halfNum + (isEven ? 1 : 0);
                float distance = indexOffset * meterInterval + (isEven ? meterInterval / 2 : 0);
                targetPos[i] = startPos + perpendicularDirection * distance;
            }

            return targetPos;
        }

        /// <summary>
        /// 计算AB与CD两条线段的交点.
        /// </summary>
        /// <param name="a">A点</param>
        /// <param name="b">B点</param>
        /// <param name="c">C点</param>
        /// <param name="d">D点</param>
        /// <returns>是否相交 true:相交 false:未相交 | AB与CD的交点</returns>
        public static (bool, Vector3) CalculateIntersectionPoint_AB_And_CD_LineSegments(
            Vector3 a,
            Vector3 b,
            Vector3 c,
            Vector3 d)
        {
            Vector3 ab = b - a;
            Vector3 ca = a - c;
            Vector3 cd = d - c;

            Vector3 v1 = Vector3.Cross(ca, cd);

            if (Mathf.Abs(Vector3.Dot(v1, ab)) > 1e-6)
            {
                // 不共面
                return (false, Vector3.zero);
            }

            if (Vector3.Cross(ab, cd).sqrMagnitude <= 1e-6)
            {
                // 平行
                return (false, Vector3.zero);
            }

            Vector3 ad = d - a;
            Vector3 cb = b - c;
            // 快速排斥
            if (Mathf.Min(a.x, b.x) > Mathf.Max(c.x, d.x) || Mathf.Max(a.x, b.x) < Mathf.Min(c.x, d.x)
            || Mathf.Min(a.y, b.y) > Mathf.Max(c.y, d.y) || Mathf.Max(a.y, b.y) < Mathf.Min(c.y, d.y)
            || Mathf.Min(a.z, b.z) > Mathf.Max(c.z, d.z) || Mathf.Max(a.z, b.z) < Mathf.Min(c.z, d.z)
            )
                return (false, Vector3.zero);

            // 跨立试验
            if (Vector3.Dot(Vector3.Cross(-ca, ab), Vector3.Cross(ab, ad)) > 0
                && Vector3.Dot(Vector3.Cross(ca, cd), Vector3.Cross(cd, cb)) > 0)
            {
                Vector3 v2 = Vector3.Cross(cd, ab);
                float ratio = Vector3.Dot(v1, v2) / v2.sqrMagnitude;
                Vector3 intersectPos = a + ab * ratio;
                return (true, intersectPos);
            }

            return (false, Vector3.zero);
        }

        /// <summary>
        /// 计算两条线段的交点
        /// </summary>
        /// <param name="ps1"></param>
        /// <param name="pe1"></param>
        /// <param name="ps2"></param>
        /// <param name="pe2"></param>
        /// <returns></returns>
        public static (bool, Vector2) LineIntersectionPoint(Vector2 ps1, Vector2 pe1, Vector2 ps2, Vector2 pe2)
        {
            // 获取第一行的A、B、C-点：ps1到pe1
            float A1 = pe1.y - ps1.y;
            float B1 = ps1.x - pe1.x;
            float C1 = A1 * ps1.x + B1 * ps1.y;

            // 获取第二行的A、B、C点：ps2到pe2
            float A2 = pe2.y - ps2.y;
            float B2 = ps2.x - pe2.x;
            float C2 = A2 * ps2.x + B2 * ps2.y;

            // 获取delta并检查直线是否平行
            float delta = A1 * B2 - A2 * B1;
            if (delta == 0)
            {
                return (false, Vector2.zero);
            }

            // 现在返回Vector2的交点
            Vector2 intersectPoint = new Vector2(
                (B2 * C1 - B1 * C2) / delta,
                (A1 * C2 - A2 * C1) / delta
            );
            return (true, intersectPoint);
        }

        /// <summary>
        /// 产生正态分布的随机数
        /// </summary>
        /// <param name="mean">均值</param>
        /// <param name="stdDev">方差</param>
        /// <returns>随机数</returns>
        public static double NextGauss(double mean, double stdDev)
        {
            double u1 = 1.0 - random.NextDouble();
            double u2 = 1.0 - random.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            return mean + stdDev * randStdNormal;
        }

        /// <summary>
        /// 呈弧形，传入一个参考点根据角度和半径计算出其它位置的坐标
        /// </summary>
        /// <param name="nNum">需要的数量</param>
        /// <param name="pAnchorPos">锚定点/参考点</param>
        /// <param name="fAngle">角度</param>
        /// <param name="nRadius">半径</param>
        /// <returns></returns>
        public static Vector3[] GetSmartNpcPoints(Vector3 startDirection, int nNum, Vector3 pAnchorPos, float fAngle, float nRadius)
        {
            Vector3[] points = new Vector3[nNum];
            // 每个点之间的角度增量
            float angleIncrement = fAngle / nNum;
            // 用于旋转的四元数
            Quaternion rotation = Quaternion.Euler(0, angleIncrement, 0);
            // 初始方向向量，确保其被规范化并乘以半径
            Vector3 direction = startDirection.normalized * nRadius;

            for (int i = 0; i < nNum; i++)
            {
                // 计算每个点的位置
                points[i] = pAnchorPos + direction;
                // 更新方向向量以指向下一个点
                direction = rotation * direction; 
            }

            return points;
        }
        #endregion

        #region 功能
        /// <summary>
        /// Fisher–Yates shuffle 洗牌算法
        /// </summary>
        public static void Shuffle<T>(IList<T> array, int randomValue)
        {
            var random = new System.Random(randomValue);
            for (int i = array.Count - 1; i > 0; i--)
            {
                int randomIndex = random.Next(0, i + 1);

                // 交换元素位置
                (array[i], array[randomIndex]) = (array[randomIndex], array[i]);
            }
        }

        /// <summary>
        /// 判断物体左右转转
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static RotationDirection GetRotationDirection(Transform value1, Transform value2)
        {
            Vector2 v1 = new Vector2(value1.forward.x, value1.forward.z); //旋转前的前方
            Vector2 v2 = new Vector2(value2.forward.x, value2.forward.z); //旋转后的前方

            float rightFloat = v1.x * v2.y - v2.x * v1.y;

            if (rightFloat < 0)
            {
                return RotationDirection.Right;
            }
            else if (rightFloat > 0)
            {
                return RotationDirection.Left;
            }
            else
            {
                return RotationDirection.None;
            }
        }

        /// <summary>
        /// 数组去重；
        /// </summary>
        /// <typeparam name="T">可比数据类型</typeparam>
        /// <param name="array">源数据</param>
        /// <returns>去重后的数据</returns>
        public static T[] Distinct<T>(IList<T> array)
            where T : IComparable
        {
            var length = array.Count;
            T[] dst = new T[length];
            int idx = 0;
            for (int i = 0; i < length; i++)
            {
                bool isDuplicate = false;
                for (int j = 0; j < i; j++)
                {
                    if (array[i].CompareTo(array[j]) == 0)
                    {
                        isDuplicate = true;
                        break;
                    }
                }

                if (!isDuplicate)
                {
                    dst[idx] = array[i];
                    idx++;
                }
            }

            Array.Resize(ref dst, idx);
            return dst;
        }

        /// <summary>
        /// 交换两个值
        /// </summary>
        /// <typeparam name="T">传入的对象类型</typeparam>
        /// <param name="lhs">第一个需要交换的值</param>
        /// <param name="rhs">第二个需要交换的值</param>
        public static void Swap<T>(ref T lhs, ref T rhs)
        {
            (lhs, rhs) = (rhs, lhs);
        }

        /// <summary>
        /// 交换数组中的两个元素
        /// </summary>
        /// <typeparam name="T">传入的对象类型</typeparam>
        /// <param name="array">传入的数组</param>
        /// <param name="lhs">序号A</param>
        /// <param name="rhs">序号B</param>
        public static void Swap<T>(IList<T> array, int lhs, int rhs)
        {
            (array[lhs], array[rhs]) = (array[rhs], array[lhs]);
        }

        /// <summary>
        /// 随机打乱数组
        /// </summary>
        /// <typeparam name="T">数组类型</typeparam>
        /// <param name="array">数组</param>
        public static void Disrupt<T>(IList<T> array)
        {
            Disrupt(array, 0, array.Count);
        }

        /// <summary>
        /// 随机打乱数组
        /// </summary>
        /// <typeparam name="T">数组类型</typeparam>
        /// <param name="array">数组</param>
        /// <param name="startIndex">起始序号</param>
        /// <param name="count">数量</param>
        public static void Disrupt<T>(IList<T> array, int startIndex, int count)
        {
            int index = 0;
            T tmp;
            var endIndex = startIndex + count;
            for (int i = startIndex; i < endIndex; i++)
            {
                index = RandomRange(startIndex, endIndex);
                if (index != i)
                {
                    tmp = array[i];
                    array[i] = array[index];
                    array[index] = tmp;
                }
            }
        }

        /// <summary>
        /// 快速排序：降序
        /// </summary>
        /// <typeparam name="T">数组类型</typeparam>
        /// <typeparam name="K">比较类型</typeparam>
        /// <param name="array">需要排序的数组对象</param>
        /// <param name="handler">排序条件</param>
        /// <param name="start">起始位</param>
        /// <param name="end">结束位</param>
        public static void SortByDescend<T, K>(IList<T> array, Func<T, K> handler, int start, int end)
            where K : IComparable<K>
        {
            if (array == null)
                throw new ArgumentNullException("SortByDescend : array is null");
            if (handler == null)
                throw new ArgumentNullException("SortByDescend : handler is null");
            if (start < 0 || end < 0 || start >= end)
            {
                return;
            }

            int pivort = start;
            T pivortValue = array[pivort];
            Swap(array, end, pivort);
            int storeIndex = start;
            for (int i = start; i <= end - 1; i++)
            {
                if (handler(array[i]).CompareTo(handler(pivortValue)) > 0)
                {
                    Swap(array, i, storeIndex);
                    storeIndex++;
                }
            }

            Swap(array, storeIndex, end);
            SortByDescend(array, handler, start, storeIndex - 1);
            SortByDescend(array, handler, storeIndex + 1, end);
        }

        /// <summary>
        /// 快速排序：升序
        /// </summary>
        /// <typeparam name="T">数组类型</typeparam>
        /// <typeparam name="K">比较类型</typeparam>
        /// <param name="array">需要排序的数组对象</param>
        /// <param name="handler">排序条件</param>
        /// <param name="start">起始位</param>
        /// <param name="end">结束位</param>
        public static void SortByAscend<T, K>(IList<T> array, Func<T, K> handler, int start, int end)
            where K : IComparable<K>
        {
            if (array == null)
                throw new ArgumentNullException("QuickSortByAscend : array is null");
            if (handler == null)
                throw new ArgumentNullException("QuickSortByAscend : handler is null");
            if (start < 0 || end < 0 || start >= end)
            {
                return;
            }

            int pivort = start;
            T pivortValue = array[pivort];
            Swap(array, end, pivort);
            int storeIndex = start;
            for (int i = start; i <= end - 1; i++)
            {
                if (handler(array[i]).CompareTo(handler(pivortValue)) < 0)
                {
                    Swap(array, i, storeIndex);
                    storeIndex++;
                }
            }

            Swap(array, storeIndex, end);
            SortByAscend(array, handler, start, storeIndex - 1);
            SortByAscend(array, handler, storeIndex + 1, end);
        }

        /// <summary>
        /// 冒泡排序：升序
        /// </summary>
        /// <typeparam name="T">数组类型</typeparam>
        /// <typeparam name="K">比较类型</typeparam>
        /// <param name="array">需要排序的数组对象</param>
        /// <param name="handler">排序条件</param>
        public static void SortByAscend<T, K>(IList<T> array, Func<T, K> handler)
            where K : IComparable<K>
        {
            for (int i = 0; i < array.Count; i++)
            {
                for (int j = 0; j < array.Count; j++)
                {
                    if (handler(array[i]).CompareTo(handler(array[j])) < 0)
                    {
                        (array[i], array[j]) = (array[j], array[i]);
                    }
                }
            }
        }

        /// <summary>
        /// 冒泡排序：降序
        /// </summary>
        /// <typeparam name="T">数组类型</typeparam>
        /// <typeparam name="K">比较类型</typeparam>
        /// <param name="array">需要排序的数组对象</param>
        /// <param name="handler">排序条件</param>
        public static void SortByDescend<T, K>(IList<T> array, Func<T, K> handler)
            where K : IComparable<K>
        {
            for (int i = 0; i < array.Count; i++)
            {
                for (int j = 0; j < array.Count; j++)
                {
                    if (handler(array[i]).CompareTo(handler(array[j])) > 0)
                    {
                        (array[i], array[j]) = (array[j], array[i]);
                    }
                }
            }
        }

        /// <summary>
        /// 冒泡排序：升序
        /// </summary>
        /// <typeparam name="T">数组类型</typeparam>
        /// <typeparam name="K">比较类型</typeparam>
        /// <param name="array">需要排序的数组对象</param>
        /// <param name="comparison">排序条件</param>
        public static void SortByAscend<T, K>(IList<T> array, Comparison<T> comparison)
        {
            for (int i = 0; i < array.Count; i++)
            {
                for (int j = 0; j < array.Count; j++)
                {
                    if (comparison(array[i], array[j]) < 0)
                    {
                        (array[i], array[j]) = (array[j], array[i]);
                    }
                }
            }
        }

        /// <summary>
        /// 冒泡排序：降序
        /// </summary>
        /// <typeparam name="T">数组类型</typeparam>
        /// <typeparam name="K">比较类型</typeparam>
        /// <param name="array">需要排序的数组对象</param>
        /// <param name="comparison">排序条件</param>
        public static void SortByDescend<T, K>(IList<T> array, Comparison<T> comparison)
        {
            for (int i = 0; i < array.Count; i++)
            {
                for (int j = 0; j < array.Count; j++)
                {
                    if (comparison(array[i], array[j]) > 0)
                    {
                        (array[i], array[j]) = (array[j], array[i]);
                    }
                }
            }
        }

        /// <summary>
        ///  获取最小
        /// </summary>
        public static T Min<T, K>(IList<T> array, Func<T, K> handler)
            where K : IComparable<K>
        {
            T temp = default(T);
            temp = array[0];
            foreach (var arr in array)
            {
                if (handler(temp).CompareTo(handler(arr)) > 0)
                {
                    temp = arr;
                }
            }

            return temp;
        }

        /// <summary>
        /// 获取最大值
        /// </summary>
        public static T Max<T, K>(IList<T> array, Func<T, K> handler)
            where K : IComparable<K>
        {
            T temp = default(T);
            temp = array[0];
            foreach (var arr in array)
            {
                if (handler(temp).CompareTo(handler(arr)) < 0)
                {
                    temp = arr;
                }
            }

            return temp;
        }

        /// <summary>
        ///  获取最小
        /// </summary>
        public static T Min<T, K>(IList<T> array, Comparison<T> comparison)
        {
            T temp = default(T);
            temp = array[0];
            foreach (var arr in array)
            {
                if (comparison(temp, arr) > 0)
                {
                    temp = arr;
                }
            }

            return temp;
        }

        /// <summary>
        /// 获取最大值
        /// </summary>
        public static T Max<T, K>(IList<T> array, Comparison<T> comparison)
        {
            T temp = default(T);
            temp = array[0];
            foreach (var arr in array)
            {
                if (comparison(temp, arr) < 0)
                {
                    temp = arr;
                }
            }

            return temp;
        }

        /// <summary>
        /// 获得传入元素某个符合条件的所有对象
        /// </summary>
        public static T Find<T>(IList<T> array, Predicate<T> handler)
        {
            T temp = default(T);
            for (int i = 0; i < array.Count; i++)
            {
                if (handler(array[i]))
                {
                    return array[i];
                }
            }

            return temp;
        }

        /// <summary>
        /// 获得传入元素某个符合条件的所有对象
        /// </summary>
        public static T[] FindAll<T>(IList<T> array, Predicate<T> handler)
        {
            var dstArray = new T[array.Count];
            int idx = 0;
            for (int i = 0; i < array.Count; i++)
            {
                if (handler(array[i]))
                {
                    dstArray[idx] = array[i];
                    idx++;
                }
            }

            Array.Resize(ref dstArray, idx);
            return dstArray;
        }
        #endregion
    }
}