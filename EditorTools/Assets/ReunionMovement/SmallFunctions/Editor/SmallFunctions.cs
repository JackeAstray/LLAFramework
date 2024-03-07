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

        static System.Version version;

        [MenuItem("工具箱/小功能", false, 6)]
        public static void SmallFunctionsWindow()
        {
            version = new System.Version(PlayerSettings.bundleVersion);

            GetAllScene();
            SmallFunctions smallFunctions = GetWindow<SmallFunctions>(true, "小功能", true);
            smallFunctions.minSize = new Vector2(400, 600);
        }

        void OnGUI()
        {
            CreateButtonGroup("屏幕日志", "生成屏幕日志控件", "移除屏幕日志控件", CreateLogComponent, CloseLogComponent);
            CreateButtonGroup("FPS", "生成FPS控件", "移除FPS控件", CreateFPSComponent, CloseFPSComponent);
            CreateButtonGroup("语言", "添加多语言脚本（Text）", "添加多语言脚本（TextMesh）", AddLanguageScript<Text, UIText>, AddLanguageScript<TextMeshProUGUI, UIText_TMP>);
            CreateButtonGroup("UI波纹", "添加波纹效果（Image）", "移除波纹效果（UIRipple）", AddRippleEffect<Image, UIRipple>, RemoveRippleEffect<UIRipple>);

            GUILayout.Label("场景切换");
            GUILayout.BeginVertical();
            for (int i = 0; i < scenesName.Count; i++)
            {
                if (GUILayout.Button(scenesName[i]))
                {
                    LoadScene(scenePaths[i]);
                }
            }
            GUILayout.EndVertical();

            GUILayout.Label("修改版本号" + "    " + "当前版本" + version);
            GUILayout.BeginVertical();
            CreateVersionButtonGroup("主版本号", (v) => new System.Version(v.Major + 1, v.Minor, 1),
                                                 (v) => new System.Version(v.Major - 1, v.Minor, 1));
            CreateVersionButtonGroup("副版本号", (v) => new System.Version(v.Major, v.Minor + 1, 1),
                                                 (v) => new System.Version(v.Major , v.Minor - 1, 1));
            CreateVersionButtonGroup("构建版本号", (v) => new System.Version(v.Major, v.Minor, v.Build + 1),
                                                   (v) => new System.Version(v.Major, v.Minor, v.Build - 1));
            GUILayout.EndVertical();
        }

        void CreateButtonGroup(string label, string button1Text, string button2Text, Action button1Action, Action button2Action)
        {
            GUILayout.Label(label);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(button1Text, GUILayout.Width(195)))
            {
                button1Action();
            }
            if (GUILayout.Button(button2Text, GUILayout.Width(195)))
            {
                button2Action();
            }
            GUILayout.EndHorizontal();
        }

        void CreateVersionButtonGroup(string label, 
                                      Func<System.Version, System.Version> versionChangeFunc1,
                                      Func<System.Version, System.Version> versionChangeFunc2)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(label + "+", GUILayout.Width(129)))
            {
                version = versionChangeFunc1(version);
                PlayerSettings.bundleVersion = version.ToString();
            }
            if (GUILayout.Button(label + "-", GUILayout.Width(129)))
            {
                version = versionChangeFunc2(version);
                PlayerSettings.bundleVersion = version.ToString();
            }
            GUILayout.EndHorizontal();
        }

        void AddLanguageScript<T, U>() where T : Component where U : Component
        {
            GameObject selectedObject = Selection.activeGameObject;
            var assetPath = EditorUtility.IsPersistent(selectedObject);
            if (assetPath == false)
            {
                if (selectedObject.GetComponent<T>())
                {
                    selectedObject.AddComponent<U>();
                }
                else
                {
                    Log.Warning("选中的对象缺少" + typeof(T).Name + "部件，不予添加！");
                }
            }
            else
            {
                Log.Warning("选中的对象必须在Hierachy视图！");
            }
        }

        void AddRippleEffect<T, U>() where T : Component where U : Component
        {
            GameObject selectedObject = Selection.activeGameObject;
            var assetPath = EditorUtility.IsPersistent(selectedObject);
            if (assetPath == false)
            {
                if (selectedObject.GetComponent<T>())
                {
                    selectedObject.AddComponent<U>();
                }
                else
                {
                    Log.Warning("选中的对象缺少" + typeof(T).Name + "部件，不予添加！");
                }
            }
            else
            {
                Log.Warning("选中的对象必须在Hierachy视图！");
            }
        }

        void RemoveRippleEffect<T>() where T : Component
        {
            GameObject selectedObject = Selection.activeGameObject;
            var assetPath = EditorUtility.IsPersistent(selectedObject);
            if (assetPath == false)
            {
                if (selectedObject.GetComponent<T>())
                {
                    DestroyImmediate(selectedObject.GetComponent<T>());
                }
                else
                {
                    Log.Warning("选中的对象缺少" + typeof(T).Name + "部件，无法移除！");
                }
            }
            else
            {
                Log.Warning("选中的对象必须在Hierachy视图！");
            }
        }

        public static void CreateLogComponent()
        {
            CreateComponent<ScreenLogger>("ScreenLogger");
        }

        public static void CloseLogComponent()
        {
            CloseComponent<ScreenLogger>();
        }

        public static void CreateFPSComponent()
        {
            CreateComponent<FPSCounter>("FPSCounter");
        }

        public static void CloseFPSComponent()
        {
            CloseComponent<FPSCounter>();
        }

        public static void CreateComponent<T>(string name) where T : Component
        {
            GameObject obj = GameObject.Find(name);

            if (obj)
            {
                if (!obj.GetComponent<T>())
                {
                    obj.AddComponent<T>();
                }
            }
            else
            {
                obj = new GameObject(name);
                Selection.activeGameObject = obj;
                obj.AddComponent<T>();
            }
        }

        public static void CloseComponent<T>() where T : Component
        {
            GameObject[] objects = GameObject.FindObjectsOfType<GameObject>();

            foreach (GameObject obj in objects)
            {
                if (obj.GetComponent<T>())
                {
                    GameObject.DestroyImmediate(obj);
                }
            }
        }

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

        public void LoadScene(string scenePaths)
        {
            EditorSceneManager.OpenScene(scenePaths, OpenSceneMode.Single);
        }
    }
}