using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LLAFramework
{
    /// <summary>
    /// 语言模块
    /// </summary>
    public class LanguagesModule : CustommModuleInitialize
    {
        #region 实例与初始化
        public static LanguagesModule Instance = new LanguagesModule();
        public bool IsInited { get; private set; }
        private double initProgress = 0;
        public double InitProgress { get { return initProgress; } }
        #endregion

        LanguageSubject subject = new LanguageSubject();
        Multilingual multilingual;

        public IEnumerator Init()
        {
            initProgress = 0;
            yield return null;
            initProgress = 100;
            IsInited = true;
            Log.Debug("LanguagesModule 初始化完成");
        }

        public void ClearData()
        {
            Log.Debug("LanguagesModule 清除数据");
        }

        /// <summary>
        /// 获取多语言类型
        /// </summary>
        /// <returns></returns>
        public Multilingual GetMultilingual()
        {
            return multilingual;
        }

        /// <summary>
        /// 选择语言
        /// </summary>
        /// <param name="value"></param>
        /// <param name="isInit"></param>
        public void SetMultilingual(int value)
        {
            switch (value)
            {
                case 0:
                    multilingual = Multilingual.ZH;
                    break;
                case 1:
                    multilingual = Multilingual.EN;
                    break;
                case 2:
                    multilingual = Multilingual.RU;
                    break;
                case 3:
                    multilingual = Multilingual.FR;
                    break;
                case 4:
                    multilingual = Multilingual.DE;
                    break;
                case 5:
                    multilingual = Multilingual.ES;
                    break;
                case 6:
                    multilingual = Multilingual.IT;
                    break;
                case 7:
                    multilingual = Multilingual.PT;
                    break;
                case 8:
                    multilingual = Multilingual.JP;
                    break;
                case 9:
                    multilingual = Multilingual.KR;
                    break;
            }

            subject.SetState();
        }

        /// <summary>
        /// 根据语言和索引获得文本内容
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string GetTextByIndex(int index)
        {
            string text = "";

            Languages language;
            if (JsonDatabaseModule.Instance.GetLanguages().TryGetValue(index, out language))
            {
                switch (multilingual)
                {
                    case Multilingual.ZH:
                        text = language.ZH;
                        break;

                    case Multilingual.EN:
                        text = language.EN;
                        break;

                    case Multilingual.RU:
                        text = language.RU;
                        break;

                    case Multilingual.FR:
                        text = language.FR;
                        break;

                    case Multilingual.DE:
                        text = language.DE;
                        break;

                    case Multilingual.ES:
                        text = language.ES;
                        break;

                    case Multilingual.IT:
                        text = language.IT;
                        break;

                    case Multilingual.PT:
                        text = language.PT;
                        break;

                    case Multilingual.JP:
                        text = language.JP;
                        break;

                    case Multilingual.KR:
                        text = language.KR;
                        break;
                }
            }
            return text;
        }

        /// <summary>
        /// 获得被观察者
        /// </summary>
        /// <returns></returns>
        public LanguageSubject GetLanguageSubject()
        {
            return subject;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        public void UpdateTime(float elapseSeconds, float realElapseSeconds)
        {

        }
    }
}