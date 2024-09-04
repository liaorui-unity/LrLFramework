using UnityEngine;
using System.Collections;
using LogInfo;


public class Singleton<T> where T : new()
{
    public Singleton() { }

    public static T instance
    {
        get { return SingletonCreator.instance; }
    }

    class SingletonCreator
    {
        static SingletonCreator() { }
        internal static readonly T instance = new T();
    }
}

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    protected static T m_instance = null;

    public static T instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<T>();

                if (FindObjectsOfType<T>().Length > 1)
                {
                    Info.LogError("存在多个单例:" + typeof(T).Name);
                    return m_instance;
                }

                if (m_instance == null)
                {
                    m_instance = GameObject.FindObjectOfType<T>();

                    if (m_instance == null)
                        m_instance = new GameObject(typeof(T).Name).AddComponent<T>();
                }
            }
            return m_instance;
        }
    }

    protected virtual void Awake()
    {
        if (m_instance != null)
        {
            Info.LogWarning(string.Format("存在多个单例:{0}，移除旧的", typeof(T).Name));
            GameObject.Destroy(m_instance.GetComponent(typeof(T)));
        }
        m_instance = this as T;
    }
}
public abstract class DontMonoSingleton<T> : MonoBehaviour where T : DontMonoSingleton<T>
{
    protected static T m_instance = null;

    public static T instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<T>();

                if (FindObjectsOfType<T>().Length > 1)
                {
                    Info.LogError("存在多个单例:" + typeof(T).Name);
                    return m_instance;
                }

                if (m_instance == null)
                {
                    m_instance = GameObject.FindObjectOfType<T>();

                    if (m_instance == null)
                        m_instance = new GameObject(typeof(T).Name).AddComponent<T>();
                    DontDestroyOnLoad(m_instance.gameObject);
                }
            }
            return m_instance;
        }
    }
    protected virtual void Awake()
    {
        if (m_instance != null)
        {
            Info.LogWarning(string.Format("存在多个单例:{0}，移除旧的", typeof(T).Name));
            GameObject.Destroy(m_instance.GetComponent(typeof(T)));
        }
        m_instance = this as T;
    }
}
