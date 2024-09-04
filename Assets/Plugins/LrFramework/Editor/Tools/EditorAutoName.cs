using UnityEngine;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;

namespace Table
{
    //public class AutoAddNamespace : UnityEditor.AssetModificationProcessor
    //{
    //    public static void OnWillCreateAsset(string path)
    //    {
    //        path = path.Replace(".meta", "");

    //        if (path.EndsWith(".cs"))
    //        {
    //            string allText = File.ReadAllText(path);
    //            string className = GetClassName(allText);
    //            //·ÀÖ¹°Ñ×Ô¼ºÌæ»»
    //            if (className == "AutoAddNamespace") return;
    //            allText = allText.Replace("#CreateTime#", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
    //            File.WriteAllText(path, allText, Encoding.UTF8);
    //        }
    //    }
    //    public static string GetClassName(string allText)
    //    {
    //        //   var patterm = "public class ([A-Za-z0-9_]+)\\s*:\\s*MonoBehaviour{\\s*\\/\\/ Use this for initialization\\s*void Start \\(\\) {\\s*}\\s*\\/\\/ Update is called once per frame\\s*void Update \\(\\) {\\s*}\\s*}";
    //        var patterm = "public class ([A-Za-z0-9_]+)\\s*:\\s*MonoBehaviour";
    //        var match = Regex.Match(allText, patterm);
    //        if (match.Success)
    //        {
    //            return match.Groups[1].Value;
    //        }
    //        return "";
    //    }
        
    //    public class ScriptBuilder
    //    {
    //        private const string NEW_LINE = "\r\n";
    //        public ScriptBuilder()
    //        {
    //            builder = new StringBuilder();
    //        }

    //        private StringBuilder builder;
    //        public int Indent { get; set; }

    //        private int currentCharIndex;

    //        public void Write(string val, bool noAutoIndent = false)
    //        {
    //            if (!noAutoIndent)
    //                val = GetIndents() + val;
    //            if (currentCharIndex == builder.Length)
    //                builder.Append(val);
    //            else
    //                builder.Insert(currentCharIndex, val);
    //            currentCharIndex += val.Length;
    //        }

    //        public void WriteLine(string val, bool noAutoIndent = false)
    //        {
    //            Write(val + NEW_LINE);
    //        }

    //        public void WriteCurlyBrackets()
    //        {
    //            var openBracket = GetIndents() + "{" + NEW_LINE;
    //            var closeBracket = GetIndents() + "}" + NEW_LINE;
    //            Write(openBracket + closeBracket, true);
    //            currentCharIndex -= closeBracket.Length;
    //        }

    //        public string GetIndents()
    //        {
    //            var str = "";
    //            for (var i = 0; i < Indent; i++)
    //                str += "    ";
    //            return str;
    //        }

    //        public override string ToString()
    //        {
    //            return builder.ToString();
    //        }
    //    }
    //}
}