using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            System.Random rng = new System.Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
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
        public static IEnumerable<T> SampleRandom<T>(this IEnumerable<T> source,
                                                    int sampleCount,
                                                    IRandom random)
        {
            if (source == null)
            {
                Debug.LogError("source is null");
            }

            if (sampleCount < 0)
            {
                Debug.LogError("sampleCount is less than 0");
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
            if (collection == null || collection.Count == 0)
            {
                return true;
            }

            return false;
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
            // 开始读取的位置
            var startIndex = Math.Max(0, source.ToList().Count - num);
            var index = 0;
            var items = new List<T>();
            if (source != null)
            {
                foreach (T item in source)
                {
                    if (index < startIndex)
                    {
                        continue;
                    }

                    items.Add(item);
                }
            }
            return items;
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
                return default(T);

            System.Random rng = new System.Random();
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
        public static void FillWithDefault<T>(this IList<T> list) => Fill(list, default);
        #endregion

        #region 删除

        #endregion

        #region 更改

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
            var list = new List<T>();
            foreach (T item in source)
            {
                list.Add(item);
            }
            return list.ToArray();
        }

        /// <summary>
        /// 转成列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static List<T> ToList<T>(this IEnumerable<T> source)
        {
            var list = new List<T>();
            foreach (T item in source)
            {
                list.Add(item);
            }
            return list;
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