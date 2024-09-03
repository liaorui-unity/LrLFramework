using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System.Reflection;
using System.IO.Pipes;
using System.IO;
using System;
using System.Linq;
using UnityEngine.Events;
using System.Runtime.InteropServices;
using System.Text;

public interface IWatcher
{
    void OnChangeFile(string[] files);
}

public class ShellExe
{
    public string _root => Environment.CurrentDirectory;
    private string _fd  = "Watcher";
    private string _exeName = "CSharpLoad";
    private string _Folder  => $"{_root}/{_fd}";
    private string _path    => $"{_Folder}/{_exeName}.exe";
    private string _outFd   => $"{_Folder}/Temp_out";
    private string _inputFd => $"{_Folder}/Temp_input";

    private bool isRuning = false;
    private Dictionary<Type, MethodInfo> baseChanges = new Dictionary<Type, MethodInfo>();


    public ShellExe(string folder,string exeName)
    {
        _fd      = folder;
        _exeName = exeName;

        AppDomain.CurrentDomain.GetAssemblies()
        .SelectMany(_ => _.GetTypes())
        .Where(_ => _.GetInterfaces().Contains(typeof(IWatcher)))
        .ToList()
        .ForEach(_ =>
        {
            baseChanges[_] = _.GetMethod(nameof(IWatcher.OnChangeFile), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        });

    }

    public int _staticID
    {
        get { return PlayerPrefs.GetInt(this.name, UnityEngine.Random.Range(0, 50000)); }
        set { PlayerPrefs.SetInt(this.name, value); }
    }

    string name => $"{_exeName}_{Application.productName}";

    public void StartProcess()
    {
        if (CheckEditor.IsProcessRunning(_staticID))
        {
            var process = CheckEditor.FindProcess(_staticID);
            if (process.HasExited)
            {
                process.Dispose();
                process.Close();
                _staticID = -1;
            }
            else
            {
                return;
            }
        }

        if (File.Exists(_path))
        {
            Process process = new Process();
            process.StartInfo.FileName = _path;
            process.StartInfo.Arguments = $"{this.name} {System.Environment.CurrentDirectory} {_fd}";
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.UseShellExecute = true;
            process.Start();
            _staticID = process.Id;

            CheckEditor.SetConfig(name, _staticID);
            isRuning = true;
        }
    }

    public void OnQuitting()
    {
        UnityEngine.Debug.Log("Unity项目已关闭");
        var find = CheckEditor.FindProcess(_staticID);
        if (find != null)
        {
            find.Kill();
            _staticID = -1;
        }
    }
    public void CheckFile()
    {
        if (isRuning == false)
            return;

        var files = Directory.GetFiles(_outFd);

        if (files.Length > 0)
        {
            foreach (var item in files)
            {
                var changeFiles = File.ReadAllLines(item);
                try
                {
                    if (CheakConfig.Instance.data.IsWriteLog)
                    {
                        UnityEngine.Debug.Log("Watcher cs:" + changeFiles);
                    }


                    if (baseChanges.Count > 0)
                    {
                        foreach (var baseClass in baseChanges)
                        {
                            var instance = Activator.CreateInstance(baseClass.Key);
                            baseClass.Value.Invoke(instance, new object[] { changeFiles });
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    UnityEngine.Debug.LogError("Wather cs error:" + ex);
                }
                File.Delete(item);
            }
        }
    }
}


public class CheckEditor
{
    // 定义P/Invoke函数
    [DllImport("user32.dll", SetLastError = true)]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int GetWindowTextLength(IntPtr hWnd);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool IsWindowVisible(IntPtr hWnd);

    public static string  saveFolder => "Watcher";
    private static string exeName    => "CSharpLoad";
    private static object loaker = new object();

    internal static Process FindProcess(int _staticID)
    {
        if (_staticID == -1)
            return null;

        try
        {
            var newProcesses = Process.GetProcessById(_staticID);

            if (newProcesses != null)
            {
                if (newProcesses.ProcessName.Contains(exeName) == false)
                {
                    return null;
                }
            }
            return newProcesses;
        }
        catch (System.Exception)
        {
            return null;
        }
    }
    internal static bool IsProcessRunning(int _staticID)
    {
        var process = FindProcess(_staticID);
        return process != null;
    }
    internal static void StartProcess()
    {
        lock (loaker)
        {
            foreach (var item in shellExes.Values)
            {
                item.StartProcess();
            }
        }
    }
    internal static void ExitProcess()
    {
        foreach (var item in shellExes.Values)
        {
            item.OnQuitting();
        }
        EditorApplication.update   -= OnUpdate;
        EditorApplication.quitting -= ExitProcess;
    }


    public static void SetConfig(string name,int id)
    {
        CheakConfig.Instance.data.runingProgressName = name;
        CheakConfig.Instance.data.runingProgressId   = id;
        CheakConfig.Instance.data.isRuning           = true;
    }

    public static Dictionary<string,ShellExe> shellExes = new Dictionary<string, ShellExe>();
  

    [InitializeOnLoadMethod]
    public static void Init()
    {
        if (CheakConfig.Instance.data.IsInitRuning == false)
        {
            return;
        }
        Start();
    }

    internal static void Start()
    {
        EditorApplication.update   += OnUpdate;
        EditorApplication.quitting += ExitProcess;

        shellExes.Clear();
        shellExes[saveFolder] = new ShellExe(saveFolder, exeName);

        StartProcess();
    }

    internal static void Restart()
    {
        ExitProcess();
        Start();
    }

    static double cycle   = 0.2f;
    static double runTIme = 0;
    static void OnUpdate()
    {
        if (EditorApplication.timeSinceStartup - runTIme > cycle)
        {
            runTIme = EditorApplication.timeSinceStartup;
        }
        else
        {
            return;
        }

        foreach (var item in shellExes.Values)
        {
            item.CheckFile();
        }
    }
}