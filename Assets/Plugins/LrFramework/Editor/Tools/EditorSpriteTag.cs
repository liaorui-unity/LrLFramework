
//=======================================================
// 作者：liusumei
// 公司：广州纷享科技发展有限公司
// 描述：图集设置tag
// 创建时间：2019-10-18 13:42:54
//=======================================================
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Table
{
    public class EditorSpriteTag
    {
        [MenuItem("Assets/设置Sprite tag", false, 1052)]
        private static void ChangeTag()
        {
            Object[] select_objs = Selection.objects;
            if (select_objs == null || select_objs.Length != 1)
                return;
            
            Object[] textures = Selection.GetFiltered(typeof(Texture2D), SelectionMode.DeepAssets);
            for (int i = 0; i < textures.Length; ++i)
            {
                Texture2D obj = textures[i] as Texture2D;
                if (obj is Texture2D == false) continue;

                string path = AssetDatabase.GetAssetPath(obj);
                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                importer.spritePackingTag = GetTagName(path);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static string GetTagName(string path)
        {
            string file_path_name = Path.GetDirectoryName(path);
            string tag_name = file_path_name.Substring("Assets\\".Length);
            tag_name = tag_name.Replace("\\", "_");
            tag_name = tag_name.Replace("/", "_");
            return tag_name;
        }
    }
}
