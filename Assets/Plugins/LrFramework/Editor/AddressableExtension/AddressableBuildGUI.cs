using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum BuildStatus
{
    RemotedGroup = 0,
    AnyGroup,
    LocalGroup,
}


public class AddressableBuildGUI
{
    string tag;
    public string setName => $"设置=>{tag}";
    public string buildName => $"资源=>{tag}";
    public string buildAppName => $"Build=>{tag}";

    public BuildStatus status;

    public string[] buildDisplay;

    public AddressableBuildGUI(string name, BuildStatus status)
    {
        tag = name;
        this.status = status;

        buildDisplay = new string[]
        {
            "设置成功",
           $"设置为{name}",
            "确定",
        };
    }
}


