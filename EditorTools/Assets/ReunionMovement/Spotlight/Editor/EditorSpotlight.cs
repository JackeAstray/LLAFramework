using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameLogic.EditorTools
{
    /// <summary>
    /// 焦点搜索工具
    /// </summary>
    public class EditorSpotlight : EditorWindow, IHasCustomMenu
    {
        static class Styles
        {
            public static readonly GUIStyle inputFieldStyle;
            public static readonly GUIStyle placeholderStyle;
            public static readonly GUIStyle resultLabelStyle;
            public static readonly GUIStyle entryEven;
            public static readonly GUIStyle entryOdd;

            public static readonly string proSkinHighlightColor = "";
            public static readonly string proSkinNormalColor = "";

            public static readonly string personalSkinHighlightColor = "";
            public static readonly string personalSkinNormalColor = "";

            static Styles()
            {
                inputFieldStyle = new GUIStyle(EditorStyles.textField)
                {
                    contentOffset = new Vector2(10, 10),
                    fontSize = 32,
                    focused = new GUIStyleState()
                };

                placeholderStyle = new GUIStyle(inputFieldStyle)
                {
                    normal =
                    {
                        textColor = EditorGUIUtility.isProSkin ? new Color(1, 1, 1, .2f) : new Color(.2f, .2f, .2f, .4f)
                    }
                };


                resultLabelStyle = new GUIStyle(EditorStyles.largeLabel)
                {
                    alignment = TextAnchor.MiddleLeft,
                    richText = true
                };

                entryOdd = new GUIStyle("CN EntryBackOdd");
                entryEven = new GUIStyle("CN EntryBackEven");
            }
        }

        [MenuItem("工具箱/焦点工具 %l")]
        private static void Init()
        {
            var window = CreateInstance<EditorSpotlight>();
            window.titleContent = new GUIContent("焦点工具");
            var pos = window.position;
            pos.height = BaseHeight;
            pos.xMin = Screen.currentResolution.width / 2 - 500 / 2;
            pos.yMin = Screen.currentResolution.height * .3f;
            window.position = pos;
            window.EnforceWindowSize();
            window.ShowUtility();

            window.Reset();
        }

        [Serializable]
        private class SearchHistory : ISerializationCallbackReceiver
        {
            public readonly Dictionary<string, int> clicks = new Dictionary<string, int>();

            [SerializeField] List<string> clickKeys = new List<string>();
            [SerializeField] List<int> clickValues = new List<int>();

            public void OnBeforeSerialize()
            {
                clickKeys.Clear();
                clickValues.Clear();

                int i = 0;
                foreach (var pair in clicks)
                {
                    clickKeys.Add(pair.Key);
                    clickValues.Add(pair.Value);
                    i++;
                }
            }

            public void OnAfterDeserialize()
            {
                clicks.Clear();
                for (var i = 0; i < clickKeys.Count; i++)
                    clicks.Add(clickKeys[i], clickValues[i]);
            }
        }

        const string PlaceholderInput = "打开资源...";
        const string SearchHistoryKey = "SearchHistoryKey";
        public const int BaseHeight = 90;

        List<string> hits = new List<string>();
        string input;
        int selectedIndex = 0;

        SearchHistory history;

        void Reset()
        {
            input = "";
            hits.Clear();
            var json = EditorPrefs.GetString(SearchHistoryKey, JsonUtility.ToJson(new SearchHistory()));
            history = JsonUtility.FromJson<SearchHistory>(json);
            Focus();
        }

        void OnLostFocus()
        {
            Close();
        }

        void OnGUI()
        {
            HandleEvents();

            GUILayout.BeginHorizontal();
            GUILayout.Space(15);
            GUILayout.BeginVertical();
            GUILayout.Space(15);

            GUI.SetNextControlName("SpotlightInput");
            var prevInput = input;
            input = GUILayout.TextField(input, Styles.inputFieldStyle, GUILayout.Height(60));
            EditorGUI.FocusTextInControl("SpotlightInput");

            if (input != prevInput)
                ProcessInput();

            if (selectedIndex >= hits.Count)
                selectedIndex = hits.Count - 1;
            else if (selectedIndex <= 0)
                selectedIndex = 0;

            if (string.IsNullOrEmpty(input))
                GUI.Label(GUILayoutUtility.GetLastRect(), PlaceholderInput, Styles.placeholderStyle);

            GUILayout.BeginHorizontal();
            GUILayout.Space(10);

            if (!string.IsNullOrEmpty(input))
            {
                VisualizeHits();
            }
            else
            {
                EnforceWindowSize();
            }

            GUILayout.Space(10);
            GUILayout.EndHorizontal();

            GUILayout.Space(15);
            GUILayout.EndVertical();
            GUILayout.Space(15);
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// 过程输入
        /// </summary>
        void ProcessInput()
        {
            input = input.ToLower();
            var assetHits = AssetDatabase.FindAssets(input) ?? new string[0];
            hits = assetHits.ToList();

            // 对搜索结果进行排序
            hits.Sort((x, y) =>
            {
                // 通常，使用点击历史记录
                int xScore;
                history.clicks.TryGetValue(x, out xScore);
                int yScore;
                history.clicks.TryGetValue(y, out yScore);

                // 实际以搜索输入较高开头的值文件
                if (xScore != 0 && yScore != 0)
                {
                    var xName = Path.GetFileName(AssetDatabase.GUIDToAssetPath(x)).ToLower();
                    var yName = Path.GetFileName(AssetDatabase.GUIDToAssetPath(y)).ToLower();
                    if (xName.StartsWith(input) && !yName.StartsWith(input))
                        return -1;
                    if (!xName.StartsWith(input) && yName.StartsWith(input))
                        return 1;
                }

                return yScore - xScore;
            });

            hits = hits.Take(10).ToList();
        }

        /// <summary>
        /// 处理事件
        /// </summary>
        void HandleEvents()
        {
            var current = Event.current;

            if (current.type == EventType.KeyDown)
            {
                if (current.keyCode == KeyCode.UpArrow)
                {
                    current.Use();
                    selectedIndex--;
                }
                else if (current.keyCode == KeyCode.DownArrow)
                {
                    current.Use();
                    selectedIndex++;
                }
                else if (current.keyCode == KeyCode.Return)
                {
                    OpenSelectedAssetAndClose();
                    current.Use();
                }
                else if (Event.current.keyCode == KeyCode.Escape)
                    Close();
            }
        }

        /// <summary>
        /// 可视化点击
        /// </summary>
        void VisualizeHits()
        {
            var current = Event.current;

            var windowRect = this.position;
            windowRect.height = BaseHeight;

            GUILayout.BeginVertical();
            GUILayout.Space(5);

            if (hits.Count == 0)
            {
                windowRect.height += EditorGUIUtility.singleLineHeight;
                GUILayout.Label("没有数据");
            }

            for (int i = 0; i < hits.Count; i++)
            {
                var style = i % 2 == 0 ? Styles.entryOdd : Styles.entryEven;

                GUILayout.BeginHorizontal(GUILayout.Height(EditorGUIUtility.singleLineHeight * 2),
                    GUILayout.ExpandWidth(true));

                var elementRect = GUILayoutUtility.GetRect(0, 0, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

                GUILayout.EndHorizontal();

                windowRect.height += EditorGUIUtility.singleLineHeight * 2;

                if (current.type == EventType.Repaint)
                {
                    style.Draw(elementRect, false, false, i == selectedIndex, false);
                    var assetPath = AssetDatabase.GUIDToAssetPath(hits[i]);
                    var icon = AssetDatabase.GetCachedIcon(assetPath);

                    var iconRect = elementRect;
                    iconRect.x = 30;
                    iconRect.width = 25;
                    GUI.DrawTexture(iconRect, icon, ScaleMode.ScaleToFit);


                    var assetName = Path.GetFileName(assetPath);
                    StringBuilder coloredAssetName = new StringBuilder();

                    int start = assetName.ToLower().IndexOf(input);
                    int end = start + input.Length;

                    var highlightColor = EditorGUIUtility.isProSkin
                        ? Styles.proSkinHighlightColor
                        : Styles.personalSkinHighlightColor;

                    var normalColor = EditorGUIUtility.isProSkin
                        ? Styles.proSkinNormalColor
                        : Styles.personalSkinNormalColor;

                    // 有时AssetDatabase会在没有搜索输入的情况下查找资产。
                    if (start == -1)
                        coloredAssetName.Append(string.Format("<color=#{0}>{1}</color>", normalColor, assetName));
                    else
                    {
                        if (0 != start)
                            coloredAssetName.Append(string.Format("<color=#{0}>{1}</color>",
                                normalColor, assetName.Substring(0, start)));

                        coloredAssetName.Append(
                            string.Format("<color=#{0}><b>{1}</b></color>", highlightColor, assetName.Substring(start, end - start)));

                        if (end != assetName.Length - end)
                            coloredAssetName.Append(string.Format("<color=#{0}>{1}</color>",
                                normalColor, assetName.Substring(end, assetName.Length - end)));
                    }

                    var labelRect = elementRect;
                    labelRect.x = 60;
                    GUI.Label(labelRect, coloredAssetName.ToString(), Styles.resultLabelStyle);
                }

                if (current.type == EventType.MouseDown && elementRect.Contains(current.mousePosition))
                {
                    selectedIndex = i;
                    if (current.clickCount == 2)
                        OpenSelectedAssetAndClose();
                    else
                    {
                        Selection.activeObject = GetSelectedAsset();
                        EditorGUIUtility.PingObject(Selection.activeGameObject);
                    }

                    Repaint();
                }
            }

            windowRect.height += 5;
            position = windowRect;

            GUILayout.EndVertical();
        }

        /// <summary>
        /// 打开所选资产并关闭
        /// </summary>
        void OpenSelectedAssetAndClose()
        {
            Close();
            if (hits.Count <= selectedIndex) return;

            AssetDatabase.OpenAsset(GetSelectedAsset());

            var guid = hits[selectedIndex];
            if (!history.clicks.ContainsKey(guid))
                history.clicks[guid] = 0;

            history.clicks[guid]++;
            EditorPrefs.SetString(SearchHistoryKey, JsonUtility.ToJson(history));
        }

        /// <summary>
        /// 获取选定资产
        /// </summary>
        /// <returns></returns>
        UnityEngine.Object GetSelectedAsset()
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(hits[selectedIndex]);
            return (AssetDatabase.LoadMainAssetAtPath(assetPath));
        }

        /// <summary>
        /// 强制窗口大小
        /// </summary>
        public void EnforceWindowSize()
        {
            var pos = position;
            pos.width = 500;
            pos.height = BaseHeight;
            position = pos;
        }

        /// <summary>
        /// 将项目添加到菜单
        /// </summary>
        /// <param name="menu"></param>
        public void AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("Reset history"), false, () =>
            {
                EditorPrefs.SetString(SearchHistoryKey, JsonUtility.ToJson(new SearchHistory()));
                Reset();
            });

            menu.AddItem(new GUIContent("Output history"), false, () =>
            {
                var json = EditorPrefs.GetString(SearchHistoryKey, JsonUtility.ToJson(new SearchHistory()));
                Debug.Log(json);
            });
        }
    }
}
