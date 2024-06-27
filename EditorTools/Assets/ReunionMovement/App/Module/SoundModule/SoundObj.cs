using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameLogic.SoundPoolModule;

namespace GameLogic
{
    public class SoundObj : MonoBehaviour
    {
        public AudioClip clip;
        PoolType poolType = PoolType.EffectSound;
        public void OnEnable()
        {

        }

        public void OnDisable()
        {
            
        }

        public void OnDestroy() 
        {

        }

        public void Processing(int index, Transform emitter, bool loop, float volume, bool nute, PoolType poolType)
        {
            StartCoroutine(ObjectProcessing(index, emitter, loop, volume, nute, poolType));
        }

        IEnumerator ObjectProcessing(int index, Transform emitter, bool loop,float volume, bool nute, PoolType poolType)
        {
            if (emitter != null)
            {
                gameObject.transform.parent = emitter;
            }
            else
            {
                switch (poolType)
                {
                    case PoolType.Voice:
                        Transform root2 = SoundPoolModule.Instance.voicePoolTempRoot.transform;
                        gameObject.transform.parent = root2.transform;
                        break;
                    case PoolType.EffectSound:
                        Transform root1 = SoundPoolModule.Instance.effectSoundPoolTempRoot.transform;
                        gameObject.transform.parent = root1.transform;
                        break;
                }
            }

            gameObject.transform.localPosition = Vector3.zero;
            gameObject.SetActive(true);

            AudioSource t_source = gameObject.GetComponent<AudioSource>();
            t_source.clip = clip;
            t_source.volume = volume;
            t_source.loop = loop;
            t_source.mute = nute;
            t_source.Play();

            yield return new WaitForSeconds(clip.length);

            SoundPoolModule.Instance.Recycle(gameObject, poolType);
        }
    }
}