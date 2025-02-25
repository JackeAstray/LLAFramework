using GameLogic;
using GameLogic.Base;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    public class ScortData
    {
        // 计小分
        public int score;
        // 计分描述
        public string scoreDescription;
        // 是否正确
        public bool isCorrect;
        // 序号
        public int index;
    }

    /// <summary>
    /// 计分信息
    /// </summary>
    public class ScoreInfo
    {
        public int score;

        public List<ScortData> scoreList;

        public ScoreInfo()
        {
            score = 0;
            scoreList = new List<ScortData>();
        }

        // 添加计分
        public void AddScore(ScortData scoreInfo)
        {
            this.score += scoreInfo.score;
            scoreList.Add(scoreInfo);
        }

        // 获取计分信息
        public ScortData GetScoreInfo(int index)
        {
            if (index < scoreList.Count)
            {
                return scoreList[index];
            }
            return null;
        }

        // 设置计分信息
        public void SetScoreInfo(int index, ScortData scoreInfo)
        {
            if (index < scoreList.Count)
            {
                scoreList[index] = scoreInfo;
            }
        }

        // 移除计分信息
        public void RemoveScoreInfo(int index)
        {
            if (index < scoreList.Count)
            {
                scoreList.RemoveAt(index);
            }
        }

        // 查找计分信息
        public void FindScoreInfo(ScortData scoreInfo)
        {
            scoreList.Find((x) => x == scoreInfo);
        }

        // 清除计分信息
        public void Clear()
        {
            score = 0;
            scoreList.Clear();
        }
    }

    /// <summary>
    /// 被观察者 - 问题
    /// </summary>
    public class QuestionObserved : Observed
    {
        public void UpdateState(params object[] args)
        {
            SetState(args);
        }
    }

    public class QuestionItem
    {
        // 是否回答过
        public bool isAnswered;
        // 是否正确
        public bool isCorrect;
        // 用户答案
        public string userAnswer;

        public QuestionConfig questionConfig;
    }

    /// <summary>
    /// 问题模块
    /// </summary>
    public class QuestionMgrBase : SingletonMgr<QuestionMgrBase>
    {
        // 计分信息
        protected ScoreInfo scortInfo = new ScoreInfo();
        // 问题IDs
        protected List<QuestionItem> questionItems = new List<QuestionItem>();
        // 问题ID到索引的映射
        protected Dictionary<int, int> questionIdToIndexMap = new Dictionary<int, int>();
        // 被观察者
        protected QuestionObserved questionObserved = new QuestionObserved();

        protected QuestionItem currentQuestionItem = new QuestionItem();

        protected int currentIndex = -1; // 当前问题索引

        public virtual void Init()
        {

        }

        /// <summary>
        /// 设置问题
        /// </summary>
        /// <param name="indexs"></param>
        public void SetQuestionItems(List<int> indexs)
        {
            questionItems.Clear();
            questionIdToIndexMap.Clear();
            for (int i = 0; i < indexs.Count; i++)
            {
                int itemId = indexs[i];
                QuestionItem questionItem = new QuestionItem();
                questionItem.isAnswered = false;
                questionItem.questionConfig = DatabaseModule.Instance.GetQuestionConfig(itemId);
                questionItems.Add(questionItem);
                questionIdToIndexMap[itemId] = i;
            }

            currentIndex = 0; // 初始化当前索引

            if (questionItems.Count > 0)
            {
                currentQuestionItem = questionItems[currentIndex];
            }

            questionObserved.UpdateState("Init", currentQuestionItem);
        }

        public void Previous()
        {
            if (currentIndex > 0)
            {
                currentIndex--;
                currentQuestionItem = questionItems[currentIndex];
                questionObserved.UpdateState("Previous", currentQuestionItem);
            }
        }

        public void Next()
        {
            if (currentIndex < questionItems.Count - 1)
            {
                currentIndex++;
                currentQuestionItem = questionItems[currentIndex];
                questionObserved.UpdateState("Next", currentQuestionItem);
            }
        }

        /// <summary>
        /// 跳转到指定题目
        /// </summary>
        /// <param name="index"></param>
        public void ToIndex(int index)
        {
            if (index >= 0 && index < questionItems.Count)
            {
                currentIndex = index;
                currentQuestionItem = questionItems[currentIndex];
                questionObserved.UpdateState("ToIndex", currentQuestionItem);
            }
        }

        /// <summary>
        /// 根据ID跳转到指定题目
        /// </summary>
        public void ToIndexById(int id)
        {
            if (questionIdToIndexMap.TryGetValue(id, out int index))
            {
                currentIndex = index;
                currentQuestionItem = questionItems[currentIndex];
                questionObserved.UpdateState("ToIndex", currentQuestionItem);
            }
        }

        public virtual void Submit(string userAnswer)
        {
            if (currentQuestionItem != null && !currentQuestionItem.isAnswered)
            {
                currentQuestionItem.userAnswer = userAnswer;
                if (userAnswer == currentQuestionItem.questionConfig.CorrectAnswer)
                {
                    // 答案正确
                    scortInfo.AddScore(new ScortData
                    {
                        score = currentQuestionItem.questionConfig.Score,
                        scoreDescription = "回答正确",
                        index = currentIndex,
                        isCorrect = true
                    });
                    currentQuestionItem.isCorrect = true;
                }
                else
                {
                    // 答案错误
                    scortInfo.AddScore(new ScortData
                    {
                        score = 0,
                        scoreDescription = "回答错误",
                        index = currentIndex,
                        isCorrect = false
                    });
                    currentQuestionItem.isCorrect = false;
                }

                currentQuestionItem.isAnswered = true;
                questionObserved.UpdateState("Submit", currentQuestionItem);
            }
        }

        public virtual void Send()
        {

        }

        public virtual QuestionObserved GetQuestionObserved()
        {
            return questionObserved;
        }
    }
}