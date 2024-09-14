using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 集合扩展
    /// </summary>
    public static class CollectionExtensions
    {
        public delegate bool FilterAction<T, K>(T t, K k);

        #region 排序
        /// <summary>
        /// 洗牌
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void Shuffle<T>(this IList<T> list)
        {
            var rng = new System.Random();
            for (int n = list.Count; n > 1; n--)
            {
                int k = rng.Next(n);
                (list[k], list[n - 1]) = (list[n - 1], list[k]);
            }
        }

        /// <summary>
        /// 获取随机元素
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="source">源数据</param>
        /// <returns>随机的元素</returns>
        public static T RandomItem<T>(this IEnumerable<T> source)
        {
            return RandomItem(source, RandomExtensions.GlobalRandom);
        }

        /// <summary>
        /// 获取随机元素
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="source">源数据</param>
        /// <param name="random">要使用的随机生成器</param>
        /// <returns>随机的元素</returns>
        public static T RandomItem<T>(this IEnumerable<T> source, IRandom random)
        {
            return source.SampleRandom(1, random).First();
        }

        /// <summary>
        /// 从源返回随机样本
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="source">源数据</param>
        /// <param name="sampleCount">返回数</param>
        /// <returns>随机的元素集</returns>
        public static IEnumerable<T> SampleRandom<T>(this IEnumerable<T> source, int sampleCount)
        {
            return SampleRandom(source, sampleCount, RandomExtensions.GlobalRandom);
        }

        /// <summary>
        /// 随机(蓄水池抽样算法)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="sampleCount"></param>
        /// <param name="random"></param>
        /// <returns></returns>
        public static IEnumerable<T> SampleRandom<T>(this IEnumerable<T> source, int sampleCount, IRandom random)
        {
            if (source == null)
            {
                Debug.LogError("source is null");
                return Enumerable.Empty<T>();
            }

            if (sampleCount < 0)
            {
                Debug.LogError("sampleCount is less than 0");
                return Enumerable.Empty<T>();
            }

            var samples = new List<T>(sampleCount);
            int i = 0;

            foreach (var item in source)
            {
                if (i < sampleCount)
                {
                    samples.Add(item);
                }
                else
                {
                    int r = random.Next(i + 1);
                    if (r < sampleCount)
                    {
                        samples[r] = item;
                    }
                }
                i++;
            }

            return samples;
        }
        #endregion

        #region 取值
        /// <summary>
        /// 判断是否为空
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static bool IsEmpty<T>(this ICollection<T> collection)
        {
            return collection == null || collection.Count == 0;
        }

        /// <summary>
        /// 从序列中获取第一个元素或者默认值
        /// </summary>
        /// <param name="source"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T FirstOrDefaultEx<T>(this IEnumerable<T> source)
        {
            return source.FirstOrDefault();
        }

        /// <summary>
        /// 从序列中获取第一个元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public static List<T> First<T>(this IEnumerable<T> source, int num)
        {
            return source.Take(num).ToList();
        }

        /// <summary>
        /// 从序列中获取最后一个元素或者默认值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T LastOrDefaultEx<T>(this IEnumerable<T> source)
        {
            return source.LastOrDefault();
        }

        /// <summary>
        /// 从序列中获取最后一个元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public static List<T> Last<T>(this IEnumerable<T> source, int num)
        {
            if (source == null)
            {
                return new List<T>();
            }

            var list = source as IList<T> ?? source.ToList();
            int startIndex = Math.Max(0, list.Count - num);
            return list.Skip(startIndex).Take(num).ToList();
        }

        /// <summary>
        /// 从一个List中随机获取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T GetRandomItemFromList<T>(IList<T> list)
        {
            if (list == null || list.Count == 0)
            {
                return default;
            }

            var rng = new System.Random();
            return list[rng.Next(list.Count)];
        }
        #endregion

        #region 筛选
        /// <summary>
        /// 筛选(列表)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="testAction"></param>
        /// <returns></returns>
        public static List<T> Filter<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            return source?.Where(predicate).ToList() ?? new List<T>();
        }

        /// <summary>
        /// 筛选(字典)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="source"></param>
        /// <param name="testAction"></param>
        /// <returns></returns>
        public static Dictionary<T, K> Filter<T, K>(this IEnumerable<KeyValuePair<T, K>> source, FilterAction<T, K> testAction)
        {
            return source.Where(pair => testAction(pair.Key, pair.Value)).ToDictionary(pair => pair.Key, pair => pair.Value);
        }
        #endregion

        #region 添加
        /// <summary>
        /// 给哈希集添加批量数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> other)
        {
            if (other == null)
            {
                return;
            }

            foreach (var obj in other)
            {
                collection.Add(obj);
            }
        }

        /// <summary>
        /// 用固定值填充列表
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="list">列表</param>
        /// <param name="value">固定值</param>
        public static void Fill<T>(this IList<T> list, T value)
        {
            if (list == null)
            {
                Debug.LogError("list is null");
                return;
            }

            for (int i = 0; i < list.Count; i++)
            {
                list[i] = value;
            }
        }

        /// <summary>
        /// 用默认值填充列表
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="list">列表</param>
        public static void FillWithDefault<T>(this IList<T> list)
        {
            if (list == null)
            {
                Debug.LogError("list is null");
                return;
            }

            Fill(list, default);
        }
        #endregion

        #region 查找
        /// <summary>
        /// 通过二分查找在集合中查找元素。
        /// </summary>
        /// <typeparam name="TCollection"></typeparam>
        /// <typeparam name="TElement"></typeparam>
        /// <param name="source"></param>
        /// <param name="value"></param>
        /// <param name="getSubElement"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static int BinarySearch<TCollection, TElement>(this IList<TCollection> source,
                                                              TElement value,
                                                              Func<TCollection, TElement> getSubElement,
                                                              int index,
                                                              int length,
                                                              IComparer<TElement> comparer)
        {
            if (index < 0)
            {
                Debug.LogError("index is less than the lower bound of array.");
                return -1;
            }

            if (length < 0)
            {
                Debug.LogError("Value has to be >= 0.");
                return -1;
            }

            if (index > source.Count - length)
            {
                Debug.LogError("index and length do not specify a valid range in array.");
                return -1;
            }

            comparer ??= Comparer<TElement>.Default;

            int min = index;
            int max = index + length - 1;

            while (min <= max)
            {
                int mid = min + ((max - min) >> 1);
                int cmp = comparer.Compare(getSubElement(source[mid]), value);

                if (cmp == 0)
                {
                    return mid;
                }

                if (cmp > 0)
                {
                    max = mid - 1;
                }
                else
                {
                    min = mid + 1;
                }
            }

            return ~min;
        }
        #endregion

        #region 删除

        #endregion

        #region 比较
        /// <summary>
        /// 比较两个集合是否相等
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public static bool AreSequencesEqual<T>(IEnumerable<T> s1, IEnumerable<T> s2) where T : IComparable
        {
            if (s1 == null)
            {
                Debug.LogError("s1 is null");
                return false;
            }

            if (s2 == null)
            {
                Debug.LogError("s2 is null");
                return false;
            }

            using (var enumerator1 = s1.GetEnumerator())
            using (var enumerator2 = s2.GetEnumerator())
            {
                while (enumerator1.MoveNext())
                {
                    if (!enumerator2.MoveNext() || enumerator1.Current.CompareTo(enumerator2.Current) != 0)
                    {
                        return false;
                    }
                }

                if (enumerator2.MoveNext())
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 比较器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="comparer"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool Less<T>(this IComparer<T> comparer, T a, T b) => comparer.Compare(a, b) < 0;

        /// <summary>
        /// 小于
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="v"></param>
        /// <param name="w"></param>
        /// <returns></returns>
        internal static bool Less<T>(T v, T w) where T : IComparable<T> => v.CompareTo(w) < 0;

        /// <summary>
        /// 小于
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        internal static bool LessAt<T>(T[] list, int i, int j) where T : IComparable<T> => Less(list[i], list[j]);

        /// <summary>
        /// 小于
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        internal static bool LessAt<T>(IList<T> list, int i, int j) where T : IComparable<T> => Less(list[i], list[j]);

        /// <summary>
        /// 小于
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        internal static bool LessAt<T>(this T[] list, int i, int j, IComparer<T> comparer) => comparer.Less(list[i], list[j]);

        /// <summary>
        /// 小于
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="comparer"></param>
        /// <param name="list"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        internal static bool LessAt<T>(IComparer<T> comparer, IList<T> list, int i, int j) => comparer.Less(list[i], list[j]);

        /// <summary>
        /// 小于等于
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="v"></param>
        /// <param name="w"></param>
        /// <returns></returns>
        internal static bool LessOrEqual<T>(T v, T w) where T : IComparable<T> => v.CompareTo(w) <= 0;

        /// <summary>
        /// 小于等于
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        internal static bool LessOrEqualAt<T>(this IList<T> list, int i, int j) where T : IComparable<T> => LessOrEqual(list[i], list[j]);

        #endregion

        #region 更改
        /// <summary>
        /// 移动到目标索引
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="sourceIndex"></param>
        /// <param name="destinationIndex"></param>
        internal static void MoveAt<T>(this IList<T> list, int sourceIndex, int destinationIndex)
        {
            if (list == null)
            {
                Debug.LogError("list is null");
                return;
            }
            if (sourceIndex < 0 || sourceIndex >= list.Count)
            {
                Debug.LogError("sourceIndex is out of range");
                return;
            }
            if (destinationIndex < 0 || destinationIndex >= list.Count)
            {
                Debug.LogError("destinationIndex is out of range");
                return;
            }

            var item = list[sourceIndex];
            list.RemoveAt(sourceIndex);
            list.Insert(destinationIndex, item);
        }

        /// <summary>
        /// 移动到目标索引
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="sourceIndex"></param>
        /// <param name="destinationIndex"></param>
        internal static void MoveAt<T>(this T[] list, int sourceIndex, int destinationIndex)
        {
            if (list == null)
            {
                Debug.LogError("list is null");
                return;
            }
            if (sourceIndex < 0 || sourceIndex >= list.Length)
            {
                Debug.LogError("sourceIndex is out of range");
                return;
            }
            if (destinationIndex < 0 || destinationIndex >= list.Length)
            {
                Debug.LogError("destinationIndex is out of range");
                return;
            }
            var item = list[sourceIndex];
            Array.Copy(list, sourceIndex + 1, list, sourceIndex, destinationIndex - sourceIndex);
            list[destinationIndex] = item;
        }

        /// <summary>
        /// 根据索引交换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        internal static void SwapAt<T>(this IList<T> list, int i, int j)
        {
            if (list == null)
            {
                Debug.LogError("list is null");
                return;
            }
            if (i < 0 || i >= list.Count)
            {
                Debug.LogError("i is out of range");
                return;
            }
            if (j < 0 || j >= list.Count)
            {
                Debug.LogError("j is out of range");
                return;
            }

            (list[i], list[j]) = (list[j], list[i]);
        }

        /// <summary>
        /// 根据索引交换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        internal static void SwapAt<T>(this T[] list, int i, int j)
        {
            if (list == null)
            {
                Debug.LogError("list is null");
                return;
            }
            if (i < 0 || i >= list.Length)
            {
                Debug.LogError("i is out of range");
                return;
            }
            if (j < 0 || j >= list.Length)
            {
                Debug.LogError("j is out of range");
                return;
            }

            (list[i], list[j]) = (list[j], list[i]);
        }
        #endregion

        #region 转换
        /// <summary>
        /// 转成数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T[] ToArray<T>(this IEnumerable<T> source)
        {
            return source.ToList().ToArray();
        }

        /// <summary>
        /// 转成列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static List<T> ToList<T>(this IEnumerable<T> source)
        {
            return new List<T>(source);
        }
        #endregion

        #region 合并
        /// <summary>
        /// 列表合并
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static List<T> Union<T>(this List<T> first, List<T> second, IEqualityComparer<T> comparer)
        {
            return first.Concat(second).Distinct(comparer).ToList();
        }
        #endregion
    }
}