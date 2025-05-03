//此脚本为工具生成，请勿手动创建 2025-04-09 08:53:31.486 <ExcelTo>
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
