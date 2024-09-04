using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Linq;

public class CSharpFileWatcher
{
    private static FileSystemWatcher fileWatcher;
    private static List<string>       m_CSharpFiles = new List<string>();
    private static List<ICSharpFlie>  m_CSharpClass = new List<ICSharpFlie>();
    public static UnityAction<string ,string > ChangeFunc;

    private static double runtime = 0.2f;


    [InitializeOnLoadMethod]
    public static void PlayWatcher()
    {
        if (CheakConfig.Instance.data.IsInitRuning)
        {
            FindBaseICSharpClass();

            // 初始化 FileSystemWatcher
            fileWatcher = new FileSystemWatcher();

            // 设置要监视的目录
            fileWatcher.Path = Application.dataPath; // 替换为你要监控的目录路径

            // 设置要监视的更改类型
            fileWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;

            // 设置监视所有文件类型
            fileWatcher.Filter = "*.cs";

            // 添加事件处理程序
            fileWatcher.Changed += OnChanged;
            fileWatcher.Renamed += OnRenamed;

            // 开始监视
            fileWatcher.EnableRaisingEvents = true;
            fileWatcher.IncludeSubdirectories = true;


            EditorApplication.update += Update;

            runtime = EditorApplication.timeSinceStartup;
            CheakConfig.Instance.data.isRuning = true;
        }
    }


    private static void FindBaseICSharpClass()
    {
        m_CSharpClass = AppDomain.CurrentDomain.GetAssemblies()
                                    .SelectMany(_ => _.GetTypes())
                                    .Where(_ => _.GetInterface(nameof(ICSharpFlie)) != null)
                                    .Select(_ => Activator.CreateInstance(_) as ICSharpFlie)
                                    .ToList();
    }

    static void Update()
    {
        if (EditorApplication.timeSinceStartup - runtime < 0.2f)
            return;

        runtime = EditorApplication.timeSinceStartup;
     
        if (m_CSharpFiles.Count > 0)
        { 
            foreach (var item in m_CSharpFiles)
            {
                try
                {
                    if (File.Exists(item))
                    {
                        var text = File.ReadAllText(item);

                        foreach (ICSharpFlie type in m_CSharpClass)
                        {
                            type.OnChangeFile(item, text);
                        }
                        ChangeFunc?.Invoke(item, text);
                    }
                }
                catch (System.Exception ex)
                {
                    return;
                }
            }
            m_CSharpFiles.Clear();
        }
    }

    private static void OnChanged(object source, FileSystemEventArgs e)
    {
        if (CheakConfig.Instance.data.IsWriteLog)
            Debug.Log($"File: {e.FullPath} {e.ChangeType}");

        if (e.FullPath.EndsWith(".cs") && m_CSharpFiles.Contains(e.FullPath) == false)
        {
            m_CSharpFiles.Add(e.FullPath);
        }
    }

    private static void OnRenamed(object source, RenamedEventArgs e)
    {
        if (CheakConfig.Instance.data.IsWriteLog)
            Debug.Log($"File renamed: {e.OldFullPath} to {e.FullPath}");

        if (e.FullPath.EndsWith(".cs")&&m_CSharpFiles.Contains(e.FullPath)==false)
        {
            m_CSharpFiles.Add(e.FullPath);
        }
    }

    public static void Release()
    {
        // 停止监视并释放资源
        if (fileWatcher != null)
        {
            fileWatcher.EnableRaisingEvents = false;
            fileWatcher.Dispose();
            EditorApplication.update -= Update;
        }

        if (m_CSharpClass != null)
        { 
            m_CSharpClass.Clear();
        }

        ChangeFunc = null;

        CheakConfig.Instance.data.isRuning = false;
    }
}