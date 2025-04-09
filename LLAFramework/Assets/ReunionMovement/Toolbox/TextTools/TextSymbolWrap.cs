using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic
{
    /// <summary>
    /// 文本符号换行
    /// </summary>
    public class TextSymbolWrap : MonoBehaviour
    {
        private Text text;
        private string origStr;
        private string finalReplaceStr;

        private string replaceStr;
        public string ReplaceStr
        {
            get { return replaceStr; }
            set
            {
                if (replaceStr != value)
                {
                    replaceStr = value;
                }
            }
        }

        /// 标记不换行的空格（换行空格Unicode编码为/u0020，不换行的/u00A0）
        public static readonly string Non_breaking_space = "\u00A0";

        /// 用于匹配标点符号，为了不破坏富文本标签，所以只匹配指定的符号
        private readonly string strPunctuation = @"[，,。.?？！!…“”‘’""'':：;：《》、]";

        /// 用于存储text组件中的内容
        private StringBuilder TempText = null;

        /// 用于存储text生成器中的内容
        private IList<UILineInfo> TextLine;

        private int screenWidth = 0;
        private int screenHeight = 0;
        private string endString = "　";
        private bool isReplacing = false;
        private Regex punctuationRegex;

        void Awake()
        {
            text = GetComponent<Text>();
            TempText = new StringBuilder("");
            punctuationRegex = new Regex(strPunctuation);
        }

        private void OnEnable()
        {
            CheckTextComponent();
            CheckScreenSizeChange();
            ReplaceTextFun();
        }

        void Update()
        {
            if (CheckScreenSizeChange() == true)
            {
                if (text != null && string.IsNullOrEmpty(origStr) == false)
                {
                    SetTextComText(origStr);
                    ReplaceStr = "";
                    finalReplaceStr = "";
                }
            }
            CheckReplaceText();
        }

        /// <summary>
        /// 检查屏幕大小更改
        /// </summary>
        /// <returns></returns>
        private bool CheckScreenSizeChange()
        {
            if (Screen.width != screenWidth || Screen.height != screenHeight)
            {
                screenWidth = Screen.width;
                screenHeight = Screen.height;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 检查文本组件
        /// </summary>
        private void CheckTextComponent()
        {
            if (text != null)
            {
                return;
            }
            text = this.gameObject.GetComponent<Text>();
        }

        /// <summary>
        /// 选中替换文本
        /// </summary>
        private void CheckReplaceText()
        {
            if (text == null)
            {
                return;
            }
            if (CheckTextIsChange() == false)
            {
                return;
            }
            ReplaceTextFun();
        }

        /// <summary>
        /// 替换文字方法
        /// </summary>
        private void ReplaceTextFun()
        {
            if (isReplacing == true)
            {
                return;
            }

            ReplaceStr = "";
            finalReplaceStr = "";
            StartCoroutine(ClearUpPunctuationMode(text));
        }

        /// <summary>
        /// 检查文本是否更改
        /// </summary>
        /// <returns></returns>
        private bool CheckTextIsChange()
        {
            if (text == null)
            {
                return false;
            }
            string txt = text.text;
            if (string.Equals(txt, finalReplaceStr) == true)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 清除标点符号模式
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        IEnumerator ClearUpPunctuationMode(Text component)
        {
            isReplacing = true;
            const float delay = 0.001f;
            yield return new WaitForSeconds(delay);

            if (string.IsNullOrEmpty(component.text))
            {
                isReplacing = false;
            }
            else
            {
                string tempTxt = component.text;
                bool isOrigStr = false;
                if (tempTxt[^1] != ' ')
                {
                    origStr = tempTxt;
                    isOrigStr = true;
                }
                TextLine = component.cachedTextGenerator.lines;
                int ChangeIndex = -1;
                TempText.Clear().Append(component.text);
                for (int i = 1; i < TextLine.Count; i++)
                {
                    UILineInfo lineInfo = TextLine[i];
                    int startCharIdx = lineInfo.startCharIdx;
                    if (TempText.Length <= startCharIdx)
                    {
                        continue;
                    }
                    bool IsPunctuation = punctuationRegex.IsMatch(TempText[startCharIdx].ToString());
                    if (TempText[startCharIdx].ToString() == Non_breaking_space)
                    {
                        IsPunctuation = true;
                    }

                    if (!IsPunctuation)
                    {
                        continue;
                    }
                    else
                    {
                        ChangeIndex = TextLine[i].startCharIdx;
                        while (IsPunctuation)
                        {
                            ChangeIndex = ChangeIndex - 1;
                            if (ChangeIndex < 0) break;

                            IsPunctuation = punctuationRegex.IsMatch(TempText[ChangeIndex].ToString());
                            if (TempText[ChangeIndex].ToString() == Non_breaking_space)
                            {
                                IsPunctuation = true;
                            }

                        }
                        if (ChangeIndex < 0) continue;

                        if (ChangeIndex > 0 && TempText[ChangeIndex - 1] != '\n')
                            TempText.Insert(ChangeIndex, "\n");
                    }

                }

                ReplaceStr = TempText.ToString();
                if (string.Equals(tempTxt, ReplaceStr) == false)
                {
                    if (isOrigStr)
                    {
                        ReplaceStr += endString;
                    }
                    component.text = ReplaceStr;

                }
                else
                {
                    finalReplaceStr = ReplaceStr;
                }
                isReplacing = false;
            }
        }

        /// <summary>
        /// 设置文本组件的文本
        /// </summary>
        /// <param name="newText"></param>
        private void SetTextComText(string newText)
        {
            if (text.text != newText)
            {
                text.text = newText;
            }
        }
    }
}