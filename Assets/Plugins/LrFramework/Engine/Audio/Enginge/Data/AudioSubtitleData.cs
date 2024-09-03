using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
    [CreateAssetMenu(menuName = "Audio/SubtitleData", fileName = "subtitle")]
    public class AudioSubtitleData : ScriptableObject
    {
        public string urlPath;
        public List<SubtitleData> audioLists;
    }


    [System.Serializable]
    public class SubtitleData
    {
        public string clipName;

        public List<SoundData> clips;

        public bool isNeverBreak = true;

        public int  clipDepth = 1;


        public bool isVerifyBreak(int depth)
        {
            if (isNeverBreak)
            {
                Debuger.LogGreen("该字幕音频无法被中止");
                return false;
            }

            var isDepth = isVerifyDepth(depth);
            if (isDepth == false)
            {
                Debuger.LogGreen("该字幕音频Depth无法提前中止上一条");
                return false;
            }

            return true;
        }



        public bool isVerifyDepth(int depth)
        { 
           return clipDepth >= depth;
        }

    }
}