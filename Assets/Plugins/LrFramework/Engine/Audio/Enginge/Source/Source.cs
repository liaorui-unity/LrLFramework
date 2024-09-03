using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace Audio
{
    public class Source : MonoBehaviour
    {
        public UnityAction updateCall;
        public UnityAction<Source> doneCall;


        AudioSource source;
        public AudioSource mainSource
        {
            get
            {
                if (source == null)
                    source = gameObject.AddComponent<AudioSource>();
                return source;
            }
        }

        string _clipName;
        public string clipName
        {
            get { return _clipName; }
            set
            {
                _clipName = value;
                gameObject.name = value;
            }
        }

        List<SoundData> sounds = new List<SoundData>();
        Queue<SoundData> clips = new Queue<SoundData>();

        internal bool isPause  = false;
        internal bool isRuning = false;

        public float dataVolume { get; set; }
        public float dataTime   { get; set; }
        public bool  dataLoop   { get; set; }
        public float radioTime  { get; set; }

        public float runTime
        {
            get { return radioTime + mainSource.time; }
        }

        private float runVolume { get; set; }
        private float clipVolume { get; set; }

        /// <summary>
        /// 设置全局音量
        /// </summary>
        /// <param name="volume">全局音量</param>
        public void SetVolume(float volume)
        {
            runVolume = dataVolume * volume;
            mainSource.volume = runVolume * clipVolume;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isLoop"></param>
        /// <param name="volume">音频自带音量</param>
        public void SetData(bool isLoop, float volume)
        {
            dataVolume = volume;
            dataLoop   = isLoop;

            gameObject.SetActive(true);
        }


        public virtual void StopClip()
        {
            doneCall?.Invoke(this);

            isPause  = false;
            isRuning = false;

            clips .Clear();
            sounds.Clear();

            mainSource.Stop();
            mainSource.clip = null;
            gameObject.SetActive(false);
        }



        /// <summary>
        /// 播放声音
        /// </summary>
        /// <param name="audioClip"></param>
        /// <param name="time">开始播放位置</param>
        public void Play()
        {
            PlayEvent(null, 0);
        }

        public void Play(float time)
        {
            PlayEvent(null, time);
        }

        public void Play(SoundData audioClip)
        {
            PlayEvent(new List<SoundData>() { audioClip }, 0);
        }


        public void Play(List<SoundData> audioClips)
        {
            PlayEvent(audioClips, 0);
        }

        public void Play(List<SoundData> audioClips , float time)
        {
            PlayEvent(audioClips, time);
        }

        void PlayEvent(List<SoundData> datas, float time)
        {
            if (isPause)
            {
                UnPause();
                return;
            }

            if (clips.Count <= 0)
            {
                if (datas == null || datas.Count == 0)
                {
                    return;
                }
            }

            sounds = datas;

            InitSelfData();

            isPause  = false;
            isRuning = true;

            mainSource.time   = time;

            PlayClip(clips.Dequeue());
        }

        void InitSelfData()
        {
            radioTime = 0;
            clips.Clear();
            foreach (var item in sounds)
            {
                clips.Enqueue(item);
            }
        }

        void PlayClip(SoundData clip)
        {
            dataTime   = clip.clipSoundData.length;
            clipVolume = clip.clipVolume;
            mainSource . volume = clipVolume * runVolume;
            mainSource . clip   = clip.clipSoundData;
            mainSource . Play();
        }


        public void UnPause()
        {
            isPause = false;
            mainSource.UnPause();
        }

        public void Pause()
        {
            isPause = true;
            mainSource.Pause();
        }

        public void Stop()
        {        
            StopClip();
        }


        public bool IsCheckComplete
        {
            get
            {
                if (!dataLoop && !mainSource.isPlaying  && (dataTime <= runTime && dataTime != 0))
                {
                    return true;
                }
                return false;
            }
        }

        private void Update()
        {
            if (isRuning)
            {
                updateCall?.Invoke();

                if (IsCheckComplete || TolerateError())
                {
                    if (clips.Count > 0)
                    {
                        PlayClip(clips.Dequeue());
                    }
                    else
                    {
                        PlayDone();
                    }
                }
            }
        }

        bool TolerateError()
        {
            if (!mainSource.isPlaying && !isPause)
            {
                if (dataTime <= runTime + 0.1f && dataTime != 0)
                {
                    return true;
                }
            }
            return false;
        }



        protected virtual void PlayDone()
        {
            if (dataLoop)
            {
                InitSelfData();
                PlayClip(clips.Dequeue());
            }
            else
            {
                StopClip();
            }
        }


        public virtual void OnDestroy()
        {
            StopClip();

            source     = null;
            updateCall = null;
            doneCall   = null;
        }
    }
}
