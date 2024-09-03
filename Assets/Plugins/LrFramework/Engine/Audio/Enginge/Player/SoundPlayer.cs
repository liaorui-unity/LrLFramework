using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Audio
{
    public class SoundPlayer : AudioPlayer
    {
        public List<Source> sources = new List<Source>();

        public new bool playing
        {
            get
            {
                return sources.Any(_ => _.isRuning);
            }
        }

        public override void SetVolume(float radio)
        {
            this.volume = radio;

            foreach (var item in sources)
            {
                item.SetVolume(AudioMgr.GameVolume.value * radio);
            }
        }


        public void Play(SoundData sound, bool isLoop, float volume)
        {
            CreateSource<Source2D>();

            playSource.doneCall = PlayClipsFunction;
            playSource.clipName = sound.clipName;
            playSource.SetData(isLoop, volume);
            playSource.SetVolume(AudioMgr.GameVolume.value * this.volume);
            playSource.Play(sound);
            playSource = null;
        }

        public void Play(List<SoundData> sounds, bool isLoop, float volume)
        {
            CreateSource<Source2D>();

            playSource.doneCall = PlayClipsFunction;
            playSource.clipName = string.Join(',', sounds.Select(_ => _.clipName));
            playSource.SetData(isLoop, volume);
            playSource.SetVolume(AudioMgr.GameVolume.value * this.volume);
            playSource.Play(sounds);
            playSource = null;
        }

        public void Play(SoundData sound, Transform follow, bool isLoop, float volume)
        {
            CreateSource<Source3D>();

            ((Source3D)playSource).SetSource(follow);
            playSource.doneCall = PlayClipsFunction;
            playSource.clipName = sound.clipName;
            playSource.SetData(isLoop, volume);
            playSource.SetVolume(AudioMgr.GameVolume.value * this.volume);
            playSource.Play(sound);
            playSource = null;
        }

        public void Play(List<SoundData> sounds, Transform follow, bool isLoop, float volume)
        {
            CreateSource<Source3D>();

            ((Source3D)playSource).SetSource(follow);
            playSource.doneCall = PlayClipsFunction;
            playSource.clipName = string.Join(',', sounds.Select(_ => _.clipName));
            playSource.SetData(isLoop, volume);
            playSource.SetVolume(AudioMgr.GameVolume.value * this.volume);
            playSource.Play(sounds);
            playSource = null;
        }


        internal override void CreateSource<T>()
        {
            playSource = ObjectPool.Get<T>();
            sources.Add(playSource);
            AudioMgr.Instance.uses.Add((T)playSource);
          
        }

        protected override void PlayClipsFunction(Source doneSource)
        {
            if (sources.Contains(doneSource))
            {
                sources.Remove(doneSource);
            }
        }

        protected override void Update()
        {
         
        }



        public override void Stop()
        {
            foreach (var item in sources)
            {
                item.Stop();
            }
            sources.Clear();
        }

        public void Stop(string name)
        {
            var currents = sources.Where(_ => _.clipName == name).ToList();

            for (int i = currents.Count - 1; i >= 0; i--)
            {
                currents[i].Stop();
            }
        }
    }
}
