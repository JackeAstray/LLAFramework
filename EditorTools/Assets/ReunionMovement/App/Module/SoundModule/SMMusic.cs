using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    public class SMMusic
    {
        private string _name;
        public AudioSource Source;

        public float Timer;
        public float FadingTime;
        public float TargetVolume;
        public bool FadingIn;
    }

    /// <summary>
    /// 音乐淡出
    /// </summary>
    public class SMMusicFadingOut
    {
        private string _name;
        public AudioSource Source;

        public float Timer;
        public float FadingTime;
        public float StartVolume;
    }
}