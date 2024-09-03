using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using UObject = UnityEngine.Object;

public class SerializbleTool
{
    public static Type GetType(string type)
    {
        var assemblys = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var item in assemblys)
        {
            if (item.GetTypes().Where(_ => _.FullName == type).Count() > 0)
            {
                return item.GetType(type);
            }
        }
        return null;
    }



    //+ -------------------------------      序列化            -------------------------
    public static string Serialize(Transform target, UObject component)
    {
        var jsonStr = JsonUtility.ToJson(component);
        // 定义正则表达式
        string pattern = @"\{""instanceID"":(.+?)\}";
        // 匹配正则表达式
        var matchs = Regex.Matches(jsonStr, pattern);

        // 获取匹配结果
        if (matchs.Count > 0)
        {

#if !UNITY_EDITOR
            var childGos = new List<GameObject>();
            var components = target.GetComponentsInChildren<Component>(true);
            GetChildGameObjects(target.transform, childGos);
#endif
            foreach (Match item in matchs)
            {
                var id = Int32.Parse(item.Groups[1].Value);

                if (id == 0) continue;

                var matchStr = "this";
#if UNITY_EDITOR
                var obj  =  UnityEditor.EditorUtility.InstanceIDToObject(id);
                matchStr =  FingObjectPath(obj, target);
#else
                try
                {
                    var comp            = components.Where(_ => _.GetInstanceID() == id).FirstOrDefault();
                    if  (comp) matchStr = MatchPath(comp,target);
                    var go              = childGos.Where(_ => _.GetInstanceID() == id).FirstOrDefault();
                    if  (go) matchStr   = MatchPath(go, target);
                }
                catch
                {
                    Debug.LogError("不是子物体或者组件：" + id);
                }
#endif
                var oldStr  = $"\"instanceID\":{item.Groups[1].Value}";
                var replace = $"\"instanceID\":{matchStr}";

                jsonStr = jsonStr.Replace(oldStr, replace);
            }
        }
        else
        {
            Debug.Log("No match found.");
        }

        return  jsonStr;
    }


#if UNITY_EDITOR
    static string FingObjectPath(UObject obj, Transform target)
    {
        string path = string.Empty;

        Transform parent = null;

        var thisID = target.GetInstanceID();

        if (obj is GameObject)
        {
            parent = ((GameObject)obj).transform;
        }
        else
        {
            parent = ((Component)obj).transform;
        }

        if (parent.GetInstanceID() == thisID)
        {
            path = MonoSerialization.SELF_SIGN;
        }
        else
        {
            do
            {
                path = parent.name + "/" + path;
                parent = parent.parent;

                if (parent == null)
                {
                    Debug.LogError("不是脚本节点下的物体:" + obj.name);
                    return MonoSerialization.OUT_SIGN + path + "*" + obj.GetType().FullName;
                }
            }
            while (parent.GetInstanceID() != thisID);

            path = MonoSerialization.IN_SIGN + path.TrimEnd('/');
        }

        return path + "*" + obj.GetType().FullName;
    }

#else
    //查找所有Assembly，然后获取Type
    static void GetChildGameObjects(Transform parent, List<GameObject> childObjects)
    {   // 添加子物体到列表
        childObjects.Add(parent.gameObject);
        // 遍历子物体
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform childTransform = parent.GetChild(i);
            // 递归查找子物体的子物体
            GetChildGameObjects(childTransform, childObjects);
        }
    }

    static string MatchPath(UObject obj,Transform target)
    {
        string path = string.Empty;

        if (obj is GameObject)
        {
            path = GetPath(((GameObject)obj).transform,target);
        }
        else
        {
            path = GetPath(((Component)obj).transform,target);
        }

        return path + "*" + obj.GetType().FullName;
    }

     static string GetPath(Transform obj,Transform target)
    {
        string path = string.Empty;

        if (obj == target.transform)
        {
            path = "[Self]";
        }
        else
        {
            path = obj.name;
            var parent = obj.transform.parent;

            while (parent != target.transform)
            {
                if (parent == null)
                {
                    Debug.LogError("不是脚本节点下的物体:" + obj.name);
                    return null;
                }

                path = parent.name + "/" + path;
                parent = parent.parent;
            }
        }

        return path;
    }
#endif





    //+ -------------------------------      反序列化            -------------------------


    static string pattern = "\"(\\w+)\":\\{\"instanceID\":(.+?)\\}";

    [ContextMenu("DeSerialize")]
    public static void DeSerialize(MonoSerialization mono)
    {
        //获取component的所有含有序列化的字符
        var infos   = mono.FieldInfos;
        var jsonObj = new JSONObject(mono.deSerializeStr);


        foreach (var item in infos)
        {
            if (item == null) continue;

            var jsonObject = jsonObj[item.Name];
            var info       = mono.ShowType.GetField(item.Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            Type infoType = null;
            
            if (info.FieldType.IsArray)
            {
                infoType = info.FieldType.GetElementType();
            }
            else if (info.FieldType.IsGenericType)
            {
                infoType = info.FieldType.GenericTypeArguments[0];
            }
            else if (info.FieldType.IsClass)
            {
                infoType = info.FieldType;
            }

            SetJsonToCs(mono, info, jsonObject, infoType);

        }
    }


    static void SetJsonToCs(MonoSerialization mono, FieldInfo info, JSONObject jobect, Type type)
    {
        var method  = typeof(SerializbleTool).GetMethod(nameof(SetJsonField), BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
        var generic = method.MakeGenericMethod(type);
        generic.Invoke(null, new object[] { mono, info, jobect, info.FieldType.IsGenericType });
    }


    public static void SetJsonField<T>(MonoSerialization mono, FieldInfo info, JSONObject jobect, bool IsGenericType = false)
    {
        if (jobect.type == JSONObject.Type.ARRAY)
        {    
            var temps = jobect.list.Select(_ => CheckInstance<T>(mono, _.ToString())).ToList();

            if (IsGenericType)
                info.SetValue(mono.component, temps);
            else
                info.SetValue(mono.component, temps == null ? null : temps.ToArray());
        }
        else if (jobect.type == JSONObject.Type.OBJECT)
        {
            info.SetValue(mono.component, CheckInstance<T>(mono, jobect.ToString()));
        }
        else
        {
            Debug.Log("目标不在序列化范围内");
        }
    }

    static T CheckInstance<T>(MonoSerialization mono, string jsonStr)
    {
        T temp = JsonUtility.FromJson<T>(jsonStr);

        if (jsonStr.Contains("instanceID"))
        {
            MatchCollection maths = Regex.Matches(jsonStr, pattern);
            if (maths.Count > 0)
            {
                foreach (Match item in maths)
                {
                    try
                    {
                        var info = typeof(T).GetField(item.Groups[1].Value, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                        var id   = int.Parse(item.Groups[2].Value);
                        var obj  = mono.objs[id];
                        
                        if (info.FieldType == obj.GetType())
                            info.SetValue(temp, obj);
                    }
                    catch ( Exception e)
                    {
                        Debug.LogError("反射赋值失败=>" + e.Message);
                    }
                }
            }
        }
        return temp;
    }

}
