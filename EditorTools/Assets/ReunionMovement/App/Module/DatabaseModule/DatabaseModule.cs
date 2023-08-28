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
        // 声音配置表
        Dictionary<int, SoundConfig> sounds = new Dictionary<int, SoundConfig>();

        string filePath = AppConfig.DatabasePath;
        string gameConfig_FileName = "GameConfig.json";
        string languages_FileName = "Languages.json";
        string audios_FileName = "SoundConfig.json";

        public IEnumerator Init()
        {
            Log.Debug("DataBaseModule 初始化");
            _initProgress = 0;
            InitConfig();
            yield return null;
            _initProgress = 100;
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
            //------------------------------------
            LoadSoundConfig();

            Log.Debug("初始化，加载Database");
        }

        /// <summary>
        /// 场景管理器轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        public void UpdateTime(float elapseSeconds, float realElapseSeconds)
        {

        }

        #region 加载数据

        public void LoadGameConfig()
        {
            List<GameConfig> configs = new List<GameConfig>();
            //获取完整路径
            string fullPath;
            bool exists = PathUtils.GetFullPath(filePath + gameConfig_FileName, out fullPath);
            if (exists)
            {
                string content = PathUtils.ReadFile(filePath, gameConfig_FileName);
                configs = JsonMapper.ToObject<List<GameConfig>>(content);
            }
            else
            {
                TextAsset json = ResourcesModule.Instance.Load<TextAsset>("AutoDatabase/GameConfig");
                PathUtils.WriteFile(json.text, filePath, gameConfig_FileName);
                configs = JsonMapper.ToObject<List<GameConfig>>(json.text);
            }

            foreach (GameConfig tempData in configs)
            {
                gameConfigs.Add(tempData.Id, tempData);
            }
        }

        public void LoadLanguages()
        {
            List<Languages> configs = new List<Languages>();
            TextAsset json = ResourcesModule.Instance.Load<TextAsset>("AutoDatabase/Languages");
            configs = JsonMapper.ToObject<List<Languages>>(json.text);

            foreach (Languages tempData in configs)
            {
                languages.Add(tempData.Id, tempData);
            }
        }

        public void LoadSoundConfig()
        {
            List<SoundConfig> configs = new List<SoundConfig>();
            TextAsset json = ResourcesModule.Instance.Load<TextAsset>("AutoDatabase/SoundConfig");
            configs = JsonMapper.ToObject<List<SoundConfig>>(json.text);

            foreach (SoundConfig tempData in configs)
            {
                sounds.Add(tempData.Id, tempData);
            }
        }
        #endregion

        #region 获取数据
        /// <summary>
        /// 获取游戏配置表数据
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, GameConfig> GetGameConfig()
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

        /// <summary>
        /// 获取声音数据
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, SoundConfig> GetSoundConfig()
        {
            return sounds;
        }
        /// <summary>
        /// 获取声音数据
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public SoundConfig GetSoundConfig(int index)
        {
            if (sounds.ContainsKey(index))
            {
                return sounds[index];
            }
            return null;
        }
        #endregion

        #region 保存
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

        public void SaveAudioConfig()
        {
            List<SoundConfig> tempList = sounds.Values.ToList();
            string jsonStr = JsonMapper.ToJson(tempList, true);
            PathUtils.WriteFile(jsonStr, filePath, audios_FileName);
        }
        #endregion
    }
}