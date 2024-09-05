
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class AssetCacheWindow : EditorWindow
{

    [System.Serializable]
    public class CacheInfo
    {
        public string path;
        public bool   isCache;
        public CacheMode mode;
    }

    [System.Serializable]
    public class CacheFile
    {
        public string path;
        public CacheMode mode;
    }

    [System.Serializable]
    public class CacheData
    {
        public List<string> txtFitters;
        public List<string> bytesFitters;

        public List<CacheFile> cacheFiles;
        public List<CacheInfo> cacheInfos;
    }

    public List<string> bytesFitters = new List<string>() { ".bytes",".dat"};
    public List<string> txtFitters   = new List<string>() { ".st",".json",".config",".txt" };
    public List<string> files        = new List<string>();
    static string cachePath = System.Environment.CurrentDirectory + "/ProjectSettings/Cache.dat";

    static CacheData _cacheData;
    public static CacheData cacheData
    {
        get
        {
            if (_cacheData == null)
            {
                if (!File.Exists(cachePath))
                {
                    _cacheData = new CacheData();
                    _cacheData.cacheInfos = new List<CacheInfo>();
                }
                else
                {
                    _cacheData = JsonUtility.FromJson<CacheData>(File.ReadAllText(cachePath));
                    if (_cacheData.cacheInfos == null)
                    {
                        _cacheData.cacheInfos = new List<CacheInfo>();
                    }
                }
            }
            return _cacheData;
        }
    }

    static EditorWindow creatWindow;

    SerializedObject   sObject;
    SerializedProperty cacha;
    SerializedProperty bytesProperty;
    SerializedProperty txtProperty;
    SerializedProperty filesProperty;

    [MenuItem("Cache/Assets Linker")]
    public static void ShowWindow()
    {
        creatWindow = EditorWindow.GetWindow(typeof(AssetCacheWindow),true, "Cache");
    }

    Color black = new Color(0.8f, 0.8f, 0.8f, 0.1f);
    Color blue  = new Color(0.1f, 0.1f, 0.8f, 0.5f);
    Color red   = new Color(0.8f, 0.1f, 0.1f, 0.5f);
    Color green = new Color(0.1f, 0.8f, 0.1f, 0.2f);


    private void OnEnable()
    {
        txtFitters   = cacheData.txtFitters;
        bytesFitters = cacheData.bytesFitters;

        if (txtFitters == null || txtFitters.Count == 0)
        {
            txtFitters = new List<string>() { ".st", ".json", ".config", ".txt" };
        }

        if (bytesFitters == null || bytesFitters.Count == 0)
        {
            bytesFitters = new List<string>() { ".bytes", ".dat" };
        }

        sObject = new SerializedObject(this);
        bytesProperty = sObject.FindProperty("bytesFitters");
        txtProperty   = sObject.FindProperty("txtFitters");
        filesProperty = sObject.FindProperty("files");
    }


    private void OnGUI()
    {
        sObject.UpdateIfRequiredOrScript();

        EditorGUILayout.PropertyField(txtProperty, true);
        EditorGUILayout.PropertyField(bytesProperty, true);
       
        sObject.ApplyModifiedProperties();

        EditorGUICustom.DrawTitleColor("Files", green, (_) =>
        {
            for (int i = cacheData.cacheFiles.Count - 1; i >= 0; i--)
            {
                var cache = cacheData.cacheFiles[i];

                GUILayout.BeginHorizontal();
                //绘制一个可以接受拖拽文件夹的框
                if (string.IsNullOrEmpty(cache.path))
                {
                    GUILayout.Label("请拖拽文件到这里");
                }
                else
                {
                    GUILayout.Label(cache.path);
                    EditorGUI.DrawRect(GUILayoutUtility.GetLastRect(), blue);
                    GUI.Label(GUILayoutUtility.GetLastRect(), cache.path);
                }

                cache.path = HandleDragAndDrop(GUILayoutUtility.GetLastRect(), cache.path);
                cache.mode = (CacheMode)EditorGUILayout.EnumPopup(cache.mode, GUILayout.Width(60));

                //删除按钮
                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    cacheData.cacheFiles.Remove(cache);
                }
                GUILayout.EndHorizontal();
            }
        });

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        //添加按钮
        if (GUILayout.Button("+", GUILayout.Width(60)))
        {
            cacheData.cacheFiles.Insert(0,new CacheFile());
        }
        GUILayout.EndHorizontal();

        //显示cacheInfos的内容
        EditorGUICustom.DrawTitleColor("Cache", green, (_) =>
        {
            for (int i = cacheData.cacheInfos.Count - 1; i >= 0; i--)
            {
                var caches = cacheData.cacheInfos[i];

                GUILayout.BeginHorizontal();
                caches.isCache = EditorGUILayout.Toggle(caches.isCache, GUILayout.Width(20));
                //绘制一个可以接受拖拽文件夹的框

                if (string.IsNullOrEmpty(cacheData.cacheInfos[i].path))
                {
                    GUILayout.Label("请拖拽文件夹到这里");
                }
                else
                {
                    GUILayout.Label(cacheData.cacheInfos[i].path);
                    EditorGUI.DrawRect(GUILayoutUtility.GetLastRect(), caches.isCache ? blue : red);
                    GUI.Label(GUILayoutUtility.GetLastRect(), caches.path);
                }

                caches.path = HandleDragAndDrop(GUILayoutUtility.GetLastRect(), caches, caches.path);

                caches.mode  = (CacheMode)EditorGUILayout.EnumPopup(caches.mode,GUILayout.Width(60));

          

                //删除按钮
                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    cacheData.cacheInfos.Remove(caches);
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            //添加按钮
            if (GUILayout.Button("+", GUILayout.Width(60)))
            {
                cacheData.cacheInfos.Insert(0,new CacheInfo() { isCache = true });
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            //保存按钮
            if (GUILayout.Button("保存", GUILayout.Width(100)))
            {
                cacheData.txtFitters   = txtFitters;
                cacheData.bytesFitters = bytesFitters;

                File.WriteAllText(cachePath, JsonUtility.ToJson(cacheData));
            }

            if (GUILayout.Button("更新配置表", GUILayout.Width(100)))
            {
                AssetCacheProsser.CreateLinker();
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        });
    }

    private CacheMode SetCacheMode(string path)
    {
        var fullName       = Path.Combine(System.Environment.CurrentDirectory, path);
        var directoryInfo  = new DirectoryInfo(fullName);
        var  files         = directoryInfo.GetFiles("*.*", SearchOption.AllDirectories);
        var  first         = files.FirstOrDefault().Name;

       var bytes = bytesFitters.Find(_ => first.EndsWith(_));

        if (bytes != null)
        {
            return CacheMode.Bytes;
        }

        var txt = txtFitters.Find(_ => first.EndsWith(_));

        if (txt != null)
        {
            return CacheMode.Txt;
        }

        return CacheMode.None;
    }

    private string HandleDragAndDrop(Rect dropArea, CacheInfo info, string inPath)
    {
        Event evt = Event.current;
        if (evt.type == EventType.DragUpdated || evt.type == EventType.DragPerform)
        {
            if (!dropArea.Contains(evt.mousePosition))
                return inPath;

            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

            if (evt.type == EventType.DragPerform)
            {
                DragAndDrop.AcceptDrag();

                foreach (var draggedObject in DragAndDrop.objectReferences)
                {
                    if (draggedObject is DefaultAsset)
                    {
                        string path = AssetDatabase.GetAssetPath(draggedObject);
                        if (AssetDatabase.IsValidFolder(path))
                        {
                            info.mode = SetCacheMode(path);
                            return inPath = path;
                        }
                    }
                }
            }
            evt.Use();
        }

        return inPath;
    }

    private string HandleDragAndDrop(Rect dropArea,  string inPath)
    {
        Event evt = Event.current;
        if (evt.type == EventType.DragUpdated || evt.type == EventType.DragPerform)
        {
            if (!dropArea.Contains(evt.mousePosition))
                return inPath;

            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

            if (evt.type == EventType.DragPerform)
            {
                DragAndDrop.AcceptDrag();

                foreach (var draggedObject in DragAndDrop.objectReferences)
                {
                    if (draggedObject is DefaultAsset)
                    {
                        string path = AssetDatabase.GetAssetPath(draggedObject);
                        if (AssetDatabase.IsValidFolder(path) == false)
                        {
                            return inPath = path;
                        }
                    }
                }
            }
            evt.Use();
        }

        return inPath;
    }
}


public class AssetCacheProsser : IPreprocessBuildWithReport
{
    public int callbackOrder => 50;

    public void OnPreprocessBuild(BuildReport report)
    {
        CreateLinker();
    }


    //[MenuItem("Cache/Create Linker")]
    //public static void EditorLinker()
    //{
    //    CreateLinker();
    //    AssetDatabase.Refresh();
    //}


    public static void CreateLinker()
    {
        //获取所有的文件夹下的文件
        List<string> files = new List<string>();
        Dictionary<string, CacheMode> caches = new Dictionary<string, CacheMode>();
        foreach (var item in AssetCacheWindow.cacheData.cacheInfos)
        {
            if (item.isCache)
            {
                var finds = Directory.GetFiles(item.path, "*.*", SearchOption.AllDirectories);

                foreach (var find in finds)
                {
                    caches[find] = item.mode;
                }

                files.AddRange(finds);
            }
        }

        for (int i = files.Count - 1; i >= 0; i--)
        {
            if (files[i].EndsWith(".meta"))
            {
                files.RemoveAt(i);
            }
        }


        //创建streamAssets下的linker文件
        var loadLinker   = new LoadLinker();
        loadLinker.lists = new List<LinkInfo>();
        foreach (var item in files)
        {
            var md5  = GetFileMD5(item);
            var path = item
                     . Replace("\\", "/")
                     . Replace("Assets/StreamingAssets/", "")
                     . Replace("\\", "/");

            loadLinker.lists.Add(new LinkInfo() { isCache = true, path = path, md5 = md5 ,mode = caches[item] });
        }

        //保存到文件
        File.WriteAllText(Application.streamingAssetsPath + "/AssetLinker.json", JsonUtility.ToJson(loadLinker));
    }


    public static string GetFileMD5(string filePath)
    {
        if (!File.Exists(filePath))
            return string.Empty;

        using (var md5 = MD5.Create())
        {
            using (var stream = File.OpenRead(filePath))
            {
                byte[] hash = md5.ComputeHash(stream);
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hash)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }
    }

}
