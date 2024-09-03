using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Audio
{
    [CreateAssetMenu(menuName = "Audio/SoundData",fileName = "sound")]
    public class AudioSoundData : ScriptableObject
    {
        public string urlPath;
        public List<SoundData> audioLists;
    }


    [System.Serializable]
    public class SoundData
    {
        public string    clipName;
        public string    clipSoundUrl;
        public AudioClip clipSoundData;

        public int   clipCD = -1;
        public float clipVolume = 1;
        public float clipLenght;


        float  time = 0;
        public bool isVerifyCD()
        {
            if (Time.time - time >= clipCD || clipCD < 0)
            {
                time = Time.time;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}



