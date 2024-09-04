
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Table
{
    /// <summary>
    /// 查找资源被哪里引用了
    /// @author hannibal
    /// @time 2016-12-17
    /// </summary>
    public class EditorFindReferences
    {
        [MenuItem("Assets/Find References", false, 1000)]
        static private void Find()
        {
           // EditorUtils.ClearLog();

            EditorSettings.serializationMode = SerializationMode.ForceText;
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (!string.IsNullOrEmpty(path))
            {
                string guid = AssetDatabase.AssetPathToGUID(path);
                List<string> withoutExtensions = new List<string>() { ".prefab", ".unity", ".mat", ".asset", ".controller", ".anim", ".fontsettings", ".playable", ".guiskin", ".spriteatlas", ".flare", ".mixer", ".cubemap", ".json", ".txt" };
                string[] files = Directory.GetFiles(Application.dataPath, "*.*", SearchOption.AllDirectories)
                    .Where(s => withoutExtensions.Contains(Path.GetExtension(s).ToLower())).ToArray();
                int startIndex = 0;
                EditorApplication.update = delegate ()
                {
                    string file = files[startIndex];

                    bool isCancel = EditorUtility.DisplayCancelableProgressBar("匹配资源中", file, (float)startIndex / (float)files.Length);

                    if (Regex.IsMatch(File.ReadAllText(file), guid))
                    {
                        Debug.Log(file, AssetDatabase.LoadAssetAtPath<Object>(GetRelativeAssetsPath(file)));
                    }

                    startIndex++;
                    if (isCancel || startIndex >= files.Length)
                    {
                        EditorUtility.ClearProgressBar();
                        EditorApplication.update = null;
                        startIndex = 0;
                        Debug.Log("匹配结束");
                    }
                };
            }
        }
        
        static private string GetRelativeAssetsPath(string path)
        {
            return "Assets" + Path.GetFullPath(path).Replace(Path.GetFullPath(Application.dataPath), "").Replace('\\', '/');
        }
    }
}