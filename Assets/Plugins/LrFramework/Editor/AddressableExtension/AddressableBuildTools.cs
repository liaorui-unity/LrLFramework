using UnityEngine;
using System.IO;
using System;
using System.Text;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
#endif
using System.Reflection;


public static class AddressableBuildTools
{
    /// <remarks>目标平台</remarks>
    public static string Platform
    {
        get
        {
            string platform = string.Empty;
            switch (EditorUserBuildSettings.activeBuildTarget)
            {
                case BuildTarget.iOS: platform = "Ios"; break;
                case BuildTarget.Android: platform = "Android"; break;
                case BuildTarget.StandaloneWindows64: platform = "StandaloneWindows64"; break;
                case BuildTarget.StandaloneWindows: platform = "Windows"; break;
                default: platform = "AssetBundles"; break;
            }
            return platform;
        }
    }

    public const string GroupTag = "(Group)";
    public const string LabelTag = "(Label)";

    public const string addressAssets = "AddressableAssets";
    public static string AddressableAssetsPath => $"{Application.dataPath}/{addressAssets}";
    public static string ShortAddressableAssetsPath => "Assets/AddressableAssets/Asset";
    public static string ShortAddressableDataPath => "Assets/AddressableAssetsData";
    public static string schemasPath => $"{ShortAddressableDataPath}/AssetGroups/Schemas";
    public static string SettingPath => $"{ShortAddressableDataPath}/AddressableAssetSettings.asset";
    public static string PackagePath => $"{AppDataPath}/BuildAssets/Packages/{AddressableBuildTools.Platform}";
    public static string AssetPath   =>  $"{AppDataPath}/BuildAssets/Assets/{AddressableBuildTools.Platform}";
    public static string LibPath     => $"{Environment.CurrentDirectory}/Library/com.unity.addressables";

    public static string AppDataPath => Environment.CurrentDirectory;

    public static string AddressablesStatebin => "addressables_content_state.bin";

    public static List<string> filterGroups = new List<string> { "Default Local Group" , "Built In Data" };


    public static string AppBuildAddressableData = "BuildAddressableData";

    public const string LocalTag   = "Local_";
    public const string RemotedTag = "Remoted_";
    public const string UpdateTag  = "UpdateGroup_";


    static AddressableAssetSettings _assetSettings;
    public static AddressableAssetSettings assetSettings => _assetSettings = _assetSettings ?? AssetDatabase.LoadAssetAtPath<AddressableAssetSettings>(SettingPath);

    private static string _outPath;

    /// <remarks>输出路径</remarks>
    public static string OutPath 
    {
        get
        {
            if (string.IsNullOrEmpty(_outPath))
            {
                _outPath = GetBuildOutPath();
            }
            return _outPath;
        }
        set
        {
            _outPath = string.Empty;
        }
    }


    private static string GetBuildOutPath()
    {
        int index = Application.dataPath.LastIndexOf("/");
        var time  = DateTime.Now.ToString("yyyy_MM_dd_HH_mm");
        var setting = AssetDatabase.LoadAssetAtPath<AddressableAssetSettings>(SettingPath);

        Debug.Log(setting);

        string environment = setting.profileSettings.GetProfileName(setting.activeProfileId);
        if (AddressableBuildTools.Platform == "Android")
        {
            return $"{PackagePath}/{environment}/{Application.version}/{environment}_{time}.apk";
        }
        else if (AddressableBuildTools.Platform == "Ios")
        {
            return $"{PackagePath}/{environment}";
        }
        return   $"{PackagePath}/{environment}/{Application.version}/{environment}_{time}";
    }

    /// <remarks>计算MD5</remarks>
    public static string CalculateMD5(string file,out long size)
    {
        string result = "";
        long fileSize=0;
        FileStream fs = new FileStream(file, FileMode.Open);
        try
        {
            fileSize = fs.Length;
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(fs);
            fs.Close();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            result = sb.ToString();
    
        }
        catch (Exception e)
        {
            UnityEngine.Debug.Log("md5file() fail, error:" + e.Message);
        }
        finally
        {
            size = fileSize;
            fs.Close();
        }
        
        return result;
    }

    /// <remarks>文件复制</remarks>
    public static void CopyFile(string filePath, string targetPath)
    {
        FileInfo file = new FileInfo(filePath);
        if (!Directory.Exists(targetPath))
        {
            Directory.CreateDirectory(targetPath);
        }
        if (file != null)
        {
            string tempPath = Path.Combine(targetPath, file.Name);
            file.CopyTo(tempPath, true);//如果文件存在则覆盖
        }
    }

    /// <remarks>文件夹复制</remarks>
    public static void CopyDirectory(string sourcePath, string targetPath, bool containSubDirs)
    {
        DirectoryInfo dir = new DirectoryInfo(sourcePath);
        if (!dir.Exists)
        {
            return;
        }
        DirectoryInfo[] dirs = dir.GetDirectories();
        if (!Directory.Exists(targetPath))
        {
            Directory.CreateDirectory(targetPath);
        }
        FileInfo[] files = dir.GetFiles();
        foreach (FileInfo file in files)
        {
            string tempPath = Path.Combine(targetPath, file.Name);
            file.CopyTo(tempPath, true);//如果文件存在则覆盖
        }
        if (containSubDirs)
        {
            foreach (var subDir in dirs)
            {
                string tempPath = Path.Combine(targetPath, subDir.Name);
                CopyDirectory(subDir.FullName, tempPath, containSubDirs);
            }
        }
    }

    /// <remarks>删除指定的文件夹</remarks>
    public static void DeleteFolder(string path)
    {
        DirectoryInfo dir = new DirectoryInfo(path);
        if (dir.Exists)
        {
            FileInfo[] files = dir.GetFiles();
            for (int i = 0; i < files.Length; i++)
            {
                files[i].Delete();
                files[i] = null;
            }
            files = null;
            var dirs = dir.GetDirectories();
            for (int i = 0; i < dirs.Length; i++)
            {
                Directory.Delete(dirs[i].FullName, true);
            }
        }
    }

    public static void ClearConsole()
    {
#if UNITY_EDITOR
        Assembly assembly = Assembly.GetAssembly(typeof(SceneView));
        System.Type logEntries = assembly.GetType("UnityEditor.LogEntries");
        var method = logEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
        if (method != null)
        {
            method.Invoke(null, null);
            GUIUtility.keyboardControl = 0;
        }
        else
        {
            Debug.LogWarning("Can't find clear method");
        }
        AssetDatabase.Refresh();
#endif
    }
}