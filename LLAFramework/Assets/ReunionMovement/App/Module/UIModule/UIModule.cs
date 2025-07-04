using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LLAFramework
{
    /// <summary>
    /// UI模块
    /// </summary>
    public class UIModule : CustommModuleInitialize
    {
        #region 实例与初始化
        public static UIModule Instance = new UIModule();
        public bool IsInited { get; private set; }
        private double initProgress = 0;
        public double InitProgress { get { return initProgress; } }

        private Dictionary<string, UILoadState> uiStateCache = new Dictionary<string, UILoadState>();
        #endregion
        //----------------------------------
        public static Action<UIController> onInitEvent;
        public static Action<UIController> onOpenEvent;
        public static Action<UIController> onCloseEvent;

        public EventSystem EventSystem;
        public GameObject uiRoot { get; private set; }
        public GameObject mainUIRoot { get; private set; }
        public GameObject normalUIRoot { get; private set; }
        public GameObject headInfoUIRoot { get; private set; }
        public GameObject tipsUIRoot { get; private set; }


        public IEnumerator Init()
        {
            initProgress = 0;
            yield return StartApp.Instance.StartCoroutine(CreateRoot());
            initProgress = 100;
            IsInited = true;
            Log.Debug("UIModule 初始化完成");
        }

        public void ClearData()
        {
            Log.Debug("UIModule 清除数据");
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        public void UpdateTime(float elapseSeconds, float realElapseSeconds)
        {

        }

        /// <summary>
        /// 正在加载的UI统计
        /// </summary>
        private int loadingUICount = 0;

        public int LoadingUICount
        {
            get => loadingUICount;
            set => loadingUICount = value;
        }

        #region 创建根路径

        /// <summary>
        /// 创建根节点
        /// </summary>
        private IEnumerator CreateRoot()
        {
            uiRoot = new GameObject("UIRoot");
            mainUIRoot = new GameObject("MainUIRoot");
            normalUIRoot = new GameObject("NormalUIRoot");
            headInfoUIRoot = new GameObject("HeadInfoUIRoot");
            tipsUIRoot = new GameObject("TipsUIRoot");
            mainUIRoot.transform.SetParent(uiRoot.transform, true);
            normalUIRoot.transform.SetParent(uiRoot.transform, true);
            headInfoUIRoot.transform.SetParent(uiRoot.transform, true);
            tipsUIRoot.transform.SetParent(uiRoot.transform, true);

            GameObject.DontDestroyOnLoad(uiRoot);

            //EventSystem = new GameObject("EventSystem").AddComponent<EventSystem>();
            //EventSystem.gameObject.AddComponent<StandaloneInputModule>();
            //GameObject.DontDestroyOnLoad(EventSystem);

            initProgress = 50;

            yield return null;
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
                    uiObj.transform.SetParent(mainUIRoot.transform);
                    break;
                case PanelType.NormalUI:
                    uiObj.transform.SetParent(normalUIRoot.transform);
                    break;
                case PanelType.HeadInfoUI:
                    uiObj.transform.SetParent(headInfoUIRoot.transform);
                    break;
                case PanelType.TipsUI:
                    uiObj.transform.SetParent(tipsUIRoot.transform);
                    break;
                default:
                    Log.Error(string.Format("没有默认PanelType", windowAsset.panelType));
                    uiObj.transform.SetParent(uiRoot.transform);
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
            if (uiObj == null)
            {
                return null;
            }

            InitUIAsset(uiObj);
            uiObj.SetActive(false);
            uiObj.name = name;
            uiObj.transform.localRotation = Quaternion.identity;
            uiObj.transform.localScale = Vector3.one;

            var uiController = uiObj.GetComponent<UIController>();

            UILoadState uiLoadState = new UILoadState(name);
            uiLoadState.uiWindow = uiController ?? CreateUIController(uiObj, name);
            uiLoadState.uiWindow.UIName = name;
            uiLoadState.isLoading = false;
            uiLoadState.openWhenFinish = openWhenFinish;
            uiLoadState.openArgs = args;
            uiLoadState.isOnInit = true;
            InitWindow(uiLoadState, uiLoadState.uiWindow, uiLoadState.openWhenFinish, uiLoadState.openArgs);

            uiStateCache.Add(name, uiLoadState);

            return uiLoadState;
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
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            uiBase.OnInit();
            stopwatch.Stop();

            Log.Debug($"OnInit UI {uiBase.gameObject.name}, cost {stopwatch.ElapsedMilliseconds * 0.001f}");

            onInitEvent?.Invoke(uiBase);

            if (open)
            {
                OnOpen(uiState, args);
            }

            if (!open)
            {
                if (!uiState.isStaticUI)
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
        /// <param name="uiName"></param>
        /// <param name="callback"></param>
        /// <param name="args"></param>
        public void CallUI(string uiName, Action<UIController, object[]> callback, params object[] args)
        {
            if (uiStateCache.TryGetValue(uiName, out UILoadState uiState))
            {
                // 加载，这样就有UIState了, 但注意因为没参数，不要随意执行OnOpen
                uiState = LoadWindow(uiName, false);
                uiStateCache[uiName] = uiState;
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
            if (uiState.isLoading)
            {
                uiState.openWhenFinish = true;
                uiState.openArgs = args;
                return;
            }

            UIController uiBase = uiState.uiWindow;

            if (uiBase.gameObject.activeSelf)
            {
                uiBase.OnClose();
                onCloseEvent?.Invoke(uiBase);
            }

            uiBase.BeforeOpen(args, () =>
            {
                uiBase.gameObject.SetActive(true);

                LogElapsedTime(() =>
                {
                    uiBase.OnOpen(args);
                }, $"OnOpen UI {uiBase.gameObject.name}");

                onOpenEvent?.Invoke(uiBase);
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
            if (!uiStateCache.TryGetValue(uiName, out uiState))
            {
                uiState = LoadWindow(uiName, true, args);
                return uiState;
            }

            if (!uiState.isOnInit)
            {
                uiState.isOnInit = true;
                if (uiState.uiWindow != null) uiState.uiWindow.OnInit();
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
            if (!uiStateCache.TryGetValue(uiName, out uiState))
            {
                uiState = LoadWindow(uiName, true, args);
                return uiState;
            }

            if (!uiState.isOnInit)
            {
                uiState.isOnInit = true;
                if (uiState.uiWindow != null) uiState.uiWindow.OnInit();
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
            if (uiState.isLoading)
            {
                uiState.openWhenFinish = true;
                uiState.openArgs = args;
                return;
            }

            UIController uiBase = uiState.uiWindow;

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

                    if (onOpenEvent != null)
                        onOpenEvent(uiBase);
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

            // 未开始Load
            if (!uiStateCache.TryGetValue(name, out uiState))
            {
                Log.Error($"[CloseWindow]没有加载的UIWindow: {name}");
                return;
            }

            // Loading中
            if (uiState.isLoading)
            {
                Log.Error($"[CloseWindow]是加载中的{name}");
                uiState.openWhenFinish = false;
                return;
            }

            uiState.uiWindow.gameObject.SetActive(false);

            uiState.uiWindow.OnClose();

            onCloseEvent?.Invoke(uiState.uiWindow);

            if (!uiState.isStaticUI)
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

            foreach (KeyValuePair<string, UILoadState> uiWindow in uiStateCache)
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

            foreach (KeyValuePair<string, UILoadState> uiWindow in uiStateCache)
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
            uiStateCache.TryGetValue(uiName, out uiState);
            if (uiState == null || uiState.uiWindow == null)
            {
                Log.Warning($"{uiName} 已被销毁");
                return;
            }
            if (destroyImmediate)
            {
                UnityEngine.Object.DestroyImmediate(uiState.uiWindow.gameObject);
            }
            else
            {
                UnityEngine.Object.Destroy(uiState.uiWindow.gameObject);
            }

            uiState.uiWindow = null;

            uiStateCache.Remove(uiName);
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
            if (uiStateCache.ContainsKey(name))
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
            uiStateCache.TryGetValue(name, out uiState);
            if (uiState != null && uiState.uiWindow != null)
                return uiState.uiWindow;

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
            UIController uiBase = uiObj.AddComponent(System.Type.GetType("LLAFramework.UI." + uiTemplateName + ", Assembly-CSharp")) as UIController;
            return uiBase;
        }

        private void LogElapsedTime(Action action, string message)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            action();
            stopwatch.Stop();

            Log.Debug($"{message}, cost {stopwatch.ElapsedMilliseconds * 0.001f}");
        }
        #endregion
    }

    public class UILoadState
    {
        public string uiName;
        public UIController uiWindow;
        public Type uiType;
        public bool isLoading;
        //非复制出来的, 静态UI
        public bool isStaticUI;
        //是否初始化
        public bool isOnInit = false;
        //完成后是否打开
        public bool openWhenFinish;
        public object[] openArgs;
        //回调
        internal Queue<Action<UIController, object[]>> callbacksWhenFinish;
        internal Queue<object[]> callbacksArgsWhenFinish;

        public UILoadState(string uiName, Type uiControllerType = default(Type))
        {
            if (uiControllerType == default(Type)) uiControllerType = typeof(UIController);

            this.uiName = uiName;
            uiWindow = null;
            uiType = uiControllerType;

            isLoading = true;
            openWhenFinish = false;
            openArgs = null;

            callbacksWhenFinish = new Queue<Action<UIController, object[]>>();
            callbacksArgsWhenFinish = new Queue<object[]>();
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

            if (isLoading) // Loading
            {
                callbacksWhenFinish.Enqueue(callback);
                callbacksArgsWhenFinish.Enqueue(args);
                return;
            }

            // 立即执行即可
            callback(uiWindow, args);
        }

        internal void OnUIWindowLoadedCallbacks(UILoadState uiState)
        {
            while (uiState.callbacksWhenFinish.Count > 0)
            {
                Action<UIController, object[]> callback = uiState.callbacksWhenFinish.Dequeue();
                object[] _args = uiState.callbacksArgsWhenFinish.Dequeue();
                DoCallback(callback, _args);
            }
        }
    }
}