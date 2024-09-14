using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// UI控制器
    /// </summary>
    public class UIController : MonoBehaviour
    {
        public string UIName = "";

        #region 每个界面都有一个Canvas
        private Canvas canvas;
        public Canvas Canvas
        {
            get
            {
                if (canvas == null && gameObject)
                {
                    canvas = gameObject.GetComponent<Canvas>();
                }

                return canvas;
            }
        }
        #endregion

        #region 每个界面都有一个UIWindowAsset
        private UIWindowAsset windowAsset;
        public UIWindowAsset WindowAsset
        {
            get
            {
                if (windowAsset == null && gameObject)
                {
                    windowAsset = gameObject.GetComponent<UIWindowAsset>();
                }

                return windowAsset;
            }
        }
        #endregion

        private bool isVisiable;
        public bool IsVisiable
        {
            get { return isVisiable; }
            set { isVisiable = value; }
        }

        public virtual void OnInit()
        {

        }

        public virtual void BeforeOpen(object[] onOpenArgs, Action doOpen)
        {
            doOpen();
        }

        public virtual void OnOpen(params object[] args)
        {
            IsVisiable = true;
        }

        public virtual void OnSet(params object[] args)
        {
        }

        public virtual void OnClose()
        {
            IsVisiable = false;
        }

        /// <summary>
        /// 输入uri搜寻控件
        /// findTrans默认参数null时使用this.transform
        /// </summary>
        public T GetControl<T>(string uri, Transform findTrans = null, bool isLog = true) where T : UnityEngine.Object
        {
            return (T)GetControl(typeof(T), uri, findTrans, isLog);
        }

        public object GetControl(Type type, string uri, Transform findTrans = null, bool isLog = true)
        {
            if (findTrans == null)
            {
                findTrans = transform;
            }

            Transform trans = findTrans.Find(uri);
            if (trans == null)
            {
                if (isLog)
                {
                    Log.Error(string.Format("Get UI<{0}> Control Error: " + uri, this));
                }
                return null;
            }

            if (type == typeof(GameObject))
            {
                return trans.gameObject;
            }

            return trans.GetComponent(type);
        }

        /// <summary>
        /// UIModule打开窗口的快捷方式
        /// </summary>
        protected void OpenWindow(string uiName, params object[] args)
        {
            UIModule.Instance.OpenWindow(uiName, args);
        }

        /// <summary>
        /// UIModule关闭窗口的快捷方式
        /// </summary>
        /// <param name="uiName"></param>
        protected void CloseWindow(string uiName = null)
        {
            UIModule.Instance.CloseWindow(uiName ?? UIName);
        }

        [Obsolete("使用字符串UI名称代替更灵活!")]
        public static void CallUI<T>(Action<T> callback) where T : UIController
        {
            UIModule.Instance.CallUI<T>(callback);
        }

        #region 功能
        /// <summary>
        /// 查找控件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T FindControl<T>(string name) where T : Component
        {
            return GameObjectExtensions.Child<T>(gameObject, name);
        }

        /// <summary>
        /// 查找对象
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public GameObject FindGameObject(string name)
        {
            return GameObjectExtensions.Child(gameObject, name);
        }

        /// <summary>
        /// 清除一个GameObject下面所有的孩子
        /// </summary>
        /// <param name="go"></param>
        public void DestroyGameObjectChildren(GameObject go)
        {
            go.ThisClearChild();
        }

        /// <summary>
        /// 从数组获取参数，并且不报错，返回null, 一般用于OnOpen, OnClose的可变参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="openArgs"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        protected T GetFromArgs<T>(object[] openArgs, int offset, bool isLog = true)
        {
            return openArgs.Get<T>(offset, isLog);
        }
        #endregion
    }
}