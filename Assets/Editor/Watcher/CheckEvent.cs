using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using CheckWather;
using UnityEditor;
using UnityEngine;


[System.Serializable]
public class InfoBinder
{
    public string name;
    public string path;
    public CSharpScript content;
}

public class CheckEvent : IWatcher
{
    public string baseClass = "MonoBehaviour";
    public string attribute = "CheckID";

    public void OnChangeFile(string[] files)
    {
        foreach (var file in files)
        {
            var info = JsonUtility.FromJson<InfoBinder>(file);

            bool isPass = false;
            foreach (var item in info.content.classes)
            {
                if (item.baseClass.Contains(baseClass))
                {
                    isPass = true;
                    break;
                }
            }

            if (isPass)
            {
                var binder = new CheckBinder();
                binder.className = info.name;
                binder.forSuffix = "PanelID";
                binder.fields    = new List<string>();

                foreach (var item in info.content.methods)
                {
                    if (item.attributes.Any(x => x.name == attribute))
                    {
                        binder.fields.Add(item.name);
                    }
                }

                IDClass.SaveClass(binder);
            }
        }
    }
}
