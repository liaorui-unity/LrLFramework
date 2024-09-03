using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

public interface IUnityMes
{
    void Set (MethodInfo method, object instance);
    void Call(params object[] objects);
    void Clear();
}

public class UnityMes
{
    public object[] objects;
    public UnityMes(params object[] vs)
    {
        objects = vs;
    }
}

public static class UnityDelegateExtend
{
    public static IUnityMes Create(this MethodInfo method, object instance)
    {
        var types = method.GetParameters().Select(_ => _.ParameterType).ToArray();

        Type type = null;
        switch (types.Length)
        {
            case 1:
                type = typeof(UnityDelegate<>).MakeGenericType(types);
                break;
            case 2:
                type = typeof(UnityDelegate<,>).MakeGenericType(types);
                break;
            case 3:
                type = typeof(UnityDelegate<,,>).MakeGenericType(types);
                break;
            case 4:
                type = typeof(UnityDelegate<,,,>).MakeGenericType(types);
                break;
            case 5:
                type = typeof(UnityDelegate<,,,,>).MakeGenericType(types);
                break;
            case 6:
                type = typeof(UnityDelegate<,,,,,>).MakeGenericType(types);
                break;
        }

        var iMes = (IUnityMes)Activator.CreateInstance(type);
        iMes.Set(method, instance);
        return iMes;
    }
}


public class UnityDelegate<T, U, I, N, O, M> : IUnityMes
{
    public event Action<T, U, I, N, O,M> action;

    public void Set(MethodInfo method, object instance)
    {
        action = (Action<T, U, I, N, O,M>)Delegate.CreateDelegate(typeof(Action<T, U, I, N, O,M>), instance, method);
    }

    public void Call(params object[] objects)
    {
        action?.Invoke((T)objects[0], (U)objects[1], (I)objects[2], (N)objects[3], (O)objects[4],(M)objects[4]);
    }

    public void Clear()
    {
        action = null;
    }
}

public class UnityDelegate<T, U, I, N, O> : IUnityMes
{
    public event Action<T, U, I, N, O> action;

    public void Set(MethodInfo method, object instance)
    {
       action = (Action<T, U, I, N, O>)Delegate.CreateDelegate(typeof(Action<T, U, I, N, O>), instance, method);
    }

    public void Call(params object[] objects)
    {
        action?.Invoke((T)objects[0], (U)objects[1], (I)objects[2], (N)objects[3], (O)objects[4]);
    }

    public void Clear()
    {
        action = null;
    }
}

public class UnityDelegate<T, U, I, N> : IUnityMes
{
    public event UnityAction<T, U, I, N> action;

    public void Set(MethodInfo method, object instance)
    {
        action = (UnityAction<T, U, I, N>)Delegate.CreateDelegate(typeof(UnityAction<T, U, I, N>), instance, method);
    }

    public void Call(params object[] objects)
    {
        action?.Invoke((T)objects[0], (U)objects[1], (I)objects[2], (N)objects[3]);
    }

    public void Clear()
    {
        action = null;
    }
}



public class UnityDelegate<T, U,I> : IUnityMes
{
    public event UnityAction<T, U, I> action;

    public void Set(MethodInfo method, object instance)
    {
        action = (UnityAction<T, U, I>)Delegate.CreateDelegate(typeof(UnityAction<T, U, I>), instance, method);
    }

    public void Call(params object[] objects)
    {
        action?.Invoke((T)objects[0], (U)objects[1], (I)objects[2]);
    }

    public void Clear()
    {
        action = null;
    }
}


public class UnityDelegate<T, U> : IUnityMes
{
    public event UnityAction<T, U> action;

    public void Set(MethodInfo method, object instance)
    {
        action = (UnityAction<T, U>)Delegate.CreateDelegate(typeof(UnityAction<T, U>), instance, method);
    }

    public void Call(params object[] objects)
    {
        action?.Invoke((T)objects[0], (U)objects[1]);
    }

    public void Clear()
    {
        action = null;
    }
}

public class UnityDelegate<T> : IUnityMes
{
    public event UnityAction<T> action;

    public void Set(MethodInfo method, object instance)
    {
        action = (UnityAction<T>)Delegate.CreateDelegate(typeof(UnityAction<T>), instance, method);
    }

    public void Call(params object[] objects)
    {
        action?.Invoke((T)objects[0]);
    }

    public void Clear()
    {
        action = null;
    }
}

public class UnityDelegate : IUnityMes
{
    public event UnityAction action;

    public void Set(MethodInfo method, object instance)
    {
        action = (UnityAction)Delegate.CreateDelegate(typeof(UnityAction), instance, method);
    }

    public void Call(params object[] objects)
    {
        action?.Invoke();
    }

    public void Clear()
    {
        action = null;
    }
}