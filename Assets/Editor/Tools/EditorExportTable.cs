using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Diagnostics;

namespace Table
{
    public class EditorExportTable
    {
        [MenuItem("配置表/打资源", false, 0)]
        static void ExportResources()
        {
            Export("打资源.bat");
        }
        [MenuItem("配置表/生成代码", false, 1)]
        static void ExportCode()
        {
            Export("生成代码.bat");
        }
        [MenuItem("配置表/生成代码+打资源", false, 2)]
        static void ExportResourcesAndCode()
        {
            Export("生成代码+打资源.bat");
        }
        [MenuItem("配置表/提交excel文件", false, 100)]
        static void CommitExcel()
        {
            string path = Application.dataPath + "/../Table/Data/excel";
            if (System.IO.Directory.Exists(path))
                Process.Start("TortoiseProc.exe", "/command:commit /path:" + path + " /closeonend:0");
            else
                UnityEngine.Debug.LogError("不存在目录:" + path);
        }
        [MenuItem("配置表/打开excel目录", false, 101)]
        static void OpenExcelExplorer()
        {
            Process.Start("explorer.exe", "file:///" + Application.dataPath + "\\..\\Table\\Data\\excel");
        }

        static void Export(string file)
        {
            string path = Application.dataPath + "/../Table/Data";
            path = path.Replace("/", "\\");

            Process proc = null;
            try
            {
                proc = new Process();
                proc.StartInfo.WorkingDirectory = path;
                proc.StartInfo.FileName = file;
                proc.StartInfo.CreateNoWindow = false;
                proc.Start();
                proc.WaitForExit();
            }
            catch (System.Exception ex)
            {
                UnityEngine.Debug.Log("执行失败:" + ex.ToString());
            }
        }
    }
}