﻿using LLAFramework.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace LLAFramework
{
    public class QuestionObserver : Observer
    {
        #region 属性
        public GameObject questionPanel;
        public RectTransform rect;
        // 上一题
        public Button previous;
        // 下一题
        public Button next;
        // 提交按钮
        public Button submit;
        // 发送按钮
        public Button send;
        // 标题
        public Text title;
        // 结果面板
        public Text resultText;

        public ToggleGroup toggleGroup;
        public List<Toggle> answers = new List<Toggle>();
        public List<Text> answersStr = new List<Text>();
        char[] options = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H' };

        private QuestionObserved questionObserved;

        QuestionItem questionItem;
        public QuestionMgrBase questionMgrBase;

        public string userAnswer;

        private float submitCooldown = 5f; // 提交按钮的冷却时间
        private bool canSubmit = false; // 是否可以提交
        #endregion

        #region 重写父类方法
        public void Start()
        {
            if (questionObserved == null)
            {
                questionObserved = questionMgrBase.GetQuestionObserved();
                questionObserved.Attach(this);
            }

            previous.onClick.AddListener(() =>
            {
                questionMgrBase.Previous();
            });

            next.onClick.AddListener(() =>
            {
                questionMgrBase.Next();
            });

            submit.onClick.AddListener(() =>
            {
                userAnswer = "";

                if (questionItem.isAnswered || !canSubmit)
                {
                    return;
                }

                for (int i = 0; i < answers.Count; i++)
                {
                    if (answers[i].isOn)
                    {
                        userAnswer += options[i];
                    }
                }

                questionMgrBase.Submit(userAnswer);
            });

            send.onClick.AddListener(() =>
            {
                questionMgrBase.Send();
            });

            if (submitCooldown > 0)
            {
                StartCoroutine(EnableSubmitButtonAfterDelay(submitCooldown));
            }
            else
            {
                canSubmit = true;
            }
        }

        private IEnumerator EnableSubmitButtonAfterDelay(float delay)
        {
            float remainingTime = delay;
            while (remainingTime > 0)
            {
                submit.GetComponentInChildren<Text>().text = $"提交 ({remainingTime:F1}s)";
                yield return new WaitForSeconds(0.1f);
                remainingTime -= 0.1f;
            }
            submit.GetComponentInChildren<Text>().text = "提交";
            canSubmit = true;
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="args"></param>
        public override void UpdateData(params object[] args)
        {
            if (args != null)
            {
                string typeStr = args[0].ToString();

                if (args.Length >= 2)
                {
                    questionItem = args[1] as QuestionItem;
                    if (questionItem != null)
                    {
                        if (questionItem.questionConfig.Type == 2)
                        {
                            toggleGroup.enabled = false;
                        }
                        else
                        {
                            toggleGroup.enabled = true;
                        }

                        title.text = questionItem.questionConfig.Title;

                        SetAnswer(questionItem.questionConfig.A, questionItem.userAnswer, questionItem.isAnswered, 0);
                        SetAnswer(questionItem.questionConfig.B, questionItem.userAnswer, questionItem.isAnswered, 1);
                        SetAnswer(questionItem.questionConfig.C, questionItem.userAnswer, questionItem.isAnswered, 2);
                        SetAnswer(questionItem.questionConfig.D, questionItem.userAnswer, questionItem.isAnswered, 3);
                        SetAnswer(questionItem.questionConfig.E, questionItem.userAnswer, questionItem.isAnswered, 4);
                        SetAnswer(questionItem.questionConfig.F, questionItem.userAnswer, questionItem.isAnswered, 5);
                        SetAnswer(questionItem.questionConfig.G, questionItem.userAnswer, questionItem.isAnswered, 6);
                        SetAnswer(questionItem.questionConfig.H, questionItem.userAnswer, questionItem.isAnswered, 7);
                    }

                    resultText.SetActive(false);

                    if (typeStr == "Submit" || questionItem.isAnswered)
                    {
                        resultText.SetActive(true);

                        if (resultText != null)
                        {
                            if (questionItem.isCorrect)
                            {
                                resultText.text = "回答正确";
                                resultText.color = Color.green;
                            }
                            else
                            {
                                resultText.text = "回答错误，正确答案是：" + questionItem.questionConfig.CorrectAnswer;
                                resultText.color = Color.red;
                            }
                        }
                    }
                }
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        }

        /// <summary>
        /// 更新答案选项
        /// </summary>
        /// <param name="answer"></param>
        /// <param name="isAnswered"></param>
        /// <param name="index"></param>
        public void SetAnswer(string answer, string userAnswer, bool isAnswered, int index)
        {
            if (!string.IsNullOrEmpty(answer) && answer != "null" && answer != "Null")
            {
                answers[index].transform.parent.SetActive(true);

                answersStr[index].text = answer;
                if (isAnswered)
                {
                    answers[index].interactable = false;

                    if (userAnswer.IndexOf(options[index]) >= 0)
                    {
                        answers[index].isOn = true;
                    }
                    else
                    {
                        answers[index].isOn = false;
                    }
                }
                else
                {
                    answers[index].interactable = true;
                    answers[index].isOn = false;
                }
            }
            else
            {
                answers[index].transform.parent.SetActive(false);
            }
        }
        #endregion
    }
}
