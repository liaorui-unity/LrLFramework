using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;
using System.Linq;
using System.Reflection;

[CustomEditor(typeof(MonoSerialization))]
public class MonoSerializeEditor :Editor
{
    MonoSerialization mono => target as MonoSerialization;

    JSONObject jObject;
    string builder;
    string[] components;
    List<string> contents = new List<string>();

    int index;
    int lastIndex;


    void OnEnable()
    {
        components = mono.GetComponents<Component>().Select(_ => _.ToString()).ToArray();
        if (mono.component) Reset();
    }


    public void Reset()
    {
        contents.Clear();
        jObject = new JSONObject(mono.content);

        if (components == null)
        {
            lastIndex = index = 0;
            return;
        }
        else
        {
            lastIndex = index = components.ToList().FindIndex(_ => _ == mono.component.ToString());
        }


        var infos = mono.GetFieldInfos();

        foreach (var item in infos)
        {
            if (item == null) continue;
            contents.Add($"{item.Name}  =>  {jObject[item.Name]}");
        }

        builder = string.Join("\n", contents);
    }


    public override void OnInspectorGUI()
    {
        EditorGUILayout.HelpBox("只序列化 Serialize 特性的字段", MessageType.Info);

        GUI.enabled = false;

        if (string.IsNullOrEmpty(builder) == false)
            GUILayout.TextArea(builder);
        GUI.enabled = true;

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.Box("Component", GUILayout.Width(100));
        index = EditorGUILayout.Popup(index, components);

        if (index != lastIndex)
        { 
            mono.component = mono.GetComponents<Component>()[index];
            Reset();
        }
        GUILayout.EndHorizontal();


  

        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Serialize"))
        {
            mono.Serialize();

            Undo.RecordObject(mono.transform, "Change Object");
            // 更改物体位置属性
            mono.transform.position += new Vector3(0, 0, 0.00001f);
        }
        if (GUILayout.Button("DeSerialize"))
        {
            mono.DeSerialize();

            Undo.RecordObject(mono.transform, "Change Object");
            // 更改物体位置属性
            mono.transform.position += new Vector3(0, 0, -0.00001f);
        }
        GUILayout.EndHorizontal();
    }
}
