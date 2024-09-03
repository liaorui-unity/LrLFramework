using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Audio
{

    public class AudioPlayer : MonoBehaviour
    {
        public Source      playSource;
        public UnityAction playComplete;

        public float volume;

        public bool  playing   = false;
        public float playRadio = 0f;

        protected virtual void Update()
        {
            if (playing)
            {
                playRadio = playSource.mainSource.time / playSource.dataTime;
            }
        }


        public virtual void SetVolume(float radio)
        {
            this.volume = radio;
            playSource?.SetVolume(AudioMgr.GameVolume.value * radio);
        }


        public virtual void Play(SoundData sound, bool isLoop, float volume, UnityAction action)
        {
            CreateSource<Source2D>();

            playSource.clipName = sound.clipName;
            playSource.doneCall = PlayClipsFunction;

            playSource.SetData(isLoop, volume);
            playSource.SetVolume(AudioMgr.GameVolume.value * this.volume);
            playSource.Play(sound);
            playComplete = action;

            playing = true;
        }


        public virtual void Play(List<SoundData> sounds, bool isLoop, float volume, UnityAction action)
        {
            CreateSource<Source2D>();

            playSource.clipName = string.Join(",", sounds.Select(x => x.clipName));
            playSource.doneCall = PlayClipsFunction;

            playSource.SetData(isLoop, volume);
            playSource.SetVolume(AudioMgr.GameVolume.value * this.volume);
            playSource.Play(sounds);

            playComplete = action;

            playing = true;
        }

  
        internal virtual void CreateSource<T>() where T :Source
        {
            if (playSource == null)
            {
                playSource = ObjectPool.Get<T>();
                AudioMgr.Instance.uses.Add((T)playSource);
            }

            if (playSource.mainSource.isPlaying)
            {
                playSource.Stop();
                playSource = ObjectPool.Get<T>();
                AudioMgr.Instance.uses.Add((T)playSource);
            }
        }

        protected virtual void PlayClipsFunction(Source doneSource)
        {
            playSource = null;
            playing    = false;
            playComplete?.Invoke();
        }



        public virtual void Stop()
        {
            if (playSource.mainSource.isPlaying)
            {
                playSource.Stop();
            }
        }
    }
}
