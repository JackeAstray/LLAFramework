using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using LitJson;

namespace GameLogic
{
    /// <summary>
    /// 数据库模块
    /// </summary>
    public class DatabaseModule : CustommModuleInitialize 
    {
        #region 实例与初始化
        public static DatabaseModule Instance = new DatabaseModule();

        public bool IsInited { get; private set; }
        private double _initProgress = 0;
        public double InitProgress { get { return _initProgress; } }
        #endregion

        // 游戏配置表
        Dictionary<int, GameConfig> gameConfigs = new Dictionary<int, GameConfig>();
        // 语言配置表
        Dictionary<int, Languages> languages = new Dictionary<int, Languages>();

        string filePath = AppConfig.DatabasePath;
        string gameConfig_FileName = "GameConfig.json";
        string languages_FileName = "Languages.json";

        public IEnumerator Init()
        {
            Log.Debug("DataBaseModule 初始化");

            Instance = this;
            
            InitConfig();

            yield return null;
            IsInited = true;
        }

        public void ClearData()
        {
            Log.Debug("DataBaseModule 清除数据");

            gameConfigs.Clear();
            languages.Clear();
        }

        ////-------------------------------------
        void InitConfig()
        {
            LoadGameConfig();
            //------------------------------------
            LoadLanguages();

            Log.Debug("初始化，加载Database");
        }

        public void Update(float elapseSeconds, float realElapseSeconds)
        {

        }

        #region Load

        public void LoadGameConfig()
        {
            List<GameConfig> gameConfigList = new List<GameConfig>();
            //获取完整路径
            string fullPath;
            bool exists = PathUtils.GetFullPath(filePath + gameConfig_FileName, out fullPath);
            if (exists)
            {
                string content = PathUtils.ReadFile(filePath, gameConfig_FileName);
                gameConfigList = JsonMapper.ToObject<List<GameConfig>>(content);
            }
            else
            {
                TextAsset json = ResourcesModule.Instance.Load<TextAsset>("AutoDatabase/GameConfig");
                PathUtils.WriteFile(json.text, filePath, gameConfig_FileName);
                gameConfigList = JsonMapper.ToObject<List<GameConfig>>(json.text);
            }

            foreach (GameConfig tempData in gameConfigList)
            {
                gameConfigs.Add(tempData.Id, tempData);
            }
        }

        public void LoadLanguages()
        {
            List<Languages> languagesConfigs = new List<Languages>();
            TextAsset json = ResourcesModule.Instance.Load<TextAsset>("AutoDatabase/Languages");
            languagesConfigs = JsonMapper.ToObject<List<Languages>>(json.text);

            foreach (Languages tempData in languagesConfigs)
            {
                languages.Add(tempData.Id, tempData);
            }
        }
        #endregion

        #region GetData
        /// <summary>
        /// 获取游戏配置表数据
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, GameConfig> GetConfig()
        {
            return gameConfigs;
        }

        /// <summary>
        /// 获取语言数据
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, Languages> GetLanguages()
        {
            return languages;
        }
        #endregion

        #region Save
        public void SaveGameConfig()
        {
            List<GameConfig> tempList = gameConfigs.Values.ToList();
            string jsonStr = JsonMapper.ToJson(tempList, true);
            PathUtils.WriteFile(jsonStr, filePath, gameConfig_FileName);
        }

        public void SaveLanguages()
        {
            List<Languages> tempList = languages.Values.ToList();
            string jsonStr = JsonMapper.ToJson(tempList, true);
            PathUtils.WriteFile(jsonStr, filePath, languages_FileName);
        }
        #endregion
    }
}