using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;
using System;
using UnityEngine.Scripting;
using System.Diagnostics;

public class ChangeEventAttribute : System.Attribute
{
    public string methodStr;
    public ChangeEventAttribute(string eventName)
    {
        methodStr = eventName;
    }
}


public class CheckData<T>
{
    public int number;
    public T lastValue;
    public object instance;

    Func<object, T> accessor;
    IUnityMes method;

    public CheckData(object instance, MethodInfo method, FieldInfo field)
    {
        this.instance = instance;
        accessor      = FieldAccessor.CreateGetter<T>(field);
        this.method   = method.Create(instance);
        number = method.GetParameters().Length;
    }

    public void Check()
    {
        T value = accessor.Invoke(instance);

        if (!value.Equals(lastValue))
        {
            if (number == 0)
            {
                method.Call();
            }
            else if (number == 1)
            {
                method.Call(lastValue);
            }
            else if (number == 2)
            {
                method.Call(lastValue, value);
            }
            lastValue = value;
        }
    }

    public void Release()
    {
        method   = null;
        accessor = null;
        instance = null;
    }
}


[Preserve]
public class ChangeEventController
{
    public static Dictionary<int, List<System.Action>> valuePairs = new Dictionary<int, List<System.Action>>();
    public static Dictionary<int, List<object>> checkDatas = new Dictionary<int, List<object>>();

    static bool isCheck = false;

    public static void InitChangeEvent<T>(T instance)
    {
        var fields =  typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
        var actions = new List<System.Action>();

        foreach (var field in fields)
        {
            var checkEvent = field.GetCustomAttribute<ChangeEventAttribute>();

            if (checkEvent != null)
            {
                var method = typeof(T).GetMethod(checkEvent.methodStr, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
                var creat  = typeof(ChangeEventController).GetMethod(nameof(CreatCheck), BindingFlags.Public| BindingFlags.Static | BindingFlags.NonPublic);

                var typeMethod = creat.MakeGenericMethod(field.FieldType);
                typeMethod?.Invoke( null,new object[] { method, field, instance, actions });
            }
        }
        valuePairs[instance.GetHashCode()] = actions;


        if (!isCheck)
        {
            isCheck = true;
            CallUnit.updateDontDestroyCall.AddListener(Check);
        }
    }

    public static void RemoveChangeEvent<T>(T instance)
    {
        if (valuePairs.TryGetValue(instance.GetHashCode(), out var actions))
        { 
            actions.Clear();
        }
        if (checkDatas.TryGetValue(instance.GetHashCode(), out var list))
        {
            list.Clear();
        }
    }

    static void CreatCheck<T>(MethodInfo method, FieldInfo field,object instance, List<System.Action> actions)
    {
        if(checkDatas.TryGetValue(instance.GetHashCode(),out var list) == false)
        {
            list = new List<object>();
            checkDatas[instance.GetHashCode()] = list;
        };

       var delegateCall = InputDelegate.CreateDelegate(method,instance);

        var check = new CheckData<T>(instance, method, field) { };
        list.Add(check);
        actions.Add(check.Check);
    }

    static void Check()
    {
        foreach (var item in valuePairs.Values)
        {
            foreach (var action in item)
            {
                action?.Invoke();
            }
        }
    }
}
