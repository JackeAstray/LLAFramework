using GameLogic.Download;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    public class StartApp : AppGame
    {
        protected override IList<CustommModuleInitialize> CreateModules()
        {
            var modules = base.CreateModules();

            modules.Add(TerminalModule.Instance);
            modules.Add(ResourcesModule.Instance);
            modules.Add(DatabaseModule.Instance);
            modules.Add(EventModule.Instance);
            modules.Add(LanguagesModule.Instance);
            modules.Add(SoundPoolModule.Instance);
            modules.Add(SoundModule.Instance);
            modules.Add(UIModule.Instance);
            modules.Add(SceneModule.Instance);
            modules.Add(ColorPaletteModule.Instance);
            modules.Add(DownloadImageModule.Instance);
            modules.Add(DownloadFileModule.Instance);

            return modules;
        }

        /// <summary>
        /// 在初始化模块之前，协同程序
        /// </summary>
        /// <returns></returns>
        public override IEnumerator OnBeforeInit()
        {
            Log.Debug("StartGame初始化前");

            ////到期销毁
            //DateTime minTime = Convert.ToDateTime("2023-8-25");
            //DateTime maxTime = Convert.ToDateTime("2023-9-10");
            //if (minTime > DateTime.Now || DateTime.Now > maxTime)
            //{
            //    Destroy(gameObject);
            //}

            yield return null;
        }

        /// <summary>
        /// 在初始化模块之后，协同路由
        /// </summary>
        /// <returns></returns>
        public override IEnumerator OnGameStart()
        {
            Log.Debug("StartGame初始化后");

            yield return null;

            //TerminalModule.Instance.terminalRequest.RegisterCommands();
            StartCoroutine(StartGame());
        }

        public IEnumerator StartGame()
        {
            //UIModule.Instance.OpenWindow("StartAppUIPlane");
            yield return new WaitForSeconds(0f);

            //DownloadFileModule.Instance.SetUrl("http://localhost:8081/Download/%E5%A3%81%E7%BA%B8/%E5%8F%A4%E5%A0%A1%E9%BE%8D%E5%A7%AC.png");

            //string savePath = Application.persistentDataPath + "/Picture/古堡龍姬.png";
            //DownloadFileModule.Instance.DownloadToFile(
            //    4,
            //    savePath,
            //    (size, count) =>
            //    {
            //        UnityEngine.Debug.LogFormat("[{0}]下载进度 >>> {1}/{2}", "多线程下载至本地", size, count);
            //    },
            //    (data) =>
            //    {
            //        UnityEngine.Debug.LogFormat("[{0}]下载完毕>>>{1}", "多线程下载至本地", data.Length);
            //    }
            //);
        }

        /// <summary>
        /// 在应用退出或者在编辑器里停止运行之前调用
        /// </summary>
        void OnApplicationQuit()
        {

        }
        /// <summary>
        /// 当程序获得或者失去焦点时
        /// </summary>
        /// <param name="focus">false失去焦点 true获得焦点</param>
        void OnApplicationFocus(bool focus)
        {

        }

        /// <summary>
        /// 当程序暂停
        /// </summary>
        /// <param name="focus">true暂停 false取消暂停</param>
        void OnApplicationPause(bool focus)
        {

        }

        void Update()
        {
            // 更新事件
            RefreshCoroutineTasks();
        }

        #region 协程
        /// <summary>
        /// 启动协程
        /// </summary>
        /// <param name="coroutine"></param>
        /// <returns></returns>
        public override Coroutine StartMyCoroutine(IEnumerator coroutine)
        {
            return StartCoroutine(coroutine);
        }

        /// <summary>
        /// 停止协程
        /// </summary>
        /// <param name="coroutine"></param>
        public override void StopMyCoroutine(Coroutine coroutine)
        {
            StopCoroutine(coroutine);
        }

        public void AddCoroutine(IEnumerator routine, Action<Coroutine> callback)
        {
            var task = new CoroutineTask();
            task.Enumerator = routine;
            task.Callback = callback;
            coroutineTasks.Add(task);
        }

        /// <summary>
        /// 启动协程
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public Coroutine StartCoroutine(Action handler)
        {
            return StartCoroutine(EnumCoroutine(handler));
        }

        /// <summary>
        /// 启动协程
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public Coroutine StartCoroutine(Action handler, Action callback)
        {
            return StartCoroutine(EnumCoroutine(handler, callback));
        }

        /// <summary>
        /// 枚举协程
        /// </summary>
        /// <param name="routine"></param>
        /// <param name="callBack"></param>
        /// <returns></returns>
        IEnumerator EnumCoroutine(Coroutine routine, Action callBack)
        {
            yield return routine;
            callBack?.Invoke();
        }

        /// <summary>
        /// 枚举协程
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        IEnumerator EnumCoroutine(Action handler)
        {
            handler?.Invoke();
            yield return null;
        }

        /// <summary>
        /// 枚举协程
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="callack"></param>
        /// <returns></returns>
        IEnumerator EnumCoroutine(Action handler, Action callack)
        {
            yield return StartCoroutine(handler);
            callack?.Invoke();
        }

        /// <summary>
        /// 枚举断言协程
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="callBack"></param>
        /// <returns></returns>
        IEnumerator EnumPredicateCoroutine(Func<bool> handler, Action callBack)
        {
            yield return new WaitUntil(handler);
            callBack();
        }

        /// <summary>
        /// 协程任务
        /// </summary>
        void RefreshCoroutineTasks()
        {
            while (coroutineTasks.Count > 0)
            {
                var task = coroutineTasks[0];
                coroutineTasks.RemoveAt(0);
                var coroutine = StartCoroutine(task.Enumerator);
                task.Callback?.Invoke(coroutine);
            }
        }

        /// <summary>
        /// 生成任务Id
        /// </summary>
        /// <returns>任务Id</returns>
        long GenerateTaskId()
        {
            if (taskIndex == long.MaxValue)
                taskIndex = 0;
            else
                taskIndex++;
            return taskIndex;
        }
        #endregion

    }
}