using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[System.Serializable]
public class CheckData
{
    [Header("设置")]
    public bool   IsInitRuning = false;
    public bool   IsWriteLog   = false;

    [Header("运行")]
    public bool   isRuning = false;
    public int    runingProgressId;
    public string runingProgressName = "";
}

[System.Serializable]
public class CheckInfo
{
    [Header("继承的基类")]
    public string className;

    [Header("检测的特性")]
    public string attribute;

    [Header("类名的前缀")]
    public string forSuffix;

    public CheckInfo( string className, string forSuffix, string attribute)
    {
        this.className = className;
        this.forSuffix = forSuffix;
        this.attribute = attribute;
    }
}

[CreateAssetMenu(menuName = "CheakConfig", fileName = "CheakConfig")]
[System.Serializable]
public class CheakConfig :ScriptableObject
{
    private static CheakConfig s_instance;
    public static CheakConfig Instance
    {
        get
        {
            if (!s_instance) CreateAndLoad();
            return s_instance;
        }
    }

    public CheckData data = new CheckData();

    public const string AssetPath = "ProjectSettings/WatherConfig.asset";

    private static void CreateAndLoad()
    {
        // Load
        var files = InternalEditorUtility.LoadSerializedFileAndForget(AssetPath);
        if (files.Length != 0)
        {
            s_instance = (CheakConfig)files[0];
        }

        // Create
        if (!s_instance)
        {
            s_instance = CreateInstance<CheakConfig>();
            EditorUtility.SetDirty(s_instance);
            s_instance.Save();
        }
    }

    public void Save()
    {
        if (!s_instance)
        {
            Debug.LogError($"Cannot save {nameof(CheakConfig)}: no instance!");
            return;
        }

        InternalEditorUtility.SaveToSerializedFileAndForget(new[] { s_instance }, AssetPath, true);
    }
}
