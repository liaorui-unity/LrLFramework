using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AddressableAssets;

namespace Audio
{
    public class AudioLoadInfo
    {
        public AudioLoadSign sign;
        public string path;
        public string name;
    }

    public class AudioLoadData
    {
        public string group;
        public AudioLoadType loadType;
        public List<AudioLoadInfo> infos;
    }

    public enum AudioLoadType
    {
        Resources,
        Addreassable,
        AssetBundle,
        Custom,
    }

    public enum AudioLoadSign
    { 
         Sound,
         Subtitle,
    }


    public class AudioLoadRes : MonoBehaviour
    {
        public static Func<string, AudioLoadData> OnGetAudioLoadInfo;

        public static UnityAction<AudioMgr, AudioLoadData> customLoadCall;

        AudioMgr AudioMgr;

        public bool isComplete = false; 


        public void LoadAudio(string group)
        {
            isComplete = false;

            AudioMgr = GetComponent<AudioMgr>();

            var groupData = OnGetAudioLoadInfo(group);
            if (groupData == null)
            {
                Debuger.LogError($"音频组 {group} 未配置");
                return;
            }

            switch (groupData.loadType)
            {
                case AudioLoadType.Resources:
                    LoadAudioByResources(groupData);
                    break;
                case AudioLoadType.Addreassable:
                    LoadAudioByAddreassable(groupData);
                    break;
                case AudioLoadType.AssetBundle:
                    LoadAudioByAssetBundle(groupData);
                    break;
                case AudioLoadType.Custom:
                     customLoadCall?.Invoke(AudioMgr, groupData);
                    break;
            }
        }

        private void LoadAudioByResources(AudioLoadData loadData)
        {     
            foreach (var item in loadData.infos)
            {
                if (item.sign == AudioLoadSign.Sound)
                {
                    var subData = Resources.Load<AudioSubtitleData>(item.path);
                    InitSubtitle(subData);
                }
                else if (item.sign == AudioLoadSign.Subtitle)
                {
                    var soundData = Resources.Load<AudioSoundData>(item.path);
                    InitSound(soundData);
                }
            }
            isComplete = true;
        }

        private async void LoadAudioByAddreassable(AudioLoadData loadData)
        {
            foreach (var item in loadData.infos)
            {
                if (item.sign == AudioLoadSign.Sound)
                {
#if ADDRESSABLES_AVAILABLE
                    var subData = await Addressables.LoadAssetAsync<AudioSoundData>(item.path);
                    InitSound((AudioSoundData)subData);
#endif
                }
                else if (item.sign == AudioLoadSign.Subtitle)
                {
#if ADDRESSABLES_AVAILABLE
                    var subData = await Addressables.LoadAssetAsync<AudioSubtitleData>(item.path);
                    InitSubtitle((AudioSubtitleData)subData);
#endif
                }
            }

            isComplete = true;
        }

        private async void LoadAudioByAssetBundle(AudioLoadData loadData)
        {
            foreach (var item in loadData.infos)
            {
                var ab = await AssetBundle.LoadFromFileAsync(item.path);
                if (ab == null)
                {
                    Debuger.LogError($"音频组 {loadData.group} 未加载成功");
                    return;
                }

                if (item.sign == AudioLoadSign.Sound)
                {
                    var soundData = ab.LoadAsset<AudioSoundData>(item.name);
                    InitSound(soundData);
                }
                else if (item.sign == AudioLoadSign.Subtitle)
                {
                    var subData = ab.LoadAsset<AudioSubtitleData>(item.name);
                    InitSubtitle(subData);
                }
            }

            isComplete = true;
        }


        private void InitSound(AudioSoundData soundData)
        {
            foreach (var audio in soundData.audioLists)
            {
                if (audio == null)
                {
                    Debuger.LogError($"音效配置文件SoundAsset {audio.clipName} 为空");
                    continue;
                }
                AudioMgr.soundClips.Add(audio.clipName, audio);

                audio.clipSoundData.LoadAudioData();
            }
        }


        private void InitSubtitle(AudioSubtitleData subData)
        {
            foreach (var audio in subData.audioLists)
            {
                if (audio == null)
                {
                    Debuger.LogError($"字幕音效配置文件SubtitleAsset {audio.clipName} 为空");
                    continue;
                }

                AudioMgr.subtitleClips.Add(audio.clipName, audio);

                foreach (var clip in audio.clips)
                {
                    clip.clipSoundData.LoadAudioData();
                }
            }
        }
    }
}
