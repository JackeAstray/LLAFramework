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
    public class QuestionMgr : SingletonMgr<QuestionMgr>
    {
        #region 属性
        // 计分信息
        private ScoreInfo scortInfo = new ScoreInfo();
        // 问题IDs
        private List<QuestionItem> questionItems = new List<QuestionItem>();
        // 被观察者
        QuestionObserved questionObserved = new QuestionObserved();

        QuestionItem currentQuestionItem = new QuestionItem();

        private int currentIndex = -1; // 当前问题索引
        #endregion

        #region 方法

        public void Start()
        {
            Invoke("Example", 2f);
        }

        public void Example()
        {
            SetQuestionItems(new List<int> { 100001, 100002 });
        }

        /// <summary>
        /// 设置问题
        /// </summary>
        /// <param name="indexs"></param>
        public void SetQuestionItems(List<int> indexs)
        {
            questionItems.Clear();
            foreach (var item in indexs)
            {
                QuestionItem questionItem = new QuestionItem();
                questionItem.isAnswered = false;
                questionItem.questionConfig = DatabaseModule.Instance.GetQuestionConfig(item);
                questionItems.Add(questionItem);
            }

            currentIndex = 0; // 初始化当前索引

            if (questionItems.Count > 0)
            {
                currentQuestionItem = questionItems[currentIndex];
            }

            questionObserved.UpdateState("Init", currentQuestionItem);
        }

        /// <summary>
        /// 上一题
        /// </summary>
        public void Previous()
        {
            if (currentIndex > 0)
            {
                currentIndex--;
                currentQuestionItem = questionItems[currentIndex];
                questionObserved.UpdateState("Previous", currentQuestionItem);
            }
        }

        /// <summary>
        /// 下一题
        /// </summary>
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
        /// 提交
        /// </summary>
        public void Submit(string userAnswer)
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

        /// <summary>
        /// 发送（上传到第三方）
        /// </summary>
        public void Send()
        {

        }

        /// <summary>
        /// 获得被观察者
        /// </summary>
        /// <returns></returns>
        public QuestionObserved GetQuestionObserved()
        {
            return questionObserved;
        }
        #endregion
    }
}