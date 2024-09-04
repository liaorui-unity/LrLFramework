using System.IO;
using UnityEditor;

public class CreatEditor : Editor
{
    static string saveTagsPath => System.Environment.CurrentDirectory + $"/Assets/Scripts/Auto/Engine/TagAndLayer.cs";


    [MenuItem("Assets/Lr/Save Tags And Layer ", false, 10)]
    static void AutoTagManager()
    {
        var auto = new AutoTagAndLayer();


        var lastID  = saveTagsPath.LastIndexOf('/');
        var dirPath = saveTagsPath.Substring(0, lastID);

        if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);

        if (File.Exists(saveTagsPath)) File.Delete(saveTagsPath);

       
        File.WriteAllText(saveTagsPath, auto.builder.ToString());

        AssetDatabase.Refresh();
    }
}


