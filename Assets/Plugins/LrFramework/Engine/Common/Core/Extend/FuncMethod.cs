//=======================================================
// 作者：LR
// 公司：广州纷享科技发展有限公司
// 描述：
// 创建时间：2021-06-23 16:40:27
//=======================================================
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;


    #region 特性事件委托

    //-----------------特性事件委托---------------start

    /// <summary>
    /// UI与外部事件特性
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Method)]
    public class FuncMethod: System.Attribute {}




    /// <summary>
    /// 触发事件委托函数
    /// </summary>
    public class UICall
    {
        public static Dictionary<string, object> keyInfos = new Dictionary<string, object>();
        public static Dictionary<object, EventInfo> eventInfos = new Dictionary<object, EventInfo>();


        public static int GetInfoCount(int hode)
        {
            EventInfo info = null;
            if (eventInfos.TryGetValue(hode, out info))
            {
                return info.InfoCounts;
            }
            return 0;
        }

        public static void Invoke(string key, params object[] values)
        {
            if (keyInfos.TryGetValue(key,out var value))
            {
                eventInfos[value].InvokeMethod(key, values);
            }
        }
    }



/// <summary>
/// 事件容器
/// </summary>
public class EventInfo
{
    public object instance;
    public Dictionary<string, IUnityMes>  mesInfos  = new Dictionary<string, IUnityMes>();

    public int InfoCounts
    {
        get => mesInfos.Count;
    }

    public void InvokeMethod(string key, params object[] values)
    {
        mesInfos[key].Call(values);
    }

    public void RemoveMethod(string key)
    {
        if (mesInfos.TryGetValue(key,out var _delegate))
        {
            _delegate = null;
            mesInfos.Remove(key);
        }
        if (UICall.keyInfos.ContainsKey(key))
        {
            UICall.keyInfos.Remove(key);
        }
    }

    public void RemoveAllMethod()
    {
        foreach (var item in mesInfos.Keys)
        {
            UICall.keyInfos.Remove(item);
        }
        mesInfos.Clear();
    }
}



/// <summary>
/// 特性数据处理
/// </summary>
public static class ExtendAttributes
{
    public static void AddMethodFuncs(this object target)
    {
        var fields = target.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        EventInfo tempEvent = new EventInfo();

        for (int i = 0; i < fields.Length; i++)
        {
            FuncMethod[] attrs = fields[i].GetCustomAttributes(typeof(FuncMethod), false) as FuncMethod[];
            if (attrs != null && attrs.Length > 0)
            {
                MethodInfo info = fields[i];
                try
                {
                    tempEvent.instance = target;
                    tempEvent.mesInfos.Add(info.Name, info.Create(target));
                   // tempEvent.methodInfos.Add(info.Name, MethodAccessor.CreateDelegate(info));
                    UICall.keyInfos.Add(info.Name, target);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Key值：{info.Name}____{e.Message}");
                }
            }
        }

        if (tempEvent.instance != null)
            UICall.eventInfos.Add(target, tempEvent);
    }

    public static void RemoveMethodFuncs(this object target)
    {
        if (UICall.eventInfos.ContainsKey(target))
        {
            UICall.eventInfos[target].RemoveAllMethod();
            UICall.eventInfos.Remove(target);
        }
    }

    public static void RemoveMethodFuncs(this object target, string key)
    {
        if (UICall.eventInfos.ContainsKey(target))
        {
            UICall.eventInfos[target].RemoveMethod(key);
            UICall.keyInfos.Remove(key);
        }
    }
}

    //-----------------特性事件委托---------------end
    #endregion


