
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
    /// 查找未被引用的资源
    /// @author hannibal
    /// @time 2018-10-8
    /// </summary>
    public class EditorFindNoReferences
    {
        private static List<string> all_scripts_context=null;
        [MenuItem("Assets/Find No References", false, 1001)]
        static private void Find()
        {
          //  EditorUtils.ClearLog();

            EditorSettings.serializationMode = SerializationMode.ForceText;
            List<string> all_files = GetAllFiles();
            List<string> list_files = new List<string>();
            all_scripts_context = null;
            List<string> list_sound_name_files = new List<string>();
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (path.IndexOf('.') == -1)
            {//选中的是目录
                List<string> files = new List<string>();
                FileUtils.GetDirectoryFiles(Application.dataPath + "/../" + path, ref files);
                files.ForEach((string file_path) =>
                {
                    file_path = file_path.Substring(file_path.IndexOf("../") + 3);
                    if (all_scripts_context == null)
                    {
                        if (AssetExtUtils.IsAudio(Path.GetExtension(file_path)))
                        {
                            all_scripts_context = GetAllScriptsContext();
                        }
                    }
                    list_files.Add(file_path);
                });
                Process(all_files, list_files);
            }
            else
            {//单个资源
                if (all_scripts_context == null)
                {
                    if (AssetExtUtils.IsAudio(Path.GetExtension(path)))
                    {
                        all_scripts_context = GetAllScriptsContext();
                    }
                }
                list_files.Add(path);
                Process(all_files, list_files);
            }
        }

        static private void Process(List<string> all_files, List<string> paths)
        {
            int startIndex = 0;
            EditorApplication.update = delegate ()
            {
                string file = paths[startIndex];
                bool isCancel = EditorUtility.DisplayCancelableProgressBar("匹配资源中", file, (float)startIndex / (float)paths.Count);
                ProcesOne(all_files, file);

                startIndex++;
                if (isCancel || startIndex >= paths.Count)
                {
                    EditorUtility.ClearProgressBar();
                    EditorApplication.update = null;
                    Debug.Log("匹配结束");
                }
            };
        }

        static private void ProcesOne(List<string> all_files, string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                //普通资源的引用查找
                if (IsDataFile(Path.GetExtension(path))) return;
                string guid = AssetDatabase.AssetPathToGUID(path);
                bool hasRef = false;
                for (int startIndex = 0; startIndex < all_files.Count; ++startIndex)
                {
                    string file = all_files[startIndex];
                    if (Regex.IsMatch(File.ReadAllText(file), guid))
                    {
                        hasRef = true;
                        break;
                    }
                }
                //音效的引用查找
                if (all_scripts_context!=null&&!hasRef)
                {
                    if (AssetExtUtils.IsAudio(Path.GetExtension(path)))
                    {
                        hasRef = ProcesOneSound(all_scripts_context, Path.GetFileNameWithoutExtension((path)));
                    }
                }
                
                if (!hasRef)
                {
                    Debug.Log(path, AssetDatabase.LoadAssetAtPath<Object>(GetRelativeAssetsPath(path)));
                }
            }
        }

        static private string GetRelativeAssetsPath(string path)
        {
            return "Assets" + Path.GetFullPath(path).Replace(Path.GetFullPath(Application.dataPath), "").Replace('\\', '/');
        }

        static private List<string> GetAllFiles()
        {
            List<string> withoutExtensions = new List<string>() { ".prefab", ".unity", ".mat", ".asset", ".controller", ".anim", ".fontsettings", ".playable", ".guiskin", ".spriteatlas", ".flare", ".mixer", ".cubemap", ".json", ".txt" };
            string[] files = Directory.GetFiles(Application.dataPath, "*.*", SearchOption.AllDirectories)
                .Where(s => withoutExtensions.Contains(Path.GetExtension(s).ToLower())).ToArray();
            List<string> list_files = files.ToList<string>();
            list_files.RemoveAll(IsSoundOrSubtitleAsset);
            return list_files;
        }

        static private bool IsSoundOrSubtitleAsset(string path)
        {
            return (path.Contains("SubtitleAsset.asset")||path.Contains("SoundAsset.asset"));
        }

        static private bool IsDataFile(string ext)
        {
            string[] exts = { ".json", ".txt", ".bytes", ".bat", ".data" };
            return exts.Contains(ext);
        }

        /// <summary>
        /// 获取指定目录下脚本的内容
        /// </summary>
        /// <returns></returns>
        static private List<string> GetAllScriptsContext()
        {
            List<string> scriptsContext = new List<string>();
            //脚本所在的文件夹位置
            string[] script_path = { "Assets/Script/", "Assets/Scripts/" };
            for(int index = 0; index < script_path.Length; index++)
            {
                if (Directory.Exists(script_path[index]))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(script_path[index]);
                    FileInfo[] fileInfos = directoryInfo.GetFiles("*", SearchOption.AllDirectories);
                    for (int i = 0; i < fileInfos.Length; i++)
                    {
                        if (fileInfos[i].Name.EndsWith(".cs"))
                        {
                            string script_context = File.ReadAllText(fileInfos[i].FullName);
                            scriptsContext.Add(script_context);
                        }
                    }
                }
                else
                {
                    Debug.LogWarning("不存在脚本路径：" + script_path[index]);
                }
            }
            return scriptsContext;
        }

        static private bool ProcesOneSound(List<string> all_scripts_context, string sound_name)
        {
            for (int startIndex = 0; startIndex < all_scripts_context.Count; startIndex++)
            {
                if (Regex.IsMatch(all_scripts_context[startIndex], sound_name))
                {
                    return true;
                }
            }
            return false;        
        }
    }
}