using ExcelDataReader.Log;
using GameLogic.AnimationUI;
using JetBrains.Annotations;
using log4net.Repository.Hierarchy;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;
using TMPro;
using Unity.VisualScripting.YamlDotNet.Core;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static CodiceApp.EventTracking.EventModelSerialization;
using static UnityEditor.Progress;

namespace GameLogic.EditorTools
{
    public class SmallFunctions : EditorWindow
    {
        public static List<string> scenesName = new List<string>();
        public static List<string> scenePaths = new List<string>();

        // 解析版本号
        static System.Version version;
        //static System.Version targetVersion;
        public void OnEnable()
        {

        }

        [MenuItem("工具箱/小功能", false, 6)]
        public static void SmallFunctionsWindow()
        {
            version = new System.Version(PlayerSettings.bundleVersion);

            GetAllScene();
            //弹出编辑器
            SmallFunctions smallFunctions = GetWindow<SmallFunctions>(true, "小功能", true);
            // 窗口的尺寸
            Vector2 windowSize = new Vector2(400, 600);
            // 设置最小尺寸
            smallFunctions.minSize = windowSize; 
        }

        void OnGUI()
        {

            #region 屏幕日志
            GUILayout.Label("屏幕日志");
            GUILayout.BeginHorizontal(); //开始水平布局
            if (GUILayout.Button("生成屏幕日志控件", GUILayout.Width(195)))
            {
                CreateLogComponent();
            }
            if (GUILayout.Button("移除屏幕日志控件", GUILayout.Width(195)))
            {
                CloseLogComponent();
            }
            GUILayout.EndHorizontal(); //结束水平布局
            #endregion

            #region FPS
            GUILayout.Label("FPS");
            GUILayout.BeginHorizontal(); //开始水平布局
            if (GUILayout.Button("生成FPS控件", GUILayout.Width(195)))
            {
                CreateFPSComponent();
            }

            if (GUILayout.Button("移除FPS控件", GUILayout.Width(195)))
            {
                CloseFPSComponent();
            }
            GUILayout.EndHorizontal(); //结束水平布局
            #endregion

            GUILayout.Space(15);

            #region 语言
            GUILayout.Label("给选中的对象添加语言文本");
            GUILayout.BeginHorizontal(); //开始水平布局
            if (GUILayout.Button("添加多语言脚本（Text）", GUILayout.Width(195)))
            {
                GameObject selectedObject = Selection.activeGameObject;
                var assetPath = EditorUtility.IsPersistent(selectedObject);
                if (assetPath == false)
                {
                    if(selectedObject.GetComponent<Text>())
                    {
                        selectedObject.AddComponent<UIText>();
                    }
                    else
                    {
                        Log.Warning("选中的对象缺少Text部件，不予添加！");
                    }
                }
                else
                {
                    Log.Warning("选中的对象必须在Hierachy视图！");
                }
            }
            if (GUILayout.Button("添加多语言脚本（TextMesh）", GUILayout.Width(195)))
            {
                GameObject selectedObject = Selection.activeGameObject;
                var assetPath = EditorUtility.IsPersistent(selectedObject);
                if (assetPath == false)
                {
                    if (selectedObject.GetComponent<TextMeshProUGUI>())
                    {
                        selectedObject.AddComponent<UIText_TMP>();
                    }
                    else
                    {
                        Log.Warning("选中的对象缺少TextMeshProUGUI部件，不予添加！");
                    }
                }
                else
                {
                    Log.Warning("选中的对象必须在Hierachy视图！");
                }
            }
            GUILayout.EndHorizontal(); //结束水平布局
            #endregion

            GUILayout.Space(15);

            #region 场景切换
            GUILayout.Label("场景切换");
            GUILayout.BeginVertical(); //开始垂直布局

            for (int i = 0; i < scenesName.Count; i++)
            {
                if (GUILayout.Button(scenesName[i]))
                {
                    LoadScene(scenePaths[i]);
                }
            }

            GUILayout.EndVertical(); //结束垂直布局
            #endregion

            #region 修改版本号
            GUILayout.Label("修改版本号" + "    " + "当前版本" + version);
            GUILayout.BeginVertical(); //开始垂直布局

            GUILayout.BeginHorizontal(); //开始水平布局
            if (GUILayout.Button("主版本号+", GUILayout.Width(129)))
            {
                version = new System.Version(version.Major + 1, version.Minor, version.Build);
                PlayerSettings.bundleVersion = version.ToString();
                version = new System.Version(PlayerSettings.bundleVersion);
            }
            if (GUILayout.Button("副版本号+", GUILayout.Width(129)))
            {
                version = new System.Version(version.Major, version.Minor + 1, version.Build);
                PlayerSettings.bundleVersion = version.ToString();
            }
            if (GUILayout.Button("构建版本号+", GUILayout.Width(129)))
            {
                version = new System.Version(version.Major, version.Minor, version.Build + 1);
                PlayerSettings.bundleVersion = version.ToString();
            }
            GUILayout.EndHorizontal(); //结束水平布局

            GUILayout.BeginHorizontal(); //开始水平布局
            if (GUILayout.Button("主版本号-", GUILayout.Width(129)))
            {
                version = new System.Version(version.Major - 1, version.Minor, version.Build);
                PlayerSettings.bundleVersion = version.ToString();
            }
            if (GUILayout.Button("副版本号-", GUILayout.Width(129)))
            {
                version = new System.Version(version.Major, version.Minor - 1, version.Build);
                PlayerSettings.bundleVersion = version.ToString();
            }
            if (GUILayout.Button("构建版本号-", GUILayout.Width(129)))
            {
                version = new System.Version(version.Major, version.Minor, version.Build - 1);
                PlayerSettings.bundleVersion = version.ToString();
            }
            GUILayout.EndHorizontal(); //结束水平布局

            GUILayout.Space(5);

            GUILayout.EndVertical(); //开始垂直布局
            #endregion

            #region UI波纹
            GUILayout.Label("给选中的对象添加波纹效果");
            GUILayout.BeginHorizontal(); //开始水平布局
            if (GUILayout.Button("添加波纹效果（Image）", GUILayout.Width(195)))
            {
                GameObject selectedObject = Selection.activeGameObject;
                var assetPath = EditorUtility.IsPersistent(selectedObject);
                if (assetPath == false)
                {
                    if (selectedObject.GetComponent<Image>())
                    {
                        selectedObject.AddComponent<UIRipple>();
                    }
                    else
                    {
                        Log.Warning("选中的对象缺少Image部件，不予添加！");
                    }
                }
                else
                {
                    Log.Warning("选中的对象必须在Hierachy视图！");
                }
            }
            if (GUILayout.Button("移除波纹效果（UIRipple）", GUILayout.Width(195)))
            {
                GameObject selectedObject = Selection.activeGameObject;
                var assetPath = EditorUtility.IsPersistent(selectedObject);
                if (assetPath == false)
                {
                    if (selectedObject.GetComponent<UIRipple>())
                    {
                        DestroyImmediate(selectedObject.GetComponent<UIRipple>());
                    }
                    else
                    {
                        Log.Warning("选中的对象缺少UIRipple部件，无法移除！");
                    }
                }
                else
                {
                    Log.Warning("选中的对象必须在Hierachy视图！");
                }
            }
            GUILayout.EndHorizontal(); //结束水平布局
            #endregion
        }

        /// <summary>
        /// 创建log部件
        /// </summary>
        public static void CreateLogComponent()
        {
            GameObject log = GameObject.Find("ScreenLogger");

            if (log)
            {
                if (!log.GetComponent<ScreenLogger>())
                {
                    log.AddComponent<ScreenLogger>();
                }
                else
                {
                    return;
                }
            }
            else
            {
                log = new GameObject("ScreenLogger");
                Selection.activeGameObject = log;
                // 添加脚本
                log.AddComponent<ScreenLogger>();
            }
        }

        /// <summary>
        /// 清除log部件
        /// </summary>
        public static void CloseLogComponent()
        {
            // 获取场景中所有的GameObject
            GameObject[] objects = GameObject.FindObjectsOfType<GameObject>();

            foreach (GameObject obj in objects)
            {
                if (obj.GetComponent<ScreenLogger>())
                {
                    // 对同名对象进行操作
                    Log.Debug("删除了所有挂载<ScreenLogger>的对象");
                    // 在编辑器中立即移除对象
                    GameObject.DestroyImmediate(obj);
                }
            }
        }

        /// <summary>
        /// 创建log部件
        /// </summary>
        public static void CreateFPSComponent()
        {
            GameObject FPS = GameObject.Find("FPSCounter");

            if (FPS)
            {
                if (!FPS.GetComponent<FPSCounter>())
                {
                    FPS.AddComponent<FPSCounter>();
                }
                else
                {
                    return;
                }
            }
            else
            {
                FPS = new GameObject("FPSCounter");
                Selection.activeGameObject = FPS;
                // 添加脚本
                FPS.AddComponent<FPSCounter>();
            }
        }

        /// <summary>
        /// 清除log部件
        /// </summary>
        public static void CloseFPSComponent()
        {
            // 获取场景中所有的GameObject
            GameObject[] objects = GameObject.FindObjectsOfType<GameObject>();

            foreach (GameObject obj in objects)
            {
                if (obj.GetComponent<FPSCounter>())
                {
                    // 对同名对象进行操作
                    Log.Debug("删除了所有挂载<FPSCounter>的对象");
                    // 在编辑器中立即移除对象
                    GameObject.DestroyImmediate(obj);
                }
            }
        }

        /// <summary>
        /// 获取全部场景
        /// </summary>
        public static void GetAllScene()
        {
            scenesName.Clear();
            scenePaths.Clear();

            foreach (UnityEditor.EditorBuildSettingsScene scene in UnityEditor.EditorBuildSettings.scenes)
            {
                string tempPath = scene.path;
                scenePaths.Add(tempPath);

                string[] Name = tempPath.Split('/');

                foreach (var item in Name)
                {
                    if (item.Contains(".unity"))
                    {
                        scenesName.Add(item.Substring(0, item.IndexOf('.')));
                    }
                }
            }
        }

        /// <summary>
        /// 加载场景
        /// </summary>
        /// <param name="inedx"></param>
        public void LoadScene(string scenePaths)
        {
            EditorSceneManager.OpenScene(scenePaths, OpenSceneMode.Single);
        }
    }
}