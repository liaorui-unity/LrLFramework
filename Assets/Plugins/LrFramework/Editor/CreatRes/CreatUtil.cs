using UnityEngine;


public class CreatUtil
{
    public const string savePath  = "Assets/Scripts/Auto/Engine";

    public const string res_FD    = "Resources";
    public const string assets_FD = "Assets";
    public const string stream_FD = "StreamingAssets";

    const int leftSpace = 66;

    public static string SetStringField(string fieldName, string value,int order=0)
    {
        fieldName = fieldName.Replace(' ', '_');
        var field = ForceFormat($"{GetFormat(order)}public const string {fieldName}", leftSpace);
        return ($"{field} = \"{value}\";");
    }

    public static string SetStringFieldSuf(string fieldName, string value,int order=0)
    {
        fieldName = fieldName.Replace(' ', '_');
        var field = ForceFormat($"{GetFormat(order)}public const string {fieldName}_Suf", leftSpace);
        return ($"{field} = \"{value}\";");
    }

    public static string SetStreamField(string fieldName, string value, int order = 0)
    {
        fieldName = fieldName.Replace(' ', '_');
        var field = ForceFormat($"     {GetFormat(order)}{fieldName} =", leftSpace/2);
        return ($"{field} = {Application.streamingAssetsPath} + \"{value}\";");
    }

    public static string ForceFormat(string souce, int count)
    {

        if (souce.Length < count)
        {
            int num = count - souce.Length;
 
            while (num > 0)
            {
                souce += " ";
                num--;
            }
        }
        return souce;
    }



    public static string LeftSymbol => "{";
    public static string RightSymbol => "}";

    public static string GetFormat(int count)
    {
        string num = string.Empty;
        for (int i = 0; i < count; i++)
        {
            num += " ";
        }
        return num;
    }

    public static string SubstringSingle(string source, string replace)
    {
        var resulf = string.Empty;

        if (source.Contains(replace))
        {
            bool isFind = false;
            var vs = source.Replace('\\', '/').Split('/');

            foreach (var item in vs)
            {
                if (isFind)
                {
                    resulf += $"{item}/";
                    continue;
                }

                if (item == replace)
                {
                    isFind = true;
                }
            }
        }

        return resulf.TrimEnd('/').TrimStart('/');
    }

    public static string SubstringToAssetsPath(string source)
    {
        var resulf = string.Empty;

        if (source.Contains(assets_FD))
        {
            bool isFind = false;
            var vs = source.Replace('\\', '/').Split('/');

            foreach (var item in vs)
            {
                if (isFind)
                {
                    resulf += $"{item}/";
                    continue;
                }

                if (item == assets_FD)
                {
                    resulf += $"{item}/";
                    isFind = true;
                }
            }
        }
        return resulf.TrimEnd('/');
    }

    public static string GetClassString(string directoryName)
    {
        directoryName = directoryName.Replace(' ', '_');
        return $"{directoryName}";
    }

    public static string NewClassString(string directoryName)
    {
        return $"new {directoryName}()";
    }


   
}
