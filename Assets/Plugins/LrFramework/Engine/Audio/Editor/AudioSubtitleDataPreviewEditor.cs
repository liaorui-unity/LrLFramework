using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(Audio.AudioSubtitleData))]
public class AudioSubtitleDataPreviewEditor : Editor
{

    SerializedProperty property;

    SerializedProperty audioLists;

    Vector2 scrollPos= Vector2.zero;

    private void OnEnable()
    {
        audioLists = serializedObject.FindProperty("audioLists");
    }


    public override void OnInspectorGUI()
    {
        // 获取目标对象的引用
        Audio.AudioSubtitleData audioSoundData = (Audio.AudioSubtitleData)target;

        // 绘制一个按钮，用来选择文件夹
    
        EditorGUICustom.BoxColor(Color.gray, (_) =>
        {
            GUILayout.Label("音频文件夹路径", GUILayout.Width(100));
        });

        audioSoundData.urlPath = EditorGUILayout.TextField(audioSoundData.urlPath);
        audioSoundData.urlPath = EditorGUICustom.HandleDragAndDrop(GUILayoutUtility.GetLastRect(), audioSoundData.urlPath, 1);

        serializedObject.UpdateIfRequiredOrScript();

        EditorGUILayout.BeginScrollView(scrollPos);

        EditorGUILayout.PropertyField(audioLists, true);

        EditorGUILayout.EndScrollView();

        serializedObject.ApplyModifiedProperties();



        if (GUILayout.Button("folder clip setting"))
        {
            if (AssetDatabase.IsValidFolder(audioSoundData.urlPath))
            {
                var clips = LoadAllFolder(audioSoundData.urlPath);

                Debug.Log("Folder:"+clips.Count);


                foreach (var item in clips)
                {
                    Debug.Log("Folder item:" + item.Count);

                    foreach (var clip in item)
                    {
                        var find = audioSoundData.audioLists.Find(x => x.clipName == clip.name);
                        if (find == null)
                        {
                            find = new Audio.SubtitleData();
                            find.clips    = new List<Audio.SoundData>();
                            find.clipName = clip.name;
                            find.isNeverBreak = false;
                            find.clipDepth    = 1;
                            audioSoundData.audioLists.Add(find);
                        }
                        if (find != null)
                        {
                            var url = find.clips.Find(x => x.clipSoundUrl == AssetDatabase.GetAssetPath(clip));
                            if (url == null)
                            {
                                find.clips.Add(new Audio.SoundData()
                                {
                                    clipVolume = 1,
                                    clipName = clip.name,
                                    clipCD = -1,
                                    clipLenght = clip.length,
                                    clipSoundData = clip,
                                    clipSoundUrl = AssetDatabase.GetAssetPath(clip)
                                });

                                if (find.clips.Count >= 2)
                                {
                                    find.clips.Sort((x, y) => EditorUtility.NaturalCompare(x.clipSoundUrl, y.clipSoundUrl));
                                }
                            }
                        }
                    }
                }
            }
        }


        if (GUILayout.Button("default setting"))
        {
            foreach (var item in audioSoundData.audioLists)
            {
                foreach (var clip in item.clips)
                {
                    if (clip.clipVolume == 0)
                        clip.clipVolume = 1;

                    if (clip.clipCD == 0)
                        clip.clipCD = -1;
                }

                if (item.clipDepth == 0)
                    item.clipDepth = 1;

                item.isNeverBreak = false;
            }
        }
    }


    List<List<AudioClip>> LoadAllFolder(string folder)
    {
        List<List<AudioClip>> clips = new List<List<AudioClip>>();
        //获取文件夹下的不同语言文件夹
        DirectoryInfo folderInfo = new DirectoryInfo(System.Environment.CurrentDirectory + "/" + folder);

        if (folderInfo.Exists)
        {
            DirectoryInfo[] folders = folderInfo.GetDirectories();
            foreach (var item in folders)
            {
                var fullName = item.FullName.Replace('\\', '/');
                var unity    = fullName.Replace(System.Environment.CurrentDirectory.Replace('\\','/'),"").TrimStart('/');
                List <AudioClip> clip = LoadAll(unity);
                clips.Add(clip);
            }
        }
        return clips;
    }

    List<AudioClip> LoadAll(string folder)
    {
        //获取文件夹下所有的音频文件
        List<AudioClip> clips = new List<AudioClip>();
        string[] guids = AssetDatabase.FindAssets("t:AudioClip", new string[] { folder });
        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            AudioClip clip = FindClip(path);
            if (clip != null)
            {
                clips.Add(clip);
            }
        }
        return clips;
    }

    AudioClip FindClip(string path)
    {
        return AssetDatabase.LoadAssetAtPath<AudioClip>(path);
    }

}
