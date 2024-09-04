using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CheckWather;
using UnityEngine;


[System.Serializable]
public class InfoBinder
{
    public string name;
    public string path;
    public CSharpScript content;
}


public class CSharpMatch
{
    public static CSharpScript ParseCSharpScript(string text)
    {
        CSharpScript script = new CSharpScript();
        // 匹配类和继承
        Regex classRegex = new Regex(@"class\s+(\w+)(?:\s*:\s*(\w+))?");
        MatchCollection classMatches = classRegex.Matches(text);

        script.classes = new List<CSharpClass>();
        foreach (Match classMatch in classMatches)
        {
            CSharpClass cSharpClass = new CSharpClass();
            cSharpClass.name        = classMatch.Groups[1].Value;
            cSharpClass.baseClass   = classMatch.Groups[2].Success ? classMatch.Groups[2].Value : "";
            cSharpClass.attributes  = new List<CSharpAttribute>();

            // 找到类后，匹配类内部的带特性的方法
            string classContent = GetClassContent(text, classMatch.Index);
            Regex attributeMethodRegex    = new Regex(@"\[(\w+)\]\s*(public|private|protected|internal)?\s*\w+\s+(\w+)\s*\(.*\)");
            MatchCollection methodMatches = attributeMethodRegex.Matches(classContent);

            script.methods = new List<CSharpMethod>();
            foreach (Match methodMatch in methodMatches)
            {
                CSharpMethod cSharpMethod = new CSharpMethod();
                cSharpMethod.name         = methodMatch.Groups[3].Value;
                cSharpMethod.attributes   = new List<CSharpAttribute>();
                cSharpMethod.attributes.Add (new CSharpAttribute() { name = methodMatch.Groups[1].Value });
                script.methods.Add(cSharpMethod);
            }

            script.classes.Add(cSharpClass);
        }

        return script;
    }
    private static string GetClassContent(string text, int classStartIndex)
    {
        int startIndex = text.IndexOf('{', classStartIndex) + 1;
        int endIndex   = text.IndexOf('}', startIndex);
        return text.Substring(startIndex, endIndex - startIndex);
    }
}


public class CSharpCheckEvent : ICSharpFlie
{
    public string baseClass = "MonoBehaviour";
    public string attribute = "Input";

    public void OnChangeFile(string path, string content)
    {
        var info     = new InfoBinder();
        info.name    = Path.GetFileNameWithoutExtension(path);
        info.path    = path;
        info.content = CSharpMatch.ParseCSharpScript(content);

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
