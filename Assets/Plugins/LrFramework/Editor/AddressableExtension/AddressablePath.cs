using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEditor;
using System;
using System.Reflection;

[CreateAssetMenu(menuName = "Scriptable/Path", fileName = "AddressablePath.asset")]
public class AddressablePath : ScriptableObject
{
    [Header("是否在脚本重构完后复制脚本dll到指定位置")]
    public bool IsScriptRebuildCopyDll = false;

    public bool isEncrypt = false;

    [Header("不同输出环境的包地址")]
    public List<PathInfo> infos;



    public void Save()
    {
        foreach (var item in infos)
        {
            item.Save();
        }
    }

    public void Load()
    {
        foreach (var item in infos)
        {
            item.Load();
        }
    }
}

[System.Serializable]
public class PathInfo
{
    public BuildEnvironment environment;
    public string version;
    public bool isServer;
    public bool isCRC;

    public string PrefKey
    {
        get { return $"{environment}_{nameof(PrefKey)}"; }
    }

    public void Save()
    {
        PlayerPrefs.SetString(PrefKey, version);
    }

    public void Load()
    {
        version = PlayerPrefs.GetString(PrefKey);
    }
}


