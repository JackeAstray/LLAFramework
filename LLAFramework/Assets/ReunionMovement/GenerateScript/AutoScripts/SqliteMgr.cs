//此脚本为工具生成，请勿手动创建 2025-05-04 13:53:01.735 <ExcelTo>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LLAFramework.Base;

namespace LLAFramework.Sqlite
{
    public class SqliteMgr : SingletonMgr<SqliteMgr>
    {
        private DataService dataService;

        // 初始化数据库
        public void Initialize(string dbName, string password = null)
        {
            dataService = new DataService(dbName, password);
        }

        
        // 查询所有GameConfig
        public IEnumerable<GameConfig> GetAllGameConfig()
        {
            return dataService.Query<GameConfig>();
        }

        // 根据条件查询GameConfig
        public IEnumerable<GameConfig> GetGameConfigByCondition(string condition, params object[] args)
        {
            return dataService.Query<GameConfig>(condition, args);
        }

        // 插入GameConfig
        public void InsertGameConfig(GameConfig obj)
        {
            dataService.Insert(obj);
        }

        // 更新GameConfig
        public void UpdateGameConfig(GameConfig obj)
        {
            dataService.Update(obj);
        }

        // 删除GameConfig
        public void DeleteGameConfig(GameConfig obj)
        {
            dataService.Delete(obj);
        }

        // 执行命令GameConfig
        public void ExecuteGameConfig(string str)
        {
            dataService.Execute(str);
        }
        

        // 查询所有InputSystemConfig
        public IEnumerable<InputSystemConfig> GetAllInputSystemConfig()
        {
            return dataService.Query<InputSystemConfig>();
        }

        // 根据条件查询InputSystemConfig
        public IEnumerable<InputSystemConfig> GetInputSystemConfigByCondition(string condition, params object[] args)
        {
            return dataService.Query<InputSystemConfig>(condition, args);
        }

        // 插入InputSystemConfig
        public void InsertInputSystemConfig(InputSystemConfig obj)
        {
            dataService.Insert(obj);
        }

        // 更新InputSystemConfig
        public void UpdateInputSystemConfig(InputSystemConfig obj)
        {
            dataService.Update(obj);
        }

        // 删除InputSystemConfig
        public void DeleteInputSystemConfig(InputSystemConfig obj)
        {
            dataService.Delete(obj);
        }

        // 执行命令InputSystemConfig
        public void ExecuteInputSystemConfig(string str)
        {
            dataService.Execute(str);
        }
        

        // 查询所有ItemConfig
        public IEnumerable<ItemConfig> GetAllItemConfig()
        {
            return dataService.Query<ItemConfig>();
        }

        // 根据条件查询ItemConfig
        public IEnumerable<ItemConfig> GetItemConfigByCondition(string condition, params object[] args)
        {
            return dataService.Query<ItemConfig>(condition, args);
        }

        // 插入ItemConfig
        public void InsertItemConfig(ItemConfig obj)
        {
            dataService.Insert(obj);
        }

        // 更新ItemConfig
        public void UpdateItemConfig(ItemConfig obj)
        {
            dataService.Update(obj);
        }

        // 删除ItemConfig
        public void DeleteItemConfig(ItemConfig obj)
        {
            dataService.Delete(obj);
        }

        // 执行命令ItemConfig
        public void ExecuteItemConfig(string str)
        {
            dataService.Execute(str);
        }
        

        // 查询所有Languages
        public IEnumerable<Languages> GetAllLanguages()
        {
            return dataService.Query<Languages>();
        }

        // 根据条件查询Languages
        public IEnumerable<Languages> GetLanguagesByCondition(string condition, params object[] args)
        {
            return dataService.Query<Languages>(condition, args);
        }

        // 插入Languages
        public void InsertLanguages(Languages obj)
        {
            dataService.Insert(obj);
        }

        // 更新Languages
        public void UpdateLanguages(Languages obj)
        {
            dataService.Update(obj);
        }

        // 删除Languages
        public void DeleteLanguages(Languages obj)
        {
            dataService.Delete(obj);
        }

        // 执行命令Languages
        public void ExecuteLanguages(string str)
        {
            dataService.Execute(str);
        }
        

        // 查询所有QuestionConfig
        public IEnumerable<QuestionConfig> GetAllQuestionConfig()
        {
            return dataService.Query<QuestionConfig>();
        }

        // 根据条件查询QuestionConfig
        public IEnumerable<QuestionConfig> GetQuestionConfigByCondition(string condition, params object[] args)
        {
            return dataService.Query<QuestionConfig>(condition, args);
        }

        // 插入QuestionConfig
        public void InsertQuestionConfig(QuestionConfig obj)
        {
            dataService.Insert(obj);
        }

        // 更新QuestionConfig
        public void UpdateQuestionConfig(QuestionConfig obj)
        {
            dataService.Update(obj);
        }

        // 删除QuestionConfig
        public void DeleteQuestionConfig(QuestionConfig obj)
        {
            dataService.Delete(obj);
        }

        // 执行命令QuestionConfig
        public void ExecuteQuestionConfig(string str)
        {
            dataService.Execute(str);
        }
        

        // 查询所有SoundConfig
        public IEnumerable<SoundConfig> GetAllSoundConfig()
        {
            return dataService.Query<SoundConfig>();
        }

        // 根据条件查询SoundConfig
        public IEnumerable<SoundConfig> GetSoundConfigByCondition(string condition, params object[] args)
        {
            return dataService.Query<SoundConfig>(condition, args);
        }

        // 插入SoundConfig
        public void InsertSoundConfig(SoundConfig obj)
        {
            dataService.Insert(obj);
        }

        // 更新SoundConfig
        public void UpdateSoundConfig(SoundConfig obj)
        {
            dataService.Update(obj);
        }

        // 删除SoundConfig
        public void DeleteSoundConfig(SoundConfig obj)
        {
            dataService.Delete(obj);
        }

        // 执行命令SoundConfig
        public void ExecuteSoundConfig(string str)
        {
            dataService.Execute(str);
        }
        

        //  销毁
        public void OnDestroy()
        {
            Close();
        }

        // 关闭数据库连接
        public void Close()
        {
            dataService.Close();
        }
    }
}
