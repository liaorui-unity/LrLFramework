 using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Audio
{
    public class BackGroundPlayer : AudioPlayer
    {
        public override void SetVolume(float radio)
        {
            this.volume = radio;
            playSource?.SetVolume(AudioMgr.BGVolume.value * radio);
        }


        public override void Play(SoundData sound,  bool isLoop, float volume, UnityAction action)
        {
            base.Play(sound, isLoop, volume, action);
            playSource.SetVolume(AudioMgr.BGVolume.value * this.volume);
        }

        public override void Play(List<SoundData> sounds,  bool isLoop, float volume, UnityAction action)
        {
            base.Play(sounds, isLoop, volume, action);
            playSource.SetVolume(AudioMgr.BGVolume.value * this.volume);
        }

    }
}
