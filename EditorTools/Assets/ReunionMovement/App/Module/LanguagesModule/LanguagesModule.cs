using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 语言模块
    /// </summary>
    public class LanguagesModule : CustommModuleInitialize
    {
        #region 实例与初始化
        public static LanguagesModule Instance = new LanguagesModule();
        public bool IsInited { get; private set; }
        private double _initProgress = 0;
        public double InitProgress { get { return _initProgress; } }
        #endregion

        LanguageSubject subject = new LanguageSubject();
        Multilingual multilingual;
        
        public IEnumerator Init()
        {
            Log.Debug("LanguagesModule 初始化");
            _initProgress = 0;
            yield return null;
            _initProgress = 100;
            IsInited = true;
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

            if (DatabaseModule.Instance.GetLanguages().ContainsKey(index))
            {
                switch (multilingual)
                {
                    case Multilingual.ZH:
                        text = DatabaseModule.Instance.GetLanguages()[index].ZH;
                        break;

                    case Multilingual.EN:
                        text = DatabaseModule.Instance.GetLanguages()[index].EN;
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
        /// 场景管理器轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        public void Update(float elapseSeconds, float realElapseSeconds)
        {

        }
    }
}