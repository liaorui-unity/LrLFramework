
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

namespace Table
{
    /// <summary>
    /// 查找资源依赖关系
    /// @author hannibal
    /// @time 2018-11-17
    /// </summary>
    public class EditorFindDependencies
    {
        [MenuItem("Assets/Find Dependencies", false, 1010)]
        static private void Find()
        {
           // EditorUtils.ClearLog();

            EditorSettings.serializationMode = SerializationMode.ForceText;
            List<Object> list_objs = GetDependencies(Selection.activeObject);
            foreach (var obj in list_objs)
            {
                Debug.Log(obj);
            }
        }

        /// <summary>
        /// 获得指定Obj的所有配置过滤后的依赖
        /// </summary>
        /// <param name="obj">指定的obj</param>
        /// <returns>过滤后所有依赖的objects</returns>
        private static List<Object> GetDependencies(Object obj)
        {
            if (obj == null) return null;
            List<Object> objList = new List<Object>();
            string url = AssetDatabase.GetAssetPath(obj);
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogError(obj.name + " url is null");
                return objList;
            }

            string[] dpcs = AssetDatabase.GetDependencies(new string[] { url });
            if (dpcs != null && dpcs.Length > 0)
            {
                for (int i = 0; i < dpcs.Length; i++)
                {
                    string dpcAssetUrl = dpcs[i];
                    string ext = Path.GetExtension(dpcAssetUrl);
                    if (!CheckExtention(ext))
                        continue;
                    Object dpcObj = AssetDatabase.LoadAssetAtPath(dpcAssetUrl, typeof(Object));
                    objList.Add(dpcObj);
                }
            }

            return objList;
        }
        private static bool CheckExtention(string ext)
        {
            if (string.IsNullOrEmpty(ext) || ext == ".meta" || ext == ".xml" || ext == ".dll" || ext == ".cs" || ext == ".js" || ext == ".lua" || ext == ".tpsheet")
                return false;
            return true;
        }
    }
}