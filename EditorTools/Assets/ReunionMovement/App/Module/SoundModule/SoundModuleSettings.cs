using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace GameLogic
{
    /// <summary>
    /// 声音管理器设置
    /// </summary>
    public class SoundModuleSettings : ScriptableObject
    {
        //自动暂停
        public bool AutoPause = true;

        //音量校正
        public float MusicVolumeCorrection = 1f;
        public float SoundVolumeCorrection = 1f;

        //淡入淡出时间
        public float MusicFadeTime = 2f;

        //混音器组
        public AudioMixerGroup MusicAudioMixerGroup;
        public AudioMixerGroup SoundAudioMixerGroup;

        //预加载的AudioClip
        public AudioClip[] PreloadedLoadedClips;

        //音量
        private float _volumeMusic;
        private float _volumeSound;

        //静音
        private bool _mutedMusic;
        private bool _mutedSound;


        public void SaveSettings()
        {
            PlayerPrefs.SetFloat("SM_MusicVolume", _volumeMusic);
            PlayerPrefs.SetFloat("SM_SoundVolume", _volumeSound);

            PlayerPrefs.SetInt("SM_MusicMute", _mutedMusic ? 1 : 0);
            PlayerPrefs.SetInt("SM_SoundMute", _mutedSound ? 1 : 0);
        }

        /// <summary>
        /// 加载配置
        /// </summary>
        public void LoadSettings()
        {
            _volumeMusic = PlayerPrefs.GetFloat("SM_MusicVolume", 1);
            _volumeSound = PlayerPrefs.GetFloat("SM_SoundVolume", 1);

            _mutedMusic = PlayerPrefs.GetInt("SM_MusicMute", 0) == 1;
            _mutedSound = PlayerPrefs.GetInt("SM_SoundMute", 0) == 1;
        }

        /// <summary>
        /// 设置音乐音量
        /// </summary>
        /// <param name="volume"></param>
        public void SetMusicVolume(float volume)
        {
            _volumeMusic = volume;
            SaveSettings();
        }

        /// <summary>
        /// 获取音乐音量
        /// </summary>
        /// <returns></returns>
        public float GetMusicVolume()
        {
            return _volumeMusic;
        }

        /// <summary>
        /// 设置声音音量
        /// </summary>
        /// <param name="volume"></param>
        public void SetSoundVolume(float volume)
        {
            _volumeSound = volume;
            SaveSettings();
        }

        /// <summary>
        /// 获取声音音量
        /// </summary>
        /// <returns></returns>
        public float GetSoundVolume()
        {
            return _volumeSound;
        }

        /// <summary>
        /// 设置音乐静音值
        /// </summary>
        /// <param name="mute"></param>
        public void SetMusicMuted(bool mute)
        {
            _mutedMusic = mute;
            SaveSettings();
        }

        /// <summary>
        /// 获取音乐静音值
        /// </summary>
        /// <returns></returns>
        public bool GetMusicMuted()
        {
            return _mutedMusic;
        }

        /// <summary>
        /// 设置声音静音
        /// </summary>
        /// <param name="mute"></param>
        public void SetSoundMuted(bool mute)
        {
            _mutedSound = mute;
            SaveSettings();
        }

        /// <summary>
        /// 获取声音静音值
        /// </summary>
        /// <returns></returns>
        public bool GetSoundMuted()
        {
            return _mutedSound;
        }

        /// <summary>
        /// 获取更正声音音量
        /// </summary>
        /// <returns></returns>
        public float GetSoundVolumeCorrected()
        {
            return _volumeSound * SoundVolumeCorrection;
        }

        /// <summary>
        /// 获取更正音乐音量
        /// </summary>
        /// <returns></returns>
        public float GetMusicVolumeCorrected()
        {
            return _volumeMusic * MusicVolumeCorrection;
        }

        //[MenuItem("SoundModule/Create SoundModuleSettings")]
        //public static void CreateAsset()
        //{
        //    SoundModuleSettings asset = ScriptableObject.CreateInstance<SoundModuleSettings>();
        //    string assetPathAndName = "Assets/SoundModule/Resources/SoundModuleSettings.asset";
        //    AssetDatabase.CreateAsset(asset, assetPathAndName);
        //    AssetDatabase.SaveAssets();
        //    AssetDatabase.Refresh();
        //    EditorUtility.FocusProjectWindow();
        //    Selection.activeObject = asset;
        //}
    }
}