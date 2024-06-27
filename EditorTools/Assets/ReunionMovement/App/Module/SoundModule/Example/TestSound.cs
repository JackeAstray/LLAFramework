using GameLogic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameLogic.SoundPoolModule;

namespace GameLogic.Example
{
    public class TestSound : MonoBehaviour
    { 
        public void PlaySound()
        {
            SoundModule.Instance.PlaySound(120012, PoolType.EffectSound);
        }

        public void PlaySound2()
        {
            SoundModule.Instance.PlaySound(120012, PoolType.Voice);
        }


        public void PlaySwitch()
        {
            SoundModule.Instance.PlaySwitch(100002);
        }
    }
}