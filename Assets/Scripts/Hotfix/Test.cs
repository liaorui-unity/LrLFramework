using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Tr : StreamableObject
{
    public string tr;
}


public class FGG : ScriptableObject
{
    public string uyyuyek;
}




public class Test : MonoBehaviour
{
    public Tr w;
    public FGG fe;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(Streamable.Get<Tr>("Trg").tr);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
