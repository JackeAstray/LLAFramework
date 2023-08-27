using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 声音模块
    /// </summary>
    public class SoundModule : MonoBehaviour, CustommModuleInitialize
    {
        #region 实例与初始化
        public static SoundModule Instance = new SoundModule();
        public bool IsInited { get; private set; }
        private double _initProgress = 0;
        public double InitProgress { get { return _initProgress; } }
        #endregion

        public GameObject root { get; private set; }

        private SoundModuleSettings _settings;

        List<SMSound> _sounds = new List<SMSound>();

        struct PreloadedClip
        {
            public AudioClip clip;
            public int level;
        }

        //预加载的AudioClip
        Dictionary<string, PreloadedClip> _preloadedClips = new Dictionary<string, PreloadedClip>(16);

        SMMusic _music;
        string _currentMusicName;

        List<SMMusicFadingOut> _musicFadingsOut = new List<SMMusicFadingOut>();

        private bool _loadingInProgress;

        public IEnumerator Init()
        {
            Log.Debug("SoundModule 初始化");
            _initProgress = 0;

            CreateAudioRoot();

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
            if (IsInited == false)
            {
                return;
            }
            // Destory only one sound per frame
            SMSound soundToDelete = null;

            foreach (SMSound sound in _sounds)
            {
                if (IsSoundFinished(sound))
                {
                    soundToDelete = sound;
                    break;
                }
            }

            if (soundToDelete != null)
            {
                soundToDelete.IsValid = false;
                _sounds.Remove(soundToDelete);
                GameObject.Destroy(soundToDelete.Source.gameObject);
            }

            if (_settings.AutoPause)
            {
                bool curPause = Time.timeScale < 0.1f;
                if (curPause != AudioListener.pause)
                {
                    AudioListener.pause = curPause;
                }
            }

            for (int i = 0; i < _musicFadingsOut.Count; i++)
            {
                SMMusicFadingOut music = _musicFadingsOut[i];
                if (music.Source == null)
                {
                    _musicFadingsOut.RemoveAt(i);
                    i--;
                }
                else
                {
                    music.Timer += Time.unscaledDeltaTime;
                    _musicFadingsOut[i] = music;
                    if (music.Timer >= music.FadingTime)
                    {
                        GameObject.Destroy(music.Source.gameObject);
                        _musicFadingsOut.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        float k = Mathf.Clamp01(music.Timer / music.FadingTime);
                        music.Source.volume = Mathf.Lerp(music.StartVolume, 0, k);
                    }
                }
            }

            if (_music != null && _music.FadingIn)
            {
                _music.Timer += Time.unscaledDeltaTime;
                if (_music.Timer >= _music.FadingTime)
                {
                    _music.Source.volume = _music.TargetVolume;
                    _music.FadingIn = false;
                }
                else
                {
                    float k = Mathf.Clamp01(_music.Timer / _music.FadingTime);
                    _music.Source.volume = Mathf.Lerp(0, _music.TargetVolume, k);
                }
            }

            foreach (SMSound sound in _sounds)
            {
                if (sound.IsAttachedToTransform && sound.Attach != null)
                {
                    sound.Source.transform.position = sound.Attach.position;
                }
            }
        }

        public void CreateAudioRoot()
        {
            root = new GameObject("AudioRoot");
            GameObject.DontDestroyOnLoad(root);

            _settings = ResourcesModule.Instance.Load<SoundModuleSettings>("SoundModuleSettings");
            if (_settings == null)
            {
                Log.Warning("SoundModuleSettings not founded resources. Using default settings");
                _settings = ScriptableObject.CreateInstance<SoundModuleSettings>();
            }

            _settings.LoadSettings();

            if (_settings.PreloadedLoadedClips != null && _settings.PreloadedLoadedClips.Length > 0)
            {
                foreach (AudioClip permanentLoadedClip in _settings.PreloadedLoadedClips)
                {
                    PreloadedClip preloadedClip;
                    preloadedClip.clip = permanentLoadedClip;
                    preloadedClip.level = 1;
                    _preloadedClips.Add(permanentLoadedClip.name, preloadedClip);
                }
            }

            ApplySoundVolume();
            ApplyMusicVolume();

            ApplySoundMuted();
            ApplyMusicMuted();
        }

        #region 功能
        public void PlayMusic(string name)
        {
            PlayMusicInternal(name);
        }

        public void StopMusic()
        {
            StopMusicInternal();
        }

        public SMSound PlaySound(AudioClip clip)
        {
            return PlaySoundClipInternal(clip, true);
        }

        public SMSound PlaySoundUI(AudioClip clip)
        {
            return PlaySoundClipInternal(clip, false);
        }

        public SMSound PlaySound(string name, AssetBundle bundle)
        {
            return PlaySoundInternal(name, true);
        }

        public SMSound PlaySoundUI(string name, AssetBundle bundle)
        {
            return PlaySoundInternal(name, false);
        }

        public SMSound PlaySound(string name)
        {
            return PlaySoundInternal(name, true);
        }

        public SMSound PlaySoundUI(string name)
        {
            return PlaySoundInternal(name, false);
        }

        // Deprecated. Will be changed in future version
        public void PlaySoundWithDelay(string name, float delay, bool pausable = true)
        {
            PlaySoundWithDelayInternal(name, delay, pausable);
        }

        public void LoadSound(string name)
        {
            LoadSoundInternal(name);
        }

        public void UnloadSound(string name, bool force = false)
        {
            UnloadSoundInternal(name, force);
        }

        public void Pause()
        {
            if (_settings.AutoPause)
                return;

            AudioListener.pause = true;
        }

        public void UnPause()
        {
            if (_settings.AutoPause)
                return;

            AudioListener.pause = false;
        }

        public void StopAllPausableSounds()
        {
            StopAllPausableSoundsInternal();
        }

        // Volume [0 - 1]
        public void SetMusicVolume(float volume)
        {
            _settings.SetMusicVolume(volume);
            ApplyMusicVolume();
        }

        // Volume [0 - 1]
        public float GetMusicVolume()
        {
            return _settings.GetMusicVolume();
        }

        public void SetMusicMuted(bool mute)
        {
            _settings.SetMusicMuted(mute);
            ApplyMusicMuted();
        }

        public bool GetMusicMuted()
        {
            return _settings.GetMusicMuted();
        }

        // Volume [0 - 1]
        public void SetSoundVolume(float volume)
        {
            _settings.SetSoundVolume(volume);
            ApplySoundVolume();
        }

        // Volume [0 - 1]
        public float GetSoundVolume()
        {
            return _settings.GetSoundVolume();
        }

        public void SetSoundMuted(bool mute)
        {
            _settings.SetSoundMuted(mute);
            ApplySoundMuted();
        }

        public bool GetSoundMuted()
        {
            return _settings.GetSoundMuted();
        }

        public SoundModuleSettings GetSettings()
        {
            return _settings;
        }

        public void Stop(SMSound smSound)
        {
            StopSoundInternal(smSound);
        }
        #endregion



        #region Music

        void PlayMusicInternal(string musicName)
        {
            if (string.IsNullOrEmpty(musicName))
            {
                Debug.Log("Music empty or null");
                return;
            }

            if (_currentMusicName == musicName)
            {
                Debug.Log("Music already playing: " + musicName);
                return;
            }

            StopMusicInternal();

            _currentMusicName = musicName;

            AudioClip musicClip = LoadClip("Music/" + musicName);

            GameObject music = new GameObject("Music: " + musicName);
            AudioSource musicSource = music.AddComponent<AudioSource>();

            music.transform.SetParent(root.transform);

            musicSource.outputAudioMixerGroup = _settings.MusicAudioMixerGroup;

            musicSource.loop = true;
            musicSource.priority = 0;
            musicSource.playOnAwake = false;
            musicSource.mute = _settings.GetMusicMuted();
            musicSource.ignoreListenerPause = true;
            musicSource.clip = musicClip;
            musicSource.Play();

            musicSource.volume = 0;

            _music = new SMMusic();
            _music.Source = musicSource;
            _music.FadingIn = true;
            _music.TargetVolume = _settings.GetMusicVolumeCorrected();
            _music.Timer = 0;
            _music.FadingTime = _settings.MusicFadeTime;
        }

        void StopMusicInternal()
        {
            _currentMusicName = "";
            if (_music != null)
            {
                StartFadingOutMusic();
                _music = null;
            }
        }

        #endregion // Music

        #region Sound

        SMSound PlaySoundInternal(string soundName, bool pausable, AssetBundle bundle = null)
        {
            SMSound sound = new SMSound();
            sound.Name = soundName;
            sound.SelfVolume = 1;

            if (string.IsNullOrEmpty(soundName))
            {
                Debug.Log("Sound null or empty");
                sound.IsValid = false;
                return sound;
            }

            int sameCountGuard = 0;
            foreach (SMSound smSound in _sounds)
            {
                if (smSound.Name == soundName)
                    sameCountGuard++;
            }

            if (sameCountGuard > 8)
            {
                Debug.Log("Too much duplicates for sound: " + soundName);
                sound.IsValid = false;
                return sound;
            }

            if (_sounds.Count > 16)
            {
                Debug.Log("Too much sounds");
                sound.IsValid = false;
                return sound;
            }


            GameObject soundGameObject = new GameObject("Sound: " + soundName);
            AudioSource soundSource = soundGameObject.AddComponent<AudioSource>();
            soundGameObject.transform.parent = root.transform;

            sound.Source = soundSource;
            sound.IsValid = true;

            soundSource.outputAudioMixerGroup = _settings.SoundAudioMixerGroup;
            soundSource.priority = 128;
            soundSource.playOnAwake = false;
            soundSource.mute = _settings.GetSoundMuted();
            soundSource.volume = _settings.GetSoundVolumeCorrected();
            soundSource.ignoreListenerPause = !pausable;

            _sounds.Add(sound);

            PreloadedClip preloadedClip;
            if (_preloadedClips.TryGetValue(soundName, out preloadedClip))
            {
                soundSource.clip = preloadedClip.clip;
                soundSource.Play();
            }
            else
            {
                sound.LoadingCoroutine = PlaySoundInternalAfterLoad(sound, soundName, bundle);
                StartCoroutine(sound.LoadingCoroutine);
            }

            return sound;
        }

        SMSound PlaySoundClipInternal(AudioClip clip, bool pausable)
        {
            SMSound sound = new SMSound();
            sound.Name = clip.name;
            sound.SelfVolume = 1;

            if (_sounds.Count > 16)
            {
                Debug.Log("Too much sounds");
                sound.IsValid = false;
                return sound;
            }


            GameObject soundGameObject = new GameObject("Sound: " + sound.Name);
            AudioSource soundSource = soundGameObject.AddComponent<AudioSource>();
            soundGameObject.transform.parent = root.transform;

            sound.Source = soundSource;
            sound.IsValid = true;

            soundSource.clip = clip;
            soundSource.outputAudioMixerGroup = _settings.SoundAudioMixerGroup;
            soundSource.priority = 128;
            soundSource.playOnAwake = false;
            soundSource.mute = _settings.GetSoundMuted();
            soundSource.volume = _settings.GetSoundVolumeCorrected();
            soundSource.ignoreListenerPause = !pausable;
            soundSource.Play();

            _sounds.Add(sound);

            return sound;
        }

        IEnumerator PlaySoundInternalAfterLoad(SMSound smSound, string soundName, AssetBundle bundle)
        {
            smSound.IsLoading = true;

            // Need to wait others sounds to be loaded to avoid Android LoadingPersistentStorage lags
            while (_loadingInProgress)
            {
                yield return null;
            }

            _loadingInProgress = true;
            smSound.IsPossessedLoading = true;
            AudioClip soundClip = null;
            if (bundle == null)
            {
                ResourceRequest request = LoadClipAsync("Sounds/" + soundName);
                while (!request.isDone)
                    yield return null;
                soundClip = (AudioClip)request.asset;
            }
            else
            {
                AssetBundleRequest request = LoadClipFromBundleAsync(bundle, soundName);
                while (!request.isDone)
                    yield return null;
                soundClip = (AudioClip)request.asset;
            }
            smSound.IsPossessedLoading = false;
            _loadingInProgress = false;

            if (null == soundClip)
            {
                Debug.Log("Sound not loaded: " + soundName);
            }

            smSound.IsLoading = false;
            smSound.Source.clip = soundClip;
            smSound.Source.Play();
        }

        void PlaySoundWithDelayInternal(string soundName, float delay, bool pausable)
        {
            StartCoroutine(PlaySoundWithDelayCoroutine(soundName, delay, pausable));
        }

        void StopAllPausableSoundsInternal()
        {
            foreach (SMSound sound in _sounds)
            {
                if (!sound.Source.ignoreListenerPause)
                {
                    StopSoundInternal(sound);
                }
            }
        }

        void StopSoundInternal(SMSound sound)
        {
            if (sound.IsLoading)
            {
                StopCoroutine(sound.LoadingCoroutine);
                if (sound.IsPossessedLoading)
                    _loadingInProgress = false;

                sound.IsLoading = false;
            }
            else
                sound.Source.Stop();
        }

        private void LoadSoundInternal(string soundName)
        {
            AudioClip clip = LoadClip("Sounds/" + soundName);
            if (clip != null)
            {
                if (!clip.preloadAudioData)
                    clip.LoadAudioData();

                PreloadedClip preloadedClip;
                if (_preloadedClips.TryGetValue(soundName, out preloadedClip))
                {
                    preloadedClip.clip = clip;
                    preloadedClip.level += 1;
                }
                else
                {
                    preloadedClip.clip = clip;
                    preloadedClip.level = 1;
                    _preloadedClips.Add(soundName, preloadedClip);
                }
            }
        }

        private void UnloadSoundInternal(string soundName, bool force)
        {
            PreloadedClip preloadedClip;
            if (_preloadedClips.TryGetValue(soundName, out preloadedClip))
            {
                if (preloadedClip.level > 1 && !force)
                {
                    preloadedClip.level -= 1;
                }
                else
                {
                    _preloadedClips.Remove(soundName);
                    if (!preloadedClip.clip.preloadAudioData)
                        preloadedClip.clip.UnloadAudioData();
                }
            }

        }
        #endregion // Sound

        void StartFadingOutMusic()
        {
            if (_music != null)
            {
                SMMusicFadingOut fader = new SMMusicFadingOut();
                fader.Source = _music.Source;
                fader.FadingTime = _settings.MusicFadeTime;
                fader.Timer = 0;
                fader.StartVolume = _music.Source.volume;
                _musicFadingsOut.Add(fader);
            }
        }

        private IEnumerator PlaySoundWithDelayCoroutine(string name, float delay, bool pausable)
        {
            float timer = delay;
            while (timer > 0)
            {
                timer -= pausable ? Time.deltaTime : Time.unscaledDeltaTime;
                yield return null;
            }

            PlaySoundInternal(name, pausable);
        }

        AudioClip LoadClip(string name)
        {
            string path = "SoundManager/" + name;
            AudioClip clip = Resources.Load<AudioClip>(path);
            return clip;
        }

        ResourceRequest LoadClipAsync(string name)
        {
            string path = "SoundManager/" + name;
            return Resources.LoadAsync<AudioClip>(path);
        }

        AssetBundleRequest LoadClipFromBundleAsync(AssetBundle bundle, string name)
        {
            return bundle.LoadAssetAsync<AudioClip>(name);
        }

        bool IsSoundFinished(SMSound sound)
        {
            if (sound.IsLoading)
                return false;

            if (sound.Source.isPlaying)
                return false;

            if (sound.Source.clip.loadState == AudioDataLoadState.Loading)
                return false;

            if (!sound.Source.ignoreListenerPause && AudioListener.pause)
                return false;

            return true;
        }

        void ApplySoundVolume()
        {
            foreach (SMSound sound in _sounds)
            {
                sound.Source.volume = _settings.GetSoundVolumeCorrected() * sound.SelfVolume;
            }
        }

        void ApplyMusicVolume()
        {
            if (_music != null)
            {
                _music.FadingIn = false;
                _music.TargetVolume = _settings.GetMusicVolumeCorrected();
                _music.Source.volume = _music.TargetVolume;
            }
        }

        void ApplySoundMuted()
        {
            foreach (SMSound sound in _sounds)
            {
                sound.Source.mute = _settings.GetSoundMuted();
            }
        }

        void ApplyMusicMuted()
        {
            if (_music != null)
            {
                _music.Source.mute = _settings.GetMusicMuted();
            }
        }
    }
}