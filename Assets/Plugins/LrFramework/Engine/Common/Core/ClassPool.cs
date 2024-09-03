//=======================================================
// 作者：LR
// 公司：广州纷享科技发展有限公司
// 描述：
// 创建时间：2021-08-11 17:46:55
//=======================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ObjectPool
{
    static Dictionary<string, object> m_ObjectPools = new Dictionary<string, object>();

    static ObjectPool<T> GetPool<T>() where T :  Component
    {
        var type = typeof(T);

        Debug.Log(type.Name);

        if (m_ObjectPools.TryGetValue(type.Name, out var pool) ==false)
        {
            Debug.Log("创建？？");
            pool = new ObjectPool<T>();
            m_ObjectPools[type.Name] = pool;
        }
        return (ObjectPool<T>)pool;
    }

    public static T Get<T>() where T :  Component
    {
        var pool = GetPool<T>();
        if (pool != null)
        {
            return pool.Get();
        }
        return default(T);
    }

    public static int GetNum<T>() where T :  Component
    {
        var pool = GetPool<T>();

        return pool.Count;
    }

    public static void Release<T>(T obj) where T :  Component
    {
        var pool = GetPool<T>();
        if (pool != null)
        {
            pool.Recycle(obj);
        }
    }

    public static void Clear<T>(T obj) where T :  Component
    {
        var type = typeof(T);

        var pool = GetPool<T>();
        if (pool != null)
        {
            pool.Clear();
        }
        m_ObjectPools.Remove(type.Name);
    }
}

public class ObjectPool<T> where T : Component
{
    private int count = 50;
    private readonly Stack<T> m_Stack = new Stack<T>();
    private object m_Lock = new object();
    public int Count
    {
        get { return m_Stack.Count; }
    }

    private Transform parent;

    public ObjectPool()
    {
        if (parent == null)
        {
            parent = new GameObject("Pool_" + typeof(T).Name).transform;
        }
    }

    public void Clear()
    {
        m_Stack.Clear();
    }

    public T Get()
    {
        lock (m_Lock)
        {
            if (m_Stack.Count <= 0)
            {
                Debug.Log("chuangj：" + m_Stack.Count);
                var go = new GameObject(nameof(T));
                go.transform.SetParent(parent);
                return go.AddComponent<T>();
            }
            else
            {
                Debug.Log("存在："+ m_Stack.Count);

                T element = m_Stack.Pop();

                Debug.Log("存在1：" + m_Stack.Count);

                return element;
            }
        }
    }

    public void Recycle(T element)
    {
        if (m_Stack.Contains(element))
        {
            Debug.LogError("Internal error. Trying to destroy object that is already released to pool.");
            return;
        }

        if (m_Stack.Count >= count)
        {
            GameObject.Destroy(element.gameObject);
            element = null;
        }
        else
        {
            Debug.Log("回收："+ element);

            element.transform.SetParent(parent);
            m_Stack.Push(element);
        }
    }

}

public class ClassPool
{
    static Dictionary<string, object> m_ObjectPools = new Dictionary<string, object>();

    static ClassPool<T> GetPool<T>() where T : class ,new()
    {
        var type = typeof(T);
        ClassPool<T> pool = null;
        if (m_ObjectPools.ContainsKey(type.Name))
        {
            pool = m_ObjectPools[type.Name] as ClassPool<T>;
        }
        else
        {
            pool = new ClassPool<T>();
            m_ObjectPools[type.Name] = pool;
        }
        return pool;
    }

    public static T Get<T>() where T : class, new()
    {
        var pool = GetPool<T>();
        if (pool != null)
        {
            return pool.Get();
        }
        return default(T);
    }

    public static int GetNum<T>() where T : class, new()
    {
        var pool = GetPool<T>();

        return pool.Count;
    }

    public static void Release<T>(T obj) where T : class, new()
    {
        var pool = GetPool<T>();
        if (pool != null)
        {
            pool.Recycle(obj);
        }
    }

    public static void Clear<T>(T obj) where T : class, new()
    {
        var type = typeof(T);

        var pool = GetPool<T>();
        if (pool != null)
        {
            pool.Clear();
        }
        m_ObjectPools.Remove(type.Name);
    }
}

public class ClassPool<T> where T : class, new()
{
    private int count = 50;
    private readonly Stack<T> m_Stack = new Stack<T>();
    private object m_Lock = new object();
    public int Count
    {
        get { return m_Stack.Count; }
    }


    public void Clear()
    {
        m_Stack.Clear();
    }

    public T Get()
    {
        lock (m_Lock)
        {
            if (m_Stack.Count <= 0)
            {
                return new T();
            }
            else
            {
                T element = m_Stack.Pop();
                return element;
            }
        }
    }

    public void Recycle(T element)
    {
        if (m_Stack.Contains(element))
        {
            Debug.LogError("Internal error. Trying to destroy object that is already released to pool.");
            return;
        }

        if (m_Stack.Count >= count)
        {
            element = null;
        }
        else
        {
            m_Stack.Push(element);
        }
    }

}

