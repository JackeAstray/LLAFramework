using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.Rendering.DebugUI;

namespace GameLogic
{
    /// <summary>
    /// 声音模块
    /// </summary>
    public class SoundModule : CustommModuleInitialize
    {
        #region 实例与初始化
        public static SoundModule Instance = new SoundModule();
        public bool IsInited { get; private set; }
        private double _initProgress = 0;
        public double InitProgress { get { return _initProgress; } }
        #endregion

        public GameObject root { get; private set; }

        private SoundModuleSettings settings;
        AudioSource sourceObj;
        int currentMusicIndex;

        public IEnumerator Init()
        {
            Log.Debug("SoundModule 初始化");
            _initProgress = 0;

            CreateAudioRootAndInit();

            yield return null;
            _initProgress = 100;
            IsInited = true;
        }

        public void ClearData()
        {
            Log.Debug("SoundModule 清除数据");
        }

        /// <summary>
        /// 场景管理器轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        public void UpdateTime(float elapseSeconds, float realElapseSeconds)
        {

        }

        /// <summary>
        /// 创建声音根节点和初始化
        /// </summary>
        public void CreateAudioRootAndInit()
        {
            root = new GameObject("AudioRoot");
            sourceObj = root.AddComponent<AudioSource>();
            GameObject.DontDestroyOnLoad(root);

            settings = new SoundModuleSettings();
            settings.LoadSettings();
        }

        #region 功能
        /// <summary>
        /// 播放音乐
        /// </summary>
        /// <param name="name"></param>
        public void PlayMusic(int index)
        {
            if (sourceObj.clip == null || !sourceObj.isPlaying)
            {
                SoundConfig soundConfig = DatabaseModule.Instance.GetSoundConfig(index);

                if (soundConfig != null)
                {
                    AudioClip audioClip = ResourcesModule.Instance.Load<AudioClip>(soundConfig.Path + soundConfig.Name);
                    sourceObj.clip = audioClip;
                    sourceObj.volume = settings.GetMusicVolume();
                    sourceObj.loop = true;
                    sourceObj.mute = settings.GetMusicMuted();
                    sourceObj.Play();
                }
            }
        }

        /// <summary>
        /// 暂停BGM
        /// </summary>
        public void PauseMusic()
        {
            if (sourceObj != null)
            {
                sourceObj.Pause();
            }
        }

        /// <summary>
        /// 结束背景音乐
        /// </summary>
        public void StopMusic()
        {
            if (sourceObj != null)
            {
                sourceObj.Stop();
            }
        }

        /// <summary>
        /// 播放声音
        /// </summary>
        /// <param name="index"></param>
        public void PlaySound(int index)
        {
            PlaySound(index, null, false);
        }

        /// <summary>
        /// 播放声音
        /// </summary>
        /// <param name="index"></param>
        /// <param name="pos"></param>
        public void PlaySound(int index, Transform pos)
        {
            PlaySound(index, pos, false);
        }

        /// <summary>
        /// 播放声音
        /// </summary>
        /// <param name="index"></param>
        /// <param name="emitter"></param>
        /// <param name="loop"></param>
        public void PlaySound(int index, Transform emitter, bool loop)
        {
            ObjectProcessing(index, emitter, loop);
        }
        void ObjectProcessing(int index , Transform emitter, bool loop)
        {
            SoundConfig soundConfig = DatabaseModule.Instance.GetSoundConfig(index);
            if (soundConfig != null)
            {
                AudioClip clip = ResourcesModule.Instance.Load<AudioClip>(soundConfig.Path + soundConfig.Name);

                if (clip != null)
                {
                    GameObject go = ObjectPoolModule.Instance.Spawn(ObjectPoolModule.Instance.startupPools[0].prefab);
                    if (go != null)
                    {
                        SoundObj soundObj = go.GetComponent<SoundObj>();
                        soundObj.clip = clip;
                        soundObj.ObjectProcessing(index, emitter, loop,settings.GetSoundVolume(),settings.GetSoundMuted());
                    }
                }
            }
        }
        #endregion

        #region 获取
        public float GetMusicVolume()
        {
            if (settings != null)
            {
                return settings.GetMusicVolume();
            }
            return 0;
        }

        public float GetSoundVolume() 
        {
            if (settings != null)
            {
                return settings.GetSoundVolume();
            }
            return 0;
        }

        public bool GetMusicMuted()
        {
            if (settings != null)
            {
                return settings.GetMusicMuted();
            }
            return false;
        }

        public bool GetSoundMuted()
        {
            if (settings != null)
            {
                return settings.GetSoundMuted();
            }
            return false;
        }
        #endregion

        #region 设置
        //音乐静音
        public void SetMusicMuted(bool value)
        {
            if (settings != null)
            {
                settings.SetMusicMuted(value);
            }
        }
        //声音静音
        public void SetSoundMuted(bool value)
        {
            if (settings != null)
            {
                settings.SetSoundMuted(value);
            }
        }
        //音乐音量
        public void SetMusicVolume(float value)
        {
            if (settings != null)
            {
                float tempValue = Mathf.Clamp(value, 0, 1);
                settings.SetMusicVolume(tempValue);
            }
        }
        //声音音量
        public void SetSoundVolume(float value)
        {
            if (settings != null)
            {
                float tempValue = Mathf.Clamp(value, 0, 1);
                settings.SetSoundVolume(tempValue);
            }
        }
        #endregion
    }
}