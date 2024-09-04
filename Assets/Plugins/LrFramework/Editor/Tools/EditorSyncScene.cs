using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Table
{
    /// <summary>
    /// 同步所有场景到Build Setting
    /// NOTE:需要跟进场景目录做修改
    /// @author hannibal
    /// @time 2015-3-4
    /// </summary>
    public class EditorSyncScene : MonoBehaviour
    {
        [MenuItem("Tools/添加场景到SceneSetting", false, 200)]
        static void CheckSceneSetting()
        {
            List<string> dirs = new List<string>();
            GetDirs(Application.dataPath + "/Scenes", ref dirs);
            EditorBuildSettingsScene[] newSettings = new EditorBuildSettingsScene[dirs.Count];
            for (int i = 0; i < dirs.Count; i++)
            {
                newSettings[i] = new EditorBuildSettingsScene(dirs[i], true);
            }
            EditorBuildSettings.scenes = newSettings;
            AssetDatabase.SaveAssets();
        }
        static void GetDirs(string dirPath, ref List<string> dirs)
        {
            string[] files = Directory.GetFiles(dirPath);
            foreach (string path in files)
            {
                if (System.IO.Path.GetExtension(path) == ".unity")
                {
                    dirs.Add(path.Substring(path.IndexOf("Assets/")));
                }
            }
            if (Directory.GetDirectories(dirPath).Length > 0)
            {
                foreach (string path in Directory.GetDirectories(dirPath))
                {
                    GetDirs(path, ref dirs);
                }
            }
        }
    }
}