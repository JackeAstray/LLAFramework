using LLAFramework.Download;
using LLAFramework.Http;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LLAFramework
{
    public class StartApp : AppGame
    {
        [SerializeField] private bool isStateMachineModule;
        [SerializeField] private bool isTaskModule;
        [SerializeField] private bool isTerminalModule;
        [SerializeField] private bool isResourcesModule;
        [SerializeField] private bool isJsonDatabaseModule;
        [SerializeField] private bool isEventModule;
        [SerializeField] private bool isLanguagesModule;
        [SerializeField] private bool isSoundPoolModule;
        [SerializeField] private bool isSoundModule;
        [SerializeField] private bool isUIModule;
        [SerializeField] private bool isSceneModule;
        [SerializeField] private bool isHttpModule;
        [SerializeField] private bool isDownloadManagerModule;
        [SerializeField] private bool isMessageModule;
        [SerializeField] private bool isInputSystemModule;

        protected override IList<CustommModuleInitialize> CreateModules()
        {
            var modules = base.CreateModules();

            if (isStateMachineModule)
            {
                modules.Add(StateMachineModule.Instance);
            }

            if (isTaskModule)
            {
                modules.Add(TaskModule.Instance);
            }

            if (isTerminalModule)
            {
                modules.Add(TerminalModule.Instance);
            }

            if (isResourcesModule)
            {
                modules.Add(ResourcesModule.Instance);
            }

            if (isJsonDatabaseModule)
            {
                modules.Add(JsonDatabaseModule.Instance);
            }

            if (isEventModule)
            {
                modules.Add(EventModule.Instance);
            }

            if (isLanguagesModule)
            {
                modules.Add(LanguagesModule.Instance);
            }

            if (isSoundPoolModule)
            {
                modules.Add(SoundPoolModule.Instance);
            }

            if (isSoundModule)
            {
                modules.Add(SoundModule.Instance);
            }

            if (isUIModule)
            {
                modules.Add(UIModule.Instance);
            }

            if (isSceneModule)
            {
                modules.Add(SceneModule.Instance);
            }

            if (isHttpModule)
            {
                modules.Add(HttpModule.Instance);
            }

            if (isDownloadManagerModule)
            {
                modules.Add(DownloadManagerModule.Instance);
            }

            if (isMessageModule)
            {
                modules.Add(MessageModule.Instance);
            }

            if (isInputSystemModule)
            {
                modules.Add(InputSystemModule.Instance);
            }

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
        /// 游戏启动
        /// </summary>
        /// <returns></returns>
        public override IEnumerator OnGameStart()
        {
            Log.Debug("StartGame初始化后");
            yield return null;
            StartCoroutine(StartGame());
        }

        public IEnumerator StartGame()
        {
            UIModule.Instance.OpenWindow("StartAppUIPlane");
            yield return new WaitForSeconds(0f);
        }

        void OnDownload()
        {
            Debug.Log("下载完成");
        }

        void OnDownloadAll()
        {
            Debug.Log("下载All完成");
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
    }
}