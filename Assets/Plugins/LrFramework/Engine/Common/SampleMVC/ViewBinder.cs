using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewBinder : MonoBehaviour
{
    [System.Serializable]
    public class Item
    {
        public string name;
        public Object main;
    }

    public  string classType;
    public  Item[] items = new Item[0];

    private Dictionary<string, Item> s_itemsDic;
    private Dictionary<string, Item> m_itemsDic
    { 
        get
        {
            if (s_itemsDic == null)
            {
                s_itemsDic = new Dictionary<string, Item>();
                foreach (var item in items)
                {
                    s_itemsDic.Add(item.name, item);
                }
            }
            return s_itemsDic;
        }
    }
   
    public T TryGet<T>(string key) where T : Object
    {
        if (m_itemsDic.TryGetValue(key, out var view))
        {
            return view.main as T;
        }
        return default(T);
    }
}
