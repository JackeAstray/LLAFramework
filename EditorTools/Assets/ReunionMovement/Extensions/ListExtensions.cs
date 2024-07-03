﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLogic
{
    public static class ListExtensions
    {
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
        /// 从序列中获取第一个元素或者默认值
        /// </summary>
        /// <param name="source"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T FirstOrDefault_V1<T>(this IEnumerable<T> source)
        {
            return source.FirstOrDefault_V1();
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

        public delegate bool FilterAction<T, K>(T t, K k);

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

        /// <summary>
        /// 从序列中获取最后一个元素或者默认值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T LastOrDefault<T>(this IEnumerable<T> source)
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
        /// 给哈希集添加批量数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public static bool AddRange<T>(this HashSet<T> @this, IEnumerable<T> items)
        {
            bool allAdded = true;
            foreach (T item in items)
            {
                allAdded &= @this.Add(item);
            }
            return allAdded;
        }

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

        /// <summary>
        /// 联合体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static List<T> Union<T>(this List<T> first, List<T> second, IEqualityComparer<T> comparer)
        {
            return first.Union(second, comparer).ToList();
        }
    }
}
