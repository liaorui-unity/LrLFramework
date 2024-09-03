using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System;
using System.Reflection;
using UnityEditor;

namespace CheckWather
{
    public class CheckBinder
    {
        public string className;
        public string forSuffix;
        public List<string> fields;
    }

    public class IDClass
    {
        public static void SaveClass(CheckBinder binder)
        {
            var path = $"{Environment.CurrentDirectory}/{CreatUtil.savePath}/{binder.forSuffix}_{binder.className}.cs";
            var viewBinder = new IDClass(binder, path);
            var dire = Path.GetDirectoryName(path);
            if (Directory.Exists(dire) == false)
            {
                Directory.CreateDirectory(dire);
            }
            File.WriteAllText(path, viewBinder.builder.ToString());
        }


        public StringBuilder builder;

        public List<string> obsoletes;

        public int order = 1;


        public void FindObsoletes(List<string> items, string path)
        {
            obsoletes = GetObsoletes(items, path);
        }

        public IDClass(CheckBinder binder, string path)
        {
            FindObsoletes(binder.fields, path);

            builder = new StringBuilder();
            builder.AppendLine($"  {CreatUtil.GetFormat(order)}//自动生成类，不能修改");
            builder.AppendLine($"  using System;");//自动生成类，不能修改");
            builder.AppendLine($"  {CreatUtil.GetFormat(order)}public partial class {binder.forSuffix}");
            builder.AppendLine($"  {CreatUtil.GetFormat(order)}{CreatUtil.LeftSymbol}");

            var order1 = order + 2;
            builder.AppendLine($"  {CreatUtil.GetFormat(order1)}public class {binder.className}");


            builder.AppendLine($"  {CreatUtil.GetFormat(order1)}{CreatUtil.LeftSymbol}");

            var order2 = order1 + 2;
            foreach (var item in obsoletes)
            {
                if (string.IsNullOrEmpty(item) == true)
                    continue;

                builder.AppendLine($"   {CreatUtil.GetFormat(order2)}[Obsolete(\"字段常量已经被弃用\")]");
                builder.AppendLine($"   {CreatUtil.SetStringField(item, item, order2)}");
            }
            foreach (var item in binder.fields)
            {
                if (string.IsNullOrEmpty(item) == true)
                    continue;
                    
                builder.AppendLine($"   {CreatUtil.SetStringField(item, item, order2)}");
            }
            builder.AppendLine($"  {CreatUtil.GetFormat(order1)}{CreatUtil.RightSymbol}");
            builder.AppendLine($"  {CreatUtil.GetFormat(order)}{CreatUtil.RightSymbol}");
        }

        static List<string> GetCurrentFile(string path)
        {
            if (File.Exists(path) == false)
                return new List<string>();

            var contents = File.ReadAllLines(path);
            var list = new List<string>();
            foreach (var item in contents)
            {
                if (item.Length > 20)
                {
                    if (string.IsNullOrEmpty(MatchName(item)) == false)
                    {
                        list.Add(MatchName(item));
                    }
                }
            }
            return list;
        }

        static List<string> GetObsoletes(List<string> items, string path)
        {

            var lists = GetCurrentFile(path);

            if (lists.Count <= 0)
                return new List<string>();

            var obsoletes = lists?.Where(_ => items.Contains(_) == false).ToList();

            return obsoletes;
        }

        static string MatchName(string input)
        {
            string pattern = @"public\s+const\s+string\s+(\w+)\s*=";

            Match match = Regex.Match(input, pattern);
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
