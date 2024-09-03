//using System.Collections.Generic;
//using System.Reflection;
//using System.Text;
//using System.Text.RegularExpressions;
//using UnityEditor;
//using UnityEngine;

//public class AutoUIViewCs : Editor
//{
//    [MenuItem("GameObject/自动生成UI代码", true)]
//    static bool AutoCreatCsShow()
//    {
//        if (Selection.activeTransform == null)
//            return false;
//        return Selection.activeTransform.GetComponent<UITypeGroup>() != null;
//    }

//    [MenuItem("GameObject/自动生成UI代码", false, -2)]
//    public static void AutoCreatCs()
//    {
//        var group = Selection.activeTransform.GetComponent<UITypeGroup>();
//        if (group != null)
//        {
//            var elements = Selection.activeTransform.GetComponentsInChildren<UITypeElement>(true);
//            CreatViewCS(group.classGroup, elements);
//        }
//    }




//    static string creatPath = $"{Application.dataPath}/Scripts/Auto/View/";


//    public static void CreatViewCS(string className, UITypeElement[] uITypeElements)
//    {
//        var path = $"{creatPath}{className}.cs";
//        var builder = new StringBuilder();
//        var values = new Dictionary<string, int>();

//        builder.AppendLine($"using UnityEngine;");
//        builder.AppendLine($"using UnityEngine.UI;");


//        builder.AppendLine($"//----------自动生成代码-----------");
//        builder.AppendLine($"//-------------请勿动--------------");


//        builder.AppendLine($"public partial class {className} : UISampleView");
//        builder.AppendLine($"{CreatUtil.GetFormat(0)}{CreatUtil.LeftSymbol}");


//        int maxArgCount = 0;

//        foreach (var item in uITypeElements)
//        {
//            ElementFormat(item);

//            if (maxArgCount < item.findArg.Length)
//                maxArgCount = item.findArg.Length;

//            builder.AppendLine($"{CreatUtil.GetFormat(3)}{ArgFormat(item, 30)}");
//        }


//        builder.AppendLine($"{CreatUtil.GetFormat(3)}public {className}(Transform target) : base(target)");
//        builder.AppendLine($"{CreatUtil.GetFormat(3)}{CreatUtil.LeftSymbol}");

//        foreach (var item in uITypeElements)
//        {
//            builder.AppendLine($"{CreatUtil.GetFormat(5)}{ArgField(item, maxArgCount)}");
//        }
//        builder.AppendLine($"{CreatUtil.GetFormat(5)}Logic();");


//        builder.AppendLine($"{CreatUtil.GetFormat(3)}{CreatUtil.RightSymbol}");
//        builder.AppendLine($"{CreatUtil.GetFormat(0)}{CreatUtil.RightSymbol}");



//        if (!System.IO.Directory.Exists(creatPath))
//            System.IO.Directory.CreateDirectory(creatPath);

//        System.IO.File.WriteAllText(path, builder.ToString());

//        AssetDatabase.Refresh();
//    }


//    static string ArgFormat(UITypeElement element, int maxCount)
//    {
//        string arg = CreatUtil.ForceFormat($"public {GetEnumType(element.findType)}", maxCount);
//        return $"{arg} {element.findArg};";
//    }

//    static string ArgField(UITypeElement element, int maxCount)
//    {
//        var find = CreatUtil.ForceFormat($"view.Find(\"{element.fingPath}\")", 30);

//        return CreatUtil.ForceFormat(element.findArg, maxCount)
//            + " = "
//            + $"{find}.GetComponent<{GetEnumType(element.findType)}>();";
//    }

//    static string GetEnumType(System.Enum enumValue)
//    {
//        FieldInfo field = enumValue.GetType().GetField(enumValue.ToString());
//        UIFindAttribute find = field.GetCustomAttribute<UIFindAttribute>();

//        if (find == null)
//            return enumValue.ToString();
//        else
//            return find.type.Name;
//    }


//    static void ElementFormat(UITypeElement element)
//    {  
//        Arg(element);

//        Path(element);
//    }

//    static void Path(UITypeElement element)
//    {
//        int maxCount     = 10;
//        var names        = new Stack<string>();
//        var temp         = element.transform;

//        element.fingPath = string.Empty;

//        while (true)
//        {
//            maxCount--;
//            if (temp.GetComponent<UITypeGroup>() != null || maxCount <= 0)
//            {
//                break;
//            }

//            names.Push(temp.name);
//            temp = temp.parent;
//        }

//        while (names.Count > 0)
//        {
//            element.fingPath += $"{names.Pop()}/";
//        }

//        element.fingPath = element.fingPath.TrimEnd('/');
//    }

//    static void Arg(UITypeElement element)
//    {
//        string arg = string.Empty;

//        if (element.transform.parent.GetComponent<UITypeGroup>() != null)
//            arg = $"{element.transform.name}";
//        else
//            arg = $"{element.transform.parent.name}_{element.transform.name}";

//        arg = Regex.Replace(arg, @"[^a-zA-Z0-9_]", "");
//        arg = arg.Substring(0, 1).ToLower() + arg.Substring(1);

//        element.findArg = arg;
//    }
//}
