using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class CSharpScript
{
    public List<CSharpClass> classes;
    public List<CSharpField> fields;
    public List<CSharpMethod> methods;
}

[System.Serializable]
public class CSharpClass
{
    public string name;
    public string baseClass;
    public List<CSharpAttribute> attributes;
}

[System.Serializable]
public class CSharpAttribute
{
    public string name;
    public List<string> nodes;
}

[System.Serializable]
public class CSharpMethod
{
    public string name;
    public List<CSharpAttribute> attributes;
}


[System.Serializable]
public class CSharpField
{
    public string name;
    public string value;
    public List<CSharpAttribute> attributes;
}
