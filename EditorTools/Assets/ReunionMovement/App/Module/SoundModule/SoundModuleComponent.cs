using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 声音模块部件
    /// </summary>
    public class SoundModuleComponent : MonoBehaviour
    {
        /// <summary>
        /// 播放声音
        /// </summary>
        /// <param name="name"></param>
        public void PlaySound(string name)
        {
            SoundModule.Instance.PlaySound(name);
        }

        /// <summary>
        /// 播放声音 - 不可暂停
        /// </summary>
        /// <param name="name"></param>
        public void PlaySoundNotPausable(string name)
        {
            SoundModule.Instance.PlaySoundUI(name);
        }

        /// <summary>
        /// 设置声音音量
        /// </summary>
        public void ChangeSoundVolume(float volume)
        {
            SoundModule.Instance.SetSoundVolume(volume);
        }

        /// <summary>
        /// 设置音乐音量
        /// </summary>
        public void ChangeMusicVolume(float volume)
        {
            SoundModule.Instance.SetMusicVolume(volume);
        }

        /// <summary>
        /// 音乐静音
        /// </summary>
        public void ToggleMusicMuted()
        {
            SoundModule.Instance.SetMusicMuted(!SoundModule.Instance.GetMusicMuted());
        }

        /// <summary>
        /// 声音静音
        /// </summary>
        public void ToggleSoundMuted()
        {
            SoundModule.Instance.SetSoundMuted(!SoundModule.Instance.GetSoundMuted());
        }
    }
}