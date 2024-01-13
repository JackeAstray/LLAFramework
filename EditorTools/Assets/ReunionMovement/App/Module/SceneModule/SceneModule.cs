using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace GameLogic
{
    /// <summary>
    /// 场景模块
    /// </summary>
    public class SceneModule : CustommModuleInitialize
    {
        #region 实例与初始化
        public static SceneModule Instance = new SceneModule();
        public bool IsInited { get; private set; }
        private double initProgress = 0;
        public double InitProgress { get { return initProgress; } }
        #endregion

        #region 加载相关
        private UnityAction beforeSceneLoadingCompletionCallback = null;  // 场景加载前回调
        private UnityAction sceneLoadingCompletionCallback = null;        // 场景加载完成回调

        private string strTargetSceneName = null;   // 将要加载的场景名
        private string strCurSceneName = null;      // 当前场景名，如若没有场景，则默认返回
        private string strPreSceneName = null;      // 上一个场景名
        private bool bLoading = false;              // 是否正在加载中
        private const string strLoadSceneName = "LoadingScene";  // 加载场景名字

        public event Action<float> GetProgress;     //事件 用于处理进度条

        public float startProgressWaitingTime;       //开始 - 等待时长
        public float endProgressWaitingTime;         //结束 - 等待时长

        public bool openLoad;

        private Coroutine onLoadingSceneCoroutine;
        private Coroutine onLoadTargetSceneCoroutine;
        #endregion

        #region Get
        public string GetCurrentSceneName() => strCurSceneName;
        #endregion

        public IEnumerator Init()
        {
            Log.Debug("SceneModule 初始化");
            initProgress = 0;

            strCurSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

            startProgressWaitingTime = 0.5f;
            endProgressWaitingTime = 0.5f;

            yield return null;
            initProgress = 100;
            IsInited = true;
        }

        public void ClearData()
        {
            Log.Debug("SceneModule 清除数据");
        }

        /// <summary>
        /// 场景管理器轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        public void UpdateTime(float elapseSeconds, float realElapseSeconds)
        {

        }

        #region Load
        /// <summary>
        /// 返回上一场景
        /// </summary>
        public void LoadPreScene()
        {
            if (string.IsNullOrEmpty(strPreSceneName))
            {
                return;
            }
            LoadScene(strPreSceneName);
        }

        /// <summary>
        /// 返回上一场景
        /// </summary>
        public void LoadPreScene_OpenLoad(UnityAction bslcc = null, UnityAction slcc = null)
        {
            if (string.IsNullOrEmpty(strPreSceneName))
            {
                return;
            }
            LoadScene(strPreSceneName, true, bslcc, slcc);
        }

        /// <summary>
        /// 加载场景
        /// </summary>
        /// <param name="strLevelName">要加载的场景名称</param>
        /// <param name="openLoad">是否开启load场景</param>
        /// <param name="bslcc">场景加载完成前回调</param>
        /// <param name="slcc">场景加载完成回调</param>
        public void LoadScene(string strLevelName, bool openLoad = false, UnityAction bslcc = null, UnityAction slcc = null)
        {
            LoadSceneAsync(strLevelName, openLoad, bslcc, slcc);
        }

        /// <summary>
        /// 加载场景
        /// </summary>
        /// <param name="strLevelName">要加载的场景名称</param>
        /// <param name="openLoad">是否开启load场景</param>
        /// <param name="slcc">场景加载完成回调</param>
        /// <param name="bslcc">场景加载完成前回调</param>
        private void LoadSceneAsync(string strLevelName, bool openLoad, UnityAction bslcc, UnityAction slcc)
        {
            if (bLoading || strCurSceneName == strLevelName)
            {
                bslcc?.Invoke();
                slcc?.Invoke();
                return;
            }

            // 锁屏
            bLoading = true;
            // 开始加载
            sceneLoadingCompletionCallback = slcc;
            beforeSceneLoadingCompletionCallback = bslcc;
            strTargetSceneName = strLevelName;
            strPreSceneName = strCurSceneName;
            strCurSceneName = strLoadSceneName;
            this.openLoad = openLoad;
            if (openLoad)
            {
                //先异步加载 Loading 界面
                onLoadingSceneCoroutine = StartApp.Instance.StartMyCoroutine(OnLoadingScene(strLoadSceneName, OnLoadingSceneLoaded, LoadSceneMode.Single));
            }
            else
            {
                OnLoadingSceneLoaded();
            }
        }

        /// <summary>
        /// 加载过渡场景
        /// </summary>
        /// <param name="OnSecenLoaded"></param>
        /// <param name="OnSceneProgress"></param>
        /// <param name="loadSceneMode"></param>
        /// <returns></returns>
        private IEnumerator OnLoadingScene(string strLoadSceneName, UnityAction OnSecenLoaded, LoadSceneMode loadSceneMode)
        {
            AsyncOperation async = SceneManager.LoadSceneAsync(strLoadSceneName, loadSceneMode);

            if (async == null)
            {
                yield break;
            }

            while (!async.isDone)
            {
                float fProgressValue;
                if (async.progress < 0.9f)
                {
                    fProgressValue = async.progress;
                }
                else
                {
                    fProgressValue = 1.0f;
                }
                yield return null;
            }

            Log.Debug("Loading场景加载完成！");
            ExecuteBslcc();
            CallbackProgress(0);
            OnSecenLoaded?.Invoke();
        }

        /// <summary>
        /// 过渡场景加载完成回调
        /// </summary>
        private void OnLoadingSceneLoaded()
        {
            if (openLoad)
            {
                ExecuteBslcc();
            }
            // 过渡场景加载完成后加载下一个场景
            onLoadTargetSceneCoroutine = StartApp.Instance.StartMyCoroutine(OnLoadTargetScene(strTargetSceneName, LoadSceneMode.Single));
        }

        /// <summary>
        /// 加载目标场景
        /// </summary>
        /// <param name="strLevelName"></param>
        /// <param name="loadSceneMode"></param>
        /// <returns></returns>
        private IEnumerator OnLoadTargetScene(string strLevelName, LoadSceneMode loadSceneMode)
        {
            AsyncOperation async = SceneManager.LoadSceneAsync(strLevelName, loadSceneMode);

            if (async == null)
            {
                Log.Error("加载场景失败：AsyncOperation 为 null");
                yield break;
            }

            if (Application.platform != RuntimePlatform.WebGLPlayer)
            {
                async.allowSceneActivation = false;
            }

            CallbackProgress(0.15f);

            yield return new WaitForSeconds(startProgressWaitingTime);

            //加载进度
            while (async.progress < 0.9f)
            {
                CallbackProgress(async.progress);
                yield return null;
            }

            yield return new WaitForSeconds(endProgressWaitingTime);

            CallbackProgress(1f);

            if (Application.platform != RuntimePlatform.WebGLPlayer)
            {
                async.allowSceneActivation = true;
            }

            while (!async.isDone)
            {
                yield return null;
            }

            OnTargetSceneLoaded();

            Log.Debug("目标场景加载完成！");
            ExecuteSlcc();
        }

        /// <summary>
        /// 加载下一场景完成回调
        /// </summary>
        private void OnTargetSceneLoaded()
        {
            bLoading = false;
            strCurSceneName = strTargetSceneName;
            strTargetSceneName = null;
        }

        /// <summary>
        /// 场景加载完成前回调
        /// </summary>
        private void ExecuteBslcc()
        {
            beforeSceneLoadingCompletionCallback?.Invoke();
        }
        /// <summary>
        /// 场景加载完成回调
        /// </summary>
        private void ExecuteSlcc()
        {
            sceneLoadingCompletionCallback?.Invoke();
        }

        /// <summary>
        /// 回调用于返回进度
        /// </summary>
        /// <param name="progress"></param>
        public void CallbackProgress(float progress)
        {
            Log.Debug(progress);
            if (GetProgress != null)
            {
                GetProgress(progress);
            }
        }
        #endregion
    }
}