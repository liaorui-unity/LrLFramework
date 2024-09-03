using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;


/// <summary>
/// 记录所有资源MD5信息
/// </summary>
public class MD5Info
{
    public string AssetPath;
    public string MD5;
    public long  Size;
}

public class AddressableBuildMD5 
{
    static Dictionary<string, MD5Info> oldFileInfos  = new Dictionary<string, MD5Info>();
    static Dictionary<string, MD5Info> newFileInfos  = new Dictionary<string, MD5Info>();


    public static void CopyNeedFiles2BuildAssets(string buildAssets)
    {
        foreach (var newItem in newFileInfos)
        {
            if (oldFileInfos.TryGetValue(newItem.Key, out MD5Info md5))
            {
                Debug.Log($"比较{newItem.Key} =>{newItem.Value.MD5 == md5.MD5}");
                if (newItem.Value.MD5 == md5.MD5)
                    continue;
            }

            oldFileInfos[newItem.Key] = newItem.Value;
            AddressableBuildTools.CopyFile(newItem.Value.AssetPath, buildAssets);
        }

        long sum = 0;
        StringBuilder builder = new StringBuilder();
        foreach (var item in oldFileInfos.Values)
        {
           var line = $"{Path.GetFileName(item.AssetPath)}*{item.MD5}*{item.Size}";
           sum += item.Size;
           builder.AppendLine(line);
        }
        builder.AppendLine(sum.ToString());
        File.WriteAllText($"{buildAssets}/AssetData.json", builder.ToString());
    }

    public static void FindOldMD5(string buildPath)
    {
        oldFileInfos.Clear();
        string path = $"{AddressableBuildTools.AppDataPath}/{buildPath}";

        Debug.Log("比较地址：" + path );

        if (Directory.Exists(path))
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            FindMD5Files(dir, oldFileInfos);
        }
    }

    public static void FindNewMD5(string buildPath)
    {
        newFileInfos.Clear();
        string path = $"{AddressableBuildTools.AppDataPath}/{buildPath}";
        if (Directory.Exists(path))
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            FindMD5Files(dir, newFileInfos);
        }
    }

    private static void FindMD5Files(DirectoryInfo info,Dictionary<string ,MD5Info> mdInfos)
    {
        Debug.Log("Md5:" + info.FullName );
        FileInfo[] files = info.GetFiles();
        foreach (var file in files)
        {
            if (file == null)
            {
                continue;
            }
            if (file.Extension == ".meta")
            {
                continue;
            }

            MD5Info md5 = new MD5Info();
            md5.MD5       = AddressableBuildTools.CalculateMD5(file.FullName, out md5.Size);
            md5.AssetPath = file.FullName.Replace(@"\", "/");
            mdInfos.Add(md5.AssetPath, md5);
        }

        DirectoryInfo[] dirs = info.GetDirectories();
        foreach (var dir in dirs)
        {
            FindMD5Files(dir, mdInfos);
        }
    }

}
