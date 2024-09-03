using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using System.Reflection;
using UnityEngine.Events;
using System.Security.Policy;
using System;
using UObject = UnityEngine.Object;

public interface IMonoAsset
{
    void OnNewHeaderGUI();
    void OnInspectorGUI();
    void OnDestroy();
}

public delegate void AssetAction(UObject uObject, IMonoAsset asset);

[CustomEditor(typeof(UnityEditor.DefaultAsset))]
[CanEditMultipleObjects]
public class DefaultAssetInspector : Editor
{  
    public static UnityEvent<UObject, AssetAction> OnSelectAsset = new UnityEvent<UObject, AssetAction>();
    
    IMonoAsset monoAsset;

    bool IsLock()
    {
        EditorWindow[] allEditorWindows = Resources.FindObjectsOfTypeAll<EditorWindow>();

        foreach (var item in allEditorWindows)
        {
            if (item.GetType().ToString() == "UnityEditor.InspectorWindow")
            {
                var tracker = item.GetType().BaseType.GetField("m_Tracker",BindingFlags.Public| BindingFlags.NonPublic| BindingFlags.Instance).GetValue(item) as ActiveEditorTracker;

                foreach (var edit in tracker.activeEditors)
                {
                    if (edit == this)
                    {
                        return tracker.isLocked;
                    }
                }
            }
        }
        return false;
    }




    void OnEnable()
    {
        if (IsLock())
        {
            return;
        }
        OnSelectAsset?.Invoke(target, Call);
    }


    void Call(UnityEngine.Object obj, IMonoAsset asset)
    {
        if (obj == target)
        {
            monoAsset = asset;
        }
    }

    protected override void OnHeaderGUI()
    {
        monoAsset?.OnNewHeaderGUI();
    }

    public override void OnInspectorGUI()
    {
        monoAsset?.OnInspectorGUI();
    }


    void OnDestroy()
    {
        if (IsLock())
        {
            return;
        }

        monoAsset?.OnDestroy();
        monoAsset = null;
    }
}
