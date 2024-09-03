
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using UnityEditorInternal;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

public class ViewBinderClassCreat
{ 
    public static void SaveViewBinderClass(ViewBinder binder)
    {
        var path = $"{CreatUtil.savePath}/ViewID_{binder.classType}.cs";
        var viewBinder = new ViewBinderIDClass(binder, path);
      
        File.WriteAllText(path, viewBinder.builder.ToString());
    }
}

public class ViewBinderIDClass
{
    
    public StringBuilder builder;
    public int order = 1;
    public ViewBinderIDClass(ViewBinder binder, string path)
    {
        var obsoletes = GetObsoletes(binder.items, path);

        builder = new StringBuilder();
        builder.AppendLine($"  {CreatUtil.GetFormat(order)}//自动生成类，不能修改");
        builder.AppendLine($"  using System;");//自动生成类，不能修改");
        builder.AppendLine($"  {CreatUtil.GetFormat(order)}public partial class ViewID");
        builder.AppendLine($"  {CreatUtil.GetFormat(order)}{CreatUtil.LeftSymbol}");

        var order1 = order  + 2;
        builder.AppendLine($"  {CreatUtil.GetFormat(order1)}public class {binder.classType}");

  
        builder.AppendLine($"  {CreatUtil.GetFormat(order1)}{CreatUtil.LeftSymbol}");

        var order2 = order1 + 2;
        foreach (var item in obsoletes)
        {
            builder.AppendLine($"   {CreatUtil.GetFormat(order2)}[Obsolete(\"字段常量已经被弃用\")]");
            builder.AppendLine($"   {CreatUtil.SetStringField(item, item, order2)}");
        }
        foreach (var item in binder.items)
        {
            builder.AppendLine($"   {CreatUtil.SetStringField(item.name, item.name, order2)}");
        }
        builder.AppendLine($"  {CreatUtil.GetFormat(order1)}{CreatUtil.RightSymbol}");
        builder.AppendLine($"  {CreatUtil.GetFormat(order)}{CreatUtil.RightSymbol}");
    }

    static List<string> GetCurrentFile(string path)
    {
        var contents = File.ReadAllLines(path);
        var list = new List<string>();
        foreach (var item in contents)
        {
            if (item.Length>20)
            {
                if (MatchName(item).IsNullOrEmpty() == false)
                {
                    list.Add(MatchName(item));
                }
            }
        }
        return list;
    }

    static List<string> GetObsoletes(ViewBinder.Item[] items, string path)
    {
        var lists     = GetCurrentFile(path);
        var names     = items.Select(_ => _.name).ToList();
        var obsoletes = lists.Where(_ => names.Contains(_) == false).ToList();

        return obsoletes;
    }

    static string MatchName(string input)
    {
        string pattern = @"public\s+const\s+string\s+(\w+)\s*=";

        Match match = Regex.Match(input, pattern);
        if (match.Success)
        {
            return  match.Groups[1].Value;
        }
        else
        {
            return string.Empty;
        }
    }
}

