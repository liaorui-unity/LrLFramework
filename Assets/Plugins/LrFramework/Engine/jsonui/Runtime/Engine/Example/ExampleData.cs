using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sailfish;

[System.Serializable]
public class ExampleData : JsonMono
{
    [Header("---")]
    public string key;

    [Range(1, 10)]
    public int id;

    [JsonLayoutSlider(0.01f,10f)]
    public float speed;

    public List<string> vs;

    public int[] values;

    public Data1[] datas;
    public Data1 data;
    public EffectorSelection2D effector;

    public Vector3 se;

}


[JsonFieldAttribute]
[System.Serializable]
public class Data1
{
    [Header("zheg")]
    public string g;
    public List<int> values;

}
