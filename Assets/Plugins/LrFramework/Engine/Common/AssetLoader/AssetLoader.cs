using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using LogInfo;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class LoadLinker
{
    public List<LinkInfo> lists;

    public Dictionary<string, LinkInfo> dicts = new Dictionary<string, LinkInfo>();
}

[System.Serializable]
public struct LinkInfo
{
    public bool   isCache;
    public CacheMode mode;
    public string path;
    public string md5;
}

public enum CacheMode
{ 
   None,
   Bytes,
   Txt
}


public class AssetLoader : Singleton<AssetLoader>
{
    const string linkName    = "AssetLinker";
    static string linkerDest = $"{Application.persistentDataPath}/{linkName}";
    static string linkerName = $"{linkName}.json";

    public LoadLinker loadLinker;

    string ToStreamPath(string path)
    {
        return $"{Application.streamingAssetsPath}/{path}";
    }

    string ToPersistent(string path)
    {
        return $"{linkerDest}/{path}";
    }


    public async Task Init()
    {
        if (!Directory.Exists(linkerDest))
        {
            Directory.CreateDirectory(linkerDest);
        }

        Debug.Log($"linker缓存文件夹:{linkerDest}");

        //读取本地的配置文件
        string selfPath  = ToStreamPath(linkerName);
        string cachePath = ToPersistent(linkerName);

        LoadLinker selfLinker = null;

#if UNITY_ANDROID && !UNITY_EDITOR
        UnityWebRequest request = UnityWebRequest.Get(selfPath); 
        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            selfLinker = JsonUtility.FromJson<LoadLinker>(request.downloadHandler.text);
        }
#else
        if (File.Exists(selfPath))
        {
            selfLinker = JsonUtility.FromJson<LoadLinker>(File.ReadAllText(selfPath));
        }
#endif
        //读取cache的配置文件
        if (File.Exists(cachePath))
        {
            Debug.Log("读取cache的配置文件:"+ cachePath);

            loadLinker = JsonUtility.FromJson<LoadLinker>(File.ReadAllText(cachePath));
        }
        else
        {
            loadLinker = new LoadLinker();
            loadLinker.lists = new List<LinkInfo>();
        }

        if (selfLinker == null)
        {
            Debug.LogError("AssetLinker.json not found");
            return;
        }
  

        foreach (var info in selfLinker.lists)
        {
            selfLinker.dicts[info.path] = info;
        }

        foreach (var info in loadLinker.lists)
        {
            loadLinker.dicts[info.path] = info;
        }

        Dictionary<string, LinkInfo> selfDict  = selfLinker.dicts;
        Dictionary<string, LinkInfo> cacheDict = loadLinker.dicts;

        List<LinkInfo> updates = new List<LinkInfo>();
        List<LinkInfo> removes = new List<LinkInfo>();

        // 比对新增和更新的资源
        foreach (var kvp in selfDict)
        {
            if (cacheDict.TryGetValue(kvp.Key, out var cachedInfo))
            {
                if (kvp.Value.md5 != cachedInfo.md5)
                {
                    Debug.Log($"linker updated: {kvp.Key} (Old MD5: {cachedInfo.md5}, New MD5: {kvp.Value.md5})");
                    updates.Add(kvp.Value);
                }
            }
            else
            {
                updates.Add(kvp.Value);
                Debug.Log($"New linker found: {kvp.Key}");
            }
        }

        // 比对删除的资源
        foreach (var kvp in cacheDict)
        {
            if (!selfDict.ContainsKey(kvp.Key))
            {
                removes.Add(kvp.Value);
                Debug.Log($"linker removed: {kvp.Key}");
            }
        }

        if (updates.Count > 0 || removes.Count > 0)
        {
            // 更新资源
            foreach (var info in updates)
            {
                if (info.isCache)
                {
                    await Stream2Cache(info);
                    loadLinker.dicts.Add(info.path, info);
                }
            }

            // 删除资源
            foreach (var info in removes)
            {
                string removePath = ToPersistent(info.path);

                if (File.Exists(removePath))
                {
                    File.Delete(removePath);
                }
                loadLinker.dicts.Remove(info.path);
            }

            loadLinker.lists = new List<LinkInfo>(loadLinker.dicts.Values);

            Info.Log("更新资源完成");
            Info.Log("cachePath:"+ cachePath);
            Info.Log("loadLinker:" + JsonUtility.ToJson(loadLinker));
            // 更新配置文件
            File.WriteAllText(cachePath, JsonUtility.ToJson(loadLinker));
        }
    }


    async Task Stream2Cache(LinkInfo info)
    {
        string selfPath  = ToStreamPath(info.path);
        string cachePath = ToPersistent(info.path);


        UnityWebRequest request = UnityWebRequest.Get(selfPath);
        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError($"{selfPath} => {request.error}");
        }
        else
        {
            var cacheDest = Path.GetDirectoryName(cachePath);
            if (!Directory.Exists(cacheDest))
            {
                Directory.CreateDirectory(cacheDest);
            }

            if (info.mode == CacheMode.Bytes)
            {
                File.WriteAllBytes(cachePath, request.downloadHandler.data);
            }
            else if (info.mode == CacheMode.Txt)
            {
                File.WriteAllText(cachePath, request.downloadHandler.text);
            }
        }
    }



    public static string LoadTxt(string path)
    {
        if (instance == null)
        {
            Debug.LogError("AssetLoader is not init");
            return string.Empty;
        }
#if !UNITY_EDITOR && UNITY_ANDROID
        if (instance.loadLinker.dicts.TryGetValue(path, out var info))
        { 
           var appendPath = instance.ToPersistent(path);
#else
           var appendPath = instance.ToStreamPath(path);
#endif
            if (File.Exists(appendPath))
            {
                return File.ReadAllText(appendPath);
            }
            else
            {
                Debug.LogError($"{path}res has not in cache");
                return string.Empty;
            }

#if !UNITY_EDITOR && UNITY_ANDROID
        }
        else
        {
            Debug.LogError($"{path} res has not in linkers");
            return string.Empty;
        }
#endif
    }


    public static byte[] LoadBytes(string path)
    {
        if (instance == null)
        {
            Debug.LogError("AssetLoader is not init");
            return new byte[] { };

        }

#if !UNITY_EDITOR && UNITY_ANDROID
        if (instance.loadLinker.dicts.TryGetValue(path, out var info))
        { 
           var appendPath = instance.ToPersistent(path);
#else
           var appendPath = instance.ToStreamPath(path);
#endif
            if (File.Exists(appendPath))
            {
                return File.ReadAllBytes(appendPath);
            }
            else
            {
                Debug.LogError($"{path} res has not in cache");
                return new byte[] { };
            }
#if !UNITY_EDITOR && UNITY_ANDROID
        }
        else
        {
            Debug.LogError($"{path}res has not in linkers");
            return new byte[] { };
        }
#endif
    }
}
