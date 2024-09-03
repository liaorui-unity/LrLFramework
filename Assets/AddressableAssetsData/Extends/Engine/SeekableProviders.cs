using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceProviders;

public class SeekableProviders : MonoBehaviour
{
    static SeekableProviders _instance;
    static SeekableProviders instance
    {
        get 
        {
            if (_instance == null)
            { 
                _instance = GameObject.FindObjectOfType<SeekableProviders>();

                if (_instance == null)
                {
                    _instance = new GameObject("SeekableProviders").AddComponent<SeekableProviders>();
                }
            }
            return _instance;
        }
    }


    List<SeekableAesStream> seekableAes = new List<SeekableAesStream>();

    public static void Add(SeekableAesStream stream)
    {
        if (instance. seekableAes.Contains(stream) == false)
        {
            instance. seekableAes.Add(stream);
        }
    }


    public void OnDisable()
    {
        foreach (var item in seekableAes)
        {
            item.Shut();
        }
        seekableAes.Clear();
        _instance = null;
    }
}
