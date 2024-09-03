//=======================================================
// 作者：LR
// 描述：工具人
// 创建时间：2021-10-21 11:18:45
//=======================================================
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pool;

public interface IPool
{
    void CreatInit();
}

public class PoolMgr : MonoBehaviour
{
    private static PoolMgr instance;
    private static PoolMgr Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject("PoolMgr").AddComponent<PoolMgr>();
            }
            return instance;
        }
    }

    public bool isLog = false;

    private Dictionary<string, PoolType> pools = new Dictionary<string, PoolType>();
    private Dictionary<int, PoolType> idForKey = new Dictionary<int, PoolType>();


    void Awake()
    {
        instance = instance ?? this;
        SetSortArray();
    }

    void SetSortArray()
    {
        PoolType[] psts = FindObjectsOfType<PoolType>();
        Debug.Log("对象池类型数量：" + psts.Length);

        foreach (var item in psts)
        {
            Debug.Log("对象池的key：" + item.UseKey);
            if (!string.IsNullOrEmpty(item.UseKey))
                pools.Add(item.UseKey, item);
        }
    }



    public static void AddPoolType(GameObject prefab, string key, int pre = 2, int max = 6)
    {
        var obj = new GameObject("Key", typeof(PoolType));
        obj.transform.SetParent(instance.transform);
        obj.transform.localScale = Vector3.one;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localEulerAngles = Vector3.zero;

        var sort = obj.GetComponent<PoolType>();
        sort.CsharpInit(prefab, key, pre, max);

        instance.pools.Add(sort.UseKey, sort);

        Debug.Log("Cs Add sort UseKey：" + sort.UseKey);
    }


    void LateUpdate()
    {
        foreach (var item in pools)
        {
            item.Value.Check();
        }
    }




    /// <summary>
    /// 拿到对应key值的新物体
    /// </summary>
    /// <param name="poolKey"></param>
    /// <returns></returns>
    public static GameObject Get(string poolKey)
    {
        var type = Instance.pools[poolKey];
        var result = type.Creat();
        var code = result.GetHashCode();

        Instance.idForKey[code] = type;

        return result;
    }


    /// <summary>
    /// 删除T类型的对象
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="oldObject">需要被删除的物体</param>
    public static void Delete(GameObject oldGo)
    {
        var code = oldGo.GetHashCode();
        var type = Instance.idForKey[code];
        type?.Delete(oldGo);

        Instance.idForKey.Remove(code);
    }


    /// <summary>
    /// 删除key类型的所有对象
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="oldObject">需要被删除的物体</param>
    public static void DestroyAll(string poolKey)
    {
        var type = Instance.pools[poolKey];
        type?.DeleteAll();
    }



    private void OnDestroy()
    {
        pools.Clear();
        idForKey.Clear();
        instance = null;
    }
}



