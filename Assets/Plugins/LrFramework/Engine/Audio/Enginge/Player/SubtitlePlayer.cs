using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

namespace Audio
{
    public class SubtitlePlayer : AudioPlayer
    {  
        public int currentDepth = -1;

        private void Start()
        {
            volume = 1;
        }


        public void Play(SoundData sound, int depth, bool isLoop, float volume, UnityAction action)
        {
            base.Play(sound, isLoop, volume, action);
            currentDepth = depth;
        }

        public void Play(List<SoundData> sounds, int depth, bool isLoop, float volume, UnityAction action)
        {
            base.Play(sounds, isLoop, volume, action);
            currentDepth = depth;
        }

        protected override void PlayClipsFunction(Source doneSource)
        {
            currentDepth = -1;
            base.PlayClipsFunction(doneSource);
        }


        public override void Stop()
        {
            currentDepth = -1;
            base.Stop();
        }
    }
}
