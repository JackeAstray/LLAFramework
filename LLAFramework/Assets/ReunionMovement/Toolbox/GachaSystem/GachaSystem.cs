using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

namespace GameLogic
{
    /// <summary>
    /// 抽卡系统
    /// </summary>
    public class GachaSystem : MonoBehaviour
    {
        // ===== 数据结构 =====
        [System.Serializable]
        public class GachaItem
        {
            public string itemName;
            public int starRating; // 3/4/5
            public Sprite icon;    // 物品图标
            public bool isWeapon;  // 是否为武器
        }

        // ===== 卡池配置 =====
        public List<GachaItem> up5StarPool;   // UP五星池
        public List<GachaItem> standard5StarPool; // 常驻五星池
        public List<GachaItem> up4StarPool;   // UP四星池
        public List<GachaItem> standard4StarPool; // 常驻四星池
        public List<GachaItem> standard3StarPool; // 三星池

        // ===== 系统状态 =====
        [SerializeField]
        private int pity5Star = 0;        // 五星保底计数
        [SerializeField]
        private int pity4Star = 0;        // 四星保底计数
        [SerializeField]
        private bool isGuaranteedUp5Star = false; // 大保底标记
        [SerializeField]
        private bool isGuaranteedUp4Star = false; // 四星保底标记
        private int last5StarPullCount = 0; // 记录第几抽抽到5星
        private bool isLastPullUp = false; // 记录最近一次抽卡是否为UP

        // ===== 概率参数 =====[1](@ref)
        [SerializeField]
        private const float BASE_5STAR_RATE = 0.006f;    // 0.6%
        [SerializeField]
        private const float BASE_4STAR_RATE = 0.051f;    // 5.1%
        [SerializeField]
        private const int HARD_PITY_5STAR = 90;          // 硬保底
        [SerializeField]
        private const int SOFT_PITY_START = 73;          // 概率递增起点

        // ===== 核心抽卡逻辑 =====
        /// <summary>
        /// 执行一次抽卡
        /// </summary>
        /// <returns></returns>
        public GachaItem PerformPull()
        {
            pity5Star++;
            pity4Star++;

            // 五星保底判断[3](@ref)
            if (Check5StarPull())
            {
                return Get5StarItem();
            }
            // 四星保底判断[5](@ref)
            else if (Check4StarPull())
            {
                return Get4StarItem();
            }
            else
            {
                return Get3StarItem();
            }
        }

        /// <summary>
        /// 五星保底判断
        /// </summary>
        /// <returns></returns>
        private bool Check5StarPull()
        {
            // 硬保底触发
            if (pity5Star >= HARD_PITY_5STAR) return true;

            // 动态概率计算[1](@ref)
            float currentRate = pity5Star >= SOFT_PITY_START ?
                BASE_5STAR_RATE + 0.06f * (pity5Star - SOFT_PITY_START) :
                BASE_5STAR_RATE;

            return Random.value <= currentRate;
        }

        /// <summary>
        /// 获取五星物品
        /// </summary>
        /// <returns></returns>
        private GachaItem Get5StarItem()
        {
            // 记录当前抽数
            last5StarPullCount = pity5Star;

            // 判断是否为UP
            bool isUp = isGuaranteedUp5Star ? true : Random.value <= 0.5f;
            isGuaranteedUp5Star = !isUp; // 未出UP则触发大保底

            // 更新是否为UP的状态
            isLastPullUp = isUp;

            List<GachaItem> pool = isUp ? up5StarPool : standard5StarPool;
            ResetCounters();
            return SelectRandomItem(pool);
        }

        /// <summary>
        /// 四星保底判断
        /// </summary>
        /// <returns></returns>
        private bool Check4StarPull()
        {
            // 硬保底触发
            if (pity4Star >= 10) return true;

            // 动态概率计算
            float currentRate = BASE_4STAR_RATE;
            if (pity4Star >= 8) // 第9抽开始递增
            {
                currentRate = Mathf.Min(0.66f + (0.34f * (pity4Star - 8)), 1.0f);
            }
            return Random.value <= currentRate;
        }

        /// <summary>
        /// 获取四星物品
        /// </summary>
        /// <returns></returns>
        private GachaItem Get4StarItem()
        {
            // 判断是否触发UP保底
            bool isUp = isGuaranteedUp4Star ? true : Random.value <= 0.5f;
            isGuaranteedUp4Star = !isUp; // 更新保底状态

            // 更新是否为UP的状态
            isLastPullUp = isUp;

            //// 动态概率验证（调试用）
            //Debug.Log($"四星触发于第{pity4Star}抽 | UP状态:{isUp}");

            // 选择卡池
            List<GachaItem> pool = isUp ? up4StarPool : standard4StarPool;
            pity4Star = 0; // 重置四星计数器
            return SelectRandomItem(pool);
        }

        /// <summary>
        /// 获取三星物品
        /// </summary>
        /// <returns></returns>
        private GachaItem Get3StarItem()
        {
            // 重置四星计数器（根据网页7保底规则）
            pity4Star = 0;

            // 从常驻三星池随机选取
            return SelectRandomItem(standard3StarPool);
        }

        // ===== 辅助方法 =====
        /// <summary>
        /// 重置计数器
        /// </summary>
        private void ResetCounters()
        {
            pity5Star = 0;
            pity4Star = 0; // 四星保底独立重置[5](@ref)
        }

        /// <summary>
        /// 随机选择物品
        /// </summary>
        /// <param name="pool"></param>
        /// <returns></returns>
        private GachaItem SelectRandomItem(List<GachaItem> pool)
        {
            return pool[Random.Range(0, pool.Count)];
        }

        // ===== 十连优化 =====
        /// <summary>
        /// 执行十连抽卡
        /// </summary>
        /// <returns></returns>
        public List<GachaItem> Perform10Pull()
        {
            List<GachaItem> results = new List<GachaItem>();
            bool hasFourStarOrAbove = false;

            for (int i = 0; i < 10; i++)
            {
                GachaItem item = PerformPull();
                results.Add(item);
                if (item.starRating >= 4) hasFourStarOrAbove = true;
            }

            // 确保至少有一个四星或以上物品
            if (!hasFourStarOrAbove)
            {
                results[9] = Get4StarItem();
            }

            return results;
        }



        // ===== 测试方法 =====
        public void Start()
        {
            Test();
        }

        public void Test()
        {
            // 配置卡池
            up5StarPool = new List<GachaItem>
            {
                new GachaItem { itemName = "UP五星1", starRating = 5 },
                new GachaItem { itemName = "UP五星2", starRating = 5 },
                new GachaItem { itemName = "UP五星3", starRating = 5 },
            };
            standard5StarPool = new List<GachaItem>
            {
                new GachaItem { itemName = "常驻五星1", starRating = 5 },
                new GachaItem { itemName = "常驻五星2", starRating = 5 },
                new GachaItem { itemName = "常驻五星3", starRating = 5 },
            };
            up4StarPool = new List<GachaItem>
            {
                new GachaItem { itemName = "UP四星1", starRating = 4 },
                new GachaItem { itemName = "UP四星2", starRating = 4 },
                new GachaItem { itemName = "UP四星3", starRating = 4 },
            };
            standard4StarPool = new List<GachaItem>
            {
                new GachaItem { itemName = "常驻四星1", starRating = 4 },
                new GachaItem { itemName = "常驻四星2", starRating = 4 },
                new GachaItem { itemName = "常驻四星3", starRating = 4 },
                new GachaItem { itemName = "常驻四星4", starRating = 4 },
                new GachaItem { itemName = "常驻四星5", starRating = 4 },
            };
            standard3StarPool = new List<GachaItem>
            {
                new GachaItem { itemName = "三星1", starRating = 3 },
                new GachaItem { itemName = "三星2", starRating = 3 },
                new GachaItem { itemName = "三星3", starRating = 3 },
                new GachaItem { itemName = "三星4", starRating = 3 },
                new GachaItem { itemName = "三星5", starRating = 3 },
                new GachaItem { itemName = "三星6", starRating = 3 },
                new GachaItem { itemName = "三星7", starRating = 3 },
                new GachaItem { itemName = "三星8", starRating = 3 },
            };
        }

        [ContextMenu("TestPull")]
        public void TestPull()
        {
            // 统计单抽星级数量
            Dictionary<int, int> singlePullStarCount = new Dictionary<int, int> { { 3, 0 }, { 4, 0 }, { 5, 0 } };
            for (int i = 0; i < 90; i++)
            {
                GachaItem item = PerformPull();
                singlePullStarCount[item.starRating]++;
                if (item.starRating == 5)
                {
                    Debug.Log($"<color=#ffd32a>第 {last5StarPullCount} 抽抽到了5星: {item.itemName}| 是否UP: {isLastPullUp}</color>");
                }
                //else
                //{
                //    Debug.Log($"第 {i + 1} 抽: {item.itemName} | 星级: {item.starRating} | 是否UP: {isLastPullUp}");
                //}
            }
            Debug.Log($"单抽统计: 三星: {singlePullStarCount[3]} | 四星: {singlePullStarCount[4]} | 五星: {singlePullStarCount[5]}");
            Debug.Log($"-------------------------");

            // 统计十连抽星级数量
            Dictionary<int, int> tenPullStarCount = new Dictionary<int, int> { { 3, 0 }, { 4, 0 }, { 5, 0 } };
            for (int i = 0; i < 9; i++)
            {
                List<GachaItem> tenPullResults = Perform10Pull();
                foreach (var item in tenPullResults)
                {
                    tenPullStarCount[item.starRating]++;
                    if (item.starRating == 5)
                    {
                        Debug.Log($"<color=#ffd32a>第 {last5StarPullCount} 抽抽到了5星: {item.itemName}| 是否UP: {isLastPullUp}</color>");
                    }
                    //else
                    //{
                    //    Debug.Log($"十连抽: {item.itemName} | 星级: {item.starRating} | 是否UP: {isLastPullUp}");
                    //}
                }
            }
            Debug.Log($"十连抽统计: 三星: {tenPullStarCount[3]} | 四星: {tenPullStarCount[4]} | 五星: {tenPullStarCount[5]}");
        }

        [ContextMenu("TestPull1")]
        public void TestPull1()
        {
            // 统计单抽星级数量
            Dictionary<int, int> singlePullStarCount = new Dictionary<int, int> { { 3, 0 }, { 4, 0 }, { 5, 0 } };
            for (int i = 0; i < 90; i++)
            {
                GachaItem item = PerformPull();
                singlePullStarCount[item.starRating]++;
                if (item.starRating == 5)
                {
                    Debug.Log($"<color=#ffd32a>第 {last5StarPullCount} 抽抽到了5星: {item.itemName}| 是否UP: {isLastPullUp}</color>");
                }
                //else
                //{
                //    Debug.Log($"第 {i + 1} 抽: {item.itemName} | 星级: {item.starRating} | 是否UP: {isLastPullUp}");
                //}
            }
            Debug.Log($"单抽统计: 三星: {singlePullStarCount[3]} | 四星: {singlePullStarCount[4]} | 五星: {singlePullStarCount[5]}");
            Debug.Log($"-------------------------");
        }

        [ContextMenu("TestPull10")]
        public void TestPull10()
        {
            // 统计十连抽星级数量
            Dictionary<int, int> tenPullStarCount = new Dictionary<int, int> { { 3, 0 }, { 4, 0 }, { 5, 0 } };
            for (int i = 0; i < 9; i++)
            {
                List<GachaItem> tenPullResults = Perform10Pull();
                foreach (var item in tenPullResults)
                {
                    tenPullStarCount[item.starRating]++;
                    if (item.starRating == 5)
                    {
                        Debug.Log($"<color=#ffd32a>第 {last5StarPullCount} 抽抽到了5星: {item.itemName}| 是否UP: {isLastPullUp}</color>");
                    }
                    //else
                    //{
                    //    Debug.Log($"十连抽: {item.itemName} | 星级: {item.starRating} | 是否UP: {isLastPullUp}");
                    //}
                }
            }
            Debug.Log($"十连抽统计: 三星: {tenPullStarCount[3]} | 四星: {tenPullStarCount[4]} | 五星: {tenPullStarCount[5]}");
        }
    }
}