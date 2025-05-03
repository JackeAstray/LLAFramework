using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace LLAFramework.EditorTools
{
    public class CreateUIPlane : EditorWindow
    {
        static string scriptOutPutPath = "Assets/ReunionMovement/GenerateScript/App/UI/UIPlane/";// 脚本输出路径
        static string prefabsOutPutPath = "Assets/Resources/Prefabs/UIs/";// 场景导出UI路径
        static string sceneOutPutPath = "Assets/ReunionMovement/Editor/Scenes/";// 场景导出UI路径

        string className = "ClassName"; // 类名
        string uiResolution = "1920,1080";

        string scriptName { get; set; }
        string prefabsName { get; set; }
        string sceneName { get; set; }
        string targetName { get; set; }
        GameObject uiObj { get; set; }


        [MenuItem("工具箱/UI工具", false, 40)]
        //创建UI
        public static void ShowWindow()
        {
            //弹出编辑器
            CreateUIPlane createUIPlane = GetWindow<CreateUIPlane>(true, "创建新UI&脚本", true);
            // 窗口的尺寸
            Vector2 windowSize = new Vector2(400, 600);
            // 设置最小尺寸
            createUIPlane.minSize = windowSize;
        }

        void OnGUI()
        {
            // 文本
            GUILayout.Label("");
            // 编辑类名和场景名
            className = EditorGUILayout.TextField("输入类名：", className);
            scriptOutPutPath = EditorGUILayout.TextField("脚本输出路径：", scriptOutPutPath);
            GUILayout.Space(5);
            if (GUILayout.Button("第一步 创建场景和在场景中的预制体"))
            {
                //按下按钮后执行的方法
                CreateUI();
            }

            if (GUILayout.Button("第二步 创建脚本"))
            {
                //按下按钮后执行的方法
                CreateSpript();
            }

            if (GUILayout.Button("第三步 绑定脚本"))
            {
                //按下按钮后执行的方法
                BindingSpriptRoot();
            }

            GUILayout.Space(20);

            if (GUILayout.Button("将当前场景中的UI导出为Prefabs（只以WindowAsset为根节点）"))
            {
                //按下按钮后执行的方法
                ExportingPrefabsFromAScene();
            }
        }

        /// <summary>
        /// 创建UI
        /// </summary>
        public void CreateUI()
        {
            var currentScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);
            sceneName = className + "UIPlaneScene";
            currentScene.name = sceneName;

            GameObject mainCamera = GameObject.Find("Main Camera");

            if (mainCamera != null)
            {
                GameObject.DestroyImmediate(mainCamera);
            }

            scriptName = className + "UIPlane";

            //创建UI对象
            CreateUIObject();
            //创建相机
            CreateCamera();
            //创建事件系统
            CreateEventSystem();

            var dir = sceneOutPutPath;
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            //保存场景
            SaveScene(currentScene, Path.Combine(sceneOutPutPath, sceneName + ".unity"));

            Selection.activeGameObject = uiObj;
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 创建UI对象
        /// </summary>
        private void CreateUIObject()
        {
            uiObj = new GameObject(scriptName);
            uiObj.layer = (int)UnityLayerDef.UI;
            uiObj.AddComponent<UIWindowAsset>().stringArgument = scriptName;
            var uiPanel = new GameObject("SafeArea").AddComponent<Image>();
            uiPanel.transform.parent = uiObj.transform;
            //设置UI为全屏
            uiPanel.rectTransform.anchoredPosition = new Vector2(0, 0);//设置 UI 元素的位置
            uiPanel.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);
            uiPanel.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
            uiPanel.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 0);
            uiPanel.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 0);
            uiPanel.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0, 0);
            uiPanel.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, 0);
            uiPanel.rectTransform.anchorMin = new Vector2(0f, 0f);
            uiPanel.rectTransform.anchorMax = new Vector2(1f, 1f);
            //给UI加安全区
            uiPanel.gameObject.AddComponent<SafeArea>();

            ResetLocalTransform(uiPanel.transform);

            var canvas = uiObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            CanvasScaler canvasScaler = uiObj.AddComponent<CanvasScaler>();
            uiObj.AddComponent<GraphicRaycaster>();

            var uiSize = new Vector2(1280, 720);

            if (!string.IsNullOrEmpty(uiResolution))
            {
                var sizeArr = uiResolution.Split(',');
                if (sizeArr.Length >= 2) { uiSize = new Vector2(sizeArr[0].ToInt32(), sizeArr[1].ToInt32()); }
            }

            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = uiSize;
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            canvasScaler.matchWidthOrHeight = 0.5f;
        }

        /// <summary>
        /// 创建相机
        /// </summary>
        private void CreateCamera()
        {
            if (GameObject.Find("Camera") == null)
            {
                GameObject cameraObj = new GameObject("Camera");
                cameraObj.layer = (int)UnityLayerDef.UI;

                Camera camera = cameraObj.AddComponent<Camera>();
                camera.clearFlags = CameraClearFlags.Skybox;
                camera.depth = 0;
                camera.backgroundColor = new Color(0.5f, 0.5f, 0.5f, 0f);
                camera.cullingMask = 1 << (int)UnityLayerDef.UI;
                camera.orthographicSize = 1f;
                camera.orthographic = false;
                camera.nearClipPlane = -2f;
                camera.farClipPlane = 2f;

                camera.gameObject.AddComponent<AudioListener>();
            }
        }

        /// <summary>
        /// 创建事件系统
        /// </summary>
        private void CreateEventSystem()
        {
            if (GameObject.Find("EventSystem") == null)
            {
                var evtSystemObj = new GameObject("EventSystem");
                evtSystemObj.AddComponent<EventSystem>();
                evtSystemObj.AddComponent<StandaloneInputModule>();
            }
        }
        private void SaveScene(Scene scene, string path)
        {
            EditorSceneManager.SaveScene(scene, path);
        }

        /// <summary>
        /// 创建脚本
        /// </summary>
        public async void CreateSpript()
        {
            scriptName = className + "UIPlane";
            await GenerateScript(scriptName);
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 绑定脚本到根节点
        /// </summary>
        public void BindingSpriptRoot()
        {
            scriptName = className + "UIPlane";
            var type = Type.GetType("LLAFramework.UI." + scriptName + ", Assembly-CSharp");
            SetUIObj();
            BindScript(uiObj, type);
        }

        /// <summary>
        /// 绑定脚本到目标
        /// </summary>
        public void BindingSpriptToTarget()
        {
            scriptName = className + "UIPlane";
            var type = Type.GetType("LLAFramework.UI." + scriptName + ", Assembly-CSharp");
            SetUIObj();
            GameObject @object = uiObj.transform.Find(targetName).gameObject;
            BindScript(@object, type);
        }

        private void BindScript(GameObject obj, Type type)
        {
            if (!obj.TryGetComponent(type, out Component component))
            {
                obj.AddComponent(type);
                obj.GetComponent<UIController>().UIName = scriptName;
                AssetDatabase.Refresh();
            }
        }

        /// <summary>
        /// 设置UI对象 防止为空
        /// </summary>
        void SetUIObj()
        {
            string objName = "";
            if (uiObj == null)
            {
                objName = className + "UIPlane";
                uiObj = GameObject.Find(objName);

                if (uiObj == null)
                {
                    Debug.LogError("请检查类名！");
                }
            }
        }

        /// <summary>
        /// 导出预制体
        /// </summary>
        void ExportingPrefabsFromAScene()
        {
            var windowAssets = GetUIWIndoeAssetsFromCurrentScene();
            var uiPrefabDir = prefabsOutPutPath;
            if (!Directory.Exists(uiPrefabDir))
                Directory.CreateDirectory(uiPrefabDir);

            foreach (var windowAsset in windowAssets)
            {
                ExportPrefab(windowAsset, uiPrefabDir);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 导出预制体
        /// </summary>
        /// <param name="windowAsset"></param>
        /// <param name="dir"></param>
        private void ExportPrefab(UIWindowAsset windowAsset, string dir)
        {
            var uiPrefabPath = Path.Combine(dir, windowAsset.name + ".prefab");
            var prefab = PrefabUtility.SaveAsPrefabAsset(windowAsset.gameObject, uiPrefabPath);
            EditorUtility.SetDirty(prefab);

            AssetDatabase.ImportAsset(uiPrefabPath, ImportAssetOptions.ForceSynchronousImport);
            Debug.Log("Create UIWindowAsset to prfab: " + uiPrefabPath);
        }

        /// <summary>
        /// 获取当前场景中的UIWindowAsset
        /// </summary>
        /// <returns></returns>
        static UIWindowAsset[] GetUIWIndoeAssetsFromCurrentScene()
        {
            var windowAssets = GameObject.FindObjectsOfType<UIWindowAsset>();
            if (windowAssets.Length <= 0)
            {
                var currentScene = EditorSceneManager.GetActiveScene().path;
                Debug.LogError(string.Format("Not found UIWindowAsset in scene `{0}`", currentScene));
            }

            return windowAssets;
        }

        /// <summary>
        /// 生成脚本
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        static async Task GenerateScript(string name)
        {
            string ScriptTemplate = @"//此脚本是由工具自动生成，请勿手动创建

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LLAFramework.UI
{
    public class {_0_} : UIController
    {
        string openWindow;
        string closeWindow;

        public override void OnInit()
        {
            base.OnInit();
        }

        public override void OnOpen(params object[] args)
        {
            base.OnOpen(args);
        }

        public override void OnSet(params object[] args)
        {
            base.OnSet(args);
        }

        public override void OnClose()
        {
            base.OnClose();
        }

        public void OnDestroy()
        {

        }
        
        //打开窗口
        public void OpenWindow()
        {
            UIModule.Instance.OpenWindow(openWindow);
        }

        //关闭窗口
        public void CloseWindow()
        {
            UIModule.Instance.CloseWindow(closeWindow);
        }
    }
}
";

            string str = ScriptTemplate;
            str = str.Replace("{_0_}", name);

            var dataName = scriptOutPutPath + name + ".cs";
            await Tools.SaveFile(dataName, str);

            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 重置本地坐标
        /// </summary>
        /// <param name="t"></param>
        public static void ResetLocalTransform(Transform t)
        {
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;
        }
    }
}