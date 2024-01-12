using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;

namespace GameLogic
{
    public class UIModule : CustommModuleInitialize
    {
        #region 实例与初始化
        public static UIModule Instance = new UIModule();
        public bool IsInited { get; private set; }
        private double initProgress = 0;
        public double InitProgress { get { return initProgress; } }
        #endregion

        public IEnumerator Init()
        {
            Log.Debug("UIModule 初始化");
            initProgress = 0;

            CreateRoot();

            yield return null;
            initProgress = 100;
            IsInited = true;
        }

        public void ClearData()
        {
            Log.Debug("UIModule 清除数据");
        }

        /// <summary>
        /// 场景管理器轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        public void UpdateTime(float elapseSeconds, float realElapseSeconds)
        {

        }

        /// <summary>
        /// 正在加载的UI统计
        /// </summary>
        private int _loadingUICount = 0;

        public int LoadingUICount
        {
            get { return _loadingUICount; }
            set
            {
                _loadingUICount = value;
                if (_loadingUICount < 0)
                {
                    Debug.LogError("Error ---- LoadingUICount < 0");
                }
            }
        }

        //----------------------------------
        public Dictionary<string, UILoadState> UIWindows = new Dictionary<string, UILoadState>();

        public static Action<UIController> OnInitEvent;
        public static Action<UIController> OnOpenEvent;
        public static Action<UIController> OnCloseEvent;

        public EventSystem EventSystem;

        #region 创建根路径

        public GameObject UIRoot { get; private set; }
        //public Camera UICamera { get; private set; }
        public GameObject MainUIRoot { get; private set; }
        public GameObject NormalUIRoot { get; private set; }
        public GameObject HeadInfoUIRoot { get; private set; }
        public GameObject TipsUIRoot { get; private set; }
        /// <summary>
        /// 创建根节点
        /// </summary>
        private void CreateRoot()
        {
            UIRoot = new GameObject("UIRoot");
            MainUIRoot = new GameObject("MainUIRoot");
            NormalUIRoot = new GameObject("NormalUIRoot");
            HeadInfoUIRoot = new GameObject("HeadInfoUIRoot");
            TipsUIRoot = new GameObject("TipsUIRoot");
            MainUIRoot.transform.SetParent(UIRoot.transform, true);
            NormalUIRoot.transform.SetParent(UIRoot.transform, true);
            HeadInfoUIRoot.transform.SetParent(UIRoot.transform, true);
            TipsUIRoot.transform.SetParent(UIRoot.transform, true);

            GameObject.DontDestroyOnLoad(UIRoot);

            EventSystem = new GameObject("EventSystem").AddComponent<EventSystem>();
            EventSystem.gameObject.AddComponent<StandaloneInputModule>();

            GameObject.DontDestroyOnLoad(EventSystem);
        }
        #endregion

        #region UI操作
        /// <summary>
        /// 初始化UI
        /// </summary>
        /// <param name="uiObj"></param>
        private void InitUIAsset(GameObject uiObj)
        {
            if (!uiObj)
            {
                Log.Error("UI对象为空。");
                return;
            }
            var windowAsset = uiObj.GetComponent<UIWindowAsset>();
            var canvas = uiObj.GetComponent<Canvas>();
            switch (windowAsset.panelType)
            {
                case PanelType.MainUI:
                    uiObj.transform.SetParent(MainUIRoot.transform);
                    break;
                case PanelType.NormalUI:
                    uiObj.transform.SetParent(NormalUIRoot.transform);
                    break;
                case PanelType.HeadInfoUI:
                    uiObj.transform.SetParent(HeadInfoUIRoot.transform);
                    break;
                case PanelType.TipsUI:
                    uiObj.transform.SetParent(TipsUIRoot.transform);
                    break;
                default:
                    Log.Error(string.Format("没有默认PanelType", windowAsset.panelType));
                    uiObj.transform.SetParent(UIRoot.transform);
                    break;
            }
        }

        /// <summary>
        /// 加载UI
        /// </summary>
        /// <param name="name"></param>
        /// <param name="openWhenFinish"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public UILoadState LoadWindow(string name, bool openWhenFinish, params object[] args)
        {
            GameObject uiObj = ResourcesModule.Instance.InstantiateAsset<GameObject>(AppConfig.UIPath + name);

            if (uiObj != null)
            {
                InitUIAsset(uiObj);
                uiObj.SetActive(false);
                uiObj.name = name;
                uiObj.transform.localRotation = Quaternion.identity;
                uiObj.transform.localScale = Vector3.one;

                var uiController = uiObj.GetComponent<UIController>();

                UILoadState uiLoadState = new UILoadState(name);
                if (uiController == null)
                {
                    var uiBase = CreateUIController(uiObj, name);
                    uiLoadState.UIWindow = uiBase;
                }
                else
                {
                    uiLoadState.UIWindow = uiController;
                }
                uiLoadState.UIWindow.UIName = name;
                uiLoadState.IsLoading = false;
                uiLoadState.OpenWhenFinish = openWhenFinish;
                uiLoadState.OpenArgs = args;
                uiLoadState.isOnInit = true;
                InitWindow(uiLoadState, uiLoadState.UIWindow, uiLoadState.OpenWhenFinish, uiLoadState.OpenArgs);

                UIWindows.Add(name, uiLoadState);

                return uiLoadState;
            }
            return null;
        }

        /// <summary>
        /// 初始化UI
        /// </summary>
        /// <param name="uiState"></param>
        /// <param name="uiBase"></param>
        /// <param name="open"></param>
        /// <param name="args"></param>
        private void InitWindow(UILoadState uiState, UIController uiBase, bool open, params object[] args)
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            uiBase.OnInit();
            stopwatch.Stop();

            Log.Debug(string.Format("OnInit UI {0}, cost {1}", uiBase.gameObject.name, stopwatch.Elapsed.TotalMilliseconds * 0.001f));


            if (OnInitEvent != null)
                OnInitEvent(uiBase);

            if (open)
            {
                OnOpen(uiState, args);
            }

            if (!open)
            {
                if (!uiState.IsStaticUI)
                {
                    CloseWindow(uiBase.UIName); // Destroy
                    return;
                }
                else
                {
                    uiBase.gameObject.SetActive(false);
                }
            }

            uiState.OnUIWindowLoadedCallbacks(uiState);
        }
        /// <summary>
        /// 和UI通讯
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="callback"></param>
        [Obsolete("使用字符串UI名称代替更灵活!")]
        public void CallUI<T>(Action<T> callback) where T : UIController
        {
            CallUI<T>((_ui, _args) => callback(_ui));
        }

        /// <summary>
        /// 和UI通讯 - 使用泛型方式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="callback"></param>
        /// <param name="args"></param>
        [Obsolete("使用字符串UI名称代替更灵活!")]
        public void CallUI<T>(Action<T, object[]> callback, params object[] args) where T : UIController
        {
            string uiName = typeof(T).Name.Remove(0, 3); // 去掉 "XUI"

            CallUI(uiName, (_uibase, _args) => { callback(_uibase as T, _args); }, args);
        }

        /// <summary>
        /// 和UI通讯
        /// 等待并获取UI实例，执行callback
        /// 源起Loadindg UI， 在加载过程中，进度条设置方法会失效
        /// 如果是DynamicWindow,，使用前务必先要Open!
        /// </summary>
        /// <param name="uiTemplateName"></param>
        /// <param name="callback"></param>
        /// <param name="args"></param>
        public void CallUI(string uiName, Action<UIController, object[]> callback, params object[] args)
        {
            UILoadState uiState;
            if (!UIWindows.TryGetValue(uiName, out uiState))
            {
                uiState = LoadWindow(uiName, false); // 加载，这样就有UIState了, 但注意因为没参数，不要随意执行OnOpen
            }

            uiState.DoCallback(callback, args);
        }

        /// <summary>
        /// 打开窗口
        /// </summary>
        /// <param name="uiState"></param>
        /// <param name="args"></param>
        private void OnOpen(UILoadState uiState, params object[] args)
        {
            if (uiState.IsLoading)
            {
                uiState.OpenWhenFinish = true;
                uiState.OpenArgs = args;
                return;
            }

            UIController uiBase = uiState.UIWindow;

            if (uiBase.gameObject.activeSelf)
            {
                uiBase.OnClose();

                if (OnCloseEvent != null)
                    OnCloseEvent(uiBase);
            }

            uiBase.BeforeOpen(args, () =>
            {
                uiBase.gameObject.SetActive(true);
                System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                stopwatch.Start();
                uiBase.OnOpen(args);
                stopwatch.Stop();

                Log.Debug(string.Format("OnOpen UI {0}, cost {1}", uiBase.gameObject.name, stopwatch.Elapsed.TotalMilliseconds * 0.001f));

                if (OnOpenEvent != null)
                    OnOpenEvent(uiBase);
            });
        }

        /// <summary>
        /// 打开窗口（非复制）
        /// </summary>
        /// <param name="uiName"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public UILoadState OpenWindow(string uiName, params object[] args)
        {
            //TOD需要先创建脚本对象，再根据脚本中的值进行加载资源
            UILoadState uiState;
            if (!UIWindows.TryGetValue(uiName, out uiState))
            {
                uiState = LoadWindow(uiName, true, args);
                return uiState;
            }

            if (!uiState.isOnInit)
            {
                uiState.isOnInit = true;
                if (uiState.UIWindow != null) uiState.UIWindow.OnInit();
            }
            OnOpen(uiState, args);
            return uiState;
        }

        /// <summary>
        /// 设置窗口（非复制）
        /// </summary>
        /// <param name="uiName"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public UILoadState SetWindow(string uiName, params object[] args)
        {
            UILoadState uiState;
            if (!UIWindows.TryGetValue(uiName, out uiState))
            {
                uiState = LoadWindow(uiName, true, args);
                return uiState;
            }

            if (!uiState.isOnInit)
            {
                uiState.isOnInit = true;
                if (uiState.UIWindow != null) uiState.UIWindow.OnInit();
            }
            OnSet(uiState, args);
            return uiState;
        }

        /// <summary>
        /// 设置窗口
        /// </summary>
        /// <param name="uiState"></param>
        /// <param name="args"></param>
        private void OnSet(UILoadState uiState, params object[] args)
        {
            if (uiState.IsLoading)
            {
                uiState.OpenWhenFinish = true;
                uiState.OpenArgs = args;
                return;
            }

            UIController uiBase = uiState.UIWindow;

            if (uiBase.gameObject.activeSelf)
            {
                uiBase.BeforeOpen(args, () =>
                {
                    uiBase.gameObject.SetActive(true);
                    System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                    stopwatch.Start();
                    uiBase.OnSet(args);
                    stopwatch.Stop();

                    Log.Debug(string.Format("OnOpen UI {0}, cost {1}", uiBase.gameObject.name, stopwatch.Elapsed.TotalMilliseconds * 0.001f));

                    if (OnOpenEvent != null)
                        OnOpenEvent(uiBase);
                });
            }

        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <param name="t"></param>
        public void CloseWindow(Type t)
        {
            CloseWindow(t.Name.Remove(0, 3)); // XUI remove
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void CloseWindow<T>()
        {
            CloseWindow(typeof(T));
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <param name="name"></param>
        public void CloseWindow(string name)
        {
            UILoadState uiState;
            if (!UIWindows.TryGetValue(name, out uiState))
            {
                Log.Error(string.Format("[CloseWindow]没有加载的UIWindow: {0}", name));
                return; // 未开始Load
            }

            if (uiState.IsLoading) // Loading中
            {
                Log.Error(string.Format("[CloseWindow]是加载中的{0}", name));
                uiState.OpenWhenFinish = false;
                return;
            }

            uiState.UIWindow.gameObject.SetActive(false);

            uiState.UIWindow.OnClose();

            if (OnCloseEvent != null)
                OnCloseEvent(uiState.UIWindow);

            if (!uiState.IsStaticUI)
            {
                DestroyWindow(name);
            }
        }

        /// <summary>
        /// 销毁所有具有LoadState的窗口。请小心使用。
        /// </summary>
        public void DestroyAllWindows()
        {
            List<string> LoadList = new List<string>();

            foreach (KeyValuePair<string, UILoadState> uiWindow in UIWindows)
            {
                if (IsLoad(uiWindow.Key))
                {
                    LoadList.Add(uiWindow.Key);
                }
            }

            foreach (string item in LoadList)
                DestroyWindow(item, true);
        }

        /// <summary>
        /// 关闭全部窗口
        /// </summary>
        public void CloseAllWindows()
        {
            List<string> toCloses = new List<string>();

            foreach (KeyValuePair<string, UILoadState> uiWindow in UIWindows)
            {
                if (IsOpen(uiWindow.Key))
                {
                    toCloses.Add(uiWindow.Key);
                }
            }

            for (int i = toCloses.Count - 1; i >= 0; i--)
            {
                CloseWindow(toCloses[i]);
            }
        }

        /// <summary>
        /// 销毁窗口
        /// </summary>
        /// <param name="uiName"></param>
        /// <param name="destroyImmediate"></param>
        public void DestroyWindow(string uiName, bool destroyImmediate = false)
        {
            UILoadState uiState;
            UIWindows.TryGetValue(uiName, out uiState);
            if (uiState == null || uiState.UIWindow == null)
            {
                Debug.Log(string.Format("{0} 已被销毁", uiName));
                return;
            }
            if (destroyImmediate)
            {
                UnityEngine.Object.DestroyImmediate(uiState.UIWindow.gameObject);
            }
            else
            {
                UnityEngine.Object.Destroy(uiState.UIWindow.gameObject);
            }

            uiState.UIWindow = null;

            UIWindows.Remove(uiName);
        }
        #endregion

        #region 工具
        /// <summary>
        /// 切换 - 打开的隐藏，隐藏的打开
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        public void ToggleWindow<T>(params object[] args)
        {
            string uiName = typeof(T).Name.Remove(0, 3); // 去掉"CUI"
            ToggleWindow(uiName, args);
        }

        /// <summary>
        /// 切换 - 打开的隐藏，隐藏的打开
        /// </summary>
        /// <param name="name"></param>
        /// <param name="args"></param>
        public void ToggleWindow(string name, params object[] args)
        {
            if (IsOpen(name))
            {
                CloseWindow(name);
            }
            else
            {
                OpenWindow(name, args);
            }
        }

        /// <summary>
        /// 是否被加载了
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool IsLoad(string name)
        {
            if (UIWindows.ContainsKey(name))
                return true;
            return false;
        }

        /// <summary>
        /// 是否已经打开
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool IsOpen(string name)
        {
            UIController uiBase = GetUIBase(name);
            return uiBase == null ? false : uiBase.gameObject.activeSelf;
        }

        /// <summary>
        /// 获取UI控制器
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>

        private UIController GetUIBase(string name)
        {
            UILoadState uiState;
            UIWindows.TryGetValue(name, out uiState);
            if (uiState != null && uiState.UIWindow != null)
                return uiState.UIWindow;

            return null;
        }

        /// <summary>
        /// 给打开的UI添加脚本（脚本从程序集查找）
        /// </summary>
        /// <param name="uiObj"></param>
        /// <param name="uiTemplateName"></param>
        /// <returns></returns>

        public virtual UIController CreateUIController(GameObject uiObj, string uiTemplateName)
        {
            UIController uiBase = uiObj.AddComponent(System.Type.GetType("GameLogic.UI." + uiTemplateName + ", Assembly-CSharp")) as UIController;
            return uiBase;
        }
        #endregion
    }

    public class UILoadState
    {
        public string UIName;
        public UIController UIWindow;
        public Type UIType;
        public bool IsLoading;
        //非复制出来的, 静态UI
        public bool IsStaticUI;
        //是否初始化
        public bool isOnInit = false;
        //完成后是否打开
        public bool OpenWhenFinish;
        public object[] OpenArgs;
        //回调
        internal Queue<Action<UIController, object[]>> CallbacksWhenFinish;
        internal Queue<object[]> CallbacksArgsWhenFinish;

        public UILoadState(string uiName, Type uiControllerType = default(Type))
        {
            if (uiControllerType == default(Type)) uiControllerType = typeof(UIController);

            UIName = uiName;
            UIWindow = null;
            UIType = uiControllerType;

            IsLoading = true;
            OpenWhenFinish = false;
            OpenArgs = null;

            CallbacksWhenFinish = new Queue<Action<UIController, object[]>>();
            CallbacksArgsWhenFinish = new Queue<object[]>();
        }

        /// <summary>
        /// 确保加载完成后的回调
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="args"></param>
        public void DoCallback(Action<UIController, object[]> callback, object[] args = null)
        {
            if (args == null)
                args = new object[0];

            if (IsLoading) // Loading
            {
                CallbacksWhenFinish.Enqueue(callback);
                CallbacksArgsWhenFinish.Enqueue(args);
                return;
            }

            // 立即执行即可
            callback(UIWindow, args);
        }

        internal void OnUIWindowLoadedCallbacks(UILoadState uiState)
        {
            while (uiState.CallbacksWhenFinish.Count > 0)
            {
                Action<UIController, object[]> callback = uiState.CallbacksWhenFinish.Dequeue();
                object[] _args = uiState.CallbacksArgsWhenFinish.Dequeue();
                DoCallback(callback, _args);
            }
        }
    }
}