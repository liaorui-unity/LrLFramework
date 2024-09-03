
//=======================================================
// 作者：liusumei
// 公司：广州纷享科技发展有限公司
// 描述：关闭mipmap
// 创建时间：2020-12-8 13:42:54
//=======================================================
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace dc
{
    public class EditorCloseMipmap
    {
        [MenuItem("Assets/关闭文件夹下贴图mipmap", false, 1053)]
        private static void CloseMipmap()
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (!string.IsNullOrEmpty(path))
            {
                string full_path = Application.dataPath.Replace("Assets","") + path;
                if (!Directory.Exists(full_path))
                {
                    Debug.LogWarning("需要选择目录");
                    return;
                }
                List<string> withoutExtensions = new List<string>() { ".png", ".jpg", ".tga", ".dds", ".bmp" };
                string[] files = Directory.GetFiles(full_path, "*.*", SearchOption.AllDirectories).Where(s => withoutExtensions.Contains(Path.GetExtension(s).ToLower())).ToArray();
                for(int i = 0; i < files.Length; ++i)
                {
                    string file = files[i];
                    string asset_file = GetRelativeAssetsPath(file);
                    TextureImporter importer = AssetImporter.GetAtPath(asset_file) as TextureImporter;
                    if (importer != null && importer.mipmapEnabled)
                    {
                        importer.mipmapEnabled = false;
                        Debug.Log(file, AssetDatabase.LoadAssetAtPath<Object>(asset_file));
                    }
                }
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
        static private string GetRelativeAssetsPath(string path)
        {
            return "Assets" + path.Substring(Application.dataPath.Length);
        }
    }
}
