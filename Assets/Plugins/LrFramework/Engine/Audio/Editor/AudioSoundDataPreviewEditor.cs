using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(Audio.AudioSoundData))]
public class AudioSoundDataPreviewEditor : Editor
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
        Audio.AudioSoundData audioSoundData = (Audio.AudioSoundData)target;

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
                var clips = LoadAll(audioSoundData.urlPath);

                foreach (var item in clips)
                {
                    bool isExist = false;
                    foreach (var soundData in audioSoundData.audioLists)
                    {
                        if (soundData.clipSoundData != null)
                        {
                            if (soundData.clipSoundData == item)
                            {
                                isExist = true;
                                break;
                            }
                        }
                        else
                        { 
                            //如果没有音频文件，就判断url是否相同
                            if (soundData.clipSoundUrl == AssetDatabase.GetAssetPath(item))
                            {
                                isExist = true;
                                break;
                            }
                        }
                    }

                    if (!isExist)
                    {
                        audioSoundData.audioLists.Add(new Audio.SoundData()
                        {
                            clipCD        = -1,
                            clipVolume    = 1,
                            clipLenght    = item.length,
                            clipName      = item.name,
                            clipSoundData = item,
                            clipSoundUrl  = AssetDatabase.GetAssetPath(item)
                        });;
                    }
                }
            }
        }


        if (GUILayout.Button("default setting"))
        {
            foreach (var item in audioSoundData.audioLists)
            {
                if (item.clipVolume == 0)
                    item.clipVolume = 1;

                if (item.clipCD == 0)
                    item.clipCD = -1;
            }
        }
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
