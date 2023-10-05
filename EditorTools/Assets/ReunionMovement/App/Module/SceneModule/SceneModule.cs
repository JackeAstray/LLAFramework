using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
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
        private double _initProgress = 0;
        public double InitProgress { get { return _initProgress; } }
        #endregion

        #region 加载相关
        private UnityAction onSceneLoaded = null;   // 场景加载完成回调
        private string strTargetSceneName = null;   // 将要加载的场景名
        private string strCurSceneName = null;      // 当前场景名，如若没有场景，则默认返回
        private string strPreSceneName = null;      // 上一个场景名
        private bool bLoading = false;              // 是否正在加载中
        private const string strLoadSceneName = "LoadingScene";  // 加载场景名字

        public event Action<float> GetProgress;     //事件 用于处理进度条

        public float startProgressWaitingTime;       //开始 - 等待时长
        public float endProgressWaitingTime;         //结束 - 等待时长  
        #endregion

        #region Get
        public string GetCurrentSceneName() => strCurSceneName;
        #endregion

        public IEnumerator Init()
        {
            Log.Debug("SceneModule 初始化");
            _initProgress = 0;

            strCurSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

            startProgressWaitingTime = 0;
            endProgressWaitingTime = 0;

            yield return null;
            _initProgress = 100;
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


        public void LoadPreScene_OpenLoad()
        {
            if (string.IsNullOrEmpty(strPreSceneName))
            {
                return;
            }
            LoadScene(strPreSceneName, true);
        }

        /// <summary>
        /// 加载场景
        /// </summary>
        /// <param name="strLevelName"></param>
        public void LoadScene(string strLevelName, bool openLoad = false, UnityAction unityAction = null)
        {
            LoadSceneAsync(strLevelName, openLoad, unityAction);
        }

        /// <summary>
        /// 加载场景
        /// </summary>
        /// <param name="strLevelName"></param>
        /// <param name="unityAction"></param>
        private async void LoadSceneAsync(string strLevelName, bool openLoad, UnityAction unityAction)
        {
            if (bLoading || strCurSceneName == strLevelName)
            {
                return;
            }

            // 锁屏
            bLoading = true;  
            // 开始加载
            onSceneLoaded = unityAction;
            strTargetSceneName = strLevelName;
            strPreSceneName = strCurSceneName;
            strCurSceneName = strLoadSceneName;

            if (openLoad)
            {
                //先异步加载 Loading 界面
                await OnLoadingScene(strLoadSceneName, OnLoadingSceneLoaded, LoadSceneMode.Single);
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
        private async Task OnLoadingScene(string strLoadSceneName, UnityAction OnSecenLoaded, LoadSceneMode loadSceneMode)
        {
            try
            {
                AsyncOperation async = SceneManager.LoadSceneAsync(strLoadSceneName, loadSceneMode);

                if (async == null)
                {
                    return;
                }

                await async;

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
                }

                CallbackProgress(0);
                OnSecenLoaded?.Invoke();
            }
            catch(Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

        /// <summary>
        /// 过渡场景加载完成回调
        /// </summary>
        private async void OnLoadingSceneLoaded()
        {
            // 过渡场景加载完成后加载下一个场景
            await OnLoadTargetScene(strTargetSceneName, LoadSceneMode.Single);
        }
        
        /// <summary>
        /// 加载目标场景
        /// </summary>
        /// <param name="strLevelName"></param>
        /// <param name="loadSceneMode"></param>
        /// <returns></returns>
        private async Task OnLoadTargetScene(string strLevelName, LoadSceneMode loadSceneMode)
        {
            try
            {
                AsyncOperation async = SceneManager.LoadSceneAsync(strLevelName, loadSceneMode);

                if (async == null)
                {
                    Log.Error("加载场景失败：AsyncOperation 为 null");
                    return;
                }

                async.allowSceneActivation = false;

                CallbackProgress(0.15f);

                await Task.Delay(TimeSpan.FromSeconds(startProgressWaitingTime));

                //加载进度
                while (!async.isDone)
                {
                    float fProgressValue = async.progress < 0.9f ? async.progress : 1.0f;
                    
                    CallbackProgress(fProgressValue);

                    if (fProgressValue >= 0.9f)
                    {
                        OnTargetSceneLoaded();

                        await Task.Delay(TimeSpan.FromSeconds(endProgressWaitingTime));

                        async.allowSceneActivation = true;

                        Log.Debug("场景加载完成！");
                    }

                    await Task.Yield(); // 让出主线程
                }
            }
            catch (Exception ex)
            {
                Log.Error("加载场景时发生异常：" + ex.ToString());
            }
        }

        /// <summary>
        /// 加载下一场景完成回调
        /// </summary>
        private void OnTargetSceneLoaded()
        {
            bLoading = false;
            strCurSceneName = strTargetSceneName;
            strTargetSceneName = null;
            onSceneLoaded?.Invoke();
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

    /// <summary>
    /// 异步操作等待器
    /// </summary>
    public class AsyncOperationAwaiter : INotifyCompletion
    {
        public Action Continuation;
        public AsyncOperation asyncOperation;
        public bool IsCompleted => asyncOperation.isDone;
        public AsyncOperationAwaiter(AsyncOperation resourceRequest)
        {
            this.asyncOperation = resourceRequest;

            //注册完成时的回调
            this.asyncOperation.completed += Accomplish;
        }

        //awati 后面的代码包装成 continuation ，保存在类中方便完成是调用
        public void OnCompleted(Action continuation) => this.Continuation = continuation;

        public void Accomplish(AsyncOperation asyncOperation) => Continuation?.Invoke();

        public void GetResult() { }
    }
}