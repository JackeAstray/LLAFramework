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
        private double initProgress = 0;
        public double InitProgress { get { return initProgress; } }
        #endregion

        public GameObject root { get; private set; }

        private SoundModuleSettings settings;
        AudioSource sourceAS;
        int currentMusicIndex;
        public float fadeDuration = 3.0f;
        public float targetVolume = 1.0f;

        public IEnumerator Init()
        {
            Log.Debug("SoundModule 初始化");
            initProgress = 0;

            CreateAudioRootAndInit();

            yield return null;
            initProgress = 100;
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
            sourceAS = root.AddComponent<AudioSource>();
            GameObject.DontDestroyOnLoad(root);
            settings = new SoundModuleSettings();
            settings.LoadSettings();
        }

        #region 功能
        /// <summary>
        /// 播放音乐
        /// </summary>
        /// <param name="name"></param>
        public void PlayMusic(int index, float volume = -1)
        {
            if (sourceAS.clip == null || !sourceAS.isPlaying)
            {
                SoundConfig soundConfig = DatabaseModule.Instance.GetSoundConfig(index);

                if (soundConfig != null)
                {
                    currentMusicIndex = index;
                    AudioClip audioClip = ResourcesModule.Instance.Load<AudioClip>(soundConfig.Path + soundConfig.Name);
                    sourceAS.clip = audioClip;
                    sourceAS.volume = volume == -1 ? settings.GetMusicVolume() : volume;
                    sourceAS.loop = true;
                    sourceAS.mute = settings.GetMusicMuted();
                    sourceAS.Play();
                }
            }
        }

        /// <summary>
        /// 播放BGM
        /// </summary>
        public void PlayMusic()
        {
            sourceAS?.Play();
        }

        /// <summary>
        /// 暂停BGM
        /// </summary>
        public void PauseMusic()
        {
            sourceAS?.Pause();
        }

        /// <summary>
        /// 结束背景音乐
        /// </summary>
        public void StopMusic()
        {
            sourceAS?.Stop();
        }

        /// <summary>
        /// 音乐切换-带渐入渐出效果
        /// </summary>
        /// <param name="index"></param>
        public async void PlaySwitch(int index)
        {
            // 渐出音频
            await FadeOut();
            //播放音乐
            PlayMusic(index, 0);
            // 渐入音频
            await FadeIn();
        }

        /// <summary>
        /// 渐入
        /// </summary>
        /// <returns></returns>
        private async Task FadeIn()
        {
            float startVolume = 0;
            float startTime = Time.time;
            targetVolume = settings.GetMusicVolume();

            while (sourceAS.volume < targetVolume)
            {
                float elapsedTime = Time.time - startTime;
                float t = Mathf.Clamp01(elapsedTime / fadeDuration);

                sourceAS.volume = Mathf.Lerp(startVolume, targetVolume, t);

                await Task.Yield();
            }
        }

        /// <summary>
        /// 渐出
        /// </summary>
        /// <returns></returns>
        private async Task FadeOut()
        {
            float startVolume = sourceAS.volume;
            float startTime = Time.time;

            while (sourceAS.volume > 0.0f)
            {
                float elapsedTime = Time.time - startTime;
                float t = Mathf.Clamp01(elapsedTime / fadeDuration);

                sourceAS.volume = Mathf.Lerp(startVolume, 0.0f, t);

                await Task.Yield();
            }

            sourceAS.Stop();
        }

        //--------------------------

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
        void ObjectProcessing(int index, Transform emitter, bool loop)
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
            return settings?.GetMusicVolume() ?? 0;
        }

        public float GetSoundVolume() 
        {
            return settings?.GetSoundVolume() ?? 0;
        }

        public bool GetMusicMuted()
        {
            return settings?.GetMusicMuted() ?? false;
        }

        public bool GetSoundMuted()
        {
            return settings?.GetSoundMuted() ?? false;
        }
        #endregion

        #region 设置
        //音乐静音
        public void SetMusicMuted(bool value)
        {
            if (settings != null)
            {
                sourceAS.mute = value;
                settings.SetMusicMuted(value);
            }
        }
        //声音静音
        public void SetSoundMuted(bool value)
        {
            settings?.SetSoundMuted(value);
        }
        //音乐音量
        public void SetMusicVolume(float value)
        {
            if (settings != null)
            {
                float tempValue = Mathf.Clamp(value, 0, 1);
                sourceAS.volume = value;
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