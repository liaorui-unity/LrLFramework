using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using UObject = UnityEngine.Object;

public class MonoSerialization : MonoBehaviour
{
    public string content;
    public UObject component;

    internal string deSerializeStr;
    internal Type ShowType => component.GetType();
    internal FieldInfo[] FieldInfos => GetFieldInfos();
    internal Dictionary<int, UObject> objs = new Dictionary<int, UObject>();

    private bool isDeSerialize = false;
    const string pattern = @"\{""instanceID"":(.+?)\}";

    public const string IN_SIGN   ="[In]";
    public const string OUT_SIGN  = "[Out]";
    public const string SELF_SIGN = "[Self]";
    public const string INSTANCE_SIGN = "instanceID";

    public static List<Type> fifters = new List<Type>()
    {
        typeof(string)
    };


    void Awake()
    {
        if (isDeSerialize) return;
        DeSerialize();
    }



    [ContextMenu("Serialize")]
    public void Serialize()
    {
        if (Application.isPlaying)
        {
            isDeSerialize = false;
        }

        content = SerializbleTool.Serialize(this.transform, component);
    }


    [ContextMenu("DeSerialize")]
    public void DeSerialize()
    {
        isDeSerialize = true;
        MatchObjectPath();
        SerializbleTool.DeSerialize(this);
    }


  

    public  FieldInfo[] GetFieldInfos()
    {
        return ShowType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy)
         .Select(item =>
          {
              if (item.FieldType.IsArray)
              {
                  var element = item.FieldType.GetElementType();
                  if (fifters.Contains(element) == false && element.IsDefined(typeof(SerializableAttribute), false))
                  {
                      return item;
                  }
              }
              else if (item.FieldType.IsGenericType)
              {
                  var genericTypes = item.FieldType.GenericTypeArguments;
                  var genericType = genericTypes[0];

                  if (fifters.Contains(genericType) == false && genericTypes.Count() == 1 && genericType.IsDefined(typeof(SerializableAttribute), false))
                  {
                      return item;
                  }
              }
              else if (item.FieldType.IsClass)
              {
                  if (fifters.Contains(item.FieldType) == false && item.FieldType.IsDefined(typeof(SerializableAttribute), false))
                  {
                      return item;
                  }
              }
              return null;
          }).ToArray();
    }


    public void MatchObjectPath()
    {
        // 匹配正则表达式
        var matchs = Regex.Matches(content, pattern);
        var tempStr = content;

        objs.Clear();

        // 获取匹配结果
        if (matchs.Count > 0)
        {
            foreach (Match item in matchs)
            {
                var vs = item.Groups[1].Value.Split('*');
                if (vs.Length < 2) continue;

                var path = vs[0];
                var type = vs[1];
                var id   = 0;

                UObject main = null;

                if (path == SELF_SIGN)
                {
                    var csType = SerializbleTool.GetType(type);
                    main = GetComponentOrGameObject(transform, csType);
                }
                else if (path.Contains(IN_SIGN))
                {
                    var obj = transform.Find(path.Replace(IN_SIGN, ""));
                    var csType = SerializbleTool.GetType(type);
                    main = GetComponentOrGameObject(obj, csType);
                }
                else if (path.Contains(OUT_SIGN))
                {
                    var obj    = GameObject.Find(path.Replace(OUT_SIGN, ""));
                    var csType = SerializbleTool.GetType(type);
                    main = GetComponentOrGameObject(obj?.transform, csType);
                }

                if(main == null) continue;

                     id  = main.GetInstanceID();
                objs[id] = main;

                var oldStr  = $"\"{INSTANCE_SIGN}\":{item.Groups[1].Value}";
                var replace = $"\"{INSTANCE_SIGN}\":{id}";

                if (Application.isEditor == false)
                    Debug.Log($"oldStr, replace=>{oldStr},{replace}");

                tempStr = tempStr.Replace(oldStr, replace);
            }
            deSerializeStr = tempStr;
        }
        else
        {
            Debug.Log("No match found.");
        }
    }

    UObject GetComponentOrGameObject(Transform target, Type csType)
    {
        try
        {
            if (csType.IsSubclassOf(typeof(Component)))
            {
                return target.GetComponent(csType);
            }
            else if (csType == typeof(GameObject))
            {
                return target.gameObject;
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
        return null;
    }


    void OnDestroy()
    {
        objs.Clear();
    }

 
}
