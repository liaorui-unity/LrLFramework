using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

public class Labels : PopupWindowContent
{

    [System.Serializable]
    public class Folded
    {
        public string label;
        public int labelCount;
        public bool isShow;
        public bool isFolded;
        public bool isRename;
    }

    [System.Serializable]
    public class FoldData : ScriptableObject
    {
        public List<Folded> infos = new List<Folded>();
    }

    internal List<Folded> foldeds
    {
        get
        {
            if (foldoutDat == null)
            {
                if (File.Exists(path))
                {
                    foldoutDat = AssetDatabase.LoadAssetAtPath<FoldData>(SecondHierarchy.path);
                    if ( foldoutDat.infos == null)
                    {
                         foldoutDat.infos = new List<Folded>();
                         foldoutDat.infos.Add(new Folded() { label = "Rename", isFolded = false });
                    }
                }
                else
                {
                    foldoutDat = ScriptableObject.CreateInstance<FoldData>();
                    AssetDatabase.CreateAsset(foldoutDat, SecondHierarchy.path);
                    foldoutDat.infos.Add(new Folded() { label = "Rename", isFolded = false });
                }
            }
            return foldoutDat.infos;
        }
    }

    internal string path = $"{System.Environment.CurrentDirectory}/{SecondHierarchy.path}";

    Dictionary<Folded, Rect> renameRects = new Dictionary<Folded, Rect>();

    FoldData foldoutDat;

    public override  void OnOpen()
    {
        AssetDatabase.SaveAssetIfDirty(foldoutDat);
    }
    public override void OnClose()
    {
        foreach (var item in foldoutDat.infos)
        {
            item.isRename = false;
        }

        AssetDatabase.ClearLabels(foldoutDat);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public override Vector2 GetWindowSize()
    {
        return new Vector2(200, foldeds.Count * 20 + 40);
    }


    public override void OnGUI(Rect rect)
    {
        if (foldoutDat.infos == null || foldoutDat.infos.Count == 0)
        {
            EditorGUILayout.LabelField("No available labels found.");
            return;
        }

        EditorGUILayout.BeginVertical("box");
        foreach (var item in foldoutDat.infos)
        {
            EditorGUILayout.BeginHorizontal();
            if (item.isRename)
            {
                item.label = EditorGUILayout.TextField(item.label);

                if (Event.current.isKey && Event.current.keyCode == KeyCode.Return)
                {
                    item.isRename = false;
                    Event.current.Use();
                }
            }
            else
            {
                item.isShow  = GUILayout.Toggle(item.isShow, item.label);
            }

         
            var currentEvent = Event.current;
            if (currentEvent.type == EventType.Repaint)
            {
                renameRects[item] = GUILayoutUtility.GetLastRect();
            }

            if (renameRects.ContainsKey(item))
            {
                var renameRect = renameRects[item];

                if (renameRect.Contains(currentEvent.mousePosition))
                {
                    if (currentEvent.keyCode == KeyCode.F2)
                    {
                        item.isRename = true;
                        currentEvent.Use();
                    }
                }
            }

            if (GUILayout.Button("x", GUILayout.Width(20)))
            {
                foldoutDat.infos.Remove(item);
                return;
            }

            EditorGUILayout.EndHorizontal();
         
        }
        EditorGUILayout.EndVertical();

        if (GUILayout.Button("add lables"))
        {
            foldoutDat.infos.Add(new Folded() { label = "Rename", isFolded = false }); 
            AssetDatabase.SetLabels(foldoutDat, new string[] { "Rename" });
        }
    }
}

public class Lately
{ 

    public Queue<PrefixGo> clickObjects = new Queue<PrefixGo>();

    public class PrefixGo
    {
        public string path;
        public Object go;

        public override bool Equals(object obj)
        {
            return ((PrefixGo)obj).go == go;
        }

        public override int GetHashCode()
        {
            return go.GetHashCode();
        }
    }

    public void AddClickObject(string path, Object go)
    {
        if (clickObjects.Count > 10)
        {
            clickObjects.Dequeue();
        }

        if (clickObjects.Contains(new PrefixGo() { path = path, go = go }))
        { 
            return;
        }

        clickObjects.Enqueue(new PrefixGo() { path = path, go = go });

    }
}

public class SecondHierarchy : EditorWindow
{
    public const string path = "Assets/Editor/Labels.asset";

    [MenuItem("Tools/Second Hierarchy")]
    public static void StartWindow()
    {
        //显示SecondHierarchy窗口
        GetWindow<SecondHierarchy>("Second Hierarchy");    
    }

    Labels labels;
    Lately Lately = new Lately();

    void OnEnable()
    {
        labels = new Labels();
        Lately = new Lately();
        //Hierarchy 上的点击事件
        Selection.selectionChanged += () =>
         {
             if (Selection.activeObject != null)
             {
                 Lately.AddClickObject(AssetDatabase.GetAssetPath(Selection.activeObject), Selection.activeObject);
             }
         };
    }

    void OnGUI()
    {
        // 显示标签Pop多选框
        if (GUILayout.Button("Select Labels"))
        {
            //鼠标的位置，显示一个窗口
            PopupWindow.Show(new Rect(Event.current.mousePosition, Vector2.zero), labels);
        }

        if (labels.foldeds.Count == 0)
        {
            EditorGUILayout.LabelField("No labels selected.");
            return;
        }

        EditorGUILayout.BeginVertical();

        foreach (var item in labels.foldeds)
        {
            if (item.isShow)
            {
                var rect = EditorGUILayout.BeginVertical("box");
                EditorGUICustom.FoldedBox($"{item.label}     size: {item.labelCount}", ref item.isFolded, (_) =>
                 {
                     item.labelCount = GetLabelAssets(item.label);
                 });
                EditorGUILayout.EndVertical();

                var draggable = EditorGUICustom.DragObject(rect);
                if (draggable)
                {
                    AssetDatabase.SetLabels(draggable, new string[] { item.label });
                }
            }
        }
        EditorGUILayout.EndVertical();

        GUILayout.FlexibleSpace();

        if (Lately.clickObjects.Count > 0)
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Box("Lately Clicked Prefabs");

            foreach (var item in Lately.clickObjects)
            {
                EditorGUILayout.ObjectField(item.go, typeof(Object), false);
            }
            EditorGUILayout.EndVertical();
        }

        Repaint();
    }

    int GetLabelAssets(string label)
    { 
        //获取标签的资源
        string[] guids = AssetDatabase.FindAssets("l:" + label);
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);

            EditorGUILayout.BeginHorizontal();
           //在编辑页面显示资源
            EditorGUILayout.ObjectField(AssetDatabase.LoadAssetAtPath<Object>(path), typeof(Object), false);

            if (GUILayout.Button("x",GUILayout.Width(30)))
            {
                //删除当前资源的标签
                AssetDatabase.SetLabels(AssetDatabase.LoadAssetAtPath<Object>(path), new string[] { });
            }

            EditorGUILayout.EndHorizontal();
        }

        return guids.Length;
    }
}
