
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LogInfo;
using SIO = StreamableIO;


public class StreamablePakeage
{
    public string name;
    public StreamableObject item;
}

public sealed class Streamable 
{
    static Dictionary<Type, StreamablePakeage> pakeages = new Dictionary<Type, StreamablePakeage>();

    static List<Type> allTypes  = new List<Type>();

    public const string suffix = ".st";
    public const string Sign = "[^AND^]";

 
    public static async Task LoadPakeage()
    {
        foreach (var item in allTypes)
        {
            var value  = StreamablePakeage(item);
            if (value != null)
            {
                pakeages.Add(item, value);
            }
        }
    }

    static  StreamablePakeage StreamablePakeage(Type type)
    {
        var pakeage = new StreamablePakeage();
        var jsonStr = SIO.ReadFiles(type.Name);

        if (string.IsNullOrEmpty(jsonStr))
        {
            pakeage.item = Activator.CreateInstance(type) as StreamableObject;
            SIO.WriteFiles(type.Name, SIO.ToJson(pakeage.item, type));
        }
        else
        {
            pakeage.item = SIO.FromJson(jsonStr, type) as StreamableObject;
        }

        pakeage.name = type.Name;
        return  pakeage;
    }


    public static void Save<T>(T obj) where T : StreamableObject
    {
        if (pakeages.TryGetValue(typeof(T), out StreamablePakeage pakeage) == false)
        {
            pakeage      = new StreamablePakeage();
            pakeage.item = Activator.CreateInstance<T>();
            pakeage.name = typeof(T).Name;
        }

        var jsonStr = SIO.ToJson(pakeage.item, pakeage.name);
        SIO.WriteFiles(pakeage.name, jsonStr);
    }



    public static void Register<T>() where T : StreamableObject
    {
        if (allTypes.Contains(typeof(T)) == false)
        {
            allTypes .Add(typeof(T));
        }
    }

    public static T Get<T>() where T : StreamableObject
    {
        if (pakeages.TryGetValue(typeof(T), out StreamablePakeage pakeage))
        {
            return (T)pakeage.item;
        }

#if UNITY_EDITOR
        else
        {
            T value = Activator.CreateInstance<T>();
            Save(value);
            return value;
        }
#else 
          return null;
#endif
    }

    public static T Get<T>(string name) where T : StreamableObject
    {
        var jsonStr = SIO.ReadFiles(name);
        if (string.IsNullOrEmpty(jsonStr) == false)
        {
            return (T)SIO.FromJson(jsonStr, typeof(T));
        }
        return  null;
    }
}
