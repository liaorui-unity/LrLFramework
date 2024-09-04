using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UniRx;

public class StreamableIO : MonoBehaviour
{
    public const string suffix = ".st";
    public const string Sign   = "[^AND^]";

    public static string streamPath     => $"{Application.streamingAssetsPath}/Streamable";
    public static string persistentPath => $"{Application.persistentDataPath}/Streamable";

    public static void WriteFiles(string name, string jsonStr)
    {
        var jsonUrl = GetJsonPath(name);

        if (File.Exists(jsonUrl) == false)
        {
            var parent = Path.GetDirectoryName(jsonUrl);
            if (Directory.Exists(parent) == false)
            {
                Directory.CreateDirectory(parent);
            }
        }

        File.WriteAllText(jsonUrl, jsonStr);
    }


    public static  string ReadFiles(string name)
    {
        var jsonStr = string.Empty;
        jsonStr    = AssetLoader.LoadTxt($"Streamable/{name}.st");
        return jsonStr;
    }

    public static async Task<string> ReadFiles(string name, string path)
    {
        var jsonStr = string.Empty;
        var jsonUrl = $"{path}/{name}{suffix}";

#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        jsonStr =  File.ReadAllText(jsonUrl);
#elif UNITY_ANDROID || UNITY_IOS
        using (UnityWebRequest www = UnityWebRequest.Get(jsonUrl))
        {
            await www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error reading file: " + www.error);
            }
            else
            {
                jsonStr = www.downloadHandler.text;
            }
        }
#endif
        return jsonStr;
    }


    public static object FromJson(string str)
    {
        var vs   = str.Split(Sign);
        var name = vs[0].Replace("{class=\"", "").Replace("\"}", "");
        var type = FindPackageType(name);

        return JsonUtility.FromJson(vs[1], type);
    }
    public static object FromJson(string str, System.Type type)
    {
        var vs   = str.Split(Sign);
        return JsonUtility.FromJson(vs[1], type);
    }

    public static string ToJson(object jsonObject, System.Type type)
    {
        return $"{{class=\"{type.Name}\"}}{Sign}{JsonUtility.ToJson(jsonObject)}";
    }
    public static string ToJson(object jsonObject, string type)
    {
        return $"{{class=\"{type}\"}}{Sign}{JsonUtility.ToJson(jsonObject)}";
    }

    public static string JsonPath()
    {
#if UNITY_STANDALONE_WIN
        return $"{Application.streamingAssetsPath}/Streamable";
#else
        return $"{Application.persistentDataPath}/Streamable";
#endif
    }


    static string GetJsonPath(string name)
    {
        return $"{JsonPath()}/{name}{suffix}";
    }


    public static List<Type> FindAllTypes()
    {
        var types = AppDomain.CurrentDomain.GetAssemblies()
                  . Where(_ => _.GetName().Name == "Assembly-CSharp")
                  . Distinct()
                  . SelectMany(_ => _.GetTypes())
                  . Where(t => t.IsSubclassOf(typeof(StreamableObject)));

        return types.ToList();
    }

    public static Type FindPackageType(string typeName)
    {
        var types = AppDomain.CurrentDomain.GetAssemblies()
                  . Where(_ => _.GetName().Name == "Assembly-CSharp")
                  . SelectMany(_ => _.GetTypes())
                  . Where(t => t.Name == typeName);

        return types?.FirstOrDefault();
    }
}
