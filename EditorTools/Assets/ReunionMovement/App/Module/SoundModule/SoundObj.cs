using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    public class SoundObj : MonoBehaviour
    {
        public AudioClip clip;

        public void OnEnable()
        {

        }

        public void OnDisable()
        {
            
        }

        public void OnDestroy() 
        {

        }

        public void ObjectProcessing(int index, Transform emitter, bool loop, float volume, bool nute)
        {
            StartCoroutine(ObjectProcessing2(index,emitter,loop,volume,nute));
        }

        IEnumerator ObjectProcessing2(int index, Transform emitter, bool loop,float volume, bool nute)
        {
            if (emitter != null)
            {
                gameObject.transform.parent = emitter;
            }
            else
            {
                Transform root = SoundModule.Instance.root.transform;
                gameObject.transform.parent = root.transform;
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

            SoundPoolModule.Instance.Recycle(gameObject);
        }
    }
}