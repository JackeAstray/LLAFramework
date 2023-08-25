using ExcelDataReader.Log;
using JetBrains.Annotations;
using log4net.Repository.Hierarchy;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameLogic.EditorTools
{
    public class SmallFunctions : EditorWindow
    {
        public static List<string> scenesName = new List<string>();
        public static List<string> scenePaths = new List<string>();
        public static int sceneIndex;

        public void OnEnable()
        {

        }

        [MenuItem("工具箱/小功能", false, 5)]
        public static void SmallFunctionsWindow()
        {
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

            GUILayout.Space(15);

            GUILayout.Label("场景切换");
            GUILayout.BeginHorizontal(); //开始水平布局

            int index = EditorGUILayout.Popup(sceneIndex, scenesName.ToArray());

            if (index != sceneIndex)
            {
                sceneIndex = index;
                LoadScene(sceneIndex);
            }

            GUILayout.EndHorizontal(); //结束水平布局
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
        /// 获取全部场景
        /// </summary>
        public static void GetAllScene()
        {
            scenesName.Clear();
            scenePaths.Clear();

            if (EditorSceneManager.sceneCountInBuildSettings > 0)
            {
                for (int i = 0; i < EditorSceneManager.sceneCountInBuildSettings; i++)
                {
                    string path = SceneUtility.GetScenePathByBuildIndex(i);
                    
                    if (!string.IsNullOrEmpty(path))
                    {
                        scenePaths.Add(path);
                        scenesName.Add(path);
                    }
                }
            }
            sceneIndex = 0;
        }

        /// <summary>
        /// 加载场景
        /// </summary>
        /// <param name="inedx"></param>
        public void LoadScene(int inedx)
        {
            EditorSceneManager.OpenScene(scenePaths[inedx],OpenSceneMode.Single);
        }
    }
}