using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Text;
using System.Linq;
using System;
using UnityEditor.Callbacks;
using System.Reflection;
using SIO = StreamableIO;
using System.Threading.Tasks;
using UnityEditorInternal;

public class CreatStreamEditor : Editor
{
    static StringBuilder builder = new StringBuilder();

    static string origin =
    @"using UnityEngine;
     public class StreamSerialized : ScriptableObject
     {
        //Add fields here
     }";

    public static string className = "StreamSerialized";

    static string savePath  = $"Assets/Scripts/Auto/Engine/{className}.cs";


    [MenuItem("Tools/StreamSerialized")]
    public static async void InitEnable()
    {
        await LoadAsyncPakeage();
        Debug.Log("StreamSerialized create done！");
    }
    public static async Task LoadAsyncPakeage()
    {
        var findTypes = SIO.FindAllTypes();

        foreach (var item in findTypes)
        {
            var pakeage = new StreamablePakeage();
            var jsonStr = SIO.ReadFiles(item.Name);

            if (string.IsNullOrEmpty(jsonStr))
            {
                pakeage.item = Activator.CreateInstance(item) as StreamableObject;
                SIO.WriteFiles(item.Name, SIO.ToJson(pakeage.item, item));
            }
        }
        AssetDatabase.Refresh();
    }


    [DidReloadScripts]
    public static void OnScriptsReloaded()
    {
        //查找所有程序集内所有继承自StreamableObject的类
        var types = SIO.FindAllTypes();

        if (types.Count <= 0)
        {
            return;
        }

        var hasFiles =  GetPathFileNames();
        var notFiles = new List<string>();

        foreach (var type in types) 
        {
            if (hasFiles.Contains(type.Name) == false)
            {
                notFiles.Add(type.Name);
            }
        }


        if (notFiles.Count <= 0)
            return;

        builder.Clear();
        foreach (var item in types)
        {
            builder.AppendLine($"  public {item} {item};");
        }
        builder.AppendLine("//Add fields here");

        origin = origin.Replace("//Add fields here", builder.ToString());

        foreach (var item in notFiles)
        {
            Save(item);
        }

        if (!Directory.Exists(Path.GetDirectoryName(savePath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(savePath));
        }

        if (File.Exists(savePath))
        {
            File.Delete(savePath);
        }

        File.WriteAllText(savePath, origin);

        AssetDatabase.ImportAsset(savePath);

        AssetDatabase.Refresh();
    }

    public static List<string> GetPathFileNames()
    {
        var path = SIO.JsonPath();

        if (Directory.Exists(path) == false)
        { 
           Directory.CreateDirectory(path);
        }

        var files = Directory.GetFiles(path, $"*{Streamable.suffix}", SearchOption.AllDirectories);
        
        return files?.Select(x => Path.GetFileNameWithoutExtension(x)).ToList();

    }

    static void Save(string name)
    {
        var serializedPackage = SIO.FindPackageType(name);

        var value  = Activator.CreateInstance(serializedPackage);

        var jsonStr = SIO.ToJson(value, serializedPackage);

        SIO.WriteFiles(name, jsonStr);
    }

}

public class StInstance
{
    public SerializedObject serialized;

    public SerializedProperty sdProperty;

    public ScriptableObject scriptable;

    public Type originType;

    public FieldInfo field;

    public string property;

    public string path;

    public void FindProperty(string path)
    {
        this.path = path;

        var txt    = JsonStr();
        var value  = SIO.FromJson(txt);


        this.property = value.GetType().ToString();
        var rootType  = SIO.FindPackageType(CreatStreamEditor.className);
        scriptable    = ScriptableObject.CreateInstance(rootType);

        if (string.IsNullOrEmpty(txt) == false)
        {
            //获取 scriptable 的fields 然后赋值
            field = scriptable.GetType().GetField(property);

            if(field != null)
            {
               field.SetValue(scriptable, value);
            }
        }

        serialized = new SerializedObject(scriptable);
        sdProperty = serialized.FindProperty(property);
        originType = StreamableIO.FindPackageType(sdProperty.type);
    }

    public void Save()
    {
        File.WriteAllText(path, SIO.ToJson( field.GetValue(scriptable),field.FieldType));
    }

    string JsonStr()
    {  
        return File.ReadAllText(path);
    }

    public void Clear()
    {
        scriptable = null;
        serialized = null;
        sdProperty = null;
    }
}

