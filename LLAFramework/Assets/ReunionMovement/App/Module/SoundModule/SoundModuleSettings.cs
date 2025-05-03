using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace LLAFramework
{
    /// <summary>
    /// 声音管理器设置
    /// </summary>
    public class SoundModuleSettings
    {
        //自动暂停
        public bool autoPause = true;

        //音量校正
        public float musicVolumeCorrection = 1f;
        public float soundVolumeCorrection = 1f;

        //淡入淡出时间
        public float musicFadeTime = 2f;

        //混音器组
        public AudioMixerGroup musicAudioMixerGroup;
        public AudioMixerGroup soundAudioMixerGroup;

        //音量
        private float volumeMusic;
        private float volumeSound;

        //静音
        private bool mutedMusic;
        private bool mutedSound;


        public void SaveSettings()
        {
            PlayerPrefs.SetFloat("SM_MusicVolume", volumeMusic);
            PlayerPrefs.SetFloat("SM_SoundVolume", volumeSound);

            PlayerPrefs.SetInt("SM_MusicMute", mutedMusic ? 1 : 0);
            PlayerPrefs.SetInt("SM_SoundMute", mutedSound ? 1 : 0);
        }

        /// <summary>
        /// 加载配置
        /// </summary>
        public void LoadSettings()
        {
            volumeMusic = PlayerPrefs.GetFloat("SM_MusicVolume", 1);
            volumeSound = PlayerPrefs.GetFloat("SM_SoundVolume", 1);

            mutedMusic = PlayerPrefs.GetInt("SM_MusicMute", 0) == 1;
            mutedSound = PlayerPrefs.GetInt("SM_SoundMute", 0) == 1;
        }

        /// <summary>
        /// 设置音乐音量
        /// </summary>
        /// <param name="volume"></param>
        public void SetMusicVolume(float volume)
        {
            volumeMusic = volume;
            SaveSettings();
        }

        /// <summary>
        /// 获取音乐音量
        /// </summary>
        /// <returns></returns>
        public float GetMusicVolume()
        {
            return volumeMusic;
        }

        /// <summary>
        /// 设置声音音量
        /// </summary>
        /// <param name="volume"></param>
        public void SetSoundVolume(float volume)
        {
            volumeSound = volume;
            SaveSettings();
        }

        /// <summary>
        /// 获取声音音量
        /// </summary>
        /// <returns></returns>
        public float GetSoundVolume()
        {
            return volumeSound;
        }

        /// <summary>
        /// 设置音乐静音值
        /// </summary>
        /// <param name="mute"></param>
        public void SetMusicMuted(bool mute)
        {
            mutedMusic = mute;
            SaveSettings();
        }

        /// <summary>
        /// 获取音乐静音值
        /// </summary>
        /// <returns></returns>
        public bool GetMusicMuted()
        {
            return mutedMusic;
        }

        /// <summary>
        /// 设置声音静音
        /// </summary>
        /// <param name="mute"></param>
        public void SetSoundMuted(bool mute)
        {
            mutedSound = mute;
            SaveSettings();
        }

        /// <summary>
        /// 获取声音静音值
        /// </summary>
        /// <returns></returns>
        public bool GetSoundMuted()
        {
            return mutedSound;
        }

        /// <summary>
        /// 获取更正声音音量
        /// </summary>
        /// <returns></returns>
        public float GetSoundVolumeCorrected()
        {
            return volumeSound * soundVolumeCorrection;
        }

        /// <summary>
        /// 获取更正音乐音量
        /// </summary>
        /// <returns></returns>
        public float GetMusicVolumeCorrected()
        {
            return volumeMusic * musicVolumeCorrection;
        }
    }
}