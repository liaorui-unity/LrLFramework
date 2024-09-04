using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Folded
{
    public string label;
    public int labelCount;
    public bool isShow;
    public bool isFolded;
    public bool isRename;
}

[System.Serializable]
public class FoldData : ScriptableObject
{
    public List<Folded> infos = new List<Folded>();
}
