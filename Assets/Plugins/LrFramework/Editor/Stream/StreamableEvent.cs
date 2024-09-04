using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using LogInfo;
using UnityEngine;


public class StreamMatch
{
    public static CSharpScript ParseStreamScript(string content)
    {
        CSharpScript script = new CSharpScript();
        // 匹配类和继承
        Regex classRegex = new Regex(@"class\s+(\w+)(?:\s*:\s*(\w+))?");
        MatchCollection classMatches = classRegex.Matches(content);

        script.classes = new List<CSharpClass>();
        foreach (Match classMatch in classMatches)
        {
            CSharpClass cSharpClass = new CSharpClass();
            cSharpClass.name       = classMatch.Groups[1].Value;
            cSharpClass.baseClass  = classMatch.Groups[2].Success ? classMatch.Groups[2].Value : "";
            cSharpClass.attributes = new List<CSharpAttribute>();
            script.classes.Add(cSharpClass);
        }
        return script;
    }
}

public class StreamableEvent : ICSharpFlie
{
    public string baseClass = "StreamableObject";

    public void OnChangeFile(string path, string content)
    {
        var info  = new InfoBinder();
        info.name = Path.GetFileNameWithoutExtension(path);
        info.path = path;
        info.content = StreamMatch.ParseStreamScript(content);
      
        foreach (var item in info.content.classes)
        {
            if (item.baseClass.Contains(baseClass))
            {
                CreatStreamEditor.InitEnable();
                break;
            }
        }
    }
}
