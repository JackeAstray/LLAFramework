using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using LitJson;

namespace GameLogic
{
    public class JsonDatabaseModule : CustommModuleInitialize
    {
        public static JsonDatabaseModule Instance = new JsonDatabaseModule();
        public bool IsInited { get; private set; }
        private double initProgress = 0;
        public double InitProgress { get { return initProgress; } }

        // GameConfig 配置表
        Dictionary<int, GameConfig> gameconfigs = new Dictionary<int, GameConfig>();
        // ItemConfig 配置表
        Dictionary<int, ItemConfig> itemconfigs = new Dictionary<int, ItemConfig>();
        // Languages 配置表
        Dictionary<int, Languages> languagess = new Dictionary<int, Languages>();
        // QuestionConfig 配置表
        Dictionary<int, QuestionConfig> questionconfigs = new Dictionary<int, QuestionConfig>();
        // SoundConfig 配置表
        Dictionary<int, SoundConfig> soundconfigs = new Dictionary<int, SoundConfig>();

        string filePath = AppConfig.DatabasePath;
        string gameconfig_FileName = "GameConfig.json";
        string itemconfig_FileName = "ItemConfig.json";
        string languages_FileName = "Languages.json";
        string questionconfig_FileName = "QuestionConfig.json";
        string soundconfig_FileName = "SoundConfig.json";

        public IEnumerator Init()
        {
            initProgress = 0;
            InitConfig();
            yield return null;
            initProgress = 100;
            IsInited = true;
            Log.Debug("JsonDatabaseModule 初始化完成");
        }

        public void ClearData()
        {
            gameconfigs.Clear();
            itemconfigs.Clear();
            languagess.Clear();
            questionconfigs.Clear();
            soundconfigs.Clear();
            Log.Debug("JsonDatabaseModule 清除数据");
        }

        void InitConfig()
        {
            LoadGameConfig();
            LoadItemConfig();
            LoadLanguages();
            LoadQuestionConfig();
            LoadSoundConfig();
        }

        public void UpdateTime(float elapseSeconds, float realElapseSeconds)
        {
        }

        #region 加载数据
        public void LoadGameConfig()
        {
            List<GameConfig> configs = new List<GameConfig>();
            string fullPath;
            bool exists = PathUtils.GetFullPath(filePath + gameconfig_FileName, out fullPath);
            if (exists)
            {
                string content = PathUtils.ReadFile(filePath, gameconfig_FileName);
                configs = JsonMapper.ToObject<List<GameConfig>>(content);
            }
            else
            {
                TextAsset json = ResourcesModule.Instance.Load<TextAsset>("AutoDatabase/GameConfig");
                PathUtils.WriteFile(json.text, filePath, gameconfig_FileName);
                configs = JsonMapper.ToObject<List<GameConfig>>(json.text);
            }
            foreach (var tempData in configs)
            {
                gameconfigs.Add(tempData.Id, tempData);
            }
        }

        public void LoadItemConfig()
        {
            List<ItemConfig> configs = new List<ItemConfig>();
            string fullPath;
            bool exists = PathUtils.GetFullPath(filePath + itemconfig_FileName, out fullPath);
            if (exists)
            {
                string content = PathUtils.ReadFile(filePath, itemconfig_FileName);
                configs = JsonMapper.ToObject<List<ItemConfig>>(content);
            }
            else
            {
                TextAsset json = ResourcesModule.Instance.Load<TextAsset>("AutoDatabase/ItemConfig");
                PathUtils.WriteFile(json.text, filePath, itemconfig_FileName);
                configs = JsonMapper.ToObject<List<ItemConfig>>(json.text);
            }
            foreach (var tempData in configs)
            {
                itemconfigs.Add(tempData.Id, tempData);
            }
        }

        public void LoadLanguages()
        {
            List<Languages> configs = new List<Languages>();
            string fullPath;
            bool exists = PathUtils.GetFullPath(filePath + languages_FileName, out fullPath);
            if (exists)
            {
                string content = PathUtils.ReadFile(filePath, languages_FileName);
                configs = JsonMapper.ToObject<List<Languages>>(content);
            }
            else
            {
                TextAsset json = ResourcesModule.Instance.Load<TextAsset>("AutoDatabase/Languages");
                PathUtils.WriteFile(json.text, filePath, languages_FileName);
                configs = JsonMapper.ToObject<List<Languages>>(json.text);
            }
            foreach (var tempData in configs)
            {
                languagess.Add(tempData.Id, tempData);
            }
        }

        public void LoadQuestionConfig()
        {
            List<QuestionConfig> configs = new List<QuestionConfig>();
            string fullPath;
            bool exists = PathUtils.GetFullPath(filePath + questionconfig_FileName, out fullPath);
            if (exists)
            {
                string content = PathUtils.ReadFile(filePath, questionconfig_FileName);
                configs = JsonMapper.ToObject<List<QuestionConfig>>(content);
            }
            else
            {
                TextAsset json = ResourcesModule.Instance.Load<TextAsset>("AutoDatabase/QuestionConfig");
                PathUtils.WriteFile(json.text, filePath, questionconfig_FileName);
                configs = JsonMapper.ToObject<List<QuestionConfig>>(json.text);
            }
            foreach (var tempData in configs)
            {
                questionconfigs.Add(tempData.Id, tempData);
            }
        }

        public void LoadSoundConfig()
        {
            List<SoundConfig> configs = new List<SoundConfig>();
            string fullPath;
            bool exists = PathUtils.GetFullPath(filePath + soundconfig_FileName, out fullPath);
            if (exists)
            {
                string content = PathUtils.ReadFile(filePath, soundconfig_FileName);
                configs = JsonMapper.ToObject<List<SoundConfig>>(content);
            }
            else
            {
                TextAsset json = ResourcesModule.Instance.Load<TextAsset>("AutoDatabase/SoundConfig");
                PathUtils.WriteFile(json.text, filePath, soundconfig_FileName);
                configs = JsonMapper.ToObject<List<SoundConfig>>(json.text);
            }
            foreach (var tempData in configs)
            {
                soundconfigs.Add(tempData.Id, tempData);
            }
        }

        #endregion

        #region 获取数据
        public Dictionary<int, GameConfig> GetGameConfig()
        {
            return gameconfigs;
        }

        public Dictionary<int, ItemConfig> GetItemConfig()
        {
            return itemconfigs;
        }

        public Dictionary<int, Languages> GetLanguages()
        {
            return languagess;
        }

        public Dictionary<int, QuestionConfig> GetQuestionConfig()
        {
            return questionconfigs;
        }

        public Dictionary<int, SoundConfig> GetSoundConfig()
        {
            return soundconfigs;
        }

        #endregion

        #region 保存
        public void SaveGameConfig()
        {
            List<GameConfig> tempList = gameconfigs.Values.ToList();
            string jsonStr = JsonMapper.ToJson(tempList, true);
            PathUtils.WriteFile(jsonStr, filePath, gameconfig_FileName);
        }

        public void SaveItemConfig()
        {
            List<ItemConfig> tempList = itemconfigs.Values.ToList();
            string jsonStr = JsonMapper.ToJson(tempList, true);
            PathUtils.WriteFile(jsonStr, filePath, itemconfig_FileName);
        }

        public void SaveLanguages()
        {
            List<Languages> tempList = languagess.Values.ToList();
            string jsonStr = JsonMapper.ToJson(tempList, true);
            PathUtils.WriteFile(jsonStr, filePath, languages_FileName);
        }

        public void SaveQuestionConfig()
        {
            List<QuestionConfig> tempList = questionconfigs.Values.ToList();
            string jsonStr = JsonMapper.ToJson(tempList, true);
            PathUtils.WriteFile(jsonStr, filePath, questionconfig_FileName);
        }

        public void SaveSoundConfig()
        {
            List<SoundConfig> tempList = soundconfigs.Values.ToList();
            string jsonStr = JsonMapper.ToJson(tempList, true);
            PathUtils.WriteFile(jsonStr, filePath, soundconfig_FileName);
        }

        #endregion

        #region 通过索引查询数据
        public GameConfig GetGameConfigByNumber(int number)
        {
            if (gameconfigs.TryGetValue(number, out var value))
            {
                return value;
            }
            return null;
        }

        public ItemConfig GetItemConfigByNumber(int number)
        {
            if (itemconfigs.TryGetValue(number, out var value))
            {
                return value;
            }
            return null;
        }

        public Languages GetLanguagesByNumber(int number)
        {
            if (languagess.TryGetValue(number, out var value))
            {
                return value;
            }
            return null;
        }

        public QuestionConfig GetQuestionConfigByNumber(int number)
        {
            if (questionconfigs.TryGetValue(number, out var value))
            {
                return value;
            }
            return null;
        }

        public SoundConfig GetSoundConfigByNumber(int number)
        {
            if (soundconfigs.TryGetValue(number, out var value))
            {
                return value;
            }
            return null;
        }

        #endregion
    }
}
