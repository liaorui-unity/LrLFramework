using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

static class SettingsUIElementsRegister
{
    static SerializedObject m_SerializedObject;
    static SerializedProperty data;

    // 注册自定义设置页面
    [SettingsProvider]
    private static SettingsProvider CreateProvider()
    {
        // 第二个参数决定显示在什么地方，如果是SettingsScope.Project, 就显示在Project Settings里面
        // 相应的，数据也需要存储在ProjectSettings文件夹里面
        // guiHandler表示使用IMGUI的展示函数， 官方也提供了UIElements版本的
        // 但是如果没有完整封装一套UIElements逻辑的话，写属性一类的编辑器UI还是用IMGUI更方便（基本不在乎效率）
        CheakConfig.Instance.Save();

        m_SerializedObject = new SerializedObject(CheakConfig.Instance);
        m_SerializedObject.Update(); // 更新SerializedObject中的数据

        data  = m_SerializedObject.FindProperty("data");

        return new SettingsProvider("FileEditorSettings", SettingsScope.Project)
        {
            label = "文件观察器",
            guiHandler = OnProviderGUI,
        };
    }



    // 自定义页面的UI函数
    private static void OnProviderGUI(string _)
    {
        m_SerializedObject.UpdateIfRequiredOrScript();
        EditorGUILayout.PropertyField(data);
        m_SerializedObject.ApplyModifiedProperties();

        GUILayout.Space(5);

        EditorGUILayout.BeginHorizontal();


        if (GUILayout.Button("启动"))
        {
            CSharpFileWatcher.PlayWatcher();
        }

        if (GUILayout.Button("重启"))
        {
            CSharpFileWatcher.Release();
            CSharpFileWatcher.PlayWatcher();
        }

        if (GUILayout.Button("停止"))
        {
            CSharpFileWatcher.Release();
        }

        EditorGUILayout.EndHorizontal();

        if (GUI.changed)
        {
            CheakConfig.Instance.Save();
        }
    }
}
