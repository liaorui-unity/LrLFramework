using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using UnityEngine;
using UnityEngine.Events;
using Audio;

public class CallValue<T>
{
    T _value;
    public T value
    {
        get => _value;
        set
        {
            _value = value;
            action?.Invoke(value);
        }
    }
    public UnityAction<T> action;

    public CallValue(T value)
    {
        this.value = value;
    }
}


public class AudioMgr : MonoBehaviour
{
    private static AudioMgr instance;
    public static AudioMgr Instance
    {
        get
        {
            return instance;
        }
    }


    public Dictionary<string, SoundData>       soundClips = new Dictionary<string, SoundData>();
    public Dictionary<string, SubtitleData> subtitleClips = new Dictionary<string, SubtitleData>();

    internal List<Source> uses = new List<Source>();


    private AudioLoadRes     loadRes;
    private SubtitlePlayer   subPlayer;
    private SoundPlayer      soundPlayer;
    private BackGroundPlayer bgPlayer;

    private VolumeToSetting  volumeSetting;

    /// <summary>
    /// 游戏音量，影响音效和旁白
    /// </summary>
    public static CallValue<float> GameVolume = new CallValue<float>(1.0f);


    /// <summary>
    /// 游戏音量，影响音效和旁白
    /// </summary>
    public static CallValue<float> BGVolume = new CallValue<float>(1.0f);


    /// <summary>
    /// 语言
    /// NOTE:需要外部设置
    /// </summary>
    public static CallValue<int> GameLanguage = new CallValue<int>(1);




    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void StartOnUnity()
    {
        instance = new GameObject("AudioMgrGame").AddComponent<AudioMgr>();
        //防止场景切换时被消耗，导致音量和语言等设置失效
        DontDestroyOnLoad(instance.gameObject);
    }


    private void Awake()
    {
        if (bgPlayer == null)
        {
            bgPlayer = gameObject.AddComponent<BackGroundPlayer>();
        }

        if (subPlayer == null)
        {
            subPlayer = gameObject.AddComponent<SubtitlePlayer>();
        }

        if (soundPlayer == null)
        {
            soundPlayer = gameObject.AddComponent<SoundPlayer>();
        }

        if (loadRes == null)
        {
            loadRes = gameObject.AddComponent<AudioLoadRes>();
        }

        if (volumeSetting == null)
        {
            volumeSetting = gameObject.AddComponent<VolumeToSetting>();
        }
    }



    #region 播放音效事件

    /// <summary>
    /// 播放指定key值的音频(2D)
    /// </summary>
    /// <param name="clipName">音频的key</param>
    /// <param name="isLoop">是否循环播放</param>
    /// <param name="volume">音频的音量</param>
    public static void Play(List<string> clipNames, float volume = 1.0f)
    {
        Play(clipNames, false, volume);
    }

    /// <summary>
    /// 播放指定key值的音频(2D)
    /// </summary>
    /// <param name="clipName">音频的key</param>
    /// <param name="isLoop">是否循环播放</param>
    /// <param name="volume">音频的音量</param>
    public static void Play(string clipName, float volume = 1.0f)
    {
        Play(clipName, false, volume);
    }

    /// <summary>
    /// 播放指定key值的音频(2D)
    /// </summary>
    /// <param name="clipName">音频的key</param>
    /// <param name="isLoop">是否循环播放</param>
    public static void Play(string clipName, bool isLoop)
    {
        Play(clipName, isLoop, 1.0f);
    }

    /// <summary>
    /// 播放指定key值的音频(2D)
    /// </summary>
    /// <param name="clipName">音频的key</param>
    /// <param name="isLoop">是否循环播放</param>
    /// <param name="volume">音频的音量</param>
    public static void Play(string clipName, bool isLoop, float volume)
    {
        try
        {
            var sound = instance.soundClips[clipName];

            if (sound.isVerifyCD())
            {
                instance.soundPlayer.Play(sound, isLoop, volume);
            }
        }
        catch (System.Exception e)
        {
            Debuger.Log(string.Format("音频报错,{0}, 原因:{1}", clipName, e.ToString()));
        }
    }

    /// <summary>
    /// 播放指定key值的音频(2D)
    /// </summary>
    /// <param name="clipName">音频的key</param>
    /// <param name="isLoop">是否循环播放</param>
    /// <param name="volume">音频的音量</param>
    public static void Play(List<string> clipNames, bool isLoop, float volume)
    {
        try
        {
            var sounds = clipNames
                       .Select(x => instance.soundClips[x])
                       .ToList();

            if (sounds[0].isVerifyCD())
            {
                instance.soundPlayer.Play(sounds, isLoop, volume);
            }
        }
        catch (System.Exception e)
        {
            Debuger.Log(string.Format("音频报错,{0}, 原因:{1}", clipNames[0], e.ToString()));
        }
    }



    /// <summary>
    /// 播放指定key值的音频(3D)
    /// </summary>
    /// <param name="clipName">音频的key</param>
    /// <param name="game3D">播放3D音效需要的物体</param>
    /// <param name="isLoop">是否循环播放</param>
    /// <param name="volume">音频的音量</param>
    public static void Play(string clipName, GameObject game3D, bool isLoop = false, float volume = 1.0f)
    {
        try
        {
            var sound = instance.soundClips[clipName];
            if (sound.isVerifyCD())
            {
                instance.soundPlayer.Play(sound, game3D.transform, isLoop, volume);
            }
        }
        catch (System.Exception e)
        {
            Debuger.Log(string.Format("音频报错,{0}, 原因:{1}", clipName, e.ToString()));
        }
    }

    /// <summary>
    /// 播放指定key值的音频(3D)
    /// </summary>
    /// <param name="clipName">音频的key</param>
    /// <param name="game3D">播放3D音效需要的物体</param>
    /// <param name="isLoop">是否循环播放</param>
    /// <param name="volume">音频的音量</param>
    public static void Play(List<string> clipNames, GameObject game3D, bool isLoop = false, float volume = 1.0f)
    {
        try
        {
            var sounds = clipNames
                       .Select(x => instance.soundClips[x])
                       .ToList();

            if (sounds[0].isVerifyCD())
            {
                instance.soundPlayer.Play(sounds, isLoop, volume);
            }
        }
        catch (System.Exception e)
        {
            Debuger.Log(string.Format("音频报错,{0}, 原因:{1}", clipNames[0], e.ToString()));
        }
    }

    /// <summary>
    /// 播放指定key值的背景音频(2D)
    /// </summary>
    /// <param name="clipName">音频的key</param>
    /// <param name="loop">是否循环</param>
    /// <param name="volume">音量</param>
    /// <param name="time">指定位置开始播放</param> 
    public static void PlayBgMusic(string clipName)
    {
        instance.PlayBgMusic(clipName, true, 1);
    }

    public void PlayBgMusic(string clipName, bool loop, float volume)
    {
        try
        {
            var sound = soundClips[clipName];
            bgPlayer.Play(sound, loop, volume, null);
        }
        catch (System.Exception e)
        {
            Debuger.Log(string.Format("音频报错,{0}, 原因:{1}", clipName, e.ToString()));
        }
    }

    public static void PlayBgMusic(List<string> clipName, bool loop)
    {
        PlayBgMusic(clipName, loop);
    }

    public static void PlayBgMusic(List<string> clipName, bool loop, float volume = 1.0f)
    {
        try
        {
            var sounds = clipName
                       .Select(x => instance. soundClips[x])
                       .ToList();

            instance.bgPlayer.Play(sounds, loop, volume, null);
        }
        catch (System.Exception e)
        {
            Debuger.Log(string.Format("音频报错,{0}, 原因:{1}", clipName, e.ToString()));
        }
    }

    #endregion


    #region 全部音效快进， 暂停和恢复播放事件

    public static void AudioScale(float radio)
    {
        for (int i = 0; i < instance.uses.Count; i++)
        {
            instance.uses[i].mainSource.pitch = radio;
        }
    }

    /// <summary>
    /// 暂停音效
    /// </summary>
    public void Pause()
    {
        foreach (var item in uses)
        {
            item.Pause();
        }
    }

    /// <summary>
    /// 恢复暂停
    /// </summary>
    public void Replay()
    {
        foreach (var item in uses)
        {
            item.UnPause();
        }
    }
    #endregion


    #region 停止播放音效和删除正在播放的音效组件

    /// <summary>
    /// 停止播放音效
    /// </summary>
    public static void Stop()
    {
        for (int i = instance. uses.Count - 1; i >= 0; i--)
        {
            instance.uses[i].Stop();
        }
        instance.uses.Clear();
    }

    public static void Stop(string clipName)
    {
        var find2ds = instance.uses
                    .Where((x) => x.clipName == clipName)
                    .ToList();

        for (int i = find2ds.Count - 1; i >= 0; i--)
        {
            find2ds[i].Stop();
        }
    }


    internal void PlayClipComplete(Source source)
    {
        if (source is Source2D)
        {
            uses.Remove((Source2D)source);
        }
    }

    #endregion


    #region 播放字幕音频

    public static void PlaySubtitle(string clipName)
    {
        PlaySubtitle(clipName, null);
    }

    public static void PlaySubtitle(List<string> clipNames)
    {
        PlaySubtitle(clipNames, null);
    }

    public static void PlaySubtitle(string clipName, UnityAction action)
    {
        if (instance.subtitleClips[clipName].isVerifyBreak(instance.subPlayer.currentDepth))
        {
            Clip(clipName, action);
        }
        else
        {
            Debug.Log($"{clipName}-字幕音频无法播出");
        }
    }

    public static void PlaySubtitle(List<string> clipNames, UnityAction action)
    {
        var clipName = clipNames[0];

        if (instance.subtitleClips[clipName].isVerifyBreak(instance.subPlayer.currentDepth))
        {
            Clip(clipNames, action);
        }
        else
        {
            Debug.Log($"{clipName}-字幕音频无法播出");
        }
    }


    static void Clip(string clipName, UnityAction action)
    {
        int lang_index = GameLanguage.value;
        if (lang_index >= instance.subtitleClips[clipName].clips.Count)
            lang_index = 0;

        var data = instance.subtitleClips[clipName];
        var clip = data.clips[lang_index];

        instance.subPlayer.Play(clip, data.clipDepth, false, clip.clipVolume, action);
    }

    static void Clip(List<string> clipNames, UnityAction action)
    {
        var lang_index = GameLanguage.value;
        var sounds = new List<SoundData>();
        int depth = -1;

        foreach (var clipName in clipNames)
        {
            if (lang_index >= instance.subtitleClips[clipName].clips.Count)
                lang_index = 0;

            var data = instance.subtitleClips[clipName];
            var clip = data.clips[lang_index];

            depth = data.clipDepth;

            sounds.Add(clip);
        }

        instance.subPlayer.Play(sounds, depth, false, 1, action);
    }


    public void StopSubtitle()
    {
        subPlayer.Stop();
    }
    #endregion

    #region 加载音频文件

    public static void ClearCacheData()
    {
        instance.soundClips   .Clear();
        instance.subtitleClips.Clear();
    }

    /// <summary>
    /// 加载音频文件（音效和字幕）
    /// </summary>
    /// <param name="isAsync">是否异步加载</param>
    public  void LoadAllClip(string group)
    {
        soundClips.Clear();
        subtitleClips.Clear();

        loadRes.LoadAudio(group);

        StartCoroutine(LoadAsync());
    }

    IEnumerator LoadAsync()
    {
        while (true)
        {
            if (loadRes.isComplete)
            {
                break;
            }
            yield return 0;
        }
        Debuger.Log("音频加载完毕！");
    }

    #endregion



    private void OnDestroy()
    {
        uses.Clear();

        soundClips   .Clear();
        subtitleClips.Clear();

        instance = null;
    }
}


